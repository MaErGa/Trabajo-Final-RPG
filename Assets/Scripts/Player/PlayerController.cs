using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Ajustes de Movimiento")]
    [SerializeField] private float velocidadMovimiento = 5f;
    [SerializeField] private float velocidadCarrera = 8f;
    [SerializeField] private Vector2 direccion;

    private Rigidbody2D rb2d;
    private float movimientoX;
    private float movimientoY;
    private Animator animator;
    private bool corriendo;

    // Guardamos la última dirección para el idle correcto
    // Empieza mirando hacia abajo (como la mayoría de juegos RPG)
    private float ultimoX = 0f;
    private float ultimoY = -1f;

    [Header("Interacción")]
    [SerializeField] private float distanciaInteraccion = 1.2f;
    [SerializeField] private LayerMask capaInteractuable;

    [Header("Sistema de Combate")]
    [SerializeField] private GameObject objetoBattleCanva;
    [SerializeField] private LayerMask capaHierba;
    [SerializeField] private int probabilidadEncuentro = 5;

    public bool PuedeMoverse { get; set; } = true;

    void Start()
    {
        animator = GetComponent<Animator>();
        rb2d = GetComponent<Rigidbody2D>();

        // Le decimos al animator la dirección inicial (mirando abajo)
        animator.SetFloat("UltimoX", ultimoX);
        animator.SetFloat("UltimoY", ultimoY);
    }

    void Update()
    {
        if (PuedeMoverse)
        {
            LeerMovimiento();

            if (movimientoX != 0 || movimientoY != 0)
            {
                ComprobarEncuentroAleatorio();
            }
        }
        else
        {
            // Si no puede moverse, paramos el movimiento
            movimientoX = 0;
            movimientoY = 0;
            animator.SetFloat("MovimientoX", 0);
            animator.SetFloat("MovimientoY", 0);
        }

        LeerBotones();
    }

    private void FixedUpdate()
    {
        if (!PuedeMoverse)
        {
            rb2d.velocity = Vector2.zero;
            return;
        }

        float velocidadActual = corriendo ? velocidadCarrera : velocidadMovimiento;
        rb2d.MovePosition(rb2d.position + direccion * velocidadActual * Time.fixedDeltaTime);
    }

    private void LeerMovimiento()
    {
        movimientoX = Input.GetAxisRaw("Horizontal");
        movimientoY = Input.GetAxisRaw("Vertical");
        corriendo = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);

        // Pasamos el movimiento actual al animator para que sepa si caminar o no
        animator.SetFloat("MovimientoX", movimientoX);
        animator.SetFloat("MovimientoY", movimientoY);

        // Calculamos la dirección normalizada para el movimiento físico
        direccion = new Vector2(movimientoX, movimientoY).normalized;

        // Velocidad le dice al animator si estamos moviéndonos o no
        // 1 = caminando, 0 = parado (esto evita el conflicto entre idle y caminar)
        animator.SetFloat("Velocidad", direccion.magnitude);

        // --- AQUÍ ESTABA EL BUG ---
        // Solo guardamos la última dirección cuando hay movimiento real.
        // Además guardamos los valores en variables propias del script,
        // no solo en el animator. Así evitamos que el idle de "arriba"
        // se confunda con caminar hacia arriba.
        if (movimientoX != 0 || movimientoY != 0)
        {
            ultimoX = movimientoX;
            ultimoY = movimientoY;

            // Actualizamos el animator con la última dirección conocida
            animator.SetFloat("UltimoX", ultimoX);
            animator.SetFloat("UltimoY", ultimoY);
        }
        // Si no hay movimiento, no tocamos UltimoX/UltimoY para que el
        // idle se quede en la dirección correcta que venía caminando
    }

    private void ComprobarEncuentroAleatorio()
    {
        if (Physics2D.OverlapCircle(rb2d.position, 0.2f, capaHierba))
        {
            Debug.Log("PISANDO HIERBA");
            if (Random.Range(1, 1001) <= probabilidadEncuentro)
            {
                IniciarCombate();
            }
        }
    }

    private void LeerBotones()
    {
        if (Input.GetKeyDown(KeyCode.Z) || Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space))
        {
            PulsarAction();
        }
    }

    private void PulsarAction()
    {
        // Usamos las variables del script en vez de leer el animator
        // (más fiable y evita problemas de sincronización)
        Vector2 dirMirada = new Vector2(ultimoX, ultimoY).normalized;
        if (dirMirada == Vector2.zero) dirMirada = Vector2.down;

        RaycastHit2D hit = Physics2D.Raycast(rb2d.position, dirMirada, distanciaInteraccion, capaInteractuable);

        if (hit.collider != null)
        {
            IInteractuable interactuable = hit.collider.GetComponentInParent<IInteractuable>();
            if (interactuable != null) interactuable.Interactuar();
        }
    }

    public void IniciarCombate()
    {
        if (objetoBattleCanva != null)
        {
            PuedeMoverse = false;
            objetoBattleCanva.SetActive(true);

            // Paramos la animación de caminar al entrar en combate
            animator.SetFloat("MovimientoX", 0);
            animator.SetFloat("MovimientoY", 0);
        }
    }
}

public interface IInteractuable { void Interactuar(); }