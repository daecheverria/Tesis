using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CambiarLupa : MonoBehaviour
{
    public Material blanco;
    public Material lupa;
    public void Soltar(){
        GetComponent<Renderer>().material = blanco;
    }
    public void Agarrar(){
        GetComponent<Renderer>().material = lupa;
    }
}
