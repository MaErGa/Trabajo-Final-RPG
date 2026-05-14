using UnityEngine;
using UnityEngine.SceneManagement;

public class EntrarEscena : MonoBehaviour
{
    public string nombreEscena;

    void OnTriggerEnter2D(Collider2D otro)
    {
        if (otro.gameObject.tag == "Player")
        {
            SceneManager.LoadScene(nombreEscena);
        }
    }
}