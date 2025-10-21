using UnityEngine;
using System.Net;
using System.Net.Mail;
using System.Threading;
using System;
using System.Text;

public class EnviarCorreo : MonoBehaviour
{
    [Header("SMTP (Gmail)")]
    public string smtpUser = "tu@gmail.com";               // correo remitente (tu cuenta)
    public string smtpPassword = "APP_PASSWORD";           // usa App Password, no tu contraseña normal
    public string smtpHost = "smtp.gmail.com";
    public int smtpPort = 587;
    public bool enableSsl = true;

    [Header("Opciones")]
    public bool debugLog = true;

    // Referencia al ScriptableObject con los datos
    public DatosSO datosSO;

    // Envía un correo simple en background. 'toAddress' es variable.
    public void SendEmail(string toAddress, string subject, string body, bool isHtml = false)
    {
        if (string.IsNullOrEmpty(toAddress))
        {
            Debug.LogError("EnviarCorreo: destinatario vacío.");
            return;
        }

        if (string.IsNullOrEmpty(smtpUser) || string.IsNullOrEmpty(smtpPassword))
        {
            Debug.LogError("EnviarCorreo: credenciales SMTP no configuradas.");
            return;
        }

        ThreadPool.QueueUserWorkItem(_ =>
        {
            try
            {
                using (var mail = new MailMessage())
                {
                    mail.From = new MailAddress(smtpUser);
                    mail.To.Add(toAddress);
                    mail.Subject = subject ?? string.Empty;
                    mail.Body = body ?? string.Empty;
                    mail.IsBodyHtml = isHtml;

                    using (var client = new SmtpClient(smtpHost, smtpPort))
                    {
                        client.EnableSsl = enableSsl;
                        client.Credentials = new NetworkCredential(smtpUser, smtpPassword);
                        client.Send(mail);
                    }
                }

                if (debugLog) Debug.Log($"EnviarCorreo: email enviado a {toAddress}");
            }
            catch (Exception ex)
            {
                Debug.LogError($"EnviarCorreo: fallo al enviar email: {ex.Message}");
            }
        });
    }

    // Envía un correo usando los datos del ScriptableObject (destinatario = datosSO.Correo)
    public void SendResultsFromDatosSO()
    {
        if (datosSO == null)
        {
            Debug.LogError("EnviarCorreo: DatosSO no asignado.");
            return;
        }

        string to = datosSO.Correo;
        if (string.IsNullOrEmpty(to))
        {
            Debug.LogError("EnviarCorreo: correo destinatario en DatosSO vacío.");
            return;
        }

        var tiempos = datosSO.Tiempos ?? new System.Collections.Generic.List<float>();
        var distancias = datosSO.Distancias ?? new System.Collections.Generic.List<float>();

        string tiemposStr = tiempos.Count > 0 ? string.Join(", ", tiempos) : "N/A";
        string distStr = distancias.Count > 0 ? string.Join(", ", distancias) : "N/A";

        string subject = "Resultados Laboratorio Resorte Helicoidal";
        string body = new StringBuilder()
            .AppendLine("Resultados:")
            .Append("Tiempo oscilaciones: ")
            .AppendLine(tiemposStr)
            .Append("Longitude: ")
            .Append(distStr)
            .ToString();

        SendEmail(to, subject, body, false);
    }

    // Método de ejemplo para pruebas desde el inspector
    public void SendTestEmail()
    {
        SendEmail("destinatario@ejemplo.com", "Prueba desde Unity", "Este es un correo de prueba.");
    }
}
