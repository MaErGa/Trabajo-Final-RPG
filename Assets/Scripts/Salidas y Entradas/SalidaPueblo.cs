using UnityEngine;
using UnityEngine.SceneManagement;

public class SalidaPueblo : MonoBehaviour
{
    [Tooltip("Nombre de la escena a la que quieres ir (ej: Underworld)")]
    public string nombreEscenaDestino = "Underworld";

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Comprobamos si lo que ha entrado en el trigger es el jugador
        if (collision.CompareTag("Player"))
        {
            // Cargamos la escena del mapa principal
            SceneManager.LoadScene(nombreEscenaDestino);
        }
    }
}