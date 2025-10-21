using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DatosSO", menuName = "Scriptable Objects/DatosSO")]
public class DatosSO : ScriptableObject
{
    [SerializeField] private string correo;
    [SerializeField] private string cedula;
    [SerializeField] private List<float> tiempos = new List<float>();
    [SerializeField] private List<float> distancias = new List<float>();

    // Propiedades (get / set)
    public string Correo
    {
        get => correo;
        set => correo = value;
    }

    public string Cedula
    {
        get => cedula;
        set => cedula = value;
    }

    public List<float> Tiempos
    {
        get => tiempos;
        set => tiempos = value ?? new List<float>();
    }

    public List<float> Distancias
    {
        get => distancias;
        set => distancias = value ?? new List<float>();
    }

    // Métodos auxiliares
    public void AddTiempo(float t) => tiempos.Add(t);
    public void AddDistancia(float d) => distancias.Add(d);

    public void ClearTiempos() => tiempos.Clear();
    public void ClearDistancias() => distancias.Clear();

    public void ResetAll()
    {
        correo = string.Empty;
        cedula = string.Empty;
        tiempos.Clear();
        distancias.Clear();
    }
}
