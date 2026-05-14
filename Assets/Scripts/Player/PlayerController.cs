using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    [Header("Velocidades")]
    public float velocidadNormal = 5f;
    public float velocidadCarrera = 8f;

    [Header("Configuracion de Enemigos")]
    public float probabilidadCombate = 0.1f;
    public DatosEnemigo[] posiblesEnemigos;
    public static DatosEnemigo enemigoSeleccionado;

    public static Vector3 posicionRetorno;
    public static bool vieneDeCombate = false;

    private Rigidbody2D rb;
    private Animator animator;

    private float moviX;
    private float moviY;

    private bool estaCaminando = false;
    private bool estaLeyendo = false; // Nueva variable para bloquear el movimiento al leer

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        // Dirección inicial: Mirando abajo
        animator.SetFloat("MovimientoX", 0);
        animator.SetFloat("MovimientoY", -1);

        if (vieneDeCombate)
        {
            transform.position = posicionRetorno;
            vieneDeCombate = false;
        }
    }

    void Update()
    {
        // Si estamos leyendo la tablilla, no procesamos el movimiento
        if (estaLeyendo)
        {
            CheckControlesDialogo();
            return;
        }

        moviX = Input.GetAxisRaw("Horizontal");
        moviY = Input.GetAxisRaw("Vertical");

        bool seEstaMoviendo = (moviX != 0 || moviY != 0);
        animator.SetBool("Moviéndose", seEstaMoviendo);

        if (seEstaMoviendo)
        {
            // Enviamos los valores solo cuando hay movimiento para que el Idle no salte a la izquierda
            if (Mathf.Abs(moviX) > Mathf.Abs(moviY))
            {
                animator.SetFloat("MovimientoX", moviX);
                animator.SetFloat("MovimientoY", 0);
            }
            else
            {
                animator.SetFloat("MovimientoX", 0);
                animator.SetFloat("MovimientoY", moviY);
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

        // Simulación de interacción con la tablilla (puedes llamar a ActivarLectura desde otro script)
        if (Input.GetKeyDown(KeyCode.X))
        {
            // Aquí iría tu lógica para detectar si hay una tablilla cerca
        }
    }

    void FixedUpdate()
    {
        if (estaLeyendo)
        {
            rb.velocity = Vector2.zero; // Detenemos al personaje en seco
            return;
        }

        bool corriendo = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
        float velocidad = corriendo ? velocidadCarrera : velocidadNormal;

        Vector2 direccion = new Vector2(moviX, moviY).normalized;
        rb.MovePosition(rb.position + direccion * velocidad * Time.fixedDeltaTime);
    }

    // Lógica para los botones X y C
    void CheckControlesDialogo()
    {
        if (Input.GetKeyDown(KeyCode.X))
        {
            Debug.Log("X pulsada: Siguiente línea de diálogo.");
            // Aquí pones la función de tu sistema de diálogo para avanzar
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            Debug.Log("C pulsada: Cerrar tablilla.");
            estaLeyendo = false; // Devolvemos el control al jugador
            // Aquí ocultas tu panel de diálogo
        }
    }

    // Lógica de combate corregida
    void ChequearCombate()
    {
        if (posiblesEnemigos != null && posiblesEnemigos.Length > 0)
        {
            if (Random.value < probabilidadCombate)
            {
                int indice = Random.Range(0, posiblesEnemigos.Length);
                enemigoSeleccionado = posiblesEnemigos[indice];
                posicionRetorno = transform.position;
                vieneDeCombate = true;
                SceneManager.LoadScene("Battle");
            }
        }
    }

    // Llama a esto cuando el jugador interactúe con la tablilla
    public void ActivarLectura()
    {
        estaLeyendo = true;
        animator.SetBool("Moviéndose", false);
    }
}