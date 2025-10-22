using Microsoft.MixedReality.Toolkit.Experimental.UI;
using TMPro;
using UnityEngine;

public class Teclado : MonoBehaviour
{
    private TMP_InputField inputField;
    public float distancia = 0.5f;
    public float altura = -0.5f;
    public Transform camara;
    void Start()
    {
        inputField = GetComponent<TMP_InputField>();
        inputField.onSelect.AddListener(x => OpenKeyboard());
    }
    void OpenKeyboard()
    {
                //TouchScreenKeyboard.Open("", TouchScreenKeyboardType.Default);
        NonNativeKeyboard.Instance.InputField  = inputField;
        NonNativeKeyboard.Instance.PresentKeyboard(inputField.text);

        Vector3  direccion = camara.forward;
        direccion.y = 0;
        direccion.Normalize();
        Vector3 posicionTeclado = camara.position + direccion * distancia + Vector3.up * altura;
        NonNativeKeyboard.Instance.RepositionKeyboard(posicionTeclado);
    }
}
