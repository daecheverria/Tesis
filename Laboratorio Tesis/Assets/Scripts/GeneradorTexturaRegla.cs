using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class GeneradorTexturaRegla : MonoBehaviour
{
    [Header("Configuración de Textura")]
    [Tooltip("El alto en píxeles de la textura. Más alto = más definición.")]
    public int altoTextura = 1024; // 1024 píxeles representarán 1 metro

    [Tooltip("El ancho en píxeles de la textura (el ancho de la cinta).")]
    public int anchoTextura = 128;

    [Header("Colores")]
    public Color colorFondo = new Color(1f, 0.9f, 0.6f); // Amarillo pálido
    public Color colorMarca = Color.black;

    void Awake()
    {
        // 1. Crear la textura en memoria
        Texture2D tex = new Texture2D(anchoTextura, altoTextura, TextureFormat.RGBA32, false);
        tex.wrapMode = TextureWrapMode.Repeat; // ¡Crucial!
        tex.filterMode = FilterMode.Bilinear;

        // 2. Llenar el fondo
        Color32[] pixels = new Color32[anchoTextura * altoTextura];
        Color32 fondo32 = colorFondo;
        for (int i = 0; i < pixels.Length; i++)
        {
            pixels[i] = fondo32;
        }

        // 3. Dibujar las marcas (de 0 a 1000 milímetros)
        Color32 marca32 = colorMarca;

        for (int mm = 0; mm < 1000; mm++) // 1000 mm = 1 metro
        {
            // Convertir milímetro a posición 'y' en la textura
            int y = (int)((mm / 1000.0f) * altoTextura);

            int largoMarcaX;    // Qué tan larga es la línea (de borde a borde)
            int grosorMarcaY = 1; // Qué tan gruesa es la línea (en píxeles)

            if (mm % 100 == 0) // Marca de 10 cm (0, 10, 20...)
            {
                largoMarcaX = anchoTextura / 2; // Ocupa todo el ancho
                grosorMarcaY = 3; // Línea gruesa
            }
            else if (mm % 50 == 0) // Marca de 5 cm (5, 15...)
            {
                largoMarcaX = (int)(anchoTextura * 0.4f); // 40% desde el borde
                grosorMarcaY = 2;
            }
            else if (mm % 10 == 0) // Marca de 1 cm (1, 2, 3...)
            {
                largoMarcaX = (int)(anchoTextura * 0.3f); // 30% desde el borde
                grosorMarcaY = 2;
            }
            else // Marca de 1 mm
            {
                largoMarcaX = (int)(anchoTextura * 0.15f); // 15% desde el borde
                grosorMarcaY = 1;
            }

            // Dibujar los píxeles de la marca
            for (int x = 0; x < largoMarcaX; x++)
            {
                for (int dy = 0; dy < grosorMarcaY; dy++)
                {
                    if (y + dy >= altoTextura) continue; // Evitar salirse

                    // Dibujar desde la izquierda
                    int pixelIndexL = (y + dy) * anchoTextura + x;
                    pixels[pixelIndexL] = marca32;

                    // Dibujar desde la derecha
                    int pixelIndexR = (y + dy) * anchoTextura + (anchoTextura - 1 - x);
                    pixels[pixelIndexR] = marca32;
                }
            }
        }

        // 4. Aplicar píxeles a la textura y asignarla al material
        tex.SetPixels32(pixels);
        tex.Apply();

        // Asignar la textura generada al material de este objeto
        Renderer rend = GetComponent<Renderer>();
        rend.material.mainTexture = tex;
    }
}