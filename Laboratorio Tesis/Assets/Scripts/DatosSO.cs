using System;
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

    // --- Nuevos métodos: actualizar por índice ---
    /// <summary>
    /// Establece el valor en la posición indicada de la lista 'tiempos'.
    /// Si index es menor que 0 lanza ArgumentOutOfRangeException.
    /// Si index es mayor que la longitud actual, la lista se rellena con 0 hasta index-1 y se añade el valor en index.
    /// </summary>
    public void SetTiempoAt(int index, float value)
    {
        if (index < 0) throw new ArgumentOutOfRangeException(nameof(index), "El índice no puede ser negativo.");
        if (index < tiempos.Count)
        {
            tiempos[index] = value;
        }
        else
        {
            while (tiempos.Count < index) tiempos.Add(0f);
            tiempos.Add(value);
        }
    }

    /// <summary>
    /// Establece el valor en la posición indicada de la lista 'distancias'.
    /// Comportamiento idéntico a SetTiempoAt.
    /// </summary>
    public void SetDistanciaAt(int index, float value)
    {
        if (index < 0) throw new ArgumentOutOfRangeException(nameof(index), "El índice no puede ser negativo.");
        if (index < distancias.Count)
        {
            distancias[index] = value;
        }
        else
        {
            while (distancias.Count < index) distancias.Add(0f);
            distancias.Add(value);
        }
    }

    /// <summary>
    /// Intenta establecer el valor en la posición indicada de 'tiempos'.
    /// Devuelve true si la operación se realizó sin lanzar excepción (incluye ampliar la lista).
    /// Devuelve false si index es negativo.
    /// </summary>
    public bool TrySetTiempoAt(int index, float value)
    {
        if (index < 0) return false;
        if (index < tiempos.Count)
        {
            tiempos[index] = value;
        }
        else
        {
            while (tiempos.Count < index) tiempos.Add(0f);
            tiempos.Add(value);
        }
        return true;
    }

    /// <summary>
    /// Intenta establecer el valor en la posición indicada de 'distancias'.
    /// Comportamiento idéntico a TrySetTiempoAt.
    /// </summary>
    public bool TrySetDistanciaAt(int index, float value)
    {
        if (index < 0) return false;
        if (index < distancias.Count)
        {
            distancias[index] = value;
        }
        else
        {
            while (distancias.Count < index) distancias.Add(0f);
            distancias.Add(value);
        }
        return true;
    }
}
