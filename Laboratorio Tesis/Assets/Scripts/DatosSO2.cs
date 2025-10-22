using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DatosSO2", menuName = "Scriptable Objects/DatosSO2")]
public class DatosSO2: ScriptableObject
{
    [SerializeField] public string nombre;
    [SerializeField] public string correo;
    [SerializeField] public string cedula;
    [SerializeField] public float[] tiempos;
    //[SerializeField] public List<float> distancias;
}
