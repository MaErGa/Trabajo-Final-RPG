using UnityEngine;
using UnityEngine.SceneManagement;

public class EntradaTienda : MonoBehaviour
{
    [SerializeField] private string escenaTienda = "Tienda";

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            SceneManager.LoadScene(escenaTienda);
        }
    }
}