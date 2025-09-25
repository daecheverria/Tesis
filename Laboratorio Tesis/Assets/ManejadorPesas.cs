using System;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

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
                    tiempoUltimaOscilacion = tiempoActual;
                }
                contadorOscilaciones++;
                if (contadorOscilaciones > objetivoOscilaciones)
                {
                    Debug.Log($"Tiempo para {objetivoOscilaciones} oscilaciones (descartando la primera): {sumaTiemposOscilaciones:F4} segundos");
                    midiendo = false;
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
        }
    }
}
