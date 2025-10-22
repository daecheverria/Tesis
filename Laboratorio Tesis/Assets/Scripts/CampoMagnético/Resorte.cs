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
    public float minLength = 0.3f; // Rest length of spring
    public float maxLength = 2f;  // Maximum length of spring
    public float[] aaaaa;
    private LineRenderer lineRenderer;
    private SpringJoint springJoint;

    void Awake()
    {
        if (endPoint == null)
        {
            Debug.LogError("Debes asignar un endPoint en el inspector.");
            return;
        }
        // endPoint.position = startPoint.position + (Vector3.down * restLength);

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
    springJoint.damper = damping;        // amortiguamiento bajo (ej. 1-2)
    springJoint.autoConfigureConnectedAnchor = false;
    springJoint.anchor = Vector3.zero;
    springJoint.connectedAnchor = Vector3.zero;
    springJoint.enableCollision = true;

    springJoint.minDistance = minLength;
    springJoint.maxDistance = maxLength;
}


    void FixedUpdate()
    {
        UpdateSpringVisual();
        //Debug.Log(startPoint.position - endPoint.position);
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