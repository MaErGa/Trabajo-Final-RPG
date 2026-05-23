using UnityEngine;

public class NPCMadre : MonoBehaviour
{
    [Header("Distancia para interactuar")]
    public float distancia = 5f;

    public enum EstadoMision { AntesDeVerAPippin, BuscandoAPippin, MisionCumplida }
    public EstadoMision estadoActual = EstadoMision.AntesDeVerAPippin;

    private Transform jugador;

    private string[] dialogoPrimerEncuentro = new string[]
    {
        "¡Ay, viajero, por favor, ayúdame!",
        "Mi hijo Pippin se ha enterado de lo que han hecho los bandidos y se ha marchado al bosque del norte.",
        "¡Salió corriendo llenito de rabia y sin avisar a nadie!",
        "Dice que va a enfrentarse a Robin Odd para recuperar lo robado... ¡Ni siquiera sé si va armado!",
        "¡Por lo que más quieras, ve a la entrada del bosque y ayúdale!"
    };

    private string[] dialogoBuscando = new string[]
    {
        "¡Se fue corriendo y ni me avisó!",
        "Por favor, encuentra a Pippin en la entrada del bosque antes de que sea tarde."
    };

    private string[] dialogoFinal = new string[]
    {
        "¡Oh, gracias a la Diosa que están a salvo!",
        "Pippin me ha contado cómo lucharon juntos.",
        "Toma, acepta esto como agradecimiento por salvar a mi hijo."
    };

    void Start()
    {
        // Busca al jugador por su etiqueta (Tag)
        GameObject obj = GameObject.FindGameObjectWithTag("Player");
        if (obj != null) jugador = obj.transform;
    }

    void Update()
    {
        if (jugador == null) return;

        // Calcula la distancia entre la madre y el héroe
        float dist = Vector2.Distance(transform.position, jugador.position);

        // Si estás cerca y pulsas X
        if (dist <= distancia && Input.GetKeyDown(KeyCode.X))
        {
            // Si el cuadro de la madre ya está abierto, no hace nada para no duplicar
            if (DialogoManagerMadre.instancia != null && DialogoManagerMadre.instancia.EstaActivo()) return;

            // Envía las frases correspondientes
            switch (estadoActual)
            {
                case EstadoMision.AntesDeVerAPippin:
                    DialogoManagerMadre.instancia.MostrarDialogo(dialogoPrimerEncuentro);
                    estadoActual = EstadoMision.BuscandoAPippin;
                    break;

                case EstadoMision.BuscandoAPippin:
                    DialogoManagerMadre.instancia.MostrarDialogo(dialogoBuscando);
                    break;

                case EstadoMision.MisionCumplida:
                    DialogoManagerMadre.instancia.MostrarDialogo(dialogoFinal);
                    DaráRecompensaAlJugador();
                    break;
            }
        }
    }

    private void DaráRecompensaAlJugador()
    {
        Debug.Log("El jugador recibe la recompensa de la madre.");
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, distancia);
    }
}