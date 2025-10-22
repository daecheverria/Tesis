using UnityEngine;
using UnityEngine.SceneManagement;

public class CambioEscena : MonoBehaviour
{
    [Tooltip("Nombre de la escena a cargar (debe estar en Build Settings)")]
    public string escenaObjetivo;

    [Tooltip("Si es true, solo reaccionará a colliders con el tag especificado")]
    public bool requerirTag = true;

    [Tooltip("Tag requerido si 'requerirTag' es true")]
    public string tagRequerido = "Player";

    [Tooltip("Usar carga asíncrona")]
    public bool cargaAsincrona = true;

    [Tooltip("Si es true, solo cargará la escena una vez")]
    public bool soloUnaVez = true;

    private bool yaCargando = false;

    private void OnTriggerEnter(Collider other)
    {
        if (yaCargando) return;

        if (string.IsNullOrEmpty(escenaObjetivo))
        {
            Debug.LogWarning("CambioEscena: 'escenaObjetivo' no está asignada.");
            return;
        }

        if (requerirTag && !other.CompareTag(tagRequerido)) return;

        if (cargaAsincrona)
        {
            yaCargando = true;
            AsyncOperation op = SceneManager.LoadSceneAsync(escenaObjetivo);
            if (soloUnaVez)
                op.completed += _ => yaCargando = true;
            else
                op.completed += _ => yaCargando = false;
        }
        else
        {
            yaCargando = true;
            SceneManager.LoadScene(escenaObjetivo);
        }

        Debug.Log($"CambioEscena: cargando escena '{escenaObjetivo}' (trigger por '{other.name}').");
    }
}
