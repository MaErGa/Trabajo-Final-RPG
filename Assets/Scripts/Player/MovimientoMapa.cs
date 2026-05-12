using UnityEngine;
using UnityEngine.SceneManagement;

public class MovimientoMapa : MonoBehaviour
{
    [Header("Ajustes de Velocidad")]
    public float velocidadCaminar = 5f;
    public float velocidadCorrer = 6f; // Ajustado a 6 como sugeriste
    public float probabilidadCombate = 0.1f;
    
    [Header("Configuración de Enemigos")]
    public DatosEnemigo[] posiblesEnemigos; 
    public static DatosEnemigo enemigoSeleccionado;

    // Variables estáticas para recordar la posición al volver de batalla
    public static Vector3 posicionRetorno; 
    public static bool vieneDeCombate = false;

    private bool estaCaminando = false;

    void Start()
    {
        // Al volver de la escena "Battle", nos situamos donde empezó el lío
        if (vieneDeCombate)
        {
            transform.position = posicionRetorno;
            vieneDeCombate = false; 
        }
    }

    void Update()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");
        Vector2 movimiento = new Vector2(moveX, moveY).normalized;

        if (movimiento != Vector2.zero)
        {
            // Lógica para correr con Shift Izquierdo
            float velocidadActual = velocidadCaminar;
            if (Input.GetKey(KeyCode.LeftShift)) 
            {
                velocidadActual = velocidadCorrer;
            }

            transform.Translate(movimiento * velocidadActual * Time.deltaTime);

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

    void ChequearCombate()
    {
        if (Random.value < probabilidadCombate && posiblesEnemigos.Length > 0)
        {
            int indice = Random.Range(0, posiblesEnemigos.Length);
            enemigoSeleccionado = posiblesEnemigos[indice];

            // Guardamos la posición actual antes de cargar la batalla
            posicionRetorno = transform.position;
            vieneDeCombate = true;

            SceneManager.LoadScene("Battle"); 
        }
    }
}