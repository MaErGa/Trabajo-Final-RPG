using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuOpciones : MonoBehaviour
{
    [Header("UI Components")]
    public Slider sliderVolumen;

    void Start()
    {
        // Al entrar a la escena, cargamos el volumen que guardó el jugador previamente.
        // Si es la primera vez que juega, el volumen por defecto será 0.5f.
        if (sliderVolumen != null)
        {
            sliderVolumen.value = PlayerPrefs.GetFloat("VolumenMusica", 0.5f);
            sliderVolumen.onValueChanged.AddListener(CambiarVolumen);
        }
    }

    public void CambiarVolumen(float valor)
    {
        // Cambia el volumen global de Unity y lo guarda en la memoria del juego (PlayerPrefs)
        AudioListener.volume = valor;
        PlayerPrefs.SetFloat("VolumenMusica", valor);
    }

    public void VolverAlMenu()
    {
        // Guarda los cambios antes de salir por si acaso
        PlayerPrefs.Save();

        // CAMBIO: Ahora carga la escena llamada "Titulo"
        SceneManager.LoadScene("Titulo"); 
    }
}