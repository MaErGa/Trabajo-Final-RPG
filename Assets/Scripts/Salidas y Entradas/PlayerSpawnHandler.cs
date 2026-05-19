using UnityEngine;

public class PlayerSpawnHandler : MonoBehaviour
{
    private void Start()
    {
        // Si hay una partida guardada pendiente de aplicar, tiene prioridad
        // y dejamos que PlayerController la aplique, ignoramos el SceneLoadManager
        if (SistemaGuardado.instancia != null && SistemaGuardado.instancia.hayPosicionGuardada)
        {
            // Reseteamos el SceneLoadManager para que no sobreescriba la posición guardada
            if (SceneLoadManager.Instance != null)
                SceneLoadManager.Instance.ResetSpawnFlag();

            // La posición la aplica PlayerController en su Start()
            return;
        }

        // Comportamiento normal: reposicionar por transición de escena
        if (SceneLoadManager.Instance != null && SceneLoadManager.Instance.ShouldRepositionPlayer)
        {
            GameObject player = GameObject.FindWithTag("Player");

            if (player != null)
                player.transform.position = SceneLoadManager.Instance.SpawnPosition;

            SceneLoadManager.Instance.ResetSpawnFlag();
        }
    }
}