using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class MovimientoMapa : MonoBehaviour
{
    [Header("Velocidades")]
    public float velocidadNormal = 5f;
    public float velocidadCarrera = 8f;

    [Header("Combate")]
    public float probabilidadCombate = 0.1f;
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

        Debug.Log("escenaOrigen al cargar: '" + escenaOrigen + "'");
        Debug.Log("vieneDeCombate: " + vieneDeCombate);

        if (vieneDeCombate)
        {
            // Vuelve de combate: restaura posición exacta antes del combate
            transform.position = posicionRetorno;
            vieneDeCombate = false;
        }
        else if (!string.IsNullOrEmpty(escenaOrigen))
        {
            // Viene de otra escena: busca punto de entrada
            bool colocado = false;
            EntradaEscena[] entradas = FindObjectsOfType<EntradaEscena>();
            foreach (EntradaEscena entrada in entradas)
            {
                if (entrada.escenaOrigen == escenaOrigen)
                {
                    transform.position = entrada.transform.position;
                    colocado = true;
                    Debug.Log("Colocado en entrada: " + entrada.transform.position);
                    break;
                }
            }
            if (!colocado)
                Debug.Log("No se encontró EntradaEscena para: " + escenaOrigen);
        }
        else
        {
            // Primera carga: usa posición guardada si existe
            if (SistemaGuardado.instancia != null && SistemaGuardado.instancia.hayPosicionGuardada)
                SistemaGuardado.instancia.AplicarPosicionJugador();
        }

        if (panelTransicion != null) panelTransicion.alpha = 0;
    }

    bool EstaEnPausa()
    {
        return MenuPausaManager.instancia != null && MenuPausaManager.instancia.MenuActivo();
    }

    void Update()
    {
        if (iniciandoCombate) return;

        // Bloqueo diálogo
        if (DialogoManager.instancia != null && DialogoManager.instancia.EstaActivo())
        {
            moviX = 0; moviY = 0;
            if (animator != null) animator.SetBool("Moviéndose", false);
            if (rb != null) rb.velocity = Vector2.zero;
            return;
        }

        // Bloqueo pausa
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

            // Chequeo de combate por pasos
            if (!estaCaminando)
            {
                estaCaminando = true;
                InvokeRepeating("ChequearCombate", 0.5f, 0.5f);
            }
        }
        else
        {
            estaCaminando = false;
            CancelInvoke("ChequearCombate");
        }
    }

    void FixedUpdate()
    {
        if (iniciandoCombate) return;
        if (DialogoManager.instancia != null && DialogoManager.instancia.EstaActivo()) return;
        if (EstaEnPausa()) return;

        bool corriendo = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
        float velocidad = corriendo ? velocidadCarrera : velocidadNormal;

        Vector2 direccion = new Vector2(moviX, moviY).normalized;
        rb.MovePosition(rb.position + direccion * velocidad * Time.fixedDeltaTime);
    }

    void ChequearCombate()
    {
        if (EstaEnPausa()) return;

        if (Random.value < probabilidadCombate && posiblesEnemigos.Length > 0)
        {
            iniciandoCombate = true;
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
        if (panelTransicion != null)
        {
            while (panelTransicion.alpha < 1)
            {
                panelTransicion.alpha += Time.deltaTime * 2f;
                yield return null;
            }
        }
        SceneManager.LoadScene("Battle");
    }
}