using Microsoft.EntityFrameworkCore;
using SistemaTurnos.Data;
using SistemaTurnos.Models;
using SistemaTurnos.Responses;
using SistemaTurnos.Services.Interfaces;

namespace SistemaTurnos.Services;

public class AuthService : IAuthService
{
    private readonly MysqlDbContext _context;

    public AuthService(MysqlDbContext context)
    {
        _context = context;
    }

    public async Task<ServiceResponse<Staff>> LoginAsync(string username, string password)
    {
        // Buscamos al miembro del staff por usuario y contraseña
        var staff = await _context.Staff
            .FirstOrDefaultAsync(s => s.Username == username && s.Password == password);

        if (staff == null)
            return ServiceResponse<Staff>.Error("Usuario o contraseña incorrectos.");

        return ServiceResponse<Staff>.Success(staff, $"Bienvenido {staff.Username}");
    }
}