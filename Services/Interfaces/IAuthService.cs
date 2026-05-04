using SistemaTurnos.Models;
using SistemaTurnos.Responses;

namespace SistemaTurnos.Services.Interfaces;

public interface IAuthService
{
    // Devuelve el objeto Staff si las credenciales son correctas
    Task<ServiceResponse<Staff>> LoginAsync(string username, string password);

}