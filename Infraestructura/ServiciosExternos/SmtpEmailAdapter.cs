using Dominio.Interfaces;
using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Identity.UI.Services;
using MimeKit;
using System;
using System.Threading.Tasks;

namespace Infraestructura.ServiciosExternos
{
    public class SmtpEmailAdapter : IEmailService
    {
        private readonly string _remitente = "noreply.sistemacitas@gmail.com";
        private readonly string _contraseñaApp = "maoz juwg swmk gucp\r\n";

        public async Task EnviarCorreoAsync(string destino, string asunto, string mensaje)
        {
            try
            {
                var emailMessage = new MimeMessage();

                // Remitente
                emailMessage.From.Add(new MailboxAddress("Sistema de Reservas", _remitente));

                // Destinatario
                emailMessage.To.Add(MailboxAddress.Parse(destino));

                // Asunto y cuerpo
                emailMessage.Subject = asunto;
                emailMessage.Body = new TextPart("plain") { Text = mensaje };

                using var client = new SmtpClient();

                // Conexión a Gmail
                await client.ConnectAsync("smtp.gmail.com", 587, MailKit.Security.SecureSocketOptions.StartTls);

                // Autenticación con contraseña de aplicación
                await client.AuthenticateAsync(_remitente, _contraseñaApp);

                // Envío
                await client.SendAsync(emailMessage);

                await client.DisconnectAsync(true);
            }
            catch (SmtpCommandException ex)
            {
                Console.WriteLine($"Error en comando SMTP: {ex.Message}");
                throw new InvalidOperationException();
            }
            catch (SmtpProtocolException ex)
            {
                Console.WriteLine($"Error en protocolo SMTP: {ex.Message}");
                throw new InvalidOperationException();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error general al enviar correo: {ex.Message}");
                throw new InvalidOperationException();
            }
        }
    }
}
