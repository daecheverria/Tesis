using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

public class IA_Profesor : MonoBehaviour
{
    [SerializeField] private string AppscriptURL;
    [SerializeField] private string prompt;
    [SerializeField] private TMP_Text respuesta;

    private void Update()
    {
        #if ENABLE_INPUT_SYSTEM && !ENABLE_LEGACY_INPUT_MANAGER
        if (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            StartCoroutine(SendDataToGAS());
        }
        #elif ENABLE_LEGACY_INPUT_MANAGER
        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartCoroutine(SendDataToGAS());
        }
        #endif
    }

    private IEnumerator SendDataToGAS()
    {
        if (string.IsNullOrEmpty(AppscriptURL))
        {
            Debug.LogError("IA_Profesor: AppscriptURL vacío. Asigna la URL de despliegue (termina en /exec).");
            yield break;
        }

        if (respuesta != null)
            respuesta.text = "Consultando...";

        // Enviar como application/x-www-form-urlencoded para que e.parameter funcione en Apps Script
        var form = new Dictionary<string, string>();
        form["parameter"] = prompt ?? string.Empty;
        UnityWebRequest www = UnityWebRequest.Post(AppscriptURL, form);
        www.timeout = 90; // segundos

        yield return www.SendWebRequest();

        string response;
        if (www.result == UnityWebRequest.Result.Success)
        {
            response = www.downloadHandler.text;
        }
        else
        {
            response = $"Error: {www.result} - {www.error}";
        }

        Debug.Log(response);
        if (respuesta != null)
            respuesta.text = response;
    }
}
