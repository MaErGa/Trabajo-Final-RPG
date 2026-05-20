using UnityEngine;

public class BosqueExitTrigger : MonoBehaviour
{
    [Header("Escena destino")]
    [SerializeField] private string escenaDestino = "Mapa";

    [Header("Posición de spawn en el Mapa")]
    [SerializeField] private Vector2 spawnEnMapa;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        SceneLoadManager.Instance.LoadSceneWithPosition(escenaDestino, spawnEnMapa);
    }
}
