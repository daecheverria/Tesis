using UnityEngine;

public class asegurarPesa : MonoBehaviour
{
    public Collider colPesa;
    public Collider colResorte;
    public ManejadorPesas manejadorPesas; // Referencia al manejador

    public void Cambiar()
    {
        if (manejadorPesas != null && manejadorPesas.pesaColgada)
        {
            if (colPesa != null && colResorte != null)
            {
                colPesa.enabled = !colPesa.enabled;
                colResorte.enabled = !colResorte.enabled;
            }
        }
    }
}
