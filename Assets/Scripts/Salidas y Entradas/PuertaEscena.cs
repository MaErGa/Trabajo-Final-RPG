using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PuertaEscena : MonoBehaviour
{
    [Header("Configuración de la Escena")]
    public string nombreEscenaDestino = "Underworld"; // Nombre de tu escena

    // Este método se activa cuando algo entra en el objeto (la puerta)
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Comprobamos si lo que tocó la puerta es el jugador
        if (collision.CompareTag("Player"))
        {
            // Opcional: Si queremos que al entrar por una puerta NO cuente como retorno de combate
            MovimientoMapa.vieneDeCombate = false;

            SceneManager.LoadScene(nombreEscenaDestino);
        }

    }
    public void MenuPrueba()
    {
        SceneManager.LoadScene("Titulo");
    }
}
