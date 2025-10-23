using System.Collections;
using UnityEngine;

public class asegurarPesa : MonoBehaviour
{
    public Collider colPesa;
    public Collider colResorte;
    public ManejadorPesas manejadorPesas; // Referencia al manejador

    public void Asegurar()
    {
        colPesa.enabled = false;
        colResorte.enabled = true;
    }
    public void Desasegurar()
    {
        colPesa.enabled = true;
        colResorte.enabled = false;
    }

}
