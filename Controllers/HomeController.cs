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
        var response = await _turnService.GetCurrentCallingAsync();
        return Json(new { 
            success = response.Status, 
            ticket = response.Data?.Ticket ?? "---" 
        });
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}