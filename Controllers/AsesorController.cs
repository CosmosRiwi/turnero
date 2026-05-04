using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SistemaTurnos.Models;
using SistemaTurnos.Services.Interfaces;

namespace SistemaTurnos.Controllers;
[Authorize] // <--- Esto obliga a que exista la Cookie
public class AsesorController : Controller
{
    private readonly IAsesorService _asesorService;
    private readonly IUserService _userService; // Inyectamos el de usuarios
    private readonly ITurnService _turnService;

    public AsesorController(IAsesorService asesorService, IUserService userService, ITurnService turnService)
    {
        _asesorService = asesorService;
        _userService = userService;
        _turnService = turnService;
    }

    public async Task<IActionResult> Index()
    {
        // Traemos los turnos pendientes para mostrar cuánta gente hay en fila
        var response = await _asesorService.GetTurnByStatusAsync((int)TurnoStatus.Pendiente);

        // Pasamos la lista a la vista
        return View(response.Data);
    }

    [HttpGet]
    public async Task<IActionResult> GetWaitingList()
    {
        // Solo traemos los que están en estado "Pendiente"
        var response = await _asesorService.GetTurnByStatusAsync((int)TurnoStatus.Pendiente);

        // Devolvemos solo el HTML parcial o la data. 
        // Lo más fácil para empezar es devolver el conteo y la lista.
        return Json(new
        {
            count = response.Data.Count(),
            items = response.Data.Select(t => new { t.Ticket, Time = t.CreatedAt.ToString() })
        });
    }

    [HttpPost]
    public async Task<IActionResult> CallNext()
    {
        // Buscamos el Claim, si es nulo usamos "1"
        var claimId = User.FindFirst("StaffId")?.Value ?? "1";
        int staffId = int.Parse(claimId);

        var response = await _asesorService.CallNextAsync(staffId);

        if (response.Data == null && response.Status == false)
        {
            return Json(new { success = false, message = response.Message, icon = "info" });
        }

        return Json(new
        {
            success = true,
            message = response.Message,
            icon = "success",
            data = new
            {
                ticket = response.Data.Ticket,
                id = response.Data.Id,
                cliente = response.Data.User != null
                    ? $"{response.Data.User.Name} {response.Data.User.LastName}"
                    : "Usuario no registrado"
            }
        });
    }


    [HttpPost]
    public async Task<IActionResult> CompleteTurn(int turnId, string comment)
    {
        var response = await _asesorService.CompleteTurnAsync(turnId, comment);

        return Json(new
        {
            success = response.Status,
            message = response.Message,
            icon = response.Status ? "success" : "error"
        });
    }

    [HttpPost]
    public async Task<IActionResult> CancelTurn(int turnId, string reason)
    {
        var response = await _asesorService.CancelTurnAsync(turnId, reason);

        return Json(new
        {
            success = response.Status,
            message = response.Message,
            icon = response.Status ? "success" : "error"
        });
    }

    [AllowAnonymous]
    // Método para registrar usuario desde el panel
    [HttpPost]
    public async Task<IActionResult> RegistrarUsuario(User nuevoUser)
    {
        var response = await _userService.CreateAsync(nuevoUser);
        return Json(response);
    }

    [AllowAnonymous]
    [HttpGet]
    public async Task<IActionResult> BuscarPorDni(string dni)
    {
        // Llamamos a tu UserService
        var response = await _userService.GetByDniAsync(dni);

        // Devolvemos el ServiceResponse completo al JS
        return Json(response);
    }

    [AllowAnonymous]
    [HttpPost]
    public async Task<IActionResult> AsignarTurno(int userId, int priorityId)
    {
        var response = await _turnService.CreateTurnAsync(userId, priorityId);

        if (response.Status)
        {
            return Json(new
            {
                success = true,
                message = response.Message,
                data = new
                {
                    ticket = response.Data.Ticket,
                    cliente = $"{response.Data.User.Name} {response.Data.User.LastName}"
                }
            });
        }

        return Json(new { success = false, message = response.Message });
    }
}