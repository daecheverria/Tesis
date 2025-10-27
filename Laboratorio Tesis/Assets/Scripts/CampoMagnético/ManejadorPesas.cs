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
    private float initialMass;
    public bool pesaColgada = false;
    public DatosSO2 datosSO2; // referencia al ScriptableObject donde guardar teóricos
    public Resorte resorte;

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
    public Collider pesaCol;

    void Awake()
    {
        if (selfRb == null)
        {
            selfRb = GetComponent<Rigidbody>();
        }
        selfRb.mass = UnityEngine.Random.Range(0.008f, 0.012f);
        initialMass = selfRb.mass;
    }
    void Start()
    {
        socket = GetComponent<XRSocketInteractor>();
        socket.selectEntered.AddListener(OnPesaAttached);
        socket.selectExited.AddListener(OnPesaDetached);
        selfRb.sleepThreshold = 0.0f;

        //if (springJoint != null)
        //{
        //    springJoint.connectedBody = selfRb;
        //    springJoint.maxDistance = 5f;
        //}
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
                    //ExportarCSV();
                }
            }
            ultimaPosicionY = posY;
        }
    }

    private void OnPesaAttached(SelectEnterEventArgs args)
    {
        Rigidbody pesaRigidbody = args.interactableObject.transform.GetComponent<Rigidbody>();
        pesaCol = args.interactableObject.transform.GetComponent<Collider>();
        pesaColgada = true;
        selfRb.mass = pesaRigidbody.mass + initialMass;
        selfRb.WakeUp();
        asegurarPesa asegurar = GetComponent<asegurarPesa>();
        asegurar.Asegurar(pesaCol);

        // --- CÁLCULO TEÓRICO Y MAPEO ---
        // Mapear masa de la pesa (kg) a índice según múltiplos de 5 g:
        // 5g -> índice 0, 10g -> 1, 15g -> 2, ... (se admite hasta 50g por defecto)
        if (datosSO2 != null)
        {
            float k = resorte.springConstant;
            float m = selfRb.mass; // masa usada en el sistema (kg)
            if (k > 0f && m > 0f)
            {
                // gravedad positiva
                float g = Mathf.Abs(Physics.gravity.y);

                // calcular periodo teórico T = 2π * sqrt(m / k)
                // (nota: la fórmula correcta del periodo para masa-resorte es T = 2π sqrt(m/k))
                float periodoTeorico = 2f * Mathf.PI * Mathf.Sqrt(m / k);

                // estiramiento teórico x = m * g / k
                float estiramientoTeorico = m * g / k;

                // convertir masa de la pesa (no selfRb) a gramos y redondear al múltiplo de 5 más cercano
                int grams = Mathf.RoundToInt(pesaRigidbody.mass * 1000f);
                int rounded = Mathf.Clamp(Mathf.RoundToInt(grams / 5f) * 5, 5, 50); // 5..50
                int index = (rounded / 5) - 1; // 5->0, 10->1, ..., 50->9

                // Asegurar tamaño de listas en datosSO2
                EnsureListSize(datosSO2.tiemposTeoricos, index + 1, 0f);
                EnsureListSize(datosSO2.estiramientosTeoricos, index + 1, 0f);

                datosSO2.tiemposTeoricos[index] = periodoTeorico;
                datosSO2.estiramientosTeoricos[index] = estiramientoTeorico;

                // Debug.Log($"ManejadorPesas: masa pesa={pesaRigidbody.mass}kg ({grams}g) -> rounded {rounded}g -> index {index}");
                // Debug.Log($"ManejadorPesas: periodoTeorico={periodoTeorico:F4}s, estiramientoTeorico={estiramientoTeorico:F4}m guardados en DatosSO2");
            }
            else
            {
                Debug.LogWarning("ManejadorPesas: springJoint.spring o masa inválida para cálculo teórico.");
            }
        }

        // Iniciar medición de oscilaciones
        contadorOscilaciones = 0;
        tiempoInicio = Time.time;
        tiempoUltimaOscilacion = tiempoInicio;
        sumaTiemposOscilaciones = 0f;
        ultimaPosicionY = selfRb.transform.position.y;
        bajando = false;
        // midiendo = true;
        posicionesY.Clear();
        tiemposOscilacion.Clear();
        // Debug.Log($"Iniciando medición de {objetivoOscilaciones} oscilaciones...");
    }

    private void OnPesaDetached(SelectExitEventArgs args)
    {
        selfRb.mass = initialMass; // Resta la masa de la pesa al objeto con el socket
        selfRb.WakeUp();
        pesaColgada = false;

        // Detener medición si estaba activa
        if (midiendo)
        {
            midiendo = false;
            Debug.Log("Medición de oscilaciones cancelada por soltar la pesa.");
            //ExportarCSV();
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
            sb.AppendLine($"{i+1};{tiemposOscilacion[i]};{posicionesY[i]}");
        }
        string filePath = Path.Combine(Application.persistentDataPath, "oscilaciones.csv");
        File.WriteAllText(filePath, sb.ToString());
        Debug.Log($"Datos exportados a CSV en: {filePath}");
    }

    private void EnsureListSize(List<float> list, int size, float defaultValue)
    {
        while (list.Count < size) list.Add(defaultValue);
    }
}
