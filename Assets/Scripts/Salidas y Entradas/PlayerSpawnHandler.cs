using UnityEngine;

public class PlayerSpawnHandler : MonoBehaviour
{
    private void Start()
    {
        // Si el manager dice que debemos reposicionar al jugador...
        if (SceneLoadManager.Instance != null && SceneLoadManager.Instance.ShouldRepositionPlayer)
        {
            // Buscamos al jugador por su Tag
            GameObject player = GameObject.FindWithTag("Player");

            if (player != null)
            {
                // Movemos al jugador a la posición guardada
                player.transform.position = SceneLoadManager.Instance.SpawnPosition;
            }

            // Le decimos al manager que ya posicionamos al jugador
            SceneLoadManager.Instance.ResetSpawnFlag();
        }
    }
}