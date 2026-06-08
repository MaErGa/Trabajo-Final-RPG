using UnityEngine;

public class GuardianAlado : MonoBehaviour
{
    public float distancia = 2f;
    private Transform jugador;
    private bool yaAprobado = false;

    private string[] lineasSinLeerTableta = {
        "...",
        "Antes de hablar conmigo, lee la inscripción del Umbral."
    };

    private string[] lineasVeredicto = {
        "Te observo, viajero.",
        "Tu mente está en orden. El peso de tus intenciones no enturbia el camino.",
        "El Umbral reconoce tu presencia.",
        "Puedes descender."
    };

    private string[] lineasYaAprobado = {
        "Ya tienes mi bendición, viajero.",
        "La puerta te aguarda."
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
            if (DialogoManager.instancia.EstaActivo()) return;

            if (yaAprobado)
            {
                DialogoManager.instancia.MostrarDialogo(lineasYaAprobado);
            }
            else if (!ControlAccesoUmbral.tablilaLeida)
            {
                DialogoManager.instancia.MostrarDialogo(lineasSinLeerTableta);
            }
            else
            {
                DialogoManager.instancia.MostrarDialogo(lineasVeredicto, OnVeredictoTerminado);
            }
        }
    }

    void OnVeredictoTerminado()
    {
        yaAprobado = true;
        ControlAccesoUmbral.guardiánAprobado = true;
        Debug.Log("Guardián aprobó el paso.");
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, distancia);
    }
}