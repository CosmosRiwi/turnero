using SistemaTurnos.Models;
using SistemaTurnos.Responses;

namespace SistemaTurnos.Services.Interfaces;

public interface IUserService
{
    // Devuelve el usuario si existe, útil para el flujo de "Si no existe, registrar"
    Task<ServiceResponse<User>> GetByDniAsync(string dni);

    // Registro de nuevos clientes
    Task<ServiceResponse<User>> CreateAsync(User user);

    // Actualización de datos desde el panel del asesor
    Task<ServiceResponse<User>> UpdateAsync(User user);

    // Validación de estado (Activo/Inactivo)
    Task<ServiceResponse<bool>> IsActiveAsync(int userId);
}