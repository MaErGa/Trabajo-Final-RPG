using UnityEngine;
using UnityEngine.SceneManagement; // Esto sirve para poder usar escenas

public class EntrarAlPueblo : MonoBehaviour
{
    // Aquí escribiremos el nombre de la escena en Unity
    public string nombreEscena;

    // Esta función se ejecuta sola cuando alguien toca el objeto
    void OnTriggerEnter2D(Collider2D otro)
    {
        // Si el objeto que nos tocó tiene la etiqueta "Player"
        if (otro.gameObject.tag == "Player")
        {
            // Cambiamos a la escena que escribimos en el cuadrito
            SceneManager.LoadScene(nombreEscena);
        }
    }
}