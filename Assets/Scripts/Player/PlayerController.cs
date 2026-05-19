using UnityEngine;

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

        // Aplicar posición guardada si existe
        if (SistemaGuardado.instancia != null && SistemaGuardado.instancia.hayPosicionGuardada)
            SistemaGuardado.instancia.AplicarPosicionJugador();
    }

    bool EstaEnPausa()
    {
        return MenuPausaManager.instancia != null && MenuPausaManager.instancia.MenuActivo();
    }

    void Update()
    {
        // --- BLOQUEO DE DIÁLOGO ---
        if (DialogoManager.instancia != null && DialogoManager.instancia.EstaActivo())
        {
            moviX = 0;
            moviY = 0;
            animator.SetBool("Moviéndose", false);
            rb.velocity = Vector2.zero;
            return;
        }

        // --- BLOQUEO DE PAUSA ---
        if (EstaEnPausa())
        {
            moviX = 0;
            moviY = 0;
            animator.SetBool("Moviéndose", false);
            rb.velocity = Vector2.zero;
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
        if (DialogoManager.instancia != null && DialogoManager.instancia.EstaActivo()) return;
        if (EstaEnPausa()) return;

        bool corriendo = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
        float velocidad = corriendo ? velocidadCarrera : velocidadNormal;

        Vector2 direccion = new Vector2(moviX, moviY).normalized;
        rb.MovePosition(rb.position + direccion * velocidad * Time.fixedDeltaTime);
    }
}