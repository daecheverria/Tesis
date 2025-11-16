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
    public float initialMass;
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
    // --- LineRenderer para la línea horizontal ---
    public LineRenderer lineRenderer;
    [Tooltip("Longitud desde el origen hacia cada lado (m). Ej: 0.5 = 0.5m a la izquierda y 0.5m a la derecha)")]
    public float halfLength = 0.5f;
    public Material lineMaterial;
    public float lineWidth = 0.01f;

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

        SetupLineRenderer();

    }
     private void SetupLineRenderer()
    {
        if (lineRenderer == null)
        {
            lineRenderer = GetComponent<LineRenderer>();
            if (lineRenderer == null) lineRenderer = gameObject.AddComponent<LineRenderer>();
        }

        lineRenderer.useWorldSpace = false; 
        lineRenderer.positionCount = 2;
        Vector3 left = new Vector3(-halfLength, 0f, 0f);
        Vector3 right = new Vector3(halfLength, 0f, 0f);
        lineRenderer.SetPosition(0, left);
        lineRenderer.SetPosition(1, right);
        lineRenderer.widthMultiplier = lineWidth;

        if (lineMaterial != null)
        {
            lineRenderer.material = lineMaterial;
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
                    float promedioTiemposOscilaciones = sumaTiemposOscilaciones / objetivoOscilaciones;
                    float tiempoTeorico = 2f * Mathf.PI * Mathf.Sqrt(selfRb.mass / resorte.springConstant);
                    float dif = Mathf.Abs(promedioTiemposOscilaciones - tiempoTeorico);
                    float k = resorte.springConstant;
                    float m = selfRb.mass;
                    Debug.LogWarning($"Promedio {promedioTiemposOscilaciones:F10} s | teorico {tiempoTeorico:F10} s | dif {dif:F10} s | k={k:F4} N/m | m={m:F4} kg");
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
        if (datosSO2 != null)
        {
            float k = resorte.springConstant;
            float m = selfRb.mass; // masa usada en el sistema (kg)
            if (k > 0f && m > 0f)
            {
                float g = Mathf.Abs(Physics.gravity.y);

                float periodoTeorico = 2f * Mathf.PI * Mathf.Sqrt(m / k);
                float estiramientoTeorico = m * g / k;

                int grams = Mathf.RoundToInt(pesaRigidbody.mass * 1000f);
                int rounded = Mathf.Clamp(Mathf.RoundToInt(grams / 5f) * 5, 5, 45); // 5..45
                int index = (rounded / 5) - 1; // 5->0, 10->1, ..., 45->8

                // Asegurar tamaño de listas en datosSO2 para el índice calculado
                EnsureListSize(datosSO2.tiemposTeoricos, index + 1, 0f);
                EnsureListSize(datosSO2.estiramientosTeoricos, index + 1, 0f);

                // AÑADIDO: asegurar que exista el elemento en índice 9 (0g) sin tocar los existentes
                EnsureListSize(datosSO2.estiramientosTeoricos, 10, 0f);

                datosSO2.tiemposTeoricos[index] = periodoTeorico;
                datosSO2.estiramientosTeoricos[index] = estiramientoTeorico;
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
        midiendo = true;
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
