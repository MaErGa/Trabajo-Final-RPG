using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using TMPro;
using UnityEngine.SceneManagement;

/// <summary>
/// ╔══════════════════════════════════════════════════════════╗
/// ║  OpcionesFF7  —  Pantalla de opciones estilo FF7         ║
/// ╠══════════════════════════════════════════════════════════╣
/// ║  • Genera toda la UI por código (nunca se descuadra)     ║
/// ║  • Slider de volumen general                             ║
/// ║  • Botón salir a escena Titulo                           ║
/// ╠══════════════════════════════════════════════════════════╣
/// ║  SETUP en el Inspector:                                  ║
/// ║    fuentePixel  → fuente TMP pixel (opcional)            ║
/// ║    mixerGeneral → AudioMixer (opcional)                  ║
/// ║    parametroVol → nombre del parámetro expuesto          ║
/// ║                   en el AudioMixer (ej: "VolGeneral")    ║
/// ╚══════════════════════════════════════════════════════════╝
/// </summary>
public class OpcionesFF7 : MonoBehaviour
{
    [Header("── Estética (opcional) ────────────────────────")]
    public TMP_FontAsset fuentePixel;

    [Header("── Audio (opcional) ───────────────────────────")]
    [Tooltip("Arrastra tu AudioMixer aquí para controlar el volumen maestro")]
    public AudioMixer mixerGeneral;
    [Tooltip("Nombre exacto del parámetro expuesto en el AudioMixer")]
    public string parametroVol = "VolGeneral";

    // ── Paleta FF7 ────────────────────────────────────────────────────────────
    static readonly Color C_FONDO  = new Color(0.05f, 0.08f, 0.35f, 1f);
    static readonly Color C_MEDIO  = new Color(0.10f, 0.15f, 0.55f, 1f);
    static readonly Color C_CLARO  = new Color(0.20f, 0.30f, 0.75f, 1f);
    static readonly Color C_BORDE  = new Color(0.55f, 0.65f, 1.00f, 1f);
    static readonly Color C_ORO    = new Color(1.00f, 0.85f, 0.20f, 1f);
    static readonly Color C_CYAN   = new Color(0.40f, 0.90f, 1.00f, 1f);
    static readonly Color C_BLANCO = Color.white;
    static readonly Color C_GRIS   = new Color(0.70f, 0.70f, 0.70f, 1f);
    static readonly Color C_ROJO   = new Color(0.70f, 0.12f, 0.12f, 1f);
    static readonly Color C_SEL    = new Color(0.30f, 0.50f, 1.00f, 1f);
    static readonly Color C_SLIDER = new Color(0.55f, 0.65f, 1.00f, 1f);

    // ── Referencias UI ────────────────────────────────────────────────────────
    TextMeshProUGUI _txtVolValor;
    Slider          _sliderVol;
    TextMeshProUGUI _txtCRTValor;
    Slider          _sliderCRT;

    // ══════════════════════════════════════════════════════════════════════════
    void Awake() => ConstruirUI();

    // ══════════════════════════════════════════════════════════════════════════
    // CONSTRUCCIÓN UI
    // Resolución de referencia: 1366 × 768
    //
    //  ┌─────────────────────────────────────────────┐
    //  │                   CONFIG                    │  ← header
    //  ├──────────────────┬──────────────────────────┤
    //  │  OPCIONES (izq)  │  CONTROLES (der)         │
    //  │                  │                          │
    //  │  Volumen General │  ████████░░  75          │
    //  │                  │                          │
    //  ├──────────────────┴──────────────────────────┤
    //  │            [ Salir al Título ]              │
    //  └─────────────────────────────────────────────┘
    // ══════════════════════════════════════════════════════════════════════════
    void ConstruirUI()
    {
        // Canvas
        var cgo = new GameObject("OpcionesFF7_Canvas");
        cgo.transform.SetParent(transform);
        var canvas = cgo.AddComponent<Canvas>();
        canvas.renderMode   = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 10;
        var cs = cgo.AddComponent<CanvasScaler>();
        cs.uiScaleMode         = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        cs.referenceResolution = new Vector2(1366, 768);
        cs.screenMatchMode     = CanvasScaler.ScreenMatchMode.Expand;
        cgo.AddComponent<GraphicRaycaster>();

        // Fondo oscuro pantalla completa
        var raiz = Nodo("Raiz", cgo.transform);
        Stretch(raiz.GetComponent<RectTransform>(), Vector2.zero, Vector2.one);
        raiz.AddComponent<Image>().color = new Color(0f, 0f, 0f, 1f);

        // ── Panel principal centrado (820 × 500) ──────────────────────────────
        var panel = PanelFF("PanelOpciones", raiz.transform,
            new Vector2(273, 134), new Vector2(820, 500));
        var relleno = panel.transform.Find("Relleno");
        Transform r = relleno ?? panel.transform;

        // ── Header ────────────────────────────────────────────────────────────
        var headerGO = Nodo("Header", r);
        var rtH = headerGO.GetComponent<RectTransform>();
        rtH.anchorMin = new Vector2(0, 1); rtH.anchorMax = new Vector2(1, 1);
        rtH.pivot     = new Vector2(0, 1);
        rtH.anchoredPosition = Vector2.zero; rtH.sizeDelta = new Vector2(0, 40);
        headerGO.AddComponent<Image>().color = C_CLARO;
        var txtHeader = CrearTMP(headerGO.transform, "TxtHeader", "CONFIG",
            16, TextAlignmentOptions.Center, C_ORO, bold: true);
        Stretch(txtHeader.rectTransform, Vector2.zero, Vector2.zero);

        // ── Separador columnas ────────────────────────────────────────────────
        // Columna izquierda: etiquetas (40% del ancho)
        // Columna derecha:   controles (60% del ancho)

        // ── Fila: Volumen General ─────────────────────────────────────────────
        float fy = -70f;
        FilaEtiqueta(r, "LblVol", "Volumen General", fy);

        // Slider
        _sliderVol = CrearSlider(r, "SliderVol",
            new Vector2(330, fy - 10), new Vector2(340, 20));
        _sliderVol.minValue = 0f;
        _sliderVol.maxValue = 1f;
        _sliderVol.value    = PlayerPrefs.GetFloat("vol_general", 1f);
        _sliderVol.onValueChanged.AddListener(AlCambiarVolumen);

        // Valor numérico del slider
        _txtVolValor = CrearTMPAnclado(r, "TxtVolVal",
            Mathf.RoundToInt(_sliderVol.value * 100).ToString(),
            13, TextAlignmentOptions.Left, C_BLANCO,
            new Vector2(0, 1), new Vector2(0, 1),
            new Vector2(682, fy - 4), new Vector2(60, 22));

        // Separador horizontal
        Separador(r, -120f);

        // ── Fila: Filtro CRT ──────────────────────────────────────────────────
        float fyCRT = -145f;
        FilaEtiqueta(r, "LblCRT", "Filtro CRT", fyCRT);

        _sliderCRT = CrearSlider(r, "SliderCRT",
            new Vector2(330, fyCRT - 10), new Vector2(340, 20));
        _sliderCRT.minValue = 0f;
        _sliderCRT.maxValue = 1f;
        _sliderCRT.value    = PlayerPrefs.GetFloat("crt_intensidad", 0.35f);
        _sliderCRT.onValueChanged.AddListener(AlCambiarCRT);

        _txtCRTValor = CrearTMPAnclado(r, "TxtCRTVal",
            Mathf.RoundToInt(_sliderCRT.value * 100).ToString(),
            13, TextAlignmentOptions.Left, C_BLANCO,
            new Vector2(0, 1), new Vector2(0, 1),
            new Vector2(682, fyCRT - 4), new Vector2(60, 22));

        Separador(r, -195f);

        // ── Botón Salir al Título ─────────────────────────────────────────────
        var btnSalir = CrearBoton(r, "BtnSalir", "Salir al Titulo",
            new Vector2(0.5f, 0), new Vector2(0.5f, 0),
            new Vector2(260, 48), new Vector2(0, 30));
        btnSalir.onClick.AddListener(SalirAlTitulo);

        // Aplicar volumen guardado al iniciar
        AplicarVolumen(_sliderVol.value);
    }

    // ══════════════════════════════════════════════════════════════════════════
    // LÓGICA
    // ══════════════════════════════════════════════════════════════════════════

    void AlCambiarCRT(float valor)
    {
        if (CRTEffect.instancia != null)
            CRTEffect.instancia.SetIntensidad(valor);
        PlayerPrefs.SetFloat("crt_intensidad", valor);
        if (_txtCRTValor) _txtCRTValor.text = Mathf.RoundToInt(valor * 100).ToString();
    }

    void AlCambiarVolumen(float valor)
    {
        AplicarVolumen(valor);
        PlayerPrefs.SetFloat("vol_general", valor);
        if (_txtVolValor) _txtVolValor.text = Mathf.RoundToInt(valor * 100).ToString();
    }

    void AplicarVolumen(float valor)
    {
        // Con AudioMixer: convierte 0-1 a dB (-80 a 0)
        if (mixerGeneral != null)
        {
            float db = valor > 0.0001f ? Mathf.Log10(valor) * 20f : -80f;
            mixerGeneral.SetFloat(parametroVol, db);
        }
        else
        {
            // Sin AudioMixer: controla el volumen global de Unity
            AudioListener.volume = valor;
        }
    }

    void SalirAlTitulo()
    {
        PlayerPrefs.Save();
        SceneManager.LoadScene("Titulo");
    }

    // ══════════════════════════════════════════════════════════════════════════
    // HELPERS DE CONSTRUCCIÓN
    // ══════════════════════════════════════════════════════════════════════════

    /// Panel azul oscuro con borde blanco. Pivot arriba-izquierda.
    GameObject PanelFF(string nombre, Transform padre, Vector2 posTopLeft, Vector2 tamano)
    {
        var go = Nodo(nombre, padre);
        var rt = go.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0, 1); rt.anchorMax = new Vector2(0, 1);
        rt.pivot     = new Vector2(0, 1);
        rt.anchoredPosition = new Vector2(posTopLeft.x, -posTopLeft.y);
        rt.sizeDelta = tamano;
        go.AddComponent<Image>().color = C_FONDO;

        var b = Nodo("Borde", go.transform);
        Stretch(b.GetComponent<RectTransform>(), new Vector2(2, 2), new Vector2(-2, -2));
        b.AddComponent<Image>().color = C_BORDE;
        b.GetComponent<Image>().raycastTarget = false;

        var relleno = Nodo("Relleno", go.transform);
        Stretch(relleno.GetComponent<RectTransform>(), new Vector2(4, 4), new Vector2(-4, -4));
        relleno.AddComponent<Image>().color = C_FONDO;
        relleno.GetComponent<Image>().raycastTarget = false;

        return go;
    }

    /// Etiqueta cyan en la columna izquierda de una fila
    void FilaEtiqueta(Transform padre, string id, string texto, float y)
    {
        CrearTMPAnclado(padre, id, texto, 13, TextAlignmentOptions.Left, C_CYAN,
            new Vector2(0, 1), new Vector2(0, 1),
            new Vector2(20, y), new Vector2(300, 22));
    }

    /// Línea separadora horizontal
    void Separador(Transform padre, float y)
    {
        var go = Nodo("Sep", padre);
        var rt = go.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0, 1); rt.anchorMax = new Vector2(1, 1);
        rt.pivot     = new Vector2(0, 1);
        rt.anchoredPosition = new Vector2(10, y); rt.sizeDelta = new Vector2(-20, 2);
        go.AddComponent<Image>().color = C_BORDE;
    }

    /// Slider estilo FF7
    Slider CrearSlider(Transform padre, string nombre, Vector2 pos, Vector2 size)
    {
        var go = Nodo(nombre, padre);
        var rt = go.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0, 1); rt.anchorMax = new Vector2(0, 1);
        rt.pivot     = new Vector2(0, 1);
        rt.anchoredPosition = pos; rt.sizeDelta = size;

        // Fondo del slider
        var bg = Nodo("Background", go.transform);
        Stretch(bg.GetComponent<RectTransform>(), Vector2.zero, Vector2.zero);
        bg.AddComponent<Image>().color = new Color(0.1f, 0.1f, 0.2f, 1f);

        // Fill area
        var fillArea = Nodo("Fill Area", go.transform);
        var rtFA = fillArea.GetComponent<RectTransform>();
        rtFA.anchorMin = Vector2.zero; rtFA.anchorMax = Vector2.one;
        rtFA.offsetMin = new Vector2(5, 2); rtFA.offsetMax = new Vector2(-5, -2);

        var fill = Nodo("Fill", fillArea.transform);
        Stretch(fill.GetComponent<RectTransform>(), Vector2.zero, Vector2.zero);
        var imgFill = fill.AddComponent<Image>();
        imgFill.color = C_SLIDER;

        // Handle
        var handleArea = Nodo("Handle Slide Area", go.transform);
        Stretch(handleArea.GetComponent<RectTransform>(), Vector2.zero, Vector2.zero);

        var handle = Nodo("Handle", handleArea.transform);
        var rtHandle = handle.GetComponent<RectTransform>();
        rtHandle.sizeDelta = new Vector2(12, 0);
        var imgHandle = handle.AddComponent<Image>();
        imgHandle.color = C_BLANCO;

        // Slider component
        var slider = go.AddComponent<Slider>();
        slider.fillRect   = fill.GetComponent<RectTransform>();
        slider.handleRect = handle.GetComponent<RectTransform>();
        slider.targetGraphic = imgHandle;
        slider.direction  = Slider.Direction.LeftToRight;

        var cb = slider.colors;
        cb.normalColor      = C_BLANCO;
        cb.highlightedColor = C_ORO;
        cb.pressedColor     = C_SEL;
        slider.colors = cb;

        return slider;
    }

    /// Botón con pivot y anclas personalizadas
    Button CrearBoton(Transform padre, string nombre, string texto,
        Vector2 anchorMin, Vector2 anchorMax, Vector2 sizeDelta, Vector2 anchoredPos)
    {
        var go = Nodo(nombre, padre);
        var rt = go.GetComponent<RectTransform>();
        rt.anchorMin = anchorMin; rt.anchorMax = anchorMax;
        rt.pivot     = new Vector2(0.5f, 0);
        rt.anchoredPosition = anchoredPos; rt.sizeDelta = sizeDelta;

        var img = go.AddComponent<Image>();
        img.color = C_ROJO;
        var btn = go.AddComponent<Button>();
        var cb  = btn.colors;
        cb.normalColor      = C_ROJO;
        cb.highlightedColor = Color.Lerp(C_ROJO, Color.white, 0.25f);
        cb.pressedColor     = Color.Lerp(C_ROJO, Color.black, 0.25f);
        btn.colors = cb;

        var tGO = Nodo("Txt", go.transform);
        Stretch(tGO.GetComponent<RectTransform>(), Vector2.zero, Vector2.zero);
        var t = tGO.AddComponent<TextMeshProUGUI>();
        t.text = texto; t.fontSize = 16; t.color = C_BLANCO;
        t.fontStyle = FontStyles.Bold;
        t.alignment = TextAlignmentOptions.Center;
        if (fuentePixel) t.font = fuentePixel;

        return btn;
    }

    TextMeshProUGUI CrearTMP(Transform padre, string nombre, string texto,
        float fs, TextAlignmentOptions align, Color color, bool bold = false)
    {
        var go = Nodo(nombre, padre);
        Stretch(go.GetComponent<RectTransform>(), Vector2.zero, Vector2.zero);
        var t = go.AddComponent<TextMeshProUGUI>();
        t.text = texto; t.fontSize = fs; t.color = color; t.alignment = align;
        if (bold) t.fontStyle = FontStyles.Bold;
        if (fuentePixel) t.font = fuentePixel;
        return t;
    }

    TextMeshProUGUI CrearTMPAnclado(Transform padre, string nombre, string texto,
        float fs, TextAlignmentOptions align, Color color,
        Vector2 anchorMin, Vector2 anchorMax, Vector2 pos, Vector2 size)
    {
        var go = Nodo(nombre, padre);
        var rt = go.GetComponent<RectTransform>();
        rt.anchorMin = anchorMin; rt.anchorMax = anchorMax;
        rt.pivot     = new Vector2(0, 1);
        rt.anchoredPosition = pos; rt.sizeDelta = size;
        var t = go.AddComponent<TextMeshProUGUI>();
        t.text = texto; t.fontSize = fs; t.color = color; t.alignment = align;
        t.overflowMode = TextOverflowModes.Ellipsis;
        if (fuentePixel) t.font = fuentePixel;
        return t;
    }

    GameObject Nodo(string nombre, Transform padre)
    {
        var go = new GameObject(nombre, typeof(RectTransform));
        go.transform.SetParent(padre, false);
        return go;
    }

    void Stretch(RectTransform rt, Vector2 offsetMin, Vector2 offsetMax)
    {
        rt.anchorMin = Vector2.zero; rt.anchorMax = Vector2.one;
        rt.offsetMin = offsetMin;   rt.offsetMax = offsetMax;
    }
}