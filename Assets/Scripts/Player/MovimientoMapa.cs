using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

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
    public CanvasGroup panelTransicion;

    public static Vector3 posicionRetorno;
    public static bool vieneDeCombate = false;
    public static string escenaOrigen = "";

    private bool estaCaminando = false;
    private bool iniciandoCombate = false;

    void Start()
    {
        if (vieneDeCombate)
        {
            transform.position = posicionRetorno;
            vieneDeCombate = false;
        }
        else if (SistemaGuardado.instancia != null && SistemaGuardado.instancia.hayPosicionGuardada)
        {
            SistemaGuardado.instancia.AplicarPosicionJugador();
        }

        if (panelTransicion != null) panelTransicion.alpha = 0;
    }

    bool EstaEnPausa()
    {
        return MenuPausaManager.instancia != null && MenuPausaManager.instancia.MenuActivo();
    }

    void Update()
    {
        if (iniciandoCombate) return;

        // --- BLOQUEO DE PAUSA ---
        if (EstaEnPausa()) return;

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
        if (EstaEnPausa()) return;

        if (Random.value < probabilidadCombate && posiblesEnemigos.Length > 0)
        {
            iniciandoCombate = true;
            int indice = Random.Range(0, posiblesEnemigos.Length);
            enemigoSeleccionado = posiblesEnemigos[indice];
            posicionRetorno = transform.position;
            vieneDeCombate = true;
            escenaOrigen = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;

            StartCoroutine(TransicionBatalla());
        }
    }

    IEnumerator TransicionBatalla()
    {
        if (panelTransicion != null)
        {
            while (panelTransicion.alpha < 1)
            {
                panelTransicion.alpha += Time.deltaTime * 2f;
                yield return null;
            }
        }
        SceneManager.LoadScene("Battle");
    }
}