using UnityEngine;

public class NPCCompañero : MonoBehaviour
{
    [Header("Distancia para interactuar")]
    public float distancia = 5f;

    [Header("Datos del jugador")]
    public DatosJugador datosJugador;

    [Header("Recompensa de despedida")]
    public int monedasDespedida = 10;

    public enum EstadoMision { EncuentroBosque, RobinOddDerrotado, YaEsCompañero, Despedida }
    public EstadoMision estadoActual = EstadoMision.EncuentroBosque;

    private Transform jugador;
    private bool esperandoUnion = false;
    private bool esperandoDespedida = false;

    private string[] dialogoEncuentro = new string[]
    {
        "¡¿Quién va ahí?! ¡Ah, no pareces uno de esos malditos bandidos de Robin Odd...!",
        "Esos malnacidos le han robado las provisiones a mi madre y el Cáliz Sagrado al pueblo.",
        "¡No pienso quedarme de brazos cruzados viendo cómo se salen con la suya!",
        "Sé que están escondidos un poco más adelante en la espesura del bosque...",
        "Oye, tú tienes pinta de saber defenderte. ¡Acompáñame y recuperemos lo que es nuestro!",
        "¡Pippin se une a tu grupo!"
    };

    private string[] dialogoVictoria = new string[]
    {
        "¡Toma ya! ¡Eso les enseñará a no volver a pisar nuestra aldea!",
        "Hemos recuperado el Cáliz, las provisiones y miralo... ¡el limo de los niños está a salvo!",
        "¡Madre mía, formamos un equipo increíble!",
        "Regresemos a la aldea para llevarle las cosas a mi madre, seguro que está preocupadísima."
    };

    private string[] dialogoAliadoFijo = new string[]
    {
        "¡Contigo iría hasta el fin del mundo, camarada!",
        "¿Cuál es nuestro próximo destino?"
    };

    private string[] dialogoDespedida = new string[]
    {
        "Pippin: ¡Lo hemos conseguido! ¡Robbin Odd ha caído y el pueblo está a salvo!",
        "Pippin: Oye... tengo que decirte algo. Ha sido un honor luchar a tu lado, de verdad.",
        "Pippin: Pero mi lugar está aquí, con mi madre y con la gente de esta aldea.",
        "Pippin: Ellos me necesitan. Y tú... tú tienes un camino mucho más grande por delante, lo sé.",
        "Pippin: Llévate esto contigo. Te entrega 10 monedas.",
        "Pippin: Si algún día vuelves por el bosque, ya sabes dónde encontrarme. ¡Hasta siempre, camarada!",
        "¡Pippin abandona el grupo!"
    };

    void Start()
    {
        GameObject obj = GameObject.FindGameObjectWithTag("Player");
        if (obj != null) jugador = obj.transform;
    }

    void OnEnable()
    {
        // Si ya está en modo despedida, lanzar la corrutina
        if (estadoActual == EstadoMision.Despedida && esperandoDespedida)
        {
            StartCoroutine(CorDespedida());
        }
        // Si Pippin ya está unido y no es despedida, desactivarse
        else if (MovimientoMapa.pippinUnido && estadoActual != EstadoMision.Despedida)
        {
            gameObject.SetActive(false);
        }
    }

    void Update()
    {
        if (jugador == null) return;

        float dist = Vector2.Distance(transform.position, jugador.position);

        // Espera a que termine el diálogo de unión
        if (esperandoUnion)
        {
            if (DialogoManagerCompañero.instancia != null && !DialogoManagerCompañero.instancia.EstaActivo())
            {
                esperandoUnion = false;
                MovimientoMapa.pippinUnido = true;
                gameObject.SetActive(false);
            }
            return;
        }

        // Durante despedida solo bloqueamos — el cierre lo gestiona CorDespedida
        if (esperandoDespedida) return;

        if (dist <= distancia && Input.GetKeyDown(KeyCode.X))
        {
            if (DialogoManagerCompañero.instancia != null && DialogoManagerCompañero.instancia.EstaActivo()) return;

            switch (estadoActual)
            {
                case EstadoMision.EncuentroBosque:
                    DialogoManagerCompañero.instancia.MostrarDialogo(dialogoEncuentro);
                    estadoActual = EstadoMision.YaEsCompañero;
                    esperandoUnion = true;
                    break;

                case EstadoMision.RobinOddDerrotado:
                    DialogoManagerCompañero.instancia.MostrarDialogo(dialogoVictoria);
                    break;

                case EstadoMision.YaEsCompañero:
                    DialogoManagerCompañero.instancia.MostrarDialogo(dialogoAliadoFijo);
                    break;

                case EstadoMision.Despedida:
                    // No hace nada, la despedida se lanza automáticamente desde IniciarDespedida()
                    break;
            }
        }
    }

    // Llamado desde NPCRobbinOdd después de su diálogo de derrota
    public void IniciarDespedida()
    {
        estadoActual = EstadoMision.Despedida;
        esperandoDespedida = true;
        // El boss hará SetActive(true) justo después, OnEnable lanzará CorDespedida
    }

    System.Collections.IEnumerator CorDespedida()
    {
        // Esperar 2 frames para que el objeto esté completamente inicializado
        yield return null;
        yield return null;

        Debug.Log("[Pippin] Mostrando diálogo de despedida");

        if (DialogoManagerCompañero.instancia == null)
        {
            Debug.LogError("[Pippin] DialogoManagerCompañero.instancia es NULL");
            yield break;
        }

        DialogoManagerCompañero.instancia.MostrarDialogo(dialogoDespedida);

        // Esperar un frame a que el diálogo arranque antes de comprobar si terminó
        yield return null;

        // Esperar a que el jugador cierre el diálogo completo
        yield return new WaitUntil(() => !DialogoManagerCompañero.instancia.EstaActivo());

        // Entregar monedas al terminar el diálogo
        if (datosJugador != null)
        {
            datosJugador.oro += monedasDespedida;
            Debug.Log("[Pippin] Entregadas " + monedasDespedida + " monedas. Oro total: " + datosJugador.oro);
        }
        else
        {
            Debug.LogWarning("[Pippin] datosJugador es NULL — arrastra el ScriptableObject al Inspector de Pippin");
        }

        // Limpieza final
        esperandoDespedida = false;
        MovimientoMapa.pippinUnido = false;
        gameObject.SetActive(false);
    }

    public void CambiarEstado(EstadoMision nuevoEstado)
    {
        estadoActual = nuevoEstado;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, distancia);
    }
}