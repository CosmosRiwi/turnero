using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using SistemaTurnos.Services.Interfaces;
using System.Security.Claims;

public class AccountController : Controller
{
    private readonly IAuthService _authService;

    public AccountController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpGet]
    public IActionResult Login() => View();

    [HttpPost]
    public async Task<IActionResult> Login(string username, string password)
    {
        var response = await _authService.LoginAsync(username, password);

        if (response.Status)
        {
            // Creamos los Claims (la "identidad" del asesor)
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, response.Data.Username),
                new Claim("StaffId", response.Data.Id.ToString()),
                new Claim(ClaimTypes.Role, response.Data.Role)
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity));

            return Json(new { success = true, redirectUrl = Url.Action("Index", "Asesor") });
        }

        return Json(new { success = false, message = response.Message });
    }

    [HttpPost] // O [HttpGet] según prefieras
    public async Task<IActionResult> Logout()
    {
        // Limpia la cookie de autenticación del sistema
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

        // Redirige al Login para que no puedan volver atrás
        return RedirectToAction("Login", "Account");
    }
}