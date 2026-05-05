using Microsoft.EntityFrameworkCore;
using SistemaTurnos.Data;
using SistemaTurnos.Models;
using SistemaTurnos.Responses;
using SistemaTurnos.Services.Interfaces;

namespace SistemaTurnos.Services;

public class AsesorService : IAsesorService
{
    private readonly MysqlDbContext _context;

    public AsesorService(MysqlDbContext context)
    {
        _context = context;
    }

    // Listar todos o por estado
    public async Task<ServiceResponse<IEnumerable<Turn>>> GetTurnByStatusAsync(int? statusId)
    {
        IQueryable<Turn> query = _context.Turns.Include(t => t.Status);
        if (statusId.HasValue)
            query = query.Where(t => t.StatusId == statusId);
        var turns = await query.ToListAsync();
        return ServiceResponse<IEnumerable<Turn>>.Success(turns);
    }

    // 1. En CallNextAsync, añade Include para que el Asesor vea a quién atiende
    public async Task<ServiceResponse<Turn>> CallNextAsync(int staffId)
    {
        try
        {
            var next = await _context.Turns
                .Include(t => t.User)
                .Where(t => t.StatusId == (int)TurnoStatus.Pendiente)
                // ORDENAR POR PRIORIDAD: VIP (3) primero, luego Prioritario (2), luego Normal (1)
                // Si tu base de datos tiene VIP=3 y Normal=1, usamos OrderByDescending
                .OrderByDescending(t => t.PriorityId)
                // Si tienen la misma prioridad, el que llegó primero (FIFO)
                .ThenBy(t => t.CreatedAt)
                .FirstOrDefaultAsync();

            if (next == null)
                return ServiceResponse<Turn>.Error("No hay turnos pendientes.");
            
            var atencion = await _context.Turns.Where(turno => turno.StatusId == (int)TurnoStatus.EnAtencion && turno.StaffId == staffId).FirstOrDefaultAsync();
            if (atencion != null)
                return ServiceResponse<Turn>.Error("No debe llamar un turno sin completar el actual");

            next.StatusId = (int)TurnoStatus.EnAtencion;
            next.StaffId = staffId;
            next.UpdatedAt = DateTime.Now;

            await _context.SaveChangesAsync();
            await _context.Entry(next).Reference(t => t.User).LoadAsync();
            return ServiceResponse<Turn>.Success(next, $"Llamando al turno {next.Ticket}");
        }
        catch (Exception e)
        {
            return ServiceResponse<Turn>.Error("Error: " + e.Message);
        }
    }
    
    // 2. En CompleteTurnAsync, valida primero
    public async Task<ServiceResponse<Turn>> CompleteTurnAsync(int turnId, string comment)
    {
        // Valida ANTES de tocar la DB
        if (string.IsNullOrWhiteSpace(comment))
            return ServiceResponse<Turn>.Error("El comentario de atención es obligatorio.");

        try {
            var turn = await _context.Turns.FindAsync(turnId);
            
            if (turn == null)
                return ServiceResponse<Turn>.Error("Turno no encontrado.");
            
            if(turn.StatusId != (int)TurnoStatus.EnAtencion)
                return ServiceResponse<Turn>.Error("No puedes finalizar este turno porque no está en estado 'En Atención'.");

            turn.StatusId = (int)TurnoStatus.Finalizado;
            turn.UpdatedAt = DateTime.Now;

            _context.TurnHistories.Add(new TurnHistory {
                TurnId = turnId,
                Comment = comment,
                CreatedAt = DateTime.Now
            });

            await _context.SaveChangesAsync();
            
            await _context.Entry(turn).Reference(t => t.User).LoadAsync();
            return ServiceResponse<Turn>.Success(turn, "Atención finalizada con éxito.");
        }
        catch (Exception e) {
            return ServiceResponse<Turn>.Error("Error: " + e.Message);
        }
    }

    // Cancelar el turno
    public async Task<ServiceResponse<Turn>> CancelTurnAsync(int turnId, string reason)
    {
        try
        {
            // 1. Buscamos el turno
            var turn = await _context.Turns.FindAsync(turnId);

            if (turn == null)
                return ServiceResponse<Turn>.Error("Ese turno no existe.");

            // 2. Actualizamos el estado del turno
            turn.StatusId = (int)TurnoStatus.Cancelado;
            turn.UpdatedAt = DateTime.Now;

            // Validamos que no este vacio el cmapo 
            if (string.IsNullOrEmpty(reason))
                return ServiceResponse<Turn>.Error("Debes de hacer un comentario");

            // 3. Creamos el registro en el historial (Tu "backend humano")
            var history = new TurnHistory
            {
                TurnId = turnId,
                Comment = reason,
                CreatedAt = DateTime.Now
                // Nota: Asegúrate de que los nombres coincidan con tu modelo generado
            };

            // 4. Guardamos ambos cambios en una sola transacción
            _context.TurnHistories.Add(history);
            await _context.SaveChangesAsync();

            return ServiceResponse<Turn>.Success(turn, "Turno cancelado");
        }
        catch (Exception e)
        {
            return ServiceResponse<Turn>.Error("Problemas internos: " + e.Message);
        }
    }
}