using UnityEngine;

public class Resorte : MonoBehaviour
{
    public Transform startPoint;
    public Transform endPoint;
    public Material springMaterial;
    public float radius = 0.2f;
    public int coils = 6;
    public int segmentsPerCoil = 12;
    [Range(0.1f, 5f)] public float compressionFactor = 2f;
    public float springConstant = 50f; // N/m
    public float damping = 10f; // Damping for stability
    public float restLength = 1f; // Rest length of spring

    private LineRenderer lineRenderer;
    private SpringJoint springJoint;

    void Start()
    {
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
    springJoint.tolerance = restLength;  // longitud de equilibrio
    springJoint.autoConfigureConnectedAnchor = false;
    springJoint.anchor = Vector3.zero;
    springJoint.connectedAnchor = Vector3.zero;

    // Desactivar min/max para que se comporte como resorte real
    springJoint.minDistance = 0f;
    springJoint.maxDistance = 0f;
}


    void Update()
    {
        UpdateSpringVisual();
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