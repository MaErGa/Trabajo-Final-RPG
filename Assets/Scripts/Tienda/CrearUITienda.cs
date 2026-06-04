using UnityEngine;
using UnityEngine.UI;
using TMPro;

#if UNITY_EDITOR
using UnityEditor;

[ExecuteInEditMode]
public class CrearUITienda : MonoBehaviour
{
    [Header("Arrastra aquí el Canvas de la escena Tienda")]
    public Canvas canvas;

    [Header("Fuente pixel art (opcional)")]
    public TMP_FontAsset fuente;

    [Header("Sprite fondo de botón (opcional)")]
    public Sprite spriteFondoBoton;

    [Header("Sprite fondo de panel (opcional)")]
    public Sprite spriteFondoPanel;

    // ─────────────────────────────────────────────────────────────────────────
    [ContextMenu("🏪 Crear UI Tienda Completa")]
    public void CrearUI()
    {
        if (canvas == null) { Debug.LogError("Asigna el Canvas primero."); return; }

        CrearPanelTienda();
        Debug.Log("✅ UI de Tienda completa creada. Asigna referencias en StoreManager y NPCTienda.");
        EditorUtility.SetDirty(canvas.gameObject);
    }

    // ─────────────────────────────────────────────────────────────────────────
    [ContextMenu("🪙 Crear Solo Panel Principal Tienda")]
    public void CrearPanelTienda()
    {
        if (canvas == null) { Debug.LogError("Asigna el Canvas primero."); return; }

        // ── Panel raíz de la tienda (centro pantalla) ─────────────────────────
        GameObject panelTienda = CrearPanel("PanelTienda", canvas.transform,
            new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f),
            new Vector2(0, 0), new Vector2(520, 400));
        panelTienda.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0.5f);
        panelTienda.SetActive(false); // empieza oculto, NPCTienda lo activa

        // ── Título ────────────────────────────────────────────────────────────
        CrearTextoTMP("TxtTituloTienda", panelTienda.transform,
            "TIENDA", 20, TextAlignmentOptions.Center,
            new Vector2(0, -10), new Vector2(500, 30));

        // ── Texto Oro del jugador ─────────────────────────────────────────────
        CrearTextoTMP("textoOroUI", panelTienda.transform,
            "Oro: 0 G", 14, TextAlignmentOptions.Right,
            new Vector2(-10, -10), new Vector2(200, 25));

        // ── Texto info / feedback ─────────────────────────────────────────────
        CrearTextoTMP("textoInfoUI", panelTienda.transform,
            "Selecciona un artículo.", 13, TextAlignmentOptions.Left,
            new Vector2(10, -45), new Vector2(500, 25));

        // ── Botones Comprar / Vender ──────────────────────────────────────────
        GameObject filaModos = CrearFilaHorizontal("FilaModos", panelTienda.transform,
            new Vector2(-250, -80), new Vector2(500, 35));
        CrearBoton("BtnComprar", filaModos.transform, "Comprar", 35);
        CrearBoton("BtnVender",  filaModos.transform, "Vender",  35);

        // ── Panel Lista de items del tendero (comprar) ────────────────────────
        GameObject panelComprar = CrearPanel("PanelListaComprar", panelTienda.transform,
            new Vector2(0, 1), new Vector2(0, 1),
            new Vector2(10, -125), new Vector2(240, 220));
        panelComprar.GetComponent<RectTransform>().pivot = new Vector2(0, 1);
        CrearTextoTMP("TxtListaComprar", panelComprar.transform,
            "Artículos del tendero", 12, TextAlignmentOptions.Center,
            new Vector2(0, -8), new Vector2(220, 20));
        // Scroll View para los botones de items a comprar
        GameObject scrollComprar = CrearScrollView("ScrollComprar", panelComprar.transform,
            new Vector2(10, -35), new Vector2(220, 175));

        // ── Panel Lista de items del jugador (vender) ─────────────────────────
        GameObject panelVender = CrearPanel("PanelListaVender", panelTienda.transform,
            new Vector2(1, 1), new Vector2(1, 1),
            new Vector2(-10, -125), new Vector2(240, 220));
        panelVender.GetComponent<RectTransform>().pivot = new Vector2(1, 1);
        panelVender.SetActive(false); // empieza oculto
        CrearTextoTMP("TxtListaVender", panelVender.transform,
            "Tu inventario", 12, TextAlignmentOptions.Center,
            new Vector2(0, -8), new Vector2(220, 20));
        GameObject scrollVender = CrearScrollView("ScrollVender", panelVender.transform,
            new Vector2(10, -35), new Vector2(220, 175));

        // ── Botón Cerrar ──────────────────────────────────────────────────────
        CrearBotonCerrar("BtnCerrarTienda", panelTienda.transform,
            new Vector2(0, -360), new Vector2(160, 30));

        Debug.Log("✅ Panel Tienda creado." +
                  "\n→ Arrastra 'textoOroUI' y 'textoInfoUI' al StoreManager." +
                  "\n→ Arrastra 'PanelTienda' al NPCTienda (campo tiendaUI / panelTienda)." +
                  "\n→ El Content de ScrollComprar es el padre de tus BotonTiendaUI." +
                  "\n→ Conecta BtnComprar/BtnVender al StoreManager.CambiarModoComprar()." +
                  "\n→ Conecta BtnCerrarTienda a NPCTienda o a PanelTienda.SetActive(false).");

        EditorUtility.SetDirty(canvas.gameObject);
    }

    // ─────────────────────────────────────────────────────────────────────────
    //  HELPERS — igual que CrearUIBatalla para mantener coherencia
    // ─────────────────────────────────────────────────────────────────────────

    GameObject CrearPanel(string nombre, Transform padre,
        Vector2 anchorMin, Vector2 anchorMax, Vector2 pos, Vector2 size)
    {
        GameObject go = new GameObject(nombre);
        go.transform.SetParent(padre, false);
        RectTransform rt = go.AddComponent<RectTransform>();
        rt.anchorMin = anchorMin; rt.anchorMax = anchorMax;
        rt.pivot = anchorMin;
        rt.anchoredPosition = pos; rt.sizeDelta = size;
        Image img = go.AddComponent<Image>();
        img.color = new Color(0f, 0f, 0.15f, 0.95f);
        if (spriteFondoPanel != null) { img.sprite = spriteFondoPanel; img.type = Image.Type.Sliced; }
        return go;
    }

    GameObject CrearFilaHorizontal(string nombre, Transform padre, Vector2 pos, Vector2 size)
    {
        GameObject go = new GameObject(nombre);
        go.transform.SetParent(padre, false);
        RectTransform rt = go.AddComponent<RectTransform>();
        rt.anchorMin = new Vector2(0, 1); rt.anchorMax = new Vector2(0, 1);
        rt.pivot = new Vector2(0, 1);
        rt.anchoredPosition = pos; rt.sizeDelta = size;
        HorizontalLayoutGroup hlg = go.AddComponent<HorizontalLayoutGroup>();
        hlg.spacing = 8;
        hlg.childControlWidth = true;
        hlg.childControlHeight = false;
        hlg.childForceExpandWidth = true;
        hlg.childForceExpandHeight = false;
        return go;
    }

    GameObject CrearScrollView(string nombre, Transform padre, Vector2 pos, Vector2 size)
    {
        // ScrollView raíz
        GameObject go = new GameObject(nombre);
        go.transform.SetParent(padre, false);
        RectTransform rt = go.AddComponent<RectTransform>();
        rt.anchorMin = new Vector2(0, 1); rt.anchorMax = new Vector2(0, 1);
        rt.pivot = new Vector2(0, 1);
        rt.anchoredPosition = pos; rt.sizeDelta = size;
        Image imgMask = go.AddComponent<Image>();
        imgMask.color = new Color(0, 0, 0, 0.01f);
        Mask mask = go.AddComponent<Mask>();
        mask.showMaskGraphic = false;
        ScrollRect sr = go.AddComponent<ScrollRect>();
        sr.horizontal = false;

        // Viewport
        GameObject viewport = new GameObject("Viewport");
        viewport.transform.SetParent(go.transform, false);
        RectTransform rtV = viewport.AddComponent<RectTransform>();
        rtV.anchorMin = Vector2.zero; rtV.anchorMax = Vector2.one;
        rtV.sizeDelta = Vector2.zero; rtV.anchoredPosition = Vector2.zero;

        // Content
        GameObject content = new GameObject("Content");
        content.transform.SetParent(viewport.transform, false);
        RectTransform rtC = content.AddComponent<RectTransform>();
        rtC.anchorMin = new Vector2(0, 1); rtC.anchorMax = new Vector2(1, 1);
        rtC.pivot = new Vector2(0.5f, 1);
        rtC.anchoredPosition = Vector2.zero;
        rtC.sizeDelta = new Vector2(0, 0);
        VerticalLayoutGroup vlg = content.AddComponent<VerticalLayoutGroup>();
        vlg.padding = new RectOffset(4, 4, 4, 4);
        vlg.spacing = 4;
        vlg.childControlWidth = true;
        vlg.childControlHeight = false;
        vlg.childForceExpandWidth = true;
        vlg.childForceExpandHeight = false;
        ContentSizeFitter csf = content.AddComponent<ContentSizeFitter>();
        csf.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

        sr.viewport = rtV;
        sr.content = rtC;

        return go;
    }

    GameObject CrearBoton(string nombre, Transform padre, string texto, float altura)
    {
        GameObject go = new GameObject(nombre);
        go.transform.SetParent(padre, false);
        RectTransform rt = go.AddComponent<RectTransform>();
        rt.sizeDelta = new Vector2(160, altura);
        LayoutElement le = go.AddComponent<LayoutElement>();
        le.minHeight = altura; le.preferredHeight = altura;
        Image img = go.AddComponent<Image>();
        img.color = new Color(0.1f, 0.1f, 0.3f, 1f);
        if (spriteFondoBoton != null) { img.sprite = spriteFondoBoton; img.type = Image.Type.Sliced; }
        Button btn = go.AddComponent<Button>();
        ColorBlock cb = btn.colors;
        cb.highlightedColor = new Color(0.3f, 0.3f, 0.6f, 1f);
        cb.pressedColor     = new Color(0.5f, 0.5f, 0.8f, 1f);
        btn.colors = cb;
        GameObject textoGO = new GameObject("Text");
        textoGO.transform.SetParent(go.transform, false);
        RectTransform rtT = textoGO.AddComponent<RectTransform>();
        rtT.anchorMin = Vector2.zero; rtT.anchorMax = Vector2.one;
        rtT.sizeDelta = Vector2.zero; rtT.anchoredPosition = Vector2.zero;
        TextMeshProUGUI tmp = textoGO.AddComponent<TextMeshProUGUI>();
        tmp.text = texto; tmp.fontSize = 15; tmp.color = Color.white;
        tmp.alignment = TextAlignmentOptions.Center;
        if (fuente != null) tmp.font = fuente;
        return go;
    }

    GameObject CrearBotonCerrar(string nombre, Transform padre, Vector2 pos, Vector2 size)
    {
        GameObject go = new GameObject(nombre);
        go.transform.SetParent(padre, false);
        RectTransform rt = go.AddComponent<RectTransform>();
        rt.anchorMin = new Vector2(0.5f, 1); rt.anchorMax = new Vector2(0.5f, 1);
        rt.pivot = new Vector2(0.5f, 1);
        rt.anchoredPosition = pos; rt.sizeDelta = size;
        Image img = go.AddComponent<Image>();
        img.color = new Color(0.4f, 0.05f, 0.05f, 1f);
        if (spriteFondoBoton != null) { img.sprite = spriteFondoBoton; img.type = Image.Type.Sliced; }
        go.AddComponent<Button>();
        GameObject textoGO = new GameObject("Text");
        textoGO.transform.SetParent(go.transform, false);
        RectTransform rtT = textoGO.AddComponent<RectTransform>();
        rtT.anchorMin = Vector2.zero; rtT.anchorMax = Vector2.one;
        rtT.sizeDelta = Vector2.zero; rtT.anchoredPosition = Vector2.zero;
        TextMeshProUGUI tmp = textoGO.AddComponent<TextMeshProUGUI>();
        tmp.text = "X  Cerrar"; tmp.fontSize = 15; tmp.color = Color.white;
        tmp.alignment = TextAlignmentOptions.Center;
        if (fuente != null) tmp.font = fuente;
        return go;
    }

    GameObject CrearTextoTMP(string nombre, Transform padre, string contenido,
        float fontSize, TextAlignmentOptions alineacion, Vector2 pos, Vector2 size)
    {
        GameObject go = new GameObject(nombre);
        go.transform.SetParent(padre, false);
        RectTransform rt = go.AddComponent<RectTransform>();
        rt.anchorMin = new Vector2(0, 1); rt.anchorMax = new Vector2(0, 1);
        rt.pivot = new Vector2(0, 1);
        rt.anchoredPosition = pos; rt.sizeDelta = size;
        TextMeshProUGUI tmp = go.AddComponent<TextMeshProUGUI>();
        tmp.text = contenido; tmp.fontSize = fontSize;
        tmp.color = Color.white; tmp.alignment = alineacion;
        if (fuente != null) tmp.font = fuente;
        return go;
    }
}
#endif
