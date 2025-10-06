using System;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

using System.Collections.Generic;
using System.IO;
using System.Text;

public class ManejadorPesas : MonoBehaviour
{
    private XRSocketInteractor socket;
    public SpringJoint springJoint;
    public Rigidbody selfRb;
    private float initialMass = 0.023f;

    // --- Añadidos para el contador y timer ---
    public int objetivoOscilaciones = 40; // Número de oscilaciones a medir
    private int contadorOscilaciones = 0;
    private float tiempoInicio = 0f;
    private float ultimaPosicionY = 0f;
    private bool bajando = false;
    private bool midiendo = false;
    private float tiempoUltimaOscilacion = 0f;
    private float sumaTiemposOscilaciones = 0f;

    // Lista para guardar posiciones Y y tiempos
    private List<float> posicionesY = new List<float>();
    private List<float> tiemposOscilacion = new List<float>();

    void Start()
    {
        socket = GetComponent<XRSocketInteractor>();
        socket.selectEntered.AddListener(OnPesaAttached);
        socket.selectExited.AddListener(OnPesaDetached);
        selfRb.sleepThreshold = 0.0f;

        if (springJoint != null)
        {
            springJoint.connectedBody = selfRb;
        }
    }

    void Update()
    {
        selfRb.WakeUp();
        //Debug.Log(selfRb.linearVelocity);
        if (midiendo)
        {
            float posY = selfRb.transform.position.y;
            // Detectar paso por el punto más bajo (cambio de dirección de bajada a subida)
            if (!bajando && posY < ultimaPosicionY)
            {
                bajando = true;
            }
            else if (bajando && posY > ultimaPosicionY)
            {
                bajando = false;
                float tiempoActual = Time.time;
                if (contadorOscilaciones == 0)
                {
                    // Primera oscilación: solo actualizar tiempoUltimaOscilacion, no contar ni imprimir
                    tiempoUltimaOscilacion = tiempoActual;
                }
                else
                {
                    float tiempoOscilacion = tiempoActual - tiempoUltimaOscilacion;
                    sumaTiemposOscilaciones += tiempoOscilacion;
                    Debug.Log($"Oscilación {contadorOscilaciones} - Tiempo: {tiempoOscilacion:F4} segundos");
                    // Guardar tiempo y posición Y
                    tiemposOscilacion.Add(tiempoOscilacion);
                    posicionesY.Add(posY);
                    tiempoUltimaOscilacion = tiempoActual;
                }
                contadorOscilaciones++;
                if (contadorOscilaciones > objetivoOscilaciones)
                {
                    Debug.Log($"Tiempo para {objetivoOscilaciones} oscilaciones (descartando la primera): {sumaTiemposOscilaciones:F4} segundos");
                    midiendo = false;
                    ExportarCSV();
                }
            }
            ultimaPosicionY = posY;
        }
    }

    private void OnPesaAttached(SelectEnterEventArgs args)
    {
        Rigidbody pesaRigidbody = args.interactableObject.transform.GetComponent<Rigidbody>();
        Debug.Log("Pesa agarrada: " + pesaRigidbody.name);
        Debug.Log(selfRb);
        selfRb.mass = pesaRigidbody.mass + initialMass; // Suma la masa de la pesa a la del objeto con el socket
        Debug.Log("Masa actual del objeto con socket: " + selfRb.mass);
        Debug.Log("Masa de la pesa añadida: " + pesaRigidbody.mass);
        selfRb.WakeUp();

        // Iniciar medición de oscilaciones
    contadorOscilaciones = 0;
    tiempoInicio = Time.time;
    tiempoUltimaOscilacion = tiempoInicio;
    sumaTiemposOscilaciones = 0f;
    ultimaPosicionY = selfRb.transform.position.y;
    bajando = false;
    midiendo = true;
    posicionesY.Clear();
    tiemposOscilacion.Clear();
    Debug.Log($"Iniciando medición de {objetivoOscilaciones} oscilaciones...");
    }

    private void OnPesaDetached(SelectExitEventArgs args)
    {
        selfRb.mass = initialMass; // Resta la masa de la pesa al objeto con el socket
        selfRb.WakeUp();
        Debug.Log("Masa actual del objeto con socket: " + selfRb.mass);

        // Detener medición si estaba activa
        if (midiendo)
        {
            midiendo = false;
            Debug.Log("Medición de oscilaciones cancelada por soltar la pesa.");
            ExportarCSV();
        }

    }

    // Exportar datos a CSV
    private void ExportarCSV()
    {
        if (tiemposOscilacion.Count == 0 || posicionesY.Count == 0) return;
        StringBuilder sb = new StringBuilder();
        sb.AppendLine("Oscilacion,Tiempo,PosicionY");
        for (int i = 0; i < tiemposOscilacion.Count; i++)
        {
            sb.AppendLine($"{i+1},{tiemposOscilacion[i]},{posicionesY[i]}");
        }
        string filePath = Path.Combine(Application.persistentDataPath, "oscilaciones.csv");
        File.WriteAllText(filePath, sb.ToString());
        Debug.Log($"Datos exportados a CSV en: {filePath}");
    }
}
