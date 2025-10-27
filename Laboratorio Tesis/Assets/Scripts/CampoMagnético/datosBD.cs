using UnityEngine;
using Firebase;
using Firebase.Firestore;
using Firebase.Extensions;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Reflection;

public class datosBD         : MonoBehaviour
{
    [Tooltip("Referencia al ScriptableObject que contiene los datos a enviar")]
    public DatosSO2 datosSO;

    [Tooltip("Nombre de la colección en Firestore")]
    public string coleccion = "Prueba";

    void Start()
    {
        // Inicializar Firebase
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            var dependencyStatus = task.Result;
            if (dependencyStatus == DependencyStatus.Available)
            {
                Debug.Log("Firebase listo.");
            }
            else
            {
                Debug.LogError($"No se pudieron resolver dependencias de Firebase: {dependencyStatus}");
            }
        });
    }

    // Método público para enviar los datos contenidos en datosSO a Firestore
    public void EnviarDatos()
    {
        if (datosSO == null)
        {
            Debug.LogWarning("datosBD.EnviarDatos: 'datosSO' no está asignado.");
            return;
        }

        FirebaseFirestore db = FirebaseFirestore.DefaultInstance;
        if (db == null)
        {
            Debug.LogError("datosBD.EnviarDatos: Firestore no está inicializado.");
            return;
        }

        // Construir payload dinámicamente a partir de los campos/propiedades del ScriptableObject
        var payload = new Dictionary<string, object>();

        var tipo = datosSO.GetType();

        // Campos (incluye privados serializables)
        var campos = tipo.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        foreach (var campo in campos)
        {
            // evita campos heredados (de UnityEngine.Object) y solo incluye los serializables del tipo concreto
            if (campo.DeclaringType != tipo) continue;

            bool incluir = campo.IsPublic || campo.GetCustomAttribute<SerializeField>() != null;
            if (!incluir) continue;

            object valor = campo.GetValue(datosSO);
            if (valor == null) { payload[campo.Name] = ""; continue; }

            // Listas y enumerables
            if (valor is IEnumerable<float> floatEnum)
            {
                payload[campo.Name] = floatEnum.Cast<object>().ToList();
            }
            else if (valor is IList list)
            {
                var outList = new List<object>();
                foreach (var it in list) outList.Add(it ?? "");
                payload[campo.Name] = outList;
            }
            else if (valor is string || valor is int || valor is long || valor is double || valor is float || valor is bool)
            {
                payload[campo.Name] = valor;
            }
            else
            {
                payload[campo.Name] = valor.ToString();
            }
        }

        // Propiedades públicas con getter
        var props = tipo.GetProperties(BindingFlags.Instance | BindingFlags.Public);
        foreach (var prop in props)
        {
            // evita propiedades heredadas (por ejemplo 'name', 'hideFlags') y propiedades indexadas
            if (prop.DeclaringType != tipo) continue;
            if (prop.GetIndexParameters().Length > 0) continue;
            if (!prop.CanRead) continue;

            // opcional: filtrar por nombre explícito
            if (prop.Name == "name" || prop.Name == "hideFlags") continue;

            object valor;
            try { valor = prop.GetValue(datosSO); }
            catch { continue; }

            if (valor == null) { payload[prop.Name] = ""; continue; }

            if (valor is IEnumerable<float> floatEnum)
            {
                payload[prop.Name] = floatEnum.Cast<object>().ToList();
            }
            else if (valor is IList list)
            {
                var outList = new List<object>();
                foreach (var it in list) outList.Add(it ?? "");
                payload[prop.Name] = outList;
            }
            else if (valor is string || valor is int || valor is long || valor is double || valor is float || valor is bool)
            {
                payload[prop.Name] = valor;
            }
            else
            {
                payload[prop.Name] = valor.ToString();
            }
        }

        // Añadir documento con ID autogenerado
        db.Collection(coleccion).AddAsync(payload).ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted && !task.IsFaulted)
            {
                Debug.Log("Documento escrito correctamente en Firestore.");
            }
            else
            {
                Debug.LogError("Error al escribir el documento en Firestore: " + (task.Exception != null ? task.Exception.ToString() : "Desconocido"));
            }
        });
    }

   
}
