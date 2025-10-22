using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Text.RegularExpressions;

public class UserRegister : MonoBehaviour
{
    // Hola, este script lo que hace es guardar los datos en un PlayerPrefs, que es una forma de almacenar datos simples en Unity.
    [Header("BotÛn de registro")]
    [SerializeField] private Button botonIniciarSesion;

    [Header("Campos comunes")]
    [SerializeField] private TMP_InputField _nombre;
    [SerializeField] private TMP_InputField _correoUnimet;
    [SerializeField] private TMP_InputField _cedula;

    [Header("Panel de errores")]
    [SerializeField] private GameObject errorCorreoUnimet;
    [SerializeField] private GameObject errorGeneral;


    private static readonly Regex caracteresPermitidos = new Regex("^[a-zA-ZÒ—0-9 .,!?]+$");
    private static readonly Regex acentosPermitidos = new Regex("^[a-zA-ZÒ—·ÈÌÛ˙¡…Õ”⁄ ]+$");

    // Verificadores
    private bool verificadorNombre = false;
    private bool verificadorCorreo = false;
    private bool verificadorCedula = false;


    void Awake()
    {
        LimpiarDatos();
    }

    void Update()
    {
        botonIniciarSesion.interactable = verificadorNombre && verificadorCorreo && verificadorCedula;
    }


    public void LimpiarDatos()
    {
        botonIniciarSesion.interactable = false;
        _nombre.text = string.Empty;
        _correoUnimet.text = string.Empty;
        _cedula.text = string.Empty;
    }

    public void CapturarNombre()
    {
        ValidarTodo();
    }


    public void CapturarCorreoUnimet()
    {
        _correoUnimet.text = _correoUnimet.text.ToLower();
        ValidarTodo();
    }

    public void CapturarCedula()
    {
        ValidarTodo();
    }

    public bool EsCorreoValidoUnimet(string valor)
    {
        if (string.IsNullOrWhiteSpace(valor) || (!valor.Contains("@correo.unimet.edu.ve") && !valor.Contains("@unimet.edu.ve")))
        {
            errorCorreoUnimet.SetActive(true);
            return false;
        }
        errorCorreoUnimet.SetActive(false);
        return true;
    }

    public bool EsInputValido(string valor)
    {
        if (string.IsNullOrWhiteSpace(valor) || !caracteresPermitidos.IsMatch(valor))
        {
            errorGeneral.SetActive(true);
            return false;
        }
        errorGeneral.SetActive(false);
        return true;
    }

    public bool EsNombreValido(string valor)
    {
        if (string.IsNullOrWhiteSpace(valor) || (!acentosPermitidos.IsMatch(valor) && !acentosPermitidos.IsMatch(valor)))
        {
            errorGeneral.SetActive(true);
            return false;
        }
        errorGeneral.SetActive(false);
        return true;
    }

    public bool EsApellidoValido(string valor)
    {
        if (string.IsNullOrWhiteSpace(valor) || (!acentosPermitidos.IsMatch(valor) && !acentosPermitidos.IsMatch(valor)))
        {
            errorGeneral.SetActive(true);
            return false;
        }
        errorGeneral.SetActive(false);
        return true;
    }

    public bool EsCedulaValida(string valor)
    {
        if (string.IsNullOrWhiteSpace(valor) || !EsSoloNumeros(valor))
        {
            return false;
        }
        return true;
    }


    private bool EsSoloNumeros(string valor)
    {
        foreach (char c in valor)
        {
            if (!char.IsDigit(c))
                return false;
        }
        return true;
    }

    public void ValidarTodo()
    {

        verificadorNombre = EsNombreValido(_nombre.text);
        if (verificadorNombre)
        {
            PlayerPrefs.SetString("name", _nombre.text);
        }

        verificadorCorreo = EsCorreoValidoUnimet(_correoUnimet.text);
        if (verificadorCorreo)
        {
            _correoUnimet.text = _correoUnimet.text.ToLower();
            PlayerPrefs.SetString("email", _correoUnimet.text);
        }

        verificadorCedula = EsCedulaValida(_cedula.text);
        if (verificadorCedula)
        {
            PlayerPrefs.SetString("cedula", _cedula.text);
        }

        // Guardado y botÛn
        PlayerPrefs.Save();
    }
}