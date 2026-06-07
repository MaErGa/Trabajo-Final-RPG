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
    public float probabilidadCombate = 0.05f; // Valor por defecto si la escena no está en la lista
    public ProbabilidadPorEscena[] probabilidadesPorEscena; // Lista de probabilidades por escena
    public DatosEnemigo[] posiblesEnemigos;

    [Header("Transición")]
    public CanvasGroup panelTransicion;

    // Statics compartidos
    public static DatosEnemigo enemigoSeleccionado;
    public static Vector3 posicionRetorno;
    public static bool vieneDeCombate = false;
    public static string escenaOrigen = "";
    public static bool pippinUnido = false;
    public static bool combateBoss = false;
    public static bool combateSecuaz = false;

    // Componentes
    private Rigidbody2D rb;
    private Animator animator;

    // Movimiento
    private float moviX;
    private float moviY;
    private float ultimoX = 0f;
    private float ultimoY = -1f;

    // Combate
    private bool estaCaminando = false;
    private bool iniciandoCombate = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        // Ajustar probabilidad según la escena actual
        string escenaActual = SceneManager.GetActiveScene().name;
        if (probabilidadesPorEscena != null)
        {
            foreach (ProbabilidadPorEscena p in probabilidadesPorEscena)
            {
                if (p.nombreEscena == escenaActual)
                {
                    probabilidadCombate = p.probabilidad;
                    Debug.Log("[MovimientoMapa] Probabilidad ajustada para '" + escenaActual + "': " + probabilidadCombate);
                    break;
                }
            }
        }

        // Crear panel de transición si no está asignado
        if (panelTransicion == null)
            panelTransicion = CrearPanelTransicion();

        panelTransicion.alpha = 0;

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

        Debug.Log("[MovimientoMapa] Panel de transición creado automáticamente.");
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
        return MenuPausaManager.instancia != null && MenuPausaManager.instancia.MenuActivo();
    }

    void Update()
    {
        if (iniciandoCombate) return;

        if (HayDialogoActivo())
        {
            moviX = 0; moviY = 0;
            if (animator != null) animator.SetBool("Moviéndose", false);
            if (rb != null) rb.velocity = Vector2.zero;
            return;
        }

        if (EstaEnPausa())
        {
            moviX = 0; moviY = 0;
            if (animator != null) animator.SetBool("Moviéndose", false);
            if (rb != null) rb.velocity = Vector2.zero;
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
                Debug.Log("[MovimientoMapa] Empezando a caminar, iniciando ChequearCombate.");
                InvokeRepeating("ChequearCombate", 0.5f, 0.5f);
            }
        }
        else
        {
            if (estaCaminando)
            {
                estaCaminando = false;
                Debug.Log("[MovimientoMapa] Parado, cancelando ChequearCombate.");
                CancelInvoke("ChequearCombate");
            }
        }
    }

    void FixedUpdate()
    {
        if (iniciandoCombate) return;
        if (HayDialogoActivo()) return;
        if (EstaEnPausa()) return;

        bool corriendo = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
        float velocidad = corriendo ? velocidadCarrera : velocidadNormal;

        Vector2 direccion = new Vector2(moviX, moviY).normalized;
        rb.MovePosition(rb.position + direccion * velocidad * Time.fixedDeltaTime);
    }

    void ChequearCombate()
    {
        if (EstaEnPausa()) return;

        float tirada = Random.value;
        Debug.Log("[ChequearCombate] Tirada: " + tirada.ToString("F3") + " | Necesario: < " + probabilidadCombate);

        if (tirada < probabilidadCombate && posiblesEnemigos.Length > 0)
        {
            Debug.Log("[ChequearCombate] ¡COMBATE! Iniciando transición...");
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
        Debug.Log("[TransicionBatalla] Iniciando fade...");
        panelTransicion.blocksRaycasts = true;

        while (panelTransicion.alpha < 1f)
        {
            panelTransicion.alpha += Time.deltaTime * 2f;
            yield return null;
        }

        Debug.Log("[TransicionBatalla] Cargando Battle...");
        SceneManager.LoadScene("Battle");
    }
}