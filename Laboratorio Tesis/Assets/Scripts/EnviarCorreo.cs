using UnityEngine;
using System.Net;
using System.Net.Mail;
using System.Threading;
using System;
using System.Text;
using TMPro;

public class EnviarCorreo : MonoBehaviour
{
    [Header("SMTP (Gmail)")]
    private string smtpUser = "laboratoriovrunimet@gmail.com";        // correo remitente (tu cuenta)
    private string smtpPassword = "usjfavbmcidtwjsh";  // usa App Password, no tu contraseña normal
    private string smtpHost = "smtp.gmail.com";
    private int smtpPort = 587;
    private bool enableSsl = true;

    [Header("Opciones")]
    public bool debugLog = true;

    // Referencia al ScriptableObject con los datos
    public DatosSO2 datosSO;
    public datosBD datosBD;
    // Envía un correo simple en background. 'toAddress' es variable.
    public TextMeshProUGUI estatusText;

    // --- NUEVAS VARIABLES PARA COMUNICACIÓN ENTRE HILOS ---
    // Usamos 'volatile' para asegurar que el valor sea leído correctamente desde ambos hilos.
    private volatile string _statusMessage = null;
    private volatile bool _isError = false;
    // ----------------------------------------------------

    public void Start()
    {
        //SendResultsFromDatosSO();
    }

    // --- NUEVO MÉTODO UPDATE ---
    // Update() se ejecuta en el HILO PRINCIPAL
    void Update()
    {
        // Si hay un mensaje nuevo del hilo secundario...
        if (_statusMessage != null)
        {
            string message = _statusMessage;
            _statusMessage = null; // "Consumimos" el mensaje

            // Ahora es SEGURO actualizar la UI y usar Debug.Log
            if (_isError)
            {
                Debug.LogError(message);
                estatusText.text = "Fallo al enviar el correo. Intente nuevamente.";
            }
            else
            {
                if (debugLog) Debug.Log(message);
                estatusText.text = "Correo enviado con exito. Recuerde revisar su carpeta de spam";
            }
        }
    }

    // --- MÉTODO SendEmail MODIFICADO ---
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

        // Esta parte se ejecuta en el hilo principal
        estatusText.text = "Enviando correo..."; // Informa al usuario que se está procesando

        ThreadPool.QueueUserWorkItem(_ =>
        {
            // Este bloque se ejecuta en el HILO SECUNDARIO
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
                
                // --- NO ACCEDAS A LA UI AQUÍ ---
                // En lugar de eso, guarda el resultado
                _isError = false;
                _statusMessage = $"EnviarCorreo: email enviado a {toAddress}";
            }
            catch (Exception ex)
            {
                // --- NO ACCEDAS A LA UI AQUÍ ---
                _isError = true;
                _statusMessage = $"EnviarCorreo: fallo al enviar email: {ex.Message}";
            }
        });
    }

    // Envía un correo usando los datos del ScriptableObject (destinatario = datosSO.Correo)
    public void SendResultsFromDatosSO()
    {
        if (datosSO == null)
        {
            Debug.LogError("EnviarCorreo: DatosSO2 no asignado.");
            return;
        }

        string to = datosSO.correo;
        if (string.IsNullOrEmpty(to))
        {
            Debug.LogError("EnviarCorreo: correo destinatario en DatosSO2 vacío.");
            return;
        }

        var tiempos = datosSO.tiempos ?? new System.Collections.Generic.List<float>();
        var distancias = datosSO.estiramientos ?? new System.Collections.Generic.List<float>();

        string tiemposStr = tiempos.Count > 0 ? string.Join(", ", tiempos) : "N/A";
        string distStr = distancias.Count > 0 ? string.Join(", ", distancias) : "N/A";

        string subject = "Resultados Laboratorio Resorte Helicoidal";
        string body = new StringBuilder()
            .AppendLine("Resultados:")
            .Append("Tiempo oscilaciones (s): ")
            .AppendLine(tiemposStr)
            .Append("Estiramientos (cm): ")
            .Append(distStr)
            .ToString();

        SendEmail(to, subject, body, false);
        datosBD.EnviarDatos();
    }
}