using System;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;


public class HundirBoton : MonoBehaviour
{
    private XRBaseInteractable interactuable;
    private bool siguiendo = false;
    public Transform objetivo;
    private Vector3 offset;
    public Vector3 ejeLocal;
    private Transform pokeTransform;
    private Vector3 initialPosition;
    public float velocidad = 5f;
    private bool congelar = false;
    public float angulo;
    public float umbralHundir = 0.01f;
    private bool eventoActivado = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        initialPosition = objetivo.localPosition;
        interactuable = GetComponent<XRBaseInteractable>();
        interactuable.hoverEntered.AddListener(Seguir);
        interactuable.hoverExited.AddListener(Reiniciar);
        interactuable.selectEntered.AddListener(Congelar);
    }

    public void Seguir(BaseInteractionEventArgs args)
    {
        Debug.Log("Seguir");
        if (args.interactorObject is XRPokeInteractor)
        {
            Debug.Log("Seguir2");
            XRPokeInteractor poke = (XRPokeInteractor)args.interactorObject;
            pokeTransform = poke.attachTransform;
            offset = objetivo.position - pokeTransform.position;
            float pokeAngle = Vector3.Angle(offset, objetivo.TransformDirection(ejeLocal));
            if (pokeAngle > angulo)
            {
                Debug.Log("Seguir3");
                siguiendo = true;
                congelar = false;
            }
        }
    }
    public void Reiniciar(BaseInteractionEventArgs args)
    {
        if (args.interactorObject is XRPokeInteractor)
        {
            siguiendo = false;
            congelar = false;
        }
    }
    public void Congelar(BaseInteractionEventArgs args)
    {
        if (args.interactorObject is XRPokeInteractor)
        {
            congelar = true;
        }
    }
    // Update is called once per frame
    void Update()
    {
        if (congelar) return;
        if (siguiendo)
        {
            Vector3 posicionLocalObjetivo = objetivo.InverseTransformPoint(pokeTransform.position + offset);
            Vector3 constrainLocal = Vector3.Project(posicionLocalObjetivo, ejeLocal);
            objetivo.position = objetivo.TransformPoint(constrainLocal);
            float desplazamiento = Vector3.Dot(objetivo.localPosition - initialPosition, ejeLocal.normalized);
            if (!eventoActivado && desplazamiento > umbralHundir)
            {
                OnBotonHundido();
                eventoActivado = true;
            }
            else if (eventoActivado && desplazamiento < umbralHundir * 0.5f)
            {
                eventoActivado = false;
            }
        }
        else
        {
            objetivo.localPosition = Vector3.Lerp(objetivo.localPosition, initialPosition, Time.deltaTime * velocidad);
        }
    }
    private void OnBotonHundido()
    {
        // Intercambiar rb pesa
        Debug.Log("¡Botón hundido!");
    }
}
