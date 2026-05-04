using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using SistemaTurnos.Data;
using SistemaTurnos.Models;
using SistemaTurnos.Services;
using SistemaTurnos.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<MysqlDbContext>(options =>
    options.UseMySql(builder.Configuration.GetConnectionString("DefaultConnection"),
        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("DefaultConnection"))));

// Instanciar las interfaces y service cuando se necesiten 
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ITurnService, TurnService>();
builder.Services.AddScoped<IAsesorService, AsesorService>();
builder.Services.AddScoped<IAuthService, AuthService>();

// Activar Autenticación por Cookies
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login"; // A donde va si no está logueado
        options.AccessDeniedPath = "/Account/AccessDenied";
    });


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

// ... después de builder.Build() ...
app.UseAuthentication(); // ¡IMPORTANTE! Antes de UseAuthorization
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();