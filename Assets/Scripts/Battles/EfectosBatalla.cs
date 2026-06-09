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

    // Crea un símbolo en el canvas raíz, posicionado en coordenadas de mundo del panelJugador.
    // Así nunca queda cortado por el Rect del panel.
    GameObject CrearSimboloSobrePanel(string simbolo, int fontSize, Color color, Vector2 offsetLocal)
    {
        Transform padre = canvasRaiz != null ? canvasRaiz.transform : panelJugador;

        // Convertir esquina del panelJugador a posición de pantalla, luego a canvas
        Vector2 posPanel = panelJugador != null
            ? (Vector2)panelJugador.position
            : Vector2.zero;

        GameObject go = new GameObject("Efecto");
        go.transform.SetParent(padre, false);
        RectTransform rt = go.AddComponent<RectTransform>();
        rt.sizeDelta = new Vector2(80, 80);
        rt.position = posPanel;
        rt.anchoredPosition += offsetLocal;

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

        int cantidad = 6;
        GameObject[] cruces = new GameObject[cantidad];

        for (int i = 0; i < cantidad; i++)
        {
            Vector2 offset = new Vector2(Random.Range(-50f, 50f), Random.Range(-20f, 40f));
            cruces[i] = CrearSimboloSobrePanel("+", 40, new Color(0.2f, 0.9f, 0.3f, 1f), offset);
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

        int cantidad = 6;
        GameObject[] escudos = new GameObject[cantidad];

        for (int i = 0; i < cantidad; i++)
        {
            Vector2 offset = new Vector2(Random.Range(-50f, 50f), Random.Range(-20f, 40f));
            escudos[i] = CrearSimboloSobrePanel("[+]", 32, new Color(0.4f, 0.7f, 1f, 1f), offset);
        }

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / 0.9f;
            for (int i = 0; i < cantidad; i++)
            {
                if (escudos[i] == null) continue;
                escudos[i].GetComponent<RectTransform>().anchoredPosition += new Vector2(0, 2f);
                escudos[i].GetComponent<TextMeshProUGUI>().color =
                    new Color(0.4f, 0.7f, 1f, 1f - t);
            }
            yield return null;
        }

        foreach (var e in escudos) if (e != null) Destroy(e);
    }

    // ── Efecto Slash estilo DQ3 SNES (sobre el enemigo) ──────────────────────
    // Tres oleadas de barras diagonales que aparecen y desaparecen rápido,
    // con un flash blanco en la imagen del enemigo al impactar.

    public IEnumerator EfectoSlash()
    {
        if (imagenEnemigo == null) yield break;

        Image imgEnemigo = imagenEnemigo.GetComponent<Image>();

        // Tres oleadas de slash
        for (int oleada = 0; oleada < 3; oleada++)
        {
            int trazos = (oleada == 1) ? 3 : 2;
            GameObject[] slashes = new GameObject[trazos];

            for (int i = 0; i < trazos; i++)
            {
                float offsetX = -40f + i * 30f + oleada * 10f;
                float offsetY = 30f - i * 20f;
                slashes[i] = CrearSimbolo(imagenEnemigo, "/",
                    52, new Color(1f, 1f, 0.85f, 1f),
                    new Vector2(offsetX, offsetY));

                slashes[i].GetComponent<RectTransform>().localRotation =
                    Quaternion.Euler(0f, 0f, -20f + i * 8f);
            }

            // Flash blanco al impactar
            if (imgEnemigo != null) imgEnemigo.color = Color.white;

            float t = 0f;
            while (t < 1f)
            {
                t += Time.deltaTime / 0.12f;
                for (int i = 0; i < trazos; i++)
                {
                    if (slashes[i] == null) continue;
                    slashes[i].GetComponent<RectTransform>().anchoredPosition +=
                        new Vector2(4f, 4f);
                    slashes[i].GetComponent<TextMeshProUGUI>().color =
                        new Color(1f, 1f, 0.85f, 1f - t);
                }
                yield return null;
            }

            if (imgEnemigo != null) imgEnemigo.color = Color.white;
            foreach (var s in slashes) if (s != null) Destroy(s);

            // Pausa breve entre oleadas (tintineo DQ3)
            yield return new WaitForSeconds(0.07f);
        }

        // Flash rojo de daño recibido
        if (imgEnemigo != null)
        {
            imgEnemigo.color = new Color(1f, 0.25f, 0.25f, 1f);
            yield return new WaitForSeconds(0.1f);
            imgEnemigo.color = Color.white;
        }
    }
}