using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

[System.Serializable]
public class ProbabilidadPorEscena
{
    public string nombreEscena;
    public float probabilidad;
}

public class MovimientoMapa : MonoBehaviour
{
    [Header("Velocidades")]
    public float velocidadNormal = 5f;
    public float velocidadCarrera = 8f;

    [Header("Combate")]
    public float probabilidadCombate = 0.05f;
    public ProbabilidadPorEscena[] probabilidadesPorEscena;
    public DatosEnemigo[] posiblesEnemigos;

    [Header("Transición")]
    public CanvasGroup panelTransicion;

    public static DatosEnemigo enemigoSeleccionado;
    public static Vector3 posicionRetorno;
    public static bool vieneDeCombate = false;
    public static string escenaOrigen = "";
    public static bool pippinUnido = false;
    public static bool combateBoss = false;
    public static bool combateSecuaz = false;

    private Rigidbody2D rb;
    private Animator animator;

    private float moviX;
    private float moviY;
    private float ultimoX = 0f;
    private float ultimoY = -1f;

    private bool estaCaminando = false;
    private bool iniciandoCombate = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        string escenaActual = SceneManager.GetActiveScene().name;
        if (probabilidadesPorEscena != null)
        {
            foreach (ProbabilidadPorEscena p in probabilidadesPorEscena)
            {
                if (p.nombreEscena == escenaActual)
                {
                    probabilidadCombate = p.probabilidad;
                    break;
                }
            }
        }

        if (panelTransicion == null)
        {
            GameObject existing = GameObject.Find("CanvasTransicionAuto");
            if (existing != null)
            {
                Transform panel = existing.transform.Find("PanelTransicion");
                if (panel != null)
                    panelTransicion = panel.GetComponent<CanvasGroup>();
            }
            if (panelTransicion == null)
                panelTransicion = CrearPanelTransicion();
        }

        panelTransicion.alpha = 0f;
        panelTransicion.blocksRaycasts = false;

        if (vieneDeCombate)
        {
            transform.position = posicionRetorno;
            vieneDeCombate = false;
        }
        else if (!string.IsNullOrEmpty(escenaOrigen))
        {
            bool colocado = false;
            EntradaEscena[] entradas = FindObjectsOfType<EntradaEscena>();
            foreach (EntradaEscena entrada in entradas)
            {
                if (entrada.escenaOrigen == escenaOrigen)
                {
                    transform.position = entrada.transform.position;
                    colocado = true;
                    break;
                }
            }
            if (!colocado)
                Debug.Log("No se encontró EntradaEscena para: " + escenaOrigen);
        }
        else
        {
            if (SistemaGuardado.instancia != null && SistemaGuardado.instancia.hayPosicionGuardada)
                SistemaGuardado.instancia.AplicarPosicionJugador();
        }
    }

    CanvasGroup CrearPanelTransicion()
    {
        GameObject canvasGO = new GameObject("CanvasTransicionAuto");
        Canvas canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 999;
        canvasGO.AddComponent<CanvasScaler>();
        canvasGO.AddComponent<GraphicRaycaster>();
        DontDestroyOnLoad(canvasGO);

        GameObject panelGO = new GameObject("PanelTransicion");
        panelGO.transform.SetParent(canvasGO.transform, false);

        RectTransform rect = panelGO.AddComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;

        Image imagen = panelGO.AddComponent<Image>();
        imagen.color = Color.black;

        CanvasGroup grupo = panelGO.AddComponent<CanvasGroup>();
        grupo.alpha = 0;
        grupo.blocksRaycasts = false;

        return grupo;
    }

    bool HayDialogoActivo()
    {
        if (DialogoManager.instancia != null && DialogoManager.instancia.EstaActivo()) return true;
        if (DialogoManagerBoss.instancia != null && DialogoManagerBoss.instancia.EstaActivo()) return true;
        if (DialogoManagerCompañero.instancia != null && DialogoManagerCompañero.instancia.EstaActivo()) return true;
        return false;
    }

    bool EstaEnPausa()
    {
        if (MenuPausaManager.instancia != null && MenuPausaManager.instancia.MenuActivo()) return true;
        if (MenuFF7.instancia != null && MenuFF7.instancia.MenuActivo()) return true;
        return false;
    }

    void Update()
    {
        if (iniciandoCombate) return;

        // ── BLOQUEO TOTAL si hay pausa o diálogo ──
        if (EstaEnPausa() || HayDialogoActivo())
        {
            moviX = 0;
            moviY = 0;
            if (rb != null) rb.velocity = Vector2.zero;
            if (estaCaminando)
            {
                estaCaminando = false;
                CancelInvoke("ChequearCombate");
            }
            if (animator != null) animator.SetBool("Moviéndose", false);
            return;
        }

        moviX = Input.GetAxisRaw("Horizontal");
        moviY = Input.GetAxisRaw("Vertical");

        bool seEstaMoviendo = (moviX != 0 || moviY != 0);
        if (animator != null) animator.SetBool("Moviéndose", seEstaMoviendo);

        if (seEstaMoviendo)
        {
            if (Mathf.Abs(moviX) > Mathf.Abs(moviY))
            {
                if (animator != null) { animator.SetFloat("MovimientoX", moviX); animator.SetFloat("MovimientoY", 0); }
                ultimoX = moviX; ultimoY = 0;
            }
            else
            {
                if (animator != null) { animator.SetFloat("MovimientoX", 0); animator.SetFloat("MovimientoY", moviY); }
                ultimoX = 0; ultimoY = moviY;
            }

            if (!estaCaminando)
            {
                estaCaminando = true;
                InvokeRepeating("ChequearCombate", 0.5f, 0.5f);
            }
        }
        else
        {
            if (estaCaminando)
            {
                estaCaminando = false;
                CancelInvoke("ChequearCombate");
            }
        }
    }

    void FixedUpdate()
    {
        if (iniciandoCombate) return;
        if (EstaEnPausa() || HayDialogoActivo()) return;

        bool corriendo = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
        float velocidad = corriendo ? velocidadCarrera : velocidadNormal;

        Vector2 direccion = new Vector2(moviX, moviY).normalized;
        rb.MovePosition(rb.position + direccion * velocidad * Time.fixedDeltaTime);
    }

    void ChequearCombate()
    {
        if (EstaEnPausa()) return;

        float tirada = Random.value;
        if (tirada < probabilidadCombate && posiblesEnemigos.Length > 0)
        {
            iniciandoCombate = true;
            CancelInvoke("ChequearCombate");
            int indice = Random.Range(0, posiblesEnemigos.Length);
            enemigoSeleccionado = posiblesEnemigos[indice];
            posicionRetorno = transform.position;
            vieneDeCombate = true;
            escenaOrigen = SceneManager.GetActiveScene().name;
            StartCoroutine(TransicionBatalla());
        }
    }

    IEnumerator TransicionBatalla()
    {
        panelTransicion.blocksRaycasts = true;

        while (panelTransicion.alpha < 1f)
        {
            panelTransicion.alpha += Time.deltaTime * 2f;
            yield return null;
        }

        SceneManager.LoadScene("Battle");
    }
}