using UnityEngine;
using Firebase;
using Firebase.Firestore;
using Firebase.Extensions;
using System.Collections.Generic;

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

        // Preparar diccionario con los campos
        var payload = new Dictionary<string, object>
        {
            { "Nombre", datosSO.nombre ?? string.Empty },
            { "Correo", datosSO.correo ?? string.Empty },
            { "Cedula", datosSO.cedula ?? string.Empty },
            { "Tiempos", datosSO.tiempos ?? new List<float>() },
            { "Distancias", datosSO.distancias ?? new List<float>() }
        };

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

    // Variante: enviar a un documento con id especificado (sobrescribe/crea)
    public void EnviarDatosConId(string documentId)
    {
        if (string.IsNullOrEmpty(documentId))
        {
            Debug.LogWarning("datosBD.EnviarDatosConId: documentId inválido.");
            return;
        }
        if (datosSO == null)
        {
            Debug.LogWarning("datosBD.EnviarDatosConId: 'datosSO' no está asignado.");
            return;
        }

        FirebaseFirestore db = FirebaseFirestore.DefaultInstance;
        var payload = new Dictionary<string, object>
        {
            { "Nombre", datosSO.nombre ?? string.Empty },
            { "Correo", datosSO.correo ?? string.Empty },
            { "Cedula", datosSO.cedula ?? string.Empty },
            { "Tiempos", datosSO.tiempos ?? new List<float>() },
            { "Distancias", datosSO.distancias ?? new List<float>() }
        };

        db.Collection(coleccion).Document(documentId).SetAsync(payload).ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted && !task.IsFaulted)
            {
                Debug.Log($"Documento '{documentId}' guardado en la colección '{coleccion}'.");
            }
            else
            {
                Debug.LogError("Error al guardar el documento en Firestore: " + (task.Exception != null ? task.Exception.ToString() : "Desconocido"));
            }
        });
    }
}
