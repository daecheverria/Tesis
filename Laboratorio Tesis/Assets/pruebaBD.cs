using UnityEngine;
using Firebase;
using Firebase.Firestore;
using Firebase.Extensions;
using System.Collections.Generic;

public class pruebaBD : MonoBehaviour
{
    void Start()
    {
        // Inicializar Firebase
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            var dependencyStatus = task.Result;
            if (dependencyStatus == DependencyStatus.Available)
            {
                WriteTestData();
            }
            else
            {
                Debug.LogError($"No se pudo resolver todas las dependencias de Firebase: {dependencyStatus}");
            }
        });
    }

    void WriteTestData()
    {
        FirebaseFirestore db = FirebaseFirestore.DefaultInstance;

        // Crear datos de ejemplo
        var data = new Dictionary<string, object>
        {
            { "Nombre", "Ejemplo" },
            { "Lab", true },
            { "Tiempo", 123.45f }
        };

        // Añadir documento con ID autogenerado
        db.Collection("Prueba").AddAsync(data).ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted && !task.IsFaulted)
            {
                Debug.Log("Documento escrito correctamente en Firestore.");
            }
            else
            {
                Debug.LogError("Error al escribir el documento: " + task.Exception);
            }
        });
    }
}
