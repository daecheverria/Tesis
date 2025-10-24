using UnityEngine;

public class Resorte : MonoBehaviour
{
    public Transform startPoint;
    public Transform endPoint;
    [HideInInspector] public GameObject generatedEndPoint;
    public Material springMaterial;
    public float radius = 0.2f;
    public int coils = 6;
    public int segmentsPerCoil = 12;
    [Range(0.1f, 5f)] public float compressionFactor = 2f;
    public float springConstant = 1f; // N/m
    public float damping = 0.001f; // Damping for stability
    public float minLength = 0f; // Rest length of spring
    public float maxLength = 0f;  // Maximum length of spring
    private LineRenderer lineRenderer;
    private SpringJoint springJoint;
    public DatosSO2 datosSO;

    [Header("Depuración de equilibrio")]
    public float settleVelocityThreshold = 0.02f; // velocidad bajo la cual consideramos "casi en reposo"
    public float settleTime = 0.4f; // tiempo que debe permanecer con velocidad baja para considerar asentado
    public bool logWhenSettled = true;

    private float settleTimer = 0f;

    void Awake()
    {
        if (endPoint == null)
        {
            Debug.LogError("Debes asignar un endPoint en el inspector.");
            return;
        }

        // Initialize LineRenderer
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.material = springMaterial;
        lineRenderer.textureMode = LineTextureMode.Tile;
        lineRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        lineRenderer.receiveShadows = false;
        lineRenderer.widthMultiplier = radius * 2;
        lineRenderer.loop = false;

        // Setup physics
        SetupSpringJoint();
        UpdateSpringVisual();
    }

    void SetupSpringJoint()
    {
        Rigidbody startRb = startPoint.GetComponent<Rigidbody>();
        if (!startRb) startRb = startPoint.gameObject.AddComponent<Rigidbody>();
        startRb.isKinematic = true; // Fijo

        Rigidbody endRb = endPoint.GetComponent<Rigidbody>();
        if (!endRb) endRb = endPoint.gameObject.AddComponent<Rigidbody>();

        springJoint = startPoint.gameObject.AddComponent<SpringJoint>();
        springJoint.connectedBody = endRb;
        springJoint.spring = springConstant; // fuerza de restitución
        springJoint.damper = damping;        // amortiguamiento
        springJoint.autoConfigureConnectedAnchor = false;
        springJoint.anchor = Vector3.zero;
        springJoint.connectedAnchor = Vector3.zero;
        springJoint.enableCollision = true;

        springJoint.minDistance = minLength;
        springJoint.maxDistance = maxLength;

        if (datosSO != null) datosSO.k = springConstant;
    }

    void FixedUpdate()
    {
        UpdateSpringVisual();

        if (springJoint == null || springJoint.connectedBody == null) return;

        Rigidbody endRb = springJoint.connectedBody;
        float distancia = Vector3.Distance(startPoint.position, endPoint.position); // distancia recta entre puntos
        float m = endRb.mass; // masa efectiva conectada al muelle
        float k = springJoint.spring;
        float g = Mathf.Abs(Physics.gravity.y);

        // estiramiento teórico (equilibrio estático): x = m * g / k
        float estiramientoTeorico = (k > 0f) ? (m * g / k) : 0f;
        float diff = distancia - estiramientoTeorico;

        // consideramos asentado cuando la velocidad es baja durante un tiempo
        if (endRb.linearVelocity.magnitude < settleVelocityThreshold)
        {
            settleTimer += Time.fixedDeltaTime;
            if (settleTimer >= settleTime)
            {
                if (logWhenSettled)
                {
                    Debug.Log($"Resorte (ASENTADO): distancia={distancia:F4} m | teorico x=mg/k={estiramientoTeorico:F4} m | diff={diff:F4} m | m={m:F4} kg k={k:F4} N/m g={g:F2} m/s^2 | vel={endRb.linearVelocity.magnitude:F4} m/s");
                }
            }
        }
        else
        {
            // todavía oscilando
            settleTimer = 0f;
            // opcional: log en oscilación si quieres trazar
            // Debug.Log($"Resorte (OSCI): distancia={distancia:F4} vel={endRb.velocity.magnitude:F4}");
        }
    }

    void UpdateSpringVisual()
    {
        int totalSegments = coils * segmentsPerCoil + 1;
        lineRenderer.positionCount = totalSegments;

        Vector3 start = startPoint.position;
        Vector3 end = endPoint.position;
        Vector3 direction = (end - start).normalized;

        Vector3 up = Vector3.Cross(direction, Vector3.right).normalized;
        if (up == Vector3.zero) up = Vector3.Cross(direction, Vector3.up).normalized;
        Vector3 right = Vector3.Cross(direction, up).normalized;

        for (int i = 0; i < totalSegments; i++)
        {
            float t = i / (float)(totalSegments - 1);
            float angle = t * coils * Mathf.PI * 2f * compressionFactor;
            Vector3 linearPos = Vector3.Lerp(start, end, t);
            Vector3 offset = up * Mathf.Sin(angle) * radius + right * Mathf.Cos(angle) * radius;
            lineRenderer.SetPosition(i, linearPos + offset);
        }

        lineRenderer.material.mainTextureScale = new Vector2(1, coils * 2);
    }
}