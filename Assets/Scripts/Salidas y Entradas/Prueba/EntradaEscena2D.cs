using UnityEngine;
using UnityEngine.SceneManagement;

public class EntradaEscena2D : MonoBehaviour
{
    [SerializeField] private string escenaDestino;

    // ¡Ojo! Lleva el "2D" al final en el método y en el objeto Collider2D
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Comprueba si el que entra es el jugador
        if (other.CompareTag("Player"))
        {
            SceneManager.LoadScene(escenaDestino);
        }
    }
}