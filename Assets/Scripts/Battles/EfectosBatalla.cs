using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class EfectosBatalla : MonoBehaviour
{
    public static EfectosBatalla instancia;

    [Header("Referencia al enemigo")]
    public RectTransform imagenEnemigo;

    [Header("Referencia al panel del jugador")]
    public RectTransform panelJugador;

    [Header("Sonidos de efectos visuales")]
    public AudioClip sonidoHielo;
    public AudioClip sonidoFuego;
    public AudioClip sonidoCuracion;
    public AudioClip sonidoEscudo;
    public AudioClip sonidoSaltoEnemigo;

    private Canvas canvasRaiz;
    private AudioSource audioSource;
    private Vector2 posOriginalEnemigo;

    void Awake()
    {
        instancia = this;
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null) audioSource = gameObject.AddComponent<AudioSource>();
        canvasRaiz = GetComponentInParent<Canvas>();
        if (canvasRaiz != null) canvasRaiz = canvasRaiz.rootCanvas;
    }

    void Start()
    {
        if (imagenEnemigo != null)
            posOriginalEnemigo = imagenEnemigo.anchoredPosition;
    }

    // Crea un símbolo como hijo de un RectTransform padre, en posición local
    GameObject CrearSimbolo(Transform padre, string simbolo, int fontSize, Color color, Vector2 posLocal)
    {
        GameObject go = new GameObject("Efecto");
        go.transform.SetParent(padre, false);
        RectTransform rt = go.AddComponent<RectTransform>();
        rt.sizeDelta = new Vector2(60, 60);
        rt.anchoredPosition = posLocal;
        TextMeshProUGUI txt = go.AddComponent<TextMeshProUGUI>();
        txt.text = simbolo;
        txt.fontSize = fontSize;
        txt.color = color;
        txt.alignment = TextAlignmentOptions.Center;
        txt.raycastTarget = false;
        return go;
    }

    // Crea un símbolo en el canvas raíz, posicionado sobre el panelJugador sin clipping.
    GameObject CrearSimboloSobrePanel(string simbolo, int fontSize, Color color, Vector2 offsetLocal)
    {
        Transform padre = canvasRaiz != null ? canvasRaiz.transform : panelJugador;

        GameObject go = new GameObject("Efecto");
        go.transform.SetParent(padre, false);

        RectTransform rt = go.AddComponent<RectTransform>();
        rt.sizeDelta = new Vector2(80, 80);
        rt.anchorMin = new Vector2(0.5f, 0.5f);
        rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.pivot = new Vector2(0.5f, 0.5f);

        // Convertir la posición en pantalla del panelJugador a coordenadas locales del canvas
        Vector2 posEnPantalla = RectTransformUtility.WorldToScreenPoint(null, panelJugador.position);
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRaiz.GetComponent<RectTransform>(),
            posEnPantalla,
            canvasRaiz.renderMode == RenderMode.ScreenSpaceOverlay ? null : Camera.main,
            out Vector2 posLocal
        );
        rt.anchoredPosition = posLocal + offsetLocal;

        TextMeshProUGUI txt = go.AddComponent<TextMeshProUGUI>();
        txt.text = simbolo;
        txt.fontSize = fontSize;
        txt.color = color;
        txt.alignment = TextAlignmentOptions.Center;
        txt.raycastTarget = false;
        return go;
    }

    // ── Salto del enemigo hacia el player (estilo DQ3 SNES) ──────────────────

    public IEnumerator EfectoSaltoEnemigo()
    {
        if (imagenEnemigo == null) yield break;
        if (sonidoSaltoEnemigo != null) audioSource.PlayOneShot(sonidoSaltoEnemigo);

        Vector2 inicio = posOriginalEnemigo;
        Vector2 destino = inicio + new Vector2(-180f, -60f);

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / 0.25f;
            float arco = Mathf.Sin(t * Mathf.PI) * 80f;
            Vector2 pos = Vector2.Lerp(inicio, destino, t);
            pos.y += arco;
            imagenEnemigo.anchoredPosition = pos;
            yield return null;
        }

        Image imgEnemigo = imagenEnemigo.GetComponent<Image>();
        if (imgEnemigo != null)
        {
            imgEnemigo.color = new Color(1f, 0.3f, 0.3f, 1f);
            yield return new WaitForSeconds(0.08f);
            imgEnemigo.color = Color.white;
        }

        yield return new WaitForSeconds(0.1f);

        t = 0f;
        Vector2 actual = imagenEnemigo.anchoredPosition;
        while (t < 1f)
        {
            t += Time.deltaTime / 0.2f;
            imagenEnemigo.anchoredPosition = Vector2.Lerp(actual, inicio, t);
            yield return null;
        }
        imagenEnemigo.anchoredPosition = inicio;
    }

    // ── Efecto Minihelada (sobre el enemigo) ──────────────────────────────────

    public IEnumerator EfectoHielo()
    {
        if (imagenEnemigo == null) yield break;
        if (sonidoHielo != null) audioSource.PlayOneShot(sonidoHielo);

        int cantidad = 12;
        GameObject[] copos = new GameObject[cantidad];

        for (int i = 0; i < cantidad; i++)
        {
            Vector2 pos = new Vector2(Random.Range(-70f, 70f), Random.Range(-60f, 60f));
            copos[i] = CrearSimbolo(imagenEnemigo, "❄", 32, new Color(0.5f, 0.85f, 1f, 1f), pos);
        }

        Image imgEnemigo = imagenEnemigo.GetComponent<Image>();
        if (imgEnemigo != null) imgEnemigo.color = new Color(0.5f, 0.8f, 1f, 1f);

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / 0.7f;
            for (int i = 0; i < cantidad; i++)
            {
                if (copos[i] == null) continue;
                copos[i].GetComponent<RectTransform>().anchoredPosition += new Vector2(0, -3f);
                copos[i].GetComponent<TextMeshProUGUI>().color = new Color(0.5f, 0.85f, 1f, 1f - t);
            }
            yield return null;
        }

        if (imgEnemigo != null) imgEnemigo.color = Color.white;
        foreach (var c in copos) if (c != null) Destroy(c);
    }

    // ── Efecto Miniincendio (sobre el enemigo) ────────────────────────────────

    public IEnumerator EfectoFuego()
    {
        if (imagenEnemigo == null) yield break;
        if (sonidoFuego != null) audioSource.PlayOneShot(sonidoFuego);

        int cantidad = 12;
        GameObject[] llamas = new GameObject[cantidad];

        for (int i = 0; i < cantidad; i++)
        {
            Vector2 pos = new Vector2(Random.Range(-70f, 70f), Random.Range(-60f, 60f));
            llamas[i] = CrearSimbolo(imagenEnemigo, "*", 36, new Color(1f, 0.5f, 0.1f, 1f), pos);
        }

        Image imgEnemigo = imagenEnemigo.GetComponent<Image>();
        if (imgEnemigo != null) imgEnemigo.color = new Color(1f, 0.5f, 0.1f, 1f);

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / 0.7f;
            for (int i = 0; i < cantidad; i++)
            {
                if (llamas[i] == null) continue;
                llamas[i].GetComponent<RectTransform>().anchoredPosition +=
                    new Vector2(Random.Range(-1.5f, 1.5f), 3.5f);
                llamas[i].GetComponent<TextMeshProUGUI>().color = new Color(1f, 0.5f, 0.1f, 1f - t);
            }
            yield return null;
        }

        if (imgEnemigo != null) imgEnemigo.color = Color.white;
        foreach (var l in llamas) if (l != null) Destroy(l);
    }

    // ── Efecto Curación (cruces verdes sobre el panel jugador) ────────────────

    public IEnumerator EfectoCuracion()
    {
        if (panelJugador == null) yield break;
        if (sonidoCuracion != null) audioSource.PlayOneShot(sonidoCuracion);

        AsegurarCanvasOverride(panelJugador);

        int cantidad = 6;
        GameObject[] cruces = new GameObject[cantidad];
        for (int i = 0; i < cantidad; i++)
        {
            Vector2 pos = new Vector2(Random.Range(-60f, 60f), Random.Range(0f, 60f));
            cruces[i] = CrearSimbolo(panelJugador, "+", 40, new Color(0.2f, 0.9f, 0.3f, 1f), pos);
        }

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / 0.8f;
            for (int i = 0; i < cantidad; i++)
            {
                if (cruces[i] == null) continue;
                cruces[i].GetComponent<RectTransform>().anchoredPosition += new Vector2(0, 2.5f);
                cruces[i].GetComponent<TextMeshProUGUI>().color = new Color(0.2f, 0.9f, 0.3f, 1f - t);
            }
            yield return null;
        }
        foreach (var c in cruces) if (c != null) Destroy(c);
    }

    // ── Efecto Fortalecimiento (escudo sobre panel jugador) ───────────────────

    public IEnumerator EfectoEscudo()
    {
        if (panelJugador == null) yield break;
        if (sonidoEscudo != null) audioSource.PlayOneShot(sonidoEscudo);

        AsegurarCanvasOverride(panelJugador);

        int cantidad = 6;
        GameObject[] escudos = new GameObject[cantidad];
        for (int i = 0; i < cantidad; i++)
        {
            Vector2 pos = new Vector2(Random.Range(-60f, 60f), Random.Range(0f, 60f));
            escudos[i] = CrearSimbolo(panelJugador, "[+]", 32, new Color(0.4f, 0.7f, 1f, 1f), pos);
        }

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / 0.9f;
            for (int i = 0; i < cantidad; i++)
            {
                if (escudos[i] == null) continue;
                escudos[i].GetComponent<RectTransform>().anchoredPosition += new Vector2(0, 2f);
                escudos[i].GetComponent<TextMeshProUGUI>().color = new Color(0.4f, 0.7f, 1f, 1f - t);
            }
            yield return null;
        }
        foreach (var e in escudos) if (e != null) Destroy(e);
    }

    // Añade Canvas + GraphicRaycaster al panel para que renderice encima sin clipping
    void AsegurarCanvasOverride(RectTransform panel)
    {
        Canvas c = panel.GetComponent<Canvas>();
        if (c == null)
        {
            c = panel.gameObject.AddComponent<Canvas>();
            c.overrideSorting = true;
            c.sortingOrder = 50;
            if (panel.GetComponent<UnityEngine.UI.GraphicRaycaster>() == null)
                panel.gameObject.AddComponent<UnityEngine.UI.GraphicRaycaster>();
        }
    }

    // ── Efecto Slash estilo DQ3 SNES (sobre el enemigo) ──────────────────────
    // Tres oleadas de barras diagonales que aparecen y desaparecen rápido,
    // con un flash blanco en la imagen del enemigo al impactar.

    public IEnumerator EfectoSlash()
    {
        if (imagenEnemigo == null) yield break;

        Image imgEnemigo = imagenEnemigo.GetComponent<Image>();

        // Dos pasadas, la segunda ligeramente desplazada (como DQ3)
        Vector2[] origenes = { new Vector2(-60f, -40f), new Vector2(-45f, -55f) };

        for (int pasada = 0; pasada < 2; pasada++)
        {
            // Una sola línea diagonal larga
            GameObject linea = CrearSimbolo(imagenEnemigo, "—", 96,
                new Color(1f, 1f, 0.9f, 1f), origenes[pasada]);

            // Rotar 45 grados para que sea diagonal
            linea.GetComponent<RectTransform>().localRotation =
                Quaternion.Euler(0f, 0f, 45f);
            // Alargar la línea horizontalmente
            linea.GetComponent<RectTransform>().sizeDelta = new Vector2(200f, 100f);

            // Flash blanco al impactar
            if (imgEnemigo != null) imgEnemigo.color = Color.white;

            // La línea cruza de abajo-izquierda a arriba-derecha rápido
            float t = 0f;
            Vector2 inicio = origenes[pasada];
            Vector2 fin = inicio + new Vector2(80f, 80f);
            while (t < 1f)
            {
                t += Time.deltaTime / 0.10f;
                RectTransform rt = linea.GetComponent<RectTransform>();
                rt.anchoredPosition = Vector2.Lerp(inicio, fin, t);
                linea.GetComponent<TextMeshProUGUI>().color =
                    new Color(1f, 1f, 0.9f, 1f - t);
                yield return null;
            }

            if (imgEnemigo != null) imgEnemigo.color = Color.white;
            Destroy(linea);

            yield return new WaitForSeconds(0.06f);
        }

        // Flash rojo de daño
        if (imgEnemigo != null)
        {
            imgEnemigo.color = new Color(1f, 0.25f, 0.25f, 1f);
            yield return new WaitForSeconds(0.1f);
            imgEnemigo.color = Color.white;
        }
    }
}