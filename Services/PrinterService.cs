using SistemaTurnos.Services.Interfaces;
using System.Diagnostics;

namespace SistemaTurnos.Services
{
    public class PrinterService : IPrinterService
    {
        public void ImprimirTicket(string ticket, string cliente, string prioridad)
        {
            // --- COMANDOS ESC/POS (Estándar Térmico) ---
            string ESC = "\x1B";
            string GS = "\x1D";
            string Centrar = $"{ESC}a1";
            string Izquierda = $"{ESC}a0";
            string NegritaOn = $"{ESC}E\x01";
            string NegritaOff = $"{ESC}E\x00";
            string FuenteGrande = $"{GS}!\x11"; // Doble ancho y alto
            string FuenteNormal = $"{GS}!\x00";
            string CortarPapel = $"{GS}V\x42\x00"; // Corte total

            // --- DISEÑO DEL TICKET ---
            string contenido = "";
            contenido += Centrar + NegritaOn + "SISTEMA DE TURNOS\n" + NegritaOff;
            contenido += "--------------------------------\n";
            contenido += "SU TURNO ES:\n\n";
            
            contenido += FuenteGrande + NegritaOn + ticket + NegritaOff + FuenteNormal + "\n\n";
            
            contenido += Izquierda + "--------------------------------\n";
            contenido += $"CLIENTE: {cliente.ToUpper()}\n";
            contenido += NegritaOn + $"PRIORIDAD: {prioridad.ToUpper()}\n" + NegritaOff;
            contenido += $"FECHA: {DateTime.Now:dd/MM/yyyy HH:mm}\n";
            contenido += "--------------------------------\n";
            contenido += Centrar + "\nGracias por su paciencia.\n";
            
            // Importante: Saltos de línea para que el papel salga y se pueda cortar
            contenido += "\n\n\n\n" + CortarPapel;
            System.IO.File.WriteAllText("/dev/usb/lp0", contenido);

            // --- EJECUCIÓN EN UBUNTU ---
            try
            {
                var process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = "bash",
                        // -e permite interpretar los caracteres de escape \x1B
                        Arguments = $"-c \"echo -e '{contenido}' | lp -d POS58\"",
                        UseShellExecute = false,
                        CreateNoWindow = true
                    }
                };
                process.Start();
            }
            catch (Exception ex)
            {
                // Si falla, al menos lo vemos en la consola de la terminal
                Console.WriteLine("--- ERROR IMPRESORA ---");
                Console.WriteLine(ex.Message);
            }
        }
    }
}
