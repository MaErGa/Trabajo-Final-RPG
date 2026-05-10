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

        animator.SetFloat("MovimientoX", movimientoX);
        animator.SetFloat("MovimientoY", movimientoY);
        direccion = new Vector2(movimientoX, movimientoY).normalized;

        if (movimientoX != 0 || movimientoY != 0)
        {
            animator.SetFloat("UltimoX", movimientoX);
            animator.SetFloat("UltimoY", movimientoY);
        }
    }

    private void ComprobarEncuentroAleatorio()
    {
        // Esto nos dirá en consola si el sensor detecta la capa correcta
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
        Vector2 dirMirada = new Vector2(animator.GetFloat("UltimoX"), animator.GetFloat("UltimoY")).normalized;
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
        }
    }
}

public interface IInteractuable { void Interactuar(); }