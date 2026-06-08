using UnityEngine;
using UnityEngine.SceneManagement;

public class PuertaEscena : MonoBehaviour
{
    public string nombreEscenaDestino = "Underworld";

    private string[] dialogoBloqueadoSinNada = {
        "La barrera del Umbral permanece cerrada.",
        "Debes leer la inscripción y buscar al guardián alado."
    };

    private string[] dialogoBloqueadoSinGuardian = {
        "La barrera no cede.",
        "El guardián alado aún no ha dado su veredicto."
    };

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;

        if (!ControlAccesoUmbral.tablilaLeida)
        {
            if (!DialogoManager.instancia.EstaActivo())
                DialogoManager.instancia.MostrarDialogo(dialogoBloqueadoSinNada);
            return;
        }

        if (!ControlAccesoUmbral.guardiánAprobado)
        {
            if (!DialogoManager.instancia.EstaActivo())
                DialogoManager.instancia.MostrarDialogo(dialogoBloqueadoSinGuardian);
            return;
        }

        // Ambas condiciones cumplidas → pasa
        MovimientoMapa.vieneDeCombate = false;
        SceneManager.LoadScene(nombreEscenaDestino);
    }

    public void MenuPrueba()
    {
        SceneManager.LoadScene("Titulo");
    }
}