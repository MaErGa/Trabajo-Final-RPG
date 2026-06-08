using UnityEngine;

public class Tablilla : MonoBehaviour
{
    public float distancia = 2f;
    private Transform jugador;

    private string[] lineas = {
        "[ Inscripción en piedra antigua ]",
        "Aquel que busque descender al mundo de los mortales debe purificar su mente.",
        "Solo tras escuchar el veredicto del guardián alado se disipará la barrera del Umbral."
    };

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
            if (!DialogoManager.instancia.EstaActivo())
            {
                DialogoManager.instancia.MostrarDialogo(lineas, OnTerminar);
            }
        }
    }

    void OnTerminar()
    {
        ControlAccesoUmbral.tablilaLeida = true;
        Debug.Log("Tablilla leída.");
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, distancia);
    }
}