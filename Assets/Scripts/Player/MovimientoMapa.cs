using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections; // Necesario para las corrutinas

public class MovimientoMapa : MonoBehaviour
{
    [Header("Ajustes de Velocidad")]
    public float velocidadCaminar = 5f;
    public float velocidadCorrer = 6f; 
    public float probabilidadCombate = 0.1f;
    
    [Header("Configuración de Enemigos")]
    public DatosEnemigo[] posiblesEnemigos; 
    public static DatosEnemigo enemigoSeleccionado;

    [Header("Transición")]
    public CanvasGroup panelTransicion; // Arrastra aquí el Canvas Group del panel negro

    public static Vector3 posicionRetorno; 
    public static bool vieneDeCombate = false;

    private bool estaCaminando = false;
    private bool iniciandoCombate = false; // Bloqueo para evitar múltiples cargas

    void Start()
    {
        if (vieneDeCombate)
        {
            transform.position = posicionRetorno;
            vieneDeCombate = false; 
        }
        if(panelTransicion != null) panelTransicion.alpha = 0; // Empieza invisible
    }

    void Update()
    {
        if (iniciandoCombate) return;

        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");
        Vector2 movimiento = new Vector2(moveX, moveY).normalized;

        if (movimiento != Vector2.zero)
        {
            float velocidadActual = velocidadCaminar;
            if (Input.GetKey(KeyCode.LeftShift)) velocidadActual = velocidadCorrer;

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
            iniciandoCombate = true;
            int indice = Random.Range(0, posiblesEnemigos.Length);
            enemigoSeleccionado = posiblesEnemigos[indice];
            posicionRetorno = transform.position;
            vieneDeCombate = true;

            StartCoroutine(TransicionBatalla());
        }
    }

    IEnumerator TransicionBatalla()
    {
        if (panelTransicion != null)
        {
            while (panelTransicion.alpha < 1)
            {
                panelTransicion.alpha += Time.deltaTime * 2f; // Velocidad del fundido
                yield return null;
            }
        }
        SceneManager.LoadScene("Battle");
    }
}