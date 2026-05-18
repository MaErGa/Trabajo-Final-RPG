using UnityEngine;

public class ExitDoor : MonoBehaviour
{
    [Header("Configuración de Escena")]
    [SerializeField] private string sceneToLoad = "Pueblo"; // Nombre exacto de la escena del pueblo

    [Header("Posición de Destino")]
    [SerializeField] private Vector2 spawnPositionInTown; // Las coordenadas X e Y de la entrada de la tienda en el pueblo

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Verificamos si el que cruza la puerta es el jugador
        if (collision.CompareTag("Player"))
        {
            // Llamamos al manager para que nos mueva de escena y guarde la posición
            SceneLoadManager.Instance.LoadSceneWithPosition(sceneToLoad, spawnPositionInTown);
        }
    }
}