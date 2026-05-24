using UnityEngine;

public class NPCCompañero : MonoBehaviour
{
    [Header("Distancia para interactuar")]
    public float distancia = 5f;

    public enum EstadoMision { EncuentroBosque, RobinOddDerrotado, YaEsCompañero }
    public EstadoMision estadoActual = EstadoMision.EncuentroBosque;

    private Transform jugador;
    private bool esperandoUnion = false;

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

    void Start()
    {
        GameObject obj = GameObject.FindGameObjectWithTag("Player");
        if (obj != null) jugador = obj.transform;

        // Si ya se unió antes, ocultar sprite directamente
        if (MovimientoMapa.pippinUnido)
        {
            gameObject.SetActive(false);
        }
    }

    void Update()
    {
        if (jugador == null) return;

        float dist = Vector2.Distance(transform.position, jugador.position);

        // Detectar fin de diálogo para hacer desaparecer a Pippin
        if (esperandoUnion)
        {
            if (DialogoManagerCompañero.instancia != null && !DialogoManagerCompañero.instancia.EstaActivo())
            {
                esperandoUnion = false;
                MovimientoMapa.pippinUnido = true;
                gameObject.SetActive(false); // Desaparece del escenario
            }
            return;
        }

        if (dist <= distancia && Input.GetKeyDown(KeyCode.X))
        {
            if (DialogoManagerCompañero.instancia != null && DialogoManagerCompañero.instancia.EstaActivo()) return;

            switch (estadoActual)
            {
                case EstadoMision.EncuentroBosque:
                    DialogoManagerCompañero.instancia.MostrarDialogo(dialogoEncuentro);
                    estadoActual = EstadoMision.YaEsCompañero;
                    esperandoUnion = true; // Espera a que termine el diálogo
                    break;

                case EstadoMision.RobinOddDerrotado:
                    DialogoManagerCompañero.instancia.MostrarDialogo(dialogoVictoria);
                    break;

                case EstadoMision.YaEsCompañero:
                    DialogoManagerCompañero.instancia.MostrarDialogo(dialogoAliadoFijo);
                    break;
            }
        }
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