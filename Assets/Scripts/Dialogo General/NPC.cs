using UnityEngine;

public class NPC : MonoBehaviour
{
    [Header("Dialogo")]
    [TextArea(2, 5)]
    public string[] lineas = {
        "Hola aventurero!",
        "Bienvenido al pueblo.",
        "Ten cuidado en el bosque..."
    };

    [Header("Distancia para interactuar")]
    public float distancia = 2f;

    private Transform jugador;

    void Start()
    {
        GameObject obj = GameObject.FindGameObjectWithTag("Player");
        if (obj != null) jugador = obj.transform;
    }

    void Update()
    {
        if (jugador == null) return;

        float dist = Vector2.Distance(transform.position, jugador.position);

        // X cerca del NPC para hablar
        if (dist <= distancia && Input.GetKeyDown(KeyCode.X))
        {
            if (!DialogoManager.instancia.EstaActivo())
                DialogoManager.instancia.MostrarDialogo(lineas);
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, distancia);
    }
}
