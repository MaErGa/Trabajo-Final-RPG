using UnityEngine;
using UnityEngine.SceneManagement;

// Adjunta este script al botón Salir
public class BotonSalir : MonoBehaviour
{
    public void VolverAlTitulo()
    {
        SceneManager.LoadScene("Titulo");
    }
}