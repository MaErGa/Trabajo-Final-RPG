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

    public static DatosEnemigo enemigoSeleccionado;
    public static Vector3 posicionRetorno;
    public static bool vieneDeCombate = false;
    public static string escenaOrigen = "";
    public static bool pippinUnido = false;

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

        if (panelTransicion != null) panelTransicion.alpha = 0;
    }

    bool EstaEnPausa()
    {
        return MenuPausaManager.instancia != null && MenuPausaManager.instancia.MenuActivo();
    }

    // Centraliza todos los bloqueos de movimiento en un solo lugar
    bool MovimientoBloqueado()
    {
        if (iniciandoCombate) return true;
        if (DialogoManager.instancia != null && DialogoManager.instancia.EstaActivo()) return true;
        if (EstaEnPausa()) return true;
        if (Cofre.dialogoActivo) return true; // ← bloqueo por cofre
        return false;
    }

    void Update()
    {
        if (MovimientoBloqueado())
        {
            moviX = 0; moviY = 0;
            if (animator != null) animator.SetBool("Moviéndose", false);
            if (rb != null) rb.velocity = Vector2.zero;

            // Detener el chequeo de combate si estaba activo
            if (estaCaminando)
            {
                estaCaminando = false;
                CancelInvoke("ChequearCombate");
            }
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
            estaCaminando = false;
            CancelInvoke("ChequearCombate");
        }
    }

    void FixedUpdate()
    {
        if (MovimientoBloqueado()) return;

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