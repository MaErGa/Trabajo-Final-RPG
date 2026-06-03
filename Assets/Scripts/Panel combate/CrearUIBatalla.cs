using UnityEngine;
using UnityEngine.UI;
using TMPro;

#if UNITY_EDITOR
using UnityEditor;

[ExecuteInEditMode]
public class CrearUIBatalla : MonoBehaviour
{
    [Header("Arrastra aquí el Canvas de la escena Battle")]
    public Canvas canvas;

    [Header("Fuente pixel art (opcional)")]
    public TMP_FontAsset fuente;

    [Header("Sprite fondo de botón (opcional)")]
    public Sprite spriteFondoBoton;

    [Header("Sprite fondo de panel (opcional)")]
    public Sprite spriteFondoPanel;

    // ─────────────────────────────────────────────────────────────────────────
    [ContextMenu("🎮 Crear Solo Panel Magia")]
    public void CrearPanelMagia()
    {
        if (canvas == null) { Debug.LogError("Asigna el Canvas primero."); return; }

        GameObject panelMagia = CrearPanel("Panel Magia", canvas.transform,
            new Vector2(0, 0), new Vector2(0, 0), new Vector2(10, 10), new Vector2(220, 260));
        panelMagia.GetComponent<RectTransform>().pivot = new Vector2(0, 0);
        panelMagia.SetActive(false);

        AñadirVLG(panelMagia, 8, 8, 8, 8, 6);

        GameObject bMinicuracion = CrearBoton("BotonMinicuracion", panelMagia.transform, "Minicuración", 35);
        GameObject bFortalecimiento = CrearBoton("BotonFortalecimiento", panelMagia.transform, "Fortalecimiento", 35);
        GameObject bMinihelada = CrearBoton("BotonMinihelada", panelMagia.transform, "Minihelada", 35);
        GameObject bMiniincendio = CrearBoton("BotonMiniincendio", panelMagia.transform, "Miniincendio", 35);

        bMinicuracion.SetActive(false);
        bFortalecimiento.SetActive(false);
        bMinihelada.SetActive(false);
        bMiniincendio.SetActive(false);

        CrearBotonCerrar("BtnCerrarMagia", panelMagia.transform);

        Debug.Log("✅ Panel Magia creado. Asigna referencias en BattleManager y conecta OnClick.");
        EditorUtility.SetDirty(canvas.gameObject);
    }

    // ─────────────────────────────────────────────────────────────────────────
    [ContextMenu("📦 Crear Solo Panel Objetos")]
    public void CrearPanelObjetos()
    {
        if (canvas == null) { Debug.LogError("Asigna el Canvas primero."); return; }

        GameObject panelObjetos = CrearPanel("Panel Objetos", canvas.transform,
            new Vector2(0, 0), new Vector2(0, 0), new Vector2(10, 10), new Vector2(220, 160));
        panelObjetos.GetComponent<RectTransform>().pivot = new Vector2(0, 0);
        panelObjetos.SetActive(false);

        AñadirVLG(panelObjetos, 8, 8, 8, 8, 6);

        CrearBoton("BotonPlanta", panelObjetos.transform, "Planta Medicinal", 35);
        CrearBoton("BotonColaDeConejo", panelObjetos.transform, "Cola de Conejo", 35);

        CrearBotonCerrar("BtnCerrarObjetos", panelObjetos.transform);

        Debug.Log("✅ Panel Objetos creado. Asigna referencias en BattleManager y conecta OnClick.");
        EditorUtility.SetDirty(canvas.gameObject);
    }

    // ─────────────────────────────────────────────────────────────────────────
    [ContextMenu("⚔️ Crear Solo Panel Decisiones")]
    public void CrearPanelDecisiones()
    {
        if (canvas == null) { Debug.LogError("Asigna el Canvas primero."); return; }

        GameObject panelDecisiones = CrearPanel("Panel Deciciones", canvas.transform,
            new Vector2(0, 0), new Vector2(0, 0), new Vector2(10, 10), new Vector2(180, 225));
        panelDecisiones.GetComponent<RectTransform>().pivot = new Vector2(0, 0);

        AñadirVLG(panelDecisiones, 8, 8, 8, 8, 6);

        CrearBoton("Boton Ataque", panelDecisiones.transform, "Ataque", 35);
        CrearBoton("Boton Conjuros", panelDecisiones.transform, "Conjuros", 35);
        CrearBoton("Boton Objetos", panelDecisiones.transform, "Objetos", 35);
        CrearBoton("Boton Defensa", panelDecisiones.transform, "Defensa", 35);
        CrearBoton("Boton Escapar", panelDecisiones.transform, "Escapar", 35);

        Debug.Log("✅ Panel Decisiones creado. Asigna referencias en BattleManager y conecta OnClick.");
        EditorUtility.SetDirty(canvas.gameObject);
    }

    // ─────────────────────────────────────────────────────────────────────────
    [ContextMenu("🔨 Crear UI Batalla Completa")]
    public void CrearUI()
    {
        if (canvas == null) { Debug.LogError("Asigna el Canvas primero."); return; }

        // Panel Stats
        GameObject panelStats = CrearPanel("Panel del jugador", canvas.transform,
            new Vector2(0, 1), new Vector2(0, 1), new Vector2(10, -10), new Vector2(200, 120));
        CrearTextoTMP("Nombre del Player", panelStats.transform, "Player", 18, TextAlignmentOptions.Left, new Vector2(10, -10), new Vector2(180, 30));
        CrearTextoTMP("Vida Jugador Texto", panelStats.transform, "HP: --", 16, TextAlignmentOptions.Left, new Vector2(10, -45), new Vector2(180, 25));
        CrearTextoTMP("Mp del Jugador", panelStats.transform, "MP: --", 16, TextAlignmentOptions.Left, new Vector2(10, -72), new Vector2(180, 25));
        CrearTextoTMP("Nivel del Jugador", panelStats.transform, "LV: --", 16, TextAlignmentOptions.Left, new Vector2(10, -99), new Vector2(180, 25));

        CrearPanelDecisiones();
        CrearPanelMagia();
        CrearPanelObjetos();

        // Panel Dialogo
        GameObject panelDialogo = CrearPanel("Panel Dialogo", canvas.transform,
            new Vector2(1, 0), new Vector2(1, 0), new Vector2(-10, 10), new Vector2(500, 130));
        panelDialogo.GetComponent<RectTransform>().pivot = new Vector2(1, 0);
        CrearTextoTMP("Texto Combate", panelDialogo.transform, "Texto Combate", 16,
            TextAlignmentOptions.TopLeft, new Vector2(10, -10), new Vector2(480, 110));

        Debug.Log("✅ UI de Batalla completa creada.");
        EditorUtility.SetDirty(canvas.gameObject);
    }

    // ── Helpers ───────────────────────────────────────────────────────────────

    GameObject CrearPanel(string nombre, Transform padre, Vector2 anchorMin, Vector2 anchorMax, Vector2 pos, Vector2 size)
    {
        GameObject go = new GameObject(nombre);
        go.transform.SetParent(padre, false);
        RectTransform rt = go.AddComponent<RectTransform>();
        rt.anchorMin = anchorMin; rt.anchorMax = anchorMax;
        rt.pivot = anchorMin;
        rt.anchoredPosition = pos; rt.sizeDelta = size;
        Image img = go.AddComponent<Image>();
        img.color = new Color(0, 0, 0, 0.88f);
        if (spriteFondoPanel != null) { img.sprite = spriteFondoPanel; img.type = Image.Type.Sliced; }
        return go;
    }

    void AñadirVLG(GameObject go, int padL, int padR, int padT, int padB, int spacing)
    {
        VerticalLayoutGroup vlg = go.AddComponent<VerticalLayoutGroup>();
        vlg.padding = new RectOffset(padL, padR, padT, padB);
        vlg.spacing = spacing;
        vlg.childAlignment = TextAnchor.UpperCenter;
        vlg.childControlWidth = true;
        vlg.childControlHeight = false;
        vlg.childForceExpandWidth = true;
        vlg.childForceExpandHeight = false;
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
        img.color = new Color(0.1f, 0.1f, 0.1f, 1f);
        if (spriteFondoBoton != null) { img.sprite = spriteFondoBoton; img.type = Image.Type.Sliced; }
        Button btn = go.AddComponent<Button>();
        ColorBlock cb = btn.colors;
        cb.highlightedColor = new Color(0.3f, 0.3f, 0.3f, 1f);
        cb.pressedColor = new Color(0.5f, 0.5f, 0.5f, 1f);
        btn.colors = cb;
        GameObject textoGO = new GameObject("Text");
        textoGO.transform.SetParent(go.transform, false);
        RectTransform rtT = textoGO.AddComponent<RectTransform>();
        rtT.anchorMin = Vector2.zero; rtT.anchorMax = Vector2.one;
        rtT.sizeDelta = Vector2.zero; rtT.anchoredPosition = Vector2.zero;
        TextMeshProUGUI tmp = textoGO.AddComponent<TextMeshProUGUI>();
        tmp.text = texto; tmp.fontSize = 16; tmp.color = Color.white;
        tmp.alignment = TextAlignmentOptions.Center;
        if (fuente != null) tmp.font = fuente;
        return go;
    }

    GameObject CrearBotonCerrar(string nombre, Transform padre)
    {
        GameObject go = new GameObject(nombre);
        go.transform.SetParent(padre, false);
        RectTransform rt = go.AddComponent<RectTransform>();
        rt.sizeDelta = new Vector2(160, 30);
        LayoutElement le = go.AddComponent<LayoutElement>();
        le.minHeight = 30; le.preferredHeight = 30;
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
        tmp.text = "X  Cerrar"; tmp.fontSize = 16; tmp.color = Color.white;
        tmp.alignment = TextAlignmentOptions.Center;
        if (fuente != null) tmp.font = fuente;
        return go;
    }

    GameObject CrearTextoTMP(string nombre, Transform padre, string contenido, float fontSize,
        TextAlignmentOptions alineacion, Vector2 pos, Vector2 size)
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