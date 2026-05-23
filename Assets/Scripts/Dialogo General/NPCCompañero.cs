using UnityEngine;

public class NPCCompañero : MonoBehaviour
{
    [Header("Distancia para interactuar")]
    public float distancia = 5f;

    // Estados: 
    // 1. Cuando le encuentras al inicio del bosque furioso
    // 2. Cuando derriban a Robin Odd
    // 3. Cuando ya se une a tu grupo y se vuelve tu aliado
    public enum EstadoMision { EncuentroBosque, RobinOddDerrotado, YaEsCompañero }
    public EstadoMision estadoActual = EstadoMision.EncuentroBosque;

    private Transform jugador;

    private string[] dialogoEncuentro = new string[]
    {
        "¡¿Quién va ahí?! ¡Ah, no pareces uno de esos malditos bandidos de Robin Odd...!",
        "Esos malnacidos le han robado las provisiones a mi madre y el Cáliz Sagrado al pueblo.",
        "¡No pienso quedarme de brazos cruzados viendo cómo se salen con la suya!",
        "Sé que están escondidos un poco más adelante en la espesura del bosque...",
        "Oye, tú tienes pinta de saber defenderte. ¡Acompáñame y recuperemos lo que es nuestro!"
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
        // Busca al jugador por su Tag
        GameObject obj = GameObject.FindGameObjectWithTag("Player");
        if (obj != null) jugador = obj.transform;
    }

    void Update()
    {
        if (jugador == null) return;

        // Calcula la distancia idéntica al sistema de la madre y el viejo
        float dist = Vector2.Distance(transform.position, jugador.position);

        if (dist <= distancia && Input.GetKeyDown(KeyCode.X))
        {
            // Evita reiniciar si el cuadro de Pippin ya está activo
            if (DialogoManagerCompañero.instancia != null && DialogoManagerCompañero.instancia.EstaActivo()) return;

            switch (estadoActual)
            {
                case EstadoMision.EncuentroBosque:
                    DialogoManagerCompañero.instancia.MostrarDialogo(dialogoEncuentro);
                    // Opcional: Aquí puedes activar la misión o cambiar el estado tras hablarle
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

    // Para cambiar el diálogo de Pippin desde otros scripts (por ejemplo, al vencer al jefe)
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