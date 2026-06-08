using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Muestra el nombre de la zona al entrar en una escena, estilo Pokémon.
/// Se genera todo por script, no necesita objetos en escena.
/// 
/// USO: Arrastra este script a un GameObject vacío en cada escena.
///      Rellena "nombreZona" en el Inspector (ej: "Nimbórum").
/// </summary>
public class ZonaNombre : MonoBehaviour
{
    [Header("Nombre de la zona")]
    public string nombreZona = "Nimbórum";

    [Header("Ajustes visuales")]
    public float duracionVisible   = 2.5f;   // segundos que se queda en pantalla
    public float duracionEntrada   = 0.4f;   // segundos que tarda en aparecer
    public float duracionSalida    = 0.6f;   // segundos que tarda en subir y desaparecer

    // ── Referencias generadas por script ─────────────────────────────────────
    private Canvas       canvas;
    private RectTransform panelRect;
    private CanvasGroup  grupo;

    void Start()
    {
        ConstruirUI();
        StartCoroutine(AnimarCartel());
    }

    // ── Construcción del UI ───────────────────────────────────────────────────
    void ConstruirUI()
    {
        // Canvas en modo overlay, encima de todo
        GameObject canvasGO = new GameObject("ZonaNombre_Canvas");
        DontDestroyOnLoad(canvasGO);
        canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 999;
        canvasGO.AddComponent<CanvasScaler>().uiScaleMode =
            CanvasScaler.ScaleMode.ScaleWithScreenSize;
        canvasGO.AddComponent<GraphicRaycaster>();

        // Panel de fondo oscuro semitransparente
        GameObject panelGO = new GameObject("Panel");
        panelGO.transform.SetParent(canvasGO.transform, false);
        panelRect = panelGO.AddComponent<RectTransform>();
        grupo = panelGO.AddComponent<CanvasGroup>();

        // Tamaño y posición: esquina superior izquierda
        panelRect.anchorMin = new Vector2(0f, 1f);
        panelRect.anchorMax = new Vector2(0f, 1f);
        panelRect.pivot     = new Vector2(0f, 1f);
        panelRect.sizeDelta = new Vector2(320f, 52f);
        panelRect.anchoredPosition = new Vector2(24f, -24f);

        // Fondo
        Image fondo = panelGO.AddComponent<Image>();
        fondo.color = new Color(0f, 0f, 0f, 0.72f);

        // Barra decorativa izquierda (acento de color)
        GameObject barra = new GameObject("Barra");
        barra.transform.SetParent(panelGO.transform, false);
        RectTransform barraRect = barra.AddComponent<RectTransform>();
        barraRect.anchorMin = Vector2.zero;
        barraRect.anchorMax = new Vector2(0f, 1f);
        barraRect.offsetMin = Vector2.zero;
        barraRect.offsetMax = new Vector2(5f, 0f);
        Image barraImg = barra.AddComponent<Image>();
        barraImg.color = new Color(0.85f, 0.72f, 0.35f, 1f); // dorado

        // Texto del nombre
        GameObject textoGO = new GameObject("Texto");
        textoGO.transform.SetParent(panelGO.transform, false);
        RectTransform textoRect = textoGO.AddComponent<RectTransform>();
        textoRect.anchorMin = Vector2.zero;
        textoRect.anchorMax = Vector2.one;
        textoRect.offsetMin = new Vector2(16f, 4f);
        textoRect.offsetMax = new Vector2(-8f, -4f);

        // Intentamos usar TextMeshPro; si no existe, caemos a Text legacy
        TMP_Text tmpTexto = textoGO.AddComponent<TextMeshProUGUI>();
        if (tmpTexto != null)
        {
            tmpTexto.text      = nombreZona;
            tmpTexto.fontSize  = 22f;
            tmpTexto.color     = Color.white;
            tmpTexto.fontStyle = FontStyles.Bold;
            tmpTexto.alignment = TextAlignmentOptions.MidlineLeft;
        }
    }

    // ── Animación: aparece deslizando, espera, sube y se desvanece ───────────
    IEnumerator AnimarCartel()
    {
        // Posición inicial: justo encima de la pantalla (oculto)
        Vector2 posOculta  = new Vector2(24f, 80f);   // fuera hacia arriba
        Vector2 posVisible = new Vector2(24f, -24f);  // posición final

        grupo.alpha = 0f;
        panelRect.anchoredPosition = posOculta;

        // ── Entrada: baja y aparece ───────────────────────────────────────────
        float t = 0f;
        while (t < duracionEntrada)
        {
            t += Time.deltaTime;
            float p = Mathf.SmoothStep(0f, 1f, t / duracionEntrada);
            panelRect.anchoredPosition = Vector2.Lerp(posOculta, posVisible, p);
            grupo.alpha = p;
            yield return null;
        }
        panelRect.anchoredPosition = posVisible;
        grupo.alpha = 1f;

        // ── Espera visible ────────────────────────────────────────────────────
        yield return new WaitForSeconds(duracionVisible);

        // ── Salida: sube y desaparece ─────────────────────────────────────────
        Vector2 posSalida = new Vector2(24f, 30f);  // sube un poco
        t = 0f;
        while (t < duracionSalida)
        {
            t += Time.deltaTime;
            float p = Mathf.SmoothStep(0f, 1f, t / duracionSalida);
            panelRect.anchoredPosition = Vector2.Lerp(posVisible, posSalida, p);
            grupo.alpha = 1f - p;
            yield return null;
        }

        // Limpieza
        Destroy(canvas.gameObject);
        Destroy(gameObject);
    }
}
