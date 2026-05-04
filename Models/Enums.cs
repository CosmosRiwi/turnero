namespace SistemaTurnos.Models
{
    public enum UserStatus
    {
        Inactivo = 0,
        Activo = 1
    }

    public enum TurnoStatus
    {
        Pendiente = 1,
        EnEspera = 2,
        EnAtencion = 3,
        Finalizado = 4,
        Cancelado = 5
    }

    public enum PriorityLevel
    {
        Normal = 1,
        Prioritario = 2,
        VIP = 3
    }
}