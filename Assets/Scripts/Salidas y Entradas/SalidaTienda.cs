using UnityEngine;
using UnityEngine.SceneManagement;

public class SalidaTienda : MonoBehaviour
{
    [SerializeField] private string escenaPueblo = "Pueblo";
    [SerializeField] private Vector2 posicionAlSalir = new Vector2(-3.24f, 21.76f);

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Spawnear.SaveSpawnPosition(posicionAlSalir);
            SceneManager.LoadScene(escenaPueblo);
        }
    }
}