namespace SistemaTurnos.Services.Interfaces;

public interface IPrinterService
{
    // El método recibe el ticket y el nombre para armar el diseño
    void ImprimirTicket(string ticket, string cliente, string prioridad);
}