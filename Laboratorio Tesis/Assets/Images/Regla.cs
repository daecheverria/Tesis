using UnityEngine;

public class Regla : MonoBehaviour
{
    [Header("Referencia al objeto que reduce el ancho")]
    [SerializeField] private Transform disminuirSize; // Objeto controlador (posición inicial esperada X=29)


    [Header("Dimensiones iniciales del SpriteRenderer (Tiled)")]
    [SerializeField] private float anchoInicial = 31.11969f;
    [SerializeField] private float altoInicial = 19.93699f;

    [Header("Ajustes")]
    [Tooltip("Factor de reducción: 1 = 1 unidad de movimiento en X reduce 1 unidad de ancho")] 
    [SerializeField] private float factorReduccion = 1f;
    [Tooltip("Ancho mínimo permitido para evitar valores no válidos")] 
    [SerializeField] private float anchoMinimo = 0.01f;

    private SpriteRenderer sr;
    private float xInicialDisminuir;
    private float xBordeDerechoMundo; // Para mantener anclado el borde derecho visualmente

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        if (sr == null)
        {
            Debug.LogError("Regla: No se encontró SpriteRenderer en el mismo objeto.");
            return;
        }

        // Asegurar modo Tiled y tamaño inicial
        if (sr.drawMode != SpriteDrawMode.Tiled)
        {
            sr.drawMode = SpriteDrawMode.Tiled;
        }
        sr.size = new Vector2(anchoInicial, altoInicial);

        // Buscar el objeto por nombre si no fue asignado desde el Inspector
        if (disminuirSize == null)
        {
            GameObject go = GameObject.Find("DisminuirSize");
            if (go != null) disminuirSize = go.transform;
        }

        if (disminuirSize != null)
        {
            xInicialDisminuir = disminuirSize.position.x; // normalmente 29
        }
        else
        {
            Debug.LogWarning("Regla: No se asignó 'disminuirSize'. Se puede asignar por Inspector o nombrar el objeto 'Disminuirsize'.");
        }

        // Guardar el borde derecho actual para mantenerlo fijo visualmente
        xBordeDerechoMundo = transform.position.x + (sr.size.x * 0.5f);
        // Corregir posición inicial por si el anchoInicial no coincide con el actual
        AjustarPosicionParaBordeDerecho(sr.size.x);
    }

    private void Update()
    {
        if (sr == null || disminuirSize == null) return;

        // Solo reducir cuando el controlador se mueve a la izquierda de su posición inicial
        float deltaIzquierda = Mathf.Max(0f, xInicialDisminuir - disminuirSize.position.x);
        float nuevoAncho = Mathf.Max(anchoMinimo, anchoInicial - deltaIzquierda * factorReduccion);

        if (!Mathf.Approximately(nuevoAncho, sr.size.x))
        {
            // Aplicar nuevo ancho manteniendo el alto
            sr.size = new Vector2(nuevoAncho, sr.size.y);
            // Mantener anclado el borde derecho (visual sin movimiento)
            AjustarPosicionParaBordeDerecho(nuevoAncho);
        }
    }

    // Mantiene el borde derecho en xBordeDerechoMundo ajustando la posición del objeto.
    private void AjustarPosicionParaBordeDerecho(float ancho)
    {
        Vector3 p = transform.position;
        p.x = xBordeDerechoMundo - (ancho * 0.5f);
        transform.position = p;
    }

    // Permite restaurar el ancho original y recalcular el anclaje del borde derecho
    public void ReiniciarRegla()
    {
        if (sr == null) return;
        sr.size = new Vector2(anchoInicial, altoInicial);
        xBordeDerechoMundo = transform.position.x + (sr.size.x * 0.5f);
        AjustarPosicionParaBordeDerecho(sr.size.x);
        if (disminuirSize != null) xInicialDisminuir = disminuirSize.position.x;
    }
}
