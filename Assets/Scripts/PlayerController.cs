using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 3.7f;
    private Rigidbody2D rb2d;
    private Vector2 movementInput;
    private Animator animator;
    public float side;
    public Vector2 _input;

    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        movementInput.x = Input.GetAxisRaw("Horizontal");
        movementInput.y = Input.GetAxisRaw("Vertical");

        movementInput = movementInput.normalized;

        animator.SetFloat("Horizontal", movementInput.x);
        animator.SetFloat("Vertical", movementInput.y);
        animator.SetFloat("Speed", movementInput.magnitude);


    }
    public void Move()
    {
        side = _input.x;
        float UpDown = _input.y;
        if (side > 0)
        {
            animator.SetBool("movimientoDerecha", true);
            animator.SetBool("movimientoIzquierda", false);
        }
        if (side == 0)
        {
            animator.SetBool("movimientoIzquierda", false);
            animator.SetBool("movimientoDerecha", false);
        }
        if (side < 0)
        {
            animator.SetBool("movimientoDerecha", false);
            animator.SetBool("movimientoIzquierda", true);
        }
        if (UpDown > 0)
        {
            animator.SetBool("arriba", true);
            animator.SetBool("abajo", false);
        }
        if (UpDown == 0)
        {
            animator.SetBool("arriba", false);
            animator.SetBool("abajo", false);
        }
        if (UpDown < 0)
        {
            animator.SetBool("arriba", false);
            animator.SetBool("abajo", true);
        }
        if (side == 0 && UpDown == 0)//si lado y arriba abajo es 0, movimiento falso
        {
            animator.SetBool("moving", false);
        }
        if (side != 0 || UpDown != 0)//si lado o arriba abajo es diferente a 0, movimiento verdadero
        {
            animator.SetBool("moving", true);
        }

    }

    private void FixedUpdate()
    {
        rb2d.velocity = movementInput * speed;
    }
}