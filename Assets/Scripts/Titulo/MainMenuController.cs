using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    [Header("Configuracion de Audio")]
    [SerializeField] private AudioSource titleAudioSource;

    [Header("Escenas a Cargar")]
    [SerializeField] private string newGameSceneName = "Pueblo";
    [SerializeField] private string continueSceneName = "Pueblo";

    private void Start()
    {
        if (titleAudioSource != null && !titleAudioSource.isPlaying)
        {
            titleAudioSource.Play();
        }
    }

    public void OnClickNewGame()
    {
        StopTitleMusic();
        SceneManager.LoadScene(newGameSceneName);
    }

    public void OnClickContinue()
    {
        StopTitleMusic();
        SceneManager.LoadScene(continueSceneName);
    }

    private void StopTitleMusic()
    {
        if (titleAudioSource != null)
        {
            titleAudioSource.Stop();
        }
    }
}