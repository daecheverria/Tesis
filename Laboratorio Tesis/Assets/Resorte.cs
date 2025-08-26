using UnityEngine;

public class Resorte : MonoBehaviour
{
    public Transform startPoint;
    public Transform endPoint;
    public Material springMaterial;
    public float radius = 0.2f;
    public int coils = 6;
    public int segmentsPerCoil = 12;
    [Range(0.1f, 5f)] public float compressionFactor = 2f; // Controla qué tan comprimido se ve

    private LineRenderer lineRenderer;

    void Start()
    {
        lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.material = springMaterial;
        lineRenderer.textureMode = LineTextureMode.Tile;
        lineRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        lineRenderer.receiveShadows = false;
        lineRenderer.widthMultiplier = radius * 2;
        lineRenderer.loop = false;
        UpdateSpringVisual();
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
        float length = Vector3.Distance(start, end);

        // Crear ejes perpendiculares
        Vector3 up = Vector3.Cross(direction, Vector3.right).normalized;
        if (up == Vector3.zero) up = Vector3.Cross(direction, Vector3.up).normalized;
        Vector3 right = Vector3.Cross(direction, up).normalized;

        for (int i = 0; i < totalSegments; i++)
        {
            float t = i / (float)(totalSegments - 1);

            // Distribuir las vueltas comprimidas de manera uniforme
            float angle = t * coils * Mathf.PI * 2f * compressionFactor;

            Vector3 linearPos = Vector3.Lerp(start, end, t);

            // Mantener radio constante en todo el resorte
            Vector3 offset = up * Mathf.Sin(angle) * radius +
                            right * Mathf.Cos(angle) * radius;

            lineRenderer.SetPosition(i, linearPos + offset);
        }

        // Ajustar la textura para que coincida con las bobinas
        lineRenderer.material.mainTextureScale = new Vector2(1, coils * 2);
    }

}