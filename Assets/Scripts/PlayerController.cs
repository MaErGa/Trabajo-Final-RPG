using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
   // Movimiento personaje
    [SerializeField] private float velocidadMovimiento;
    [SerializeField] private float velocidadCarrera = 8f;
    [SerializeField] private Vector2 direccion;

    private Rigidbody2D rb2d;
    private float movimientoX;
    private float movimientoY;
    private Animator animator;
    private bool corriendo;

    //Interaccion del personaje con el entorno

    [Header("Interacción")]
    [Tooltip("Distancia a la que el jugador detecta objetos/NPCs con los que interactuar.")]
    [SerializeField] private float distanciaInteraccion = 1.2f;

    [Tooltip("Capas que contienen NPCs, objetos, cofres, etc.")]
    [SerializeField] private LayerMask capaInteractuable;

      public bool PuedeMoverse { get; set; } = true;

    void Start()
    {
        animator = GetComponent<Animator>();
        rb2d     = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (PuedeMoverse)
            LeerMovimiento();

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

    // Acciones del jugador para accion y cancelar.

    private void LeerBotones()
    {
        // Accion (para hablar y aceptar en el menu) con Z, Barra Espaciadora, o Return
        if (Input.GetKeyDown(KeyCode.Z)      ||
            Input.GetKeyDown(KeyCode.Return) ||
            Input.GetKeyDown(KeyCode.Space)  ||
            Input.GetButtonDown("Submit"))
        {
            PulsarAccion();
        }

        // Boton para cancelar y cerrar menus acciones, X,escape. 
        
        if (Input.GetKeyDown(KeyCode.X)      ||
            Input.GetKeyDown(KeyCode.Escape) ||
            Input.GetButtonDown("Cancel"))
        {
            PulsarCancelar();
        }
    }

    
    private void PulsarAccion()
    {
        
        Vector2 dirMirada = new Vector2(
            animator.GetFloat("UltimoX"),
            animator.GetFloat("UltimoY")
        ).normalized;

        
        if (dirMirada == Vector2.zero)
            dirMirada = Vector2.down;

        
        RaycastHit2D hit = Physics2D.Raycast(
            rb2d.position,
            dirMirada,
            distanciaInteraccion,
            capaInteractuable
        );

        
        Debug.DrawRay(rb2d.position, dirMirada * distanciaInteraccion, Color.yellow, 0.5f);

        if (hit.collider != null)
        {
            
            IInteractuable interactuable = hit.collider.GetComponentInParent<IInteractuable>();

            if (interactuable != null)
            {
                interactuable.Interactuar();
                Debug.Log($"[PlayerController] Interactuando con: {hit.collider.name}");
            }
        }
        else
        {
            Debug.Log("[PlayerController] Acción pulsada — nada delante.");
        }
    }

    
    
    private void PulsarCancelar()
    {
        Debug.Log("[PlayerController] Cancelar pulsado.");
        // Aquí conectas con tu sistema de menú/diálogo:
        // Ejemplo: MenuManager.Instancia.Cancelar();
        //          DialogoManager.Instancia.Cancelar();
    }
}

// ════════════════════════════════════════════════════════════════
//  INTERFAZ INTERACTUABLE
//  Cualquier NPC, cofre, objeto u letrero debe implementar esta
//  interfaz para ser detectado por el botón de Acción.

//  Ejemplo de uso en un NPC:
//
//    public class NPC : MonoBehaviour, IInteractuable
//    {
//        public void Interactuar()
//        {
//            DialogoManager.Instancia.MostrarDialogo("¡Hola aventurero!");
//        }
//    }

public interface IInteractuable
{
    void Interactuar();
}
