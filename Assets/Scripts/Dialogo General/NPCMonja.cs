using UnityEngine;

public class NPCMonja : MonoBehaviour
{
    [Header("Distancia para interactuar")]
    public float distancia = 2f;

    private Transform jugador;

    private string[][] conversaciones = new string[][]
    {
        new string[] {
            "Esta estatua lleva aquí desde tiempos inmemoriales...",
            "Se dice que su luz protege a los viajeros que se pierden en la oscuridad.",
            "Muchos aventureros se detienen aquí antes de adentrarse en el bosque.",
        },
        new string[] {
            "Curiosamente... quienes se arrodillan ante ella parecen continuar su viaje con más fuerzas.",
            "Como si la diosa recordara sus pasos y los guiara de vuelta a casa.",
        },
        new string[] {
            "Yo me limito a cuidarla.",
            "Lo demás... lo decide el destino.",
            "Quizás deberías acercarte a ella, aventurero."
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
            if (DialogoManagerMonja.instancia != null && DialogoManagerMonja.instancia.EstaActivo()) return;

            DialogoManagerMonja.instancia.MostrarDialogo(conversaciones[conversacionActual]);

            // Avanza a la siguiente conversación o vuelve a la última
            if (conversacionActual < conversaciones.Length - 1)
                conversacionActual++;
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, distancia);
    }
}