using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class asegurarPesa : MonoBehaviour
{
    private Collider colPesa;
    public Collider colResorte;
    public ManejadorPesas manejadorPesas; // Referencia al manejador
    public XRSocketInteractor socket;

    public void Asegurar(Collider colPesa)
    {
        colPesa.enabled = false;
        colResorte.enabled = true;
    }
    public void Desasegurar()
    {
        colPesa = manejadorPesas.pesaCol;
        colPesa.enabled = true;
        colResorte.enabled = false;
        StartCoroutine(TemporarilyDisableSocket(0.2f));
        
    }
    private IEnumerator TemporarilyDisableSocket(float delay)
    {
        // Desactiva el componente para evitar que vuelva a enganchar inmediatamente
        socket.enabled = false;
        yield return new WaitForSeconds(delay);
        socket.enabled = true;
    }
}
