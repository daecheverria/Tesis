using UnityEngine;
using TMPro;

public class Cronometro : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TMP_Text textoCronometro; // Mostrar formato MM:SS:CS (centésimas)

    private float tiempoTranscurrido; // En segundos
    private bool corriendo;
    private int estadoAlternar;

    private void Awake()
    {
        ActualizarTexto(0f);
    }

    private void Update()
    {
        if (!corriendo) return;

        tiempoTranscurrido += Time.deltaTime;
        ActualizarTexto(tiempoTranscurrido);
    }

    public void IniciarSimulacion()
    {
        switch (estadoAlternar)
        {
            case 0:
                Iniciar();
                break;
            case 1: 
                Pausar();
                break;
            case 2:
                Reiniciar();
                break;
        }

        // Avanza el estado cíclicamente 0 -> 1 -> 2 -> 0 ...
        estadoAlternar = (estadoAlternar + 1) % 3;
    }

    public void Iniciar()
    {
        corriendo = true;
    }

    public void Pausar()
    {
        corriendo = false;
    }

    public void Reiniciar()
    {
        corriendo = false;
        tiempoTranscurrido = 0f;
        ActualizarTexto(0f);
    }

    public string ObtenerTextoActual()
    {
        return FormatearTiempo(tiempoTranscurrido);
    }

    private void ActualizarTexto(float segundos)
    {
        if (textoCronometro != null)
        {
            textoCronometro.text = FormatearTiempo(segundos);
        }
    }

    // Formato "00:00:00" de derecha a izquierda: centésimas, segundos, minutos
    private string FormatearTiempo(float segundos)
    {
        int totalSegundos = Mathf.FloorToInt(segundos);
        int minutos = totalSegundos / 60;
        int seg = totalSegundos % 60;
        int centesimas = Mathf.FloorToInt((segundos * 100f)) % 100; // 00-99

        return string.Format("{0:00}:{1:00}:{2:00}", minutos, seg, centesimas);
    }
}
