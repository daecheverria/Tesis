using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public GameObject panelPausa;

    public void Pausar()
    {
        Time.timeScale = 0f;
        panelPausa.SetActive(true);
    }
    public void Reanudar()
    {
        Time.timeScale = 1f;
        panelPausa.SetActive(false);
    }
    public void Salir()
    {
        Application.Quit();
    }
    public void Reiniciar()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }
}
