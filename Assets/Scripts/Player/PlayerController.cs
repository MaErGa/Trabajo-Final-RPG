using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    [Header("Velocidades")]
    public float velocidadNormal = 5f;
    public float velocidadCarrera = 8f;

    private Rigidbody2D rb;
    private Animator animator;

    private float moviX;
    private float moviY;

    private float ultimoX = 0f;
    private float ultimoY = -1f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        if (SistemaGuardado.instancia != null && SistemaGuardado.instancia.hayPosicionGuardada)
            SistemaGuardado.instancia.AplicarPosicionJugador();
    }

    bool HayDialogoActivo()
    {
        if (DialogoManager.instancia != null && DialogoManager.instancia.EstaActivo()) return true;
        if (DialogoManagerBoss.instancia != null && DialogoManagerBoss.instancia.EstaActivo()) return true;
        if (DialogoManagerCompañero.instancia != null && DialogoManagerCompañero.instancia.EstaActivo()) return true;
        if (DialogoManagerMadre.instancia != null && DialogoManagerMadre.instancia.EstaActivo()) return true;
        if (DialogoManagerMonja.instancia != null && DialogoManagerMonja.instancia.EstaActivo()) return true;
        if (DialogoManagerViejo.instancia != null && DialogoManagerViejo.instancia.EstaActivo()) return true;
        return false;
    }

    bool HayTiendaAbierta()
    {
        TiendaFF[] tiendas = FindObjectsOfType<TiendaFF>();
        foreach (var t in tiendas)
            if (t.EstaAbierta()) return true;
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
        // ── BLOQUEO TOTAL si hay pausa, diálogo o tienda ──
        if (EstaEnPausa() || HayDialogoActivo() || HayTiendaAbierta())
        {
            moviX = 0;
            moviY = 0;
            if (rb != null) rb.velocity = Vector2.zero;
            if (animator != null) animator.SetBool("Moviéndose", false);
            return;
        }

        moviX = Input.GetAxisRaw("Horizontal");
        moviY = Input.GetAxisRaw("Vertical");

        bool seEstaMoviendo = (moviX != 0 || moviY != 0);
        animator.SetBool("Moviéndose", seEstaMoviendo);

        if (seEstaMoviendo)
        {
            if (Mathf.Abs(moviX) > Mathf.Abs(moviY))
            {
                animator.SetFloat("MovimientoX", moviX);
                animator.SetFloat("MovimientoY", 0);
                ultimoX = moviX;
                ultimoY = 0;
            }
            else
            {
                animator.SetFloat("MovimientoX", 0);
                animator.SetFloat("MovimientoY", moviY);
                ultimoX = 0;
                ultimoY = moviY;
            }
        }
    }

    void FixedUpdate()
    {
        if (EstaEnPausa() || HayDialogoActivo() || HayTiendaAbierta()) return;

        bool corriendo = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
        float velocidad = corriendo ? velocidadCarrera : velocidadNormal;

        Vector2 direccion = new Vector2(moviX, moviY).normalized;
        rb.MovePosition(rb.position + direccion * velocidad * Time.fixedDeltaTime);
    }
}