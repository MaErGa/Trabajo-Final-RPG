using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // Estas variables aparecen en el Inspector de Unity para que puedas cambiarlas fácilmente
    [Header("Velocidades")]
    public float velocidadNormal = 5f;
    public float velocidadCarrera = 8f;

    // Variables privadas (solo las usa este script)
    private Rigidbody2D rb;
    private Animator animator;

    private float moviX;
    private float moviY;

    // Guardamos hacia donde miraba el personaje por última vez
    private float ultimoX = 0f;
    private float ultimoY = -1f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        // Dirección inicial
        animator.SetFloat("UltimoX", ultimoX);
        animator.SetFloat("UltimoY", ultimoY);
    }

    void Update()
    {
        // --- BLOQUEO DE DIÁLOGO ---
        // Si el manager de diálogo dice que está activo, el personaje no hace nada
        if (DialogoManager.instancia != null && DialogoManager.instancia.EstaActivo())
        {
            moviX = 0;
            moviY = 0;
            animator.SetBool("Moviéndose", false);
            rb.velocity = Vector2.zero; // Forzamos que se detenga físicamente
            return; // Salimos del Update para no leer el teclado
        }

        // Leemos las teclas WASD o las flechas del teclado
        moviX = Input.GetAxisRaw("Horizontal");
        moviY = Input.GetAxisRaw("Vertical");

        // Determinamos si hay movimiento real
        bool seEstaMoviendo = (moviX != 0 || moviY != 0);
        animator.SetBool("Moviéndose", seEstaMoviendo);

        if (seEstaMoviendo)
        {
            // Prioridad de eje para evitar el parpadeo visual
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

            animator.SetFloat("UltimoX", ultimoX);
            animator.SetFloat("UltimoY", ultimoY);
        }
    }

    void FixedUpdate()
    {
        // No movemos el Rigidbody si estamos en un diálogo
        if (DialogoManager.instancia != null && DialogoManager.instancia.EstaActivo()) return;

        bool corriendo = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
        float velocidad = corriendo ? velocidadCarrera : velocidadNormal;

        Vector2 direccion = new Vector2(moviX, moviY).normalized;
        rb.MovePosition(rb.position + direccion * velocidad * Time.fixedDeltaTime);
    }
}