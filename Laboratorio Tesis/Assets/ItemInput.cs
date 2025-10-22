using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InputItem : MonoBehaviour
{
    public TMP_InputField inputField;
    public DatosSO2 datosSO;
    public int indiceEnLista;
    public string listaNombre;

    void Start()
    {
        if (inputField != null)
        {
            inputField.onEndEdit.AddListener(delegate { OnValueChanged(inputField.text); });
        }
    }

    // Función llamada cuando el InputField termina de editarse
    private void OnValueChanged(string texto)
    {
        if (datosSO == null) return;
        if (indiceEnLista < 0) return;

        float valor = ParseFloatOrZero(texto);

        if (listaNombre == "tiempos")
        {
            //datosSO.tiempos[indiceEnLista] = valor;
        }
        else if (listaNombre == "distancias")
        {
            //datosSO.distancias[indiceEnLista] = valor;
        }
    }

    // Intenta convertir texto a float. Devuelve 0 si la conversión falla.
    private float ParseFloatOrZero(string texto)
    {
        if (string.IsNullOrWhiteSpace(texto)) return 0f;

        // Intentar con InvariantCulture (punto decimal)
        if (float.TryParse(texto, NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.InvariantCulture, out float resultado))
            return resultado;

        // Intentar con la cultura actual (coma o punto según configuración)
        if (float.TryParse(texto, NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.CurrentCulture, out resultado))
            return resultado;

        return 0f;
    }
}