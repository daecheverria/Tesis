using UnityEngine;
using UnityEngine.SceneManagement;

public class ManagerResorte : MonoBehaviour
{
    public void RecargarEscena()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    public void EnviarCorreo()
    {
        print("Enviando correo...");
        EnviarCorreo enviarCorreo = GetComponent<EnviarCorreo>();
        enviarCorreo.SendResultsFromDatosSO();
    }
}
