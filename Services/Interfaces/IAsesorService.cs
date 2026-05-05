using SistemaTurnos.Models;
using SistemaTurnos.Responses;

namespace SistemaTurnos.Services.Interfaces;

public interface IAsesorService
{
    // El motor de búsqueda de turnos (FIFO + Prioridad)
    Task<ServiceResponse<Turn>?> CallNextAsync(int staffId);

    // Guardar comentarios y cambiar a estado 'Finalizado'
    Task<ServiceResponse<Turn>> CompleteTurnAsync(int turnId, string comment);

    // Por si el usuario nunca llegó
    Task<ServiceResponse<Turn>> CancelTurnAsync(int turnId, string reason);

    // Listar turnos para el dashboard del asesor
    Task<ServiceResponse<IEnumerable<Turn>>> GetTurnByStatusAsync(int? statusId);
    
    // Mirar si el asesor tiene turnos pendientes al entrar o refrescar
    Task<ServiceResponse<Turn>> GetActiveTurn(int staffId);
}