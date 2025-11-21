using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UserRegister : MonoBehaviour
{
    // Hola, este script lo que hace es guardar los datos en un PlayerPrefs, que es una forma de almacenar datos simples en Unity.
    [Header("BotÛn de registro")]
    [SerializeField] private Button botonIniciarSesion;

    [Header("Campos comunes")]
    [SerializeField] private TMP_InputField _nombre;
    [SerializeField] private TMP_InputField _correoUnimet;
    [SerializeField] private TMP_InputField _cedula;

    private static readonly Regex caracteresPermitidos = new Regex("^[a-zA-ZÒ—0-9 .,!?]+$");
    private static readonly Regex acentosPermitidos = new Regex("^[a-zA-ZÒ—·ÈÌÛ˙¡…Õ”⁄ ]+$");
    private static readonly Regex emailLocalRegex = new Regex("^[A-Za-z0-9._%+-]+$");

    // Verificadores
    private bool verificadorNombre = false;
    private bool verificadorCorreo = false;
    private bool verificadorCedula = false;

    public DatosSO2 datosSO;


    void Awake()
    {
        LimpiarDatos();
    }

    void Update()
    {
       // botonIniciarSesion.interactable = verificadorNombre && verificadorCorreo && verificadorCedula;
    }


    public void LimpiarDatos()
    {
        //botonIniciarSesion.interactable = false;
        _nombre.text = string.Empty;
        _correoUnimet.text = string.Empty;
        _cedula.text = string.Empty;
    }

    public void CapturarNombre()
    {
        print("Nombre capturado: " + _nombre.text);
    }


    public void CapturarCorreoUnimet()
    {
        print("Correo Unimet capturado: " + _correoUnimet.text);
        _correoUnimet.text = _correoUnimet.text.ToLower();
    }

    public void CapturarCedula()
    {
        print("Cedula capturada: " + _cedula.text);
    }

    public bool EsCorreoValidoUnimet(string valor)
    {
        // Normalizar
        if (string.IsNullOrWhiteSpace(valor))
        {
            return false;
        }

        string correo = valor.Trim().ToLower();

        // Debe contener una @ y algo antes y despuÈs
        int atIndex = correo.LastIndexOf('@');
        if (atIndex <= 0 || atIndex >= correo.Length - 1)
        {
            return false;
        }

        string dominio = correo.Substring(atIndex);
        if (dominio != "@correo.unimet.edu.ve" && dominio != "@unimet.edu.ve")
        {
            return false;
        }

        string parteLocal = correo.Substring(0, atIndex);
        if (!emailLocalRegex.IsMatch(parteLocal))
        {
            return false;
        }

        return true;
    }

    public bool EsInputValido(string valor)
    {
        if (string.IsNullOrWhiteSpace(valor) || !caracteresPermitidos.IsMatch(valor))
        {
            return false;
        }

        return true;
    }

    public bool EsNombreValido(string valor)
    {
        if (string.IsNullOrWhiteSpace(valor) || (!acentosPermitidos.IsMatch(valor) && !acentosPermitidos.IsMatch(valor)))
        {
            return false;
        }
        return true;
    }

    public bool EsApellidoValido(string valor)
    {
        if (string.IsNullOrWhiteSpace(valor) || (!acentosPermitidos.IsMatch(valor) && !acentosPermitidos.IsMatch(valor)))
        {
            return false;
        }
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

    public void CambioEscena2()
    {

        datosSO.Reiniciar();
        if (datosSO != null) datosSO.nombre = _nombre.text;
            _correoUnimet.text = _correoUnimet.text.ToLower();
            if (datosSO != null) datosSO.correo = _correoUnimet.text;
            if (datosSO != null) datosSO.cedula = _cedula.text;

        SceneManager.LoadScene("Laboratory Scene");
    }
}