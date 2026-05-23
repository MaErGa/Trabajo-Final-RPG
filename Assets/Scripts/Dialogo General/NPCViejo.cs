using UnityEngine;

public class NPCViejo : MonoBehaviour
{
    [Header("Distancia para interactuar")]
    public float distancia = 5f;

    private Transform jugador;
    private bool yaHabloTodas = false;

    private string[][] conversaciones = new string[][]
    {
        new string[] {
            "¡Ay, ay, ay! ¡Qué desastre, viajero!",
            "Ese bribón de Robin Odd y su banda del bosque del norte han cruzado la valla esta madrugada...",
            "¡Y nos han dejado temblando!"
        },
        new string[] {
            "Se han llevado todo el pescado ahumado y las provisiones que teníamos para pasar el mes.",
            "¡Pero eso no es lo peor!",
            "También se han agenciado el Cáliz de Oro Sagrado que usamos en las festividades del pueblo..."
        },
        new string[] {
            "¡Y en mitad de la confusión, se han llevado arrastrando al pobre limo mascota de los niños!",
            "¡Pensando que era un botín valioso, el muy embustero!",
            "Por favor, tú que tienes cara de valiente...",
            "¡Ve al bosque del norte, recupera nuestras provisiones y el Cáliz,",
            "y salva al pobrecito limo antes de que lo hagan sopa!"
        },
        new string[] {
            "¡Sigue en el bosque del norte, viajero!",
            "¡Ese Robin Odd no se saldrá con la suya!",
            "¡Te lo ruego, date prisa!"
        }
    };

    private int conversacionActual = 0;

    void Start()
    {
        GameObject obj = GameObject.FindGameObjectWithTag("Player");
        if (obj != null) jugador = obj.transform;
    }

    void Update()
    {
        if (jugador == null) return;

        float dist = Vector2.Distance(transform.position, jugador.position);

        if (dist <= distancia && Input.GetKeyDown(KeyCode.X))
        {
            if (DialogoManagerViejo.instancia != null && DialogoManagerViejo.instancia.EstaActivo()) return;

            DialogoManagerViejo.instancia.MostrarDialogo(conversaciones[conversacionActual]);

            // Avanza conversación hasta la última que se repite
            if (conversacionActual < conversaciones.Length - 1)
                conversacionActual++;
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, distancia);
    }
}
