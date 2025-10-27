using System.Collections;
using UnityEngine;

public class asegurarPesa : MonoBehaviour
{
    private Collider colPesa;
    public Collider colResorte;
    public ManejadorPesas manejadorPesas; // Referencia al manejador

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
    }

}
