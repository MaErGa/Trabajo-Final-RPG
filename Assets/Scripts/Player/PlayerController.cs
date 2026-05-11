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
    // Empieza mirando hacia abajo (como en los RPG clásicos)
    private float ultimoX = 0f;
    private float ultimoY = -1f;

    // Start se ejecuta UNA VEZ al arrancar el juego
    void Start()
    {
        // Buscamos los componentes que están en este mismo objeto
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        // Le decimos al Animator que empiece mirando hacia abajo
        animator.SetFloat("UltimoX", ultimoX);
        animator.SetFloat("UltimoY", ultimoY);
    }

    // Update se ejecuta en CADA FOTOGRAMA
    void Update()
    {
        // Leemos las teclas WASD o las flechas del teclado
        moviX = Input.GetAxisRaw("Horizontal"); // -1 izquierda, 0 nada, 1 derecha
        moviY = Input.GetAxisRaw("Vertical");   // -1 abajo,     0 nada, 1 arriba

        // Le decimos al Animator si nos estamos moviendo o no
        // Usamos Mathf.Abs para saber si hay movimiento en cualquier dirección
        bool seEstaMoviendo = (moviX != 0 || moviY != 0);
        animator.SetBool("Moviéndose", seEstaMoviendo);

        // Actualizamos el Animator con la dirección actual de movimiento
        animator.SetFloat("MovimientoX", moviX);
        animator.SetFloat("MovimientoY", moviY);

        // Solo guardamos la última dirección cuando hay movimiento real
        // Así el personaje queda mirando hacia donde iba cuando se para
        if (seEstaMoviendo)
        {
            ultimoX = moviX;
            ultimoY = moviY;

            animator.SetFloat("UltimoX", ultimoX);
            animator.SetFloat("UltimoY", ultimoY);
        }
    }

    // FixedUpdate se usa para físicas (movimiento con Rigidbody)
    // Se ejecuta a un ritmo fijo, independiente del framerate
    void FixedUpdate()
    {
        // Comprobamos si está pulsado Shift para correr
        bool corriendo = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
        float velocidad = corriendo ? velocidadCarrera : velocidadNormal;

        // Creamos la dirección y la normalizamos
        // (normalizar evita que ir en diagonal sea más rápido)
        Vector2 direccion = new Vector2(moviX, moviY).normalized;

        // Movemos el personaje
        rb.MovePosition(rb.position + direccion * velocidad * Time.fixedDeltaTime);
    }
}