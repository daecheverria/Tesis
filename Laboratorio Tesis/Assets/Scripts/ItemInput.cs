using System.Globalization;
using System.Collections.Generic;
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
            // Añadir listener directamente (firma compatible)
            inputField.onValueChanged.AddListener(OnValueChanged);
            //inputField.onSelect.AddListener(OnSelect);
            if (listaNombre == "Tiempos")
            {
                if (datosSO.tiempos != null) inputField.text = datosSO.tiempos[indiceEnLista].ToString(CultureInfo.InvariantCulture);

            }
            else if (listaNombre == "Distancias")
            {
                if (datosSO.distancias != null) inputField.text = datosSO.distancias[indiceEnLista].ToString(CultureInfo.InvariantCulture);

            }
            //Debug.Log($"InputItem: Listener añadido a '{inputField.name}' para lista '{listaNombre}' índice {indiceEnLista}");
        }
        else
        {
            Debug.LogWarning("InputItem: inputField no está asignado en el inspector.");
        }
    }

    //private void OnSelect(string texto)
    //{
    //    //Debug.Log($"InputItem: OnSelect llamado con '{texto}'");
    //    inputField.text = "";
    //}
    private void OnValueChanged(string texto)
    {
        Debug.Log($"InputItem: OnValueChanged llamado con '{texto}'");

        if (datosSO == null)
        {
            Debug.LogWarning("InputItem: datosSO no asignado.");
            return;
        }
        if (indiceEnLista < 0)
        {
            Debug.LogWarning("InputItem: indiceEnLista es negativo.");
            return;
        }

        float valor = ParseFloatOrZero(texto);
        //Debug.Log($"InputItem: Valor convertido a float: {valor}");

        try
        {
            if (listaNombre == "Tiempos")
            {
                if (datosSO.tiempos == null) datosSO.tiempos = new List<float>();
                EnsureListSize(datosSO.tiempos, indiceEnLista + 1, 0f);
                datosSO.tiempos[indiceEnLista] = valor;
                //Debug.Log($"InputItem: Tiempos[{indiceEnLista}] = {valor}");
            }
            else if (listaNombre == "Distancias")
            {
                if (datosSO.distancias == null) datosSO.distancias = new List<float>();
                EnsureListSize(datosSO.distancias, indiceEnLista + 1, 0f);
                datosSO.distancias[indiceEnLista] = valor;
                //Debug.Log($"InputItem: Distancias[{indiceEnLista}] = {valor}");
            }
            else
            {
                Debug.LogWarning($"InputItem: listaNombre '{listaNombre}' no reconocida. Usa 'Tiempos' o 'Distancias'.");
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"InputItem: Excepción al actualizar la lista: {ex}");
        }
    }

    // Intenta convertir texto a float. Devuelve 0 si la conversión falla.
    private float ParseFloatOrZero(string texto)
    {
        if (string.IsNullOrWhiteSpace(texto)) return 0f;

        // Intentar con InvariantCulture (punto decimal)
        if (float.TryParse(texto, System.Globalization.NumberStyles.Float | System.Globalization.NumberStyles.AllowThousands, CultureInfo.InvariantCulture, out float resultado))
            return resultado;

        // Intentar con la cultura actual (coma o punto según configuración)
        if (float.TryParse(texto, System.Globalization.NumberStyles.Float | System.Globalization.NumberStyles.AllowThousands, CultureInfo.CurrentCulture, out resultado))
            return resultado;
        inputField.text = "0";
        return 0f;
    }

    private void EnsureListSize<T>(List<T> list, int size, T defaultValue)
    {
        while (list.Count < size) list.Add(defaultValue);
    }
}