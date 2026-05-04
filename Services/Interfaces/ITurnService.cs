using SistemaTurnos.Models;
using SistemaTurnos.Responses;

namespace SistemaTurnos.Services.Interfaces;

public interface ITurnService
{
    // Lo usa la pantalla de registro de usuarios
    Task<ServiceResponse<Turn>> CreateTurnAsync(int userId, int priorityId);

    // Lo usa Isabella para la pantalla de Sala de Espera
    Task<ServiceResponse<IEnumerable<Turn>>> GetCurrentQueueAsync();

    // Lo usas tú para saber cuánta gente hay antes de llamar al siguiente
    Task<ServiceResponse<int>> GetWaitingCountAsync();

    // Método para validar que un usuario no pida dos turnos a la vez
    Task<ServiceResponse<bool>> HasActiveTurnAsync(int userId);

    // Obtiene el turno que se está atendiendo actualmente
    Task<ServiceResponse<Turn>> GetCurrentCallingAsync();

    // Obtiene la lista de espera simplificada para la pantalla
    Task<ServiceResponse<IEnumerable<Turn>>> GetWaitingListForDisplayAsync(int limit);
}