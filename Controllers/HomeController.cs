using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using SistemaTurnos.Models;
using SistemaTurnos.Services.Interfaces;

namespace SistemaTurnos.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly ITurnService _turnService;

    public HomeController(ILogger<HomeController> logger, ITurnService turnService)
    {
        _logger = logger;
        _turnService = turnService;
    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    public async Task<IActionResult> SalaEspera()
    {
        var currentResponse = await _turnService.GetCurrentCallingAsync();
        var waitingResponse = await _turnService.GetWaitingListForDisplayAsync(5);

        ViewBag.Siguientes = waitingResponse.Data;

        // Si no hay nadie en atención, mandamos el objeto nulo pero la vista lo maneja
        return View(currentResponse.Data);
    }
    
    [HttpGet]
    public async Task<IActionResult> GetSalaData()
    {
        var actual = await _turnService.GetCurrentCallingAsync();
        var siguientes = await _turnService.GetWaitingListForDisplayAsync(5);

        return Json(new { 
            success = actual.Status, 
            // Datos del turno actual
            ticket = actual.Data?.Ticket ?? "---",
            cliente = actual.Data != null ? $"{actual.Data.User.Name} {actual.Data.User.LastName}" : "OFICINA DISPONIBLE",
            modulo = actual.Data?.StaffId ?? 0, 
            // Lista de los siguientes 
            proximos = siguientes.Data.Select(t => new { t.Ticket})
        });
    }


    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}