using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ╔══════════════════════════════════════════════════════════╗
/// ║  CRTEffect  —  Filtro CRT por Canvas, sin shaders        ║
/// ╠══════════════════════════════════════════════════════════╣
/// ║  • Funciona en WebGL y PC                                ║
/// ║  • Genera la textura de scanlines por código             ║
/// ║  • Controla opacidad desde OpcionesFF7                   ║
/// ╠══════════════════════════════════════════════════════════╣
/// ║  SETUP:                                                  ║
/// ║  1. Añade este script a un GameObject vacío              ║
/// ║     que esté en DontDestroyOnLoad (o en cada escena)     ║
/// ║  2. Llama CRTEffect.instancia.SetIntensidad(0-1)         ║
/// ║     desde OpcionesFF7                                    ║
/// ╚══════════════════════════════════════════════════════════╝
/// </summary>
public class CRTEffect : MonoBehaviour
{
    public static CRTEffect instancia;

    [Range(0f, 1f)]
    [Tooltip("0 = sin efecto, 1 = máximo")]
    public float intensidad = 0.35f;

    [Tooltip("Grosor de cada línea oscura en píxeles")]
    public int grosorLinea = 1;

    [Tooltip("Cada cuántos píxeles aparece una línea (2 = una línea de cada 2)")]
    public int frecuencia = 2;

    // ── Referencias internas ──────────────────────────────────────────────────
    Canvas          _canvas;
    RawImage        _rawImage;
    Texture2D       _textura;

    // ═════════════════════════════════════════════════════════════════════════
    void Awake()
    {
        // Singleton
        if (instancia != null && instancia != this) { Destroy(gameObject); return; }
        instancia = this;
        DontDestroyOnLoad(gameObject);

        ConstruirOverlay();
        GenerarTextura();
        AplicarIntensidad(intensidad);

        // Cargar preferencia guardada
        float saved = PlayerPrefs.GetFloat("crt_intensidad", intensidad);
        SetIntensidad(saved);
    }

    // ═════════════════════════════════════════════════════════════════════════
    // API PÚBLICA
    // ═════════════════════════════════════════════════════════════════════════

    /// <summary>Cambia la intensidad del filtro (0 = off, 1 = máximo)</summary>
    public void SetIntensidad(float valor)
    {
        intensidad = Mathf.Clamp01(valor);
        PlayerPrefs.SetFloat("crt_intensidad", intensidad);
        AplicarIntensidad(intensidad);
    }

    public float GetIntensidad() => intensidad;

    // ═════════════════════════════════════════════════════════════════════════
    // CONSTRUCCIÓN
    // ═════════════════════════════════════════════════════════════════════════

    void ConstruirOverlay()
    {
        // Canvas encima de todo
        var cgo = new GameObject("CRT_Canvas");
        cgo.transform.SetParent(transform);
        _canvas = cgo.AddComponent<Canvas>();
        _canvas.renderMode   = RenderMode.ScreenSpaceOverlay;
        _canvas.sortingOrder = 999; // siempre encima

        var cs = cgo.AddComponent<UnityEngine.UI.CanvasScaler>();
        cs.uiScaleMode = UnityEngine.UI.CanvasScaler.ScaleMode.ScaleWithScreenSize;
        cs.referenceResolution = new Vector2(1366, 768);

        // RawImage que ocupa toda la pantalla
        var go = new GameObject("Scanlines", typeof(RectTransform));
        go.transform.SetParent(cgo.transform, false);
        var rt = go.GetComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;

        _rawImage = go.AddComponent<RawImage>();
        _rawImage.raycastTarget = false; // no bloquea clicks
    }

    void GenerarTextura()
    {
        // Textura pequeña que se repite (tiling)
        // Alto = frecuencia píxeles, ancho = 1 (se estira horizontalmente)
        int alto = frecuencia;
        int ancho = 1;

        _textura = new Texture2D(ancho, alto, TextureFormat.RGBA32, false);
        _textura.filterMode = FilterMode.Point; // pixel art, sin blur
        _textura.wrapMode   = TextureWrapMode.Repeat;

        for (int y = 0; y < alto; y++)
        {
            // Las primeras grosorLinea filas son oscuras, el resto transparentes
            Color c = (y < grosorLinea)
                ? new Color(0f, 0f, 0f, 1f)   // línea oscura
                : new Color(0f, 0f, 0f, 0f);  // transparente
            _textura.SetPixel(0, y, c);
        }
        _textura.Apply();

        _rawImage.texture = _textura;

        // Tiling: repetir la textura para cubrir toda la pantalla
        // uvRect controla cuántas veces se repite
        ActualizarTiling();
    }

    void ActualizarTiling()
    {
        // Repetir la textura tantas veces como píxeles de pantalla / frecuencia
        float repeticionesY = Screen.height / (float)frecuencia;
        _rawImage.uvRect = new Rect(0, 0, 1, repeticionesY);
    }

    void AplicarIntensidad(float valor)
    {
        if (_rawImage == null) return;

        if (valor <= 0.001f)
        {
            _rawImage.enabled = false;
            return;
        }

        _rawImage.enabled = true;
        // La alpha de la imagen controla cuánto se ven las scanlines
        _rawImage.color = new Color(0f, 0f, 0f, valor);
    }

    void Update()
    {
        // Actualizar tiling si cambia el tamaño de pantalla (redimensión ventana)
        ActualizarTiling();
    }

    void OnDestroy()
    {
        if (_textura != null) Destroy(_textura);
    }
}
