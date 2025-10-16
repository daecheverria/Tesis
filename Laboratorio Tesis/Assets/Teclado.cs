using Microsoft.MixedReality.Toolkit.Experimental.UI;
using TMPro;
using UnityEngine;

public class Teclado : MonoBehaviour
{
    private TMP_InputField inputField;
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
    }
}
