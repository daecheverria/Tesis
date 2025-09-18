using System;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class ManejadorPesas : MonoBehaviour
{
    private XRSocketInteractor socket;
    public SpringJoint springJoint; // Arrastra tu SpringJoint aquí desde el Inspector
    public Rigidbody selfRb;
    private float initialMass = 0.023f;

    void Start()
    {
        socket = GetComponent<XRSocketInteractor>();
        socket.selectEntered.AddListener(OnPesaAttached);
        socket.selectExited.AddListener(OnPesaDetached);
        selfRb.sleepThreshold = 0.0f; // Evita que el Rigidbody entre en modo de sueño

        // Al inicio, conecta el springJoint al propio objeto
        if (springJoint != null)
        {
            springJoint.connectedBody = selfRb;
        }
    }

    private void OnPesaAttached(SelectEnterEventArgs args)
    {
        // El objeto que acaba de ser agarrado por el socket
        Rigidbody pesaRigidbody = args.interactableObject.transform.GetComponent<Rigidbody>();
        Debug.Log("Pesa agarrada: " + pesaRigidbody.name);
        Debug.Log(selfRb);
        selfRb.mass = pesaRigidbody.mass + initialMass; // Suma la masa de la pesa a la del objeto con el socket
        Debug.Log("Masa actual del objeto con socket: " + selfRb.mass);
        Debug.Log("Masa de la pesa añadida: " + pesaRigidbody.mass);
        selfRb.WakeUp(); // Asegura que el Rigidbody esté activo
        // if (pesaRigidbody != null && springJoint != null)
        // {
        //     // Conecta el Rigidbody de la pesa al joint del resorte
        //     springJoint.connectedBody = pesaRigidbody;
        // }
    }

    private void OnPesaDetached(SelectExitEventArgs args)
    {
        selfRb.mass = initialMass; // Resta la masa de la pesa al objeto con el socket
        selfRb.WakeUp(); 
        Debug.Log("Masa actual del objeto con socket: " + selfRb.mass);
        // Si se quita la pesa, conecta el joint al propio objeto
        // if (springJoint != null)
        // {
        //     Rigidbody selfRb = GetComponent<Rigidbody>();
        //     springJoint.connectedBody = selfRb;
        // }
    }
}
