using Microsoft.EntityFrameworkCore;
using SistemaTurnos.Data;
using SistemaTurnos.Models;
using SistemaTurnos.Responses;
using SistemaTurnos.Services.Interfaces;

namespace SistemaTurnos.Services;

public class TurnService : ITurnService
{
    private readonly MysqlDbContext _context;
    private readonly IPrinterService _printerService; // Inyectamos la interfaz
    public TurnService(MysqlDbContext context, IPrinterService printerService)
    {
        _context = context;
        _printerService = printerService;
    }

    public async Task<ServiceResponse<Turn>> CreateTurnAsync(int userId, int priorityId)
    {
        try
        {
            // 1. Validaciones previas (Igual que antes)
            var tieneTurno = await _context.Turns.AnyAsync(t => t.UserId == userId &&
                                                                (t.StatusId == (int)TurnoStatus.Pendiente ||
                                                                 t.StatusId == (int)TurnoStatus.EnAtencion));

            if (tieneTurno)
                return ServiceResponse<Turn>.Error("El usuario ya tiene un turno en proceso.");

            // 2. Lógica de Ticket (Igual que antes)
            int totalHoy = await _context.Turns.CountAsync() + 1;
            string letra = priorityId == (int)PriorityLevel.VIP ? "V" :
                priorityId == (int)PriorityLevel.Prioritario ? "P" : "N";
            string ticket = $"{letra}-{totalHoy.ToString("D3")}";

            // 3. Creación del registro
            var nuevoTurno = new Turn
            {
                UserId = userId,
                PriorityId = priorityId,
                StatusId = (int)TurnoStatus.Pendiente,
                Ticket = ticket,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };

            _context.Turns.Add(nuevoTurno);
            await _context.SaveChangesAsync();

            // 4. Cargar relaciones para el ticket físico y el SweetAlert
            await _context.Entry(nuevoTurno).Reference(t => t.User).LoadAsync();
            await _context.Entry(nuevoTurno).Reference(t => t.Priority).LoadAsync();

            // 5. LLAMADA MÁGICA A LA IMPRESORA
            // Pasamos los datos reales: Ticket, Nombre y el nombre de la Prioridad (VIP, etc)
            _printerService.ImprimirTicket(
                nuevoTurno.Ticket, 
                $"{nuevoTurno.User.Name} {nuevoTurno.User.LastName}", 
                nuevoTurno.Priority.Name
            );

            return ServiceResponse<Turn>.Success(nuevoTurno, $"Ticket {ticket} generado correctamente.");
        }
        catch (Exception ex)
        {
            return ServiceResponse<Turn>.Error("Error técnico al generar turno: " + ex.Message);
        }
    }

    public async Task<ServiceResponse<IEnumerable<Turn>>> GetCurrentQueueAsync()
    {
        var queue = await _context.Turns
            .Include(t => t.User)
            .Where(t => t.StatusId == (int)TurnoStatus.Pendiente)
            .OrderByDescending(t => t.PriorityId)
            .ThenBy(t => t.CreatedAt)
            .ToListAsync();

        return ServiceResponse<IEnumerable<Turn>>.Success(queue);
    }

    public async Task<ServiceResponse<int>> GetWaitingCountAsync()
    {
        int count = await _context.Turns.CountAsync(t => t.StatusId == (int)TurnoStatus.Pendiente);
        return ServiceResponse<int>.Success(count);
    }

    public async Task<ServiceResponse<bool>> HasActiveTurnAsync(int userId)
    {
        bool active =
            await _context.Turns.AnyAsync(t => t.UserId == userId && t.StatusId == (int)TurnoStatus.Pendiente);
        return ServiceResponse<bool>.Success(active);
    }

    public async Task<ServiceResponse<Turn>> GetCurrentCallingAsync()
    {
        var turn = await _context.Turns
            .Include(t => t.User)
            .Include(t => t.Staff)
            .Where(t => t.StatusId == (int)TurnoStatus.EnAtencion)
            .OrderByDescending(t => t.UpdatedAt)
            .FirstOrDefaultAsync();

        // Si es nulo, mandamos Success pero con Data nula para que el JS sepa que "no hay nadie"
        if (turn == null) 
            return ServiceResponse<Turn>.Success(null, "No hay turnos activos");

        return ServiceResponse<Turn>.Success(turn);
    }

    public async Task<ServiceResponse<IEnumerable<Turn>>> GetWaitingListForDisplayAsync(int limit)
    {
        var list = await _context.Turns
            .Where(t => t.StatusId == (int)TurnoStatus.Pendiente)
            .OrderByDescending(t => t.PriorityId)
            .ThenBy(t => t.CreatedAt)
            .Take(limit)
            .ToListAsync();

        return ServiceResponse<IEnumerable<Turn>>.Success(list);
    }
}