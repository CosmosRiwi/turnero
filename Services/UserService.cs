using Microsoft.EntityFrameworkCore;
using SistemaTurnos.Data;
using SistemaTurnos.Models;
using SistemaTurnos.Responses;
using SistemaTurnos.Services.Interfaces;

namespace SistemaTurnos.Services;

public class UserService : IUserService
{
    private readonly MysqlDbContext _context;

    public UserService(MysqlDbContext context)
    {
        _context = context;
    }

    public async Task<ServiceResponse<User>> GetByDniAsync(string dni)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Dni == dni);

        if (user == null)
            return ServiceResponse<User>.Error("El usuario no se encuentra registrado.");

        return ServiceResponse<User>.Success(user);
    }

    public async Task<ServiceResponse<User>> CreateAsync(User user)
    {
        // Regla: No duplicados (Principio de integridad)
        var exists = await _context.Users.AnyAsync(u => u.Dni == user.Dni);
        if (exists)
            return ServiceResponse<User>.Error("Ya existe un usuario con este documento.");

        user.Status = "Activo"; // Estado por defecto
        user.CreatedAt = DateTime.Now;

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return ServiceResponse<User>.Success(user, "Usuario registrado exitosamente.");
    }

    public async Task<ServiceResponse<User>> UpdateAsync(User user)
    {
        var userDb = await _context.Users.FindAsync(user.Id);
        if (userDb == null) return ServiceResponse<User>.Error("Usuario no encontrado.");

        userDb.Name = user.Name;
        userDb.LastName = user.LastName;
        userDb.Email = user.Email;
        userDb.Status = user.Status;

        await _context.SaveChangesAsync();
        return ServiceResponse<User>.Success(userDb, "Información actualizada.");
    }

    public async Task<ServiceResponse<bool>> IsActiveAsync(int userId)
    {
        var user = await _context.Users.FindAsync(userId);
        bool active = user?.Status == "Activo";
        return ServiceResponse<bool>.Success(active);
    }
}