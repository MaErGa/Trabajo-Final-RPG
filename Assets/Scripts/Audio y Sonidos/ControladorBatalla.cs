using UnityEngine;

public class ControladorBatalla : MonoBehaviour
{
    [Header("Reproductor de Audio")]
    public AudioSource fuenteDeAudio;

    [Header("Efectos de Sonido (AudioClips)")]
    public AudioClip sonidoEspada;
    public AudioClip sonidoDefensa; // <-- ¡NUEVO! Sonido de escudo/bloqueo
    public AudioClip sonidoEscapar;
    public AudioClip sonidoRecibirDaño;
    public AudioClip sonidoVictoria;

    // Función de ataque
    public void Atacar()
    {
        // Lógica de ataque...
        fuenteDeAudio.PlayOneShot(sonidoEspada);
    }

    // NUEVA: Función para cuando el jugador elige defenderse
    public void Defenderse()
    {
        Debug.Log("¡El héroe se defiende con el escudo!");

        // Reproduce el sonido de defensa
        fuenteDeAudio.PlayOneShot(sonidoDefensa);

        // Aquí pondrías tu lógica de juego (por ejemplo: activar un booleano 
        // como 'estaDefendiendose = true' para que el próximo golpe enemigo haga la mitad de daño)
    }

    // Función para cuando el jugador intenta escapar
    public void IntentarEscapar()
    {
        Debug.Log("¡El equipo intenta escapar!");
        fuenteDeAudio.PlayOneShot(sonidoEscapar);
    }

    // Función para cuando el enemigo te golpea
    public void HeroeRecibeDaño()
    {
        fuenteDeAudio.PlayOneShot(sonidoRecibirDaño);
    }

    // Función para cuando ganas la batalla
    public void BatallaGanada()
    {
        fuenteDeAudio.Stop();
        fuenteDeAudio.PlayOneShot(sonidoVictoria);
    }
}