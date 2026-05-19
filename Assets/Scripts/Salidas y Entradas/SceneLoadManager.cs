using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoadManager : MonoBehaviour
{
    public static SceneLoadManager Instance { get; private set; }

    // Aquí guardaremos la posición donde debe aparecer el jugador
    public Vector2 SpawnPosition { get; private set; }
    public bool ShouldRepositionPlayer { get; private set; }

    private void Awake()
    {
        // Sistema Singleton para que este objeto no se destruya al cambiar de escena
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Método que llamaremos para cambiar de escena guardando la posición de destino
    public void LoadSceneWithPosition(string sceneName, Vector2 targetSpawnPosition)
    {
        SpawnPosition = targetSpawnPosition;
        ShouldRepositionPlayer = true;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // Método para resetear el estado una vez que el jugador ya se posicionó
    public void ResetSpawnFlag()
    {
        ShouldRepositionPlayer = false;
    }
}