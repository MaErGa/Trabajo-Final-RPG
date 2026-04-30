using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; // Biblioteca de cambio de escena

public class ChangeLevel1 : MonoBehaviour
{
    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            SceneManager.LoadScene("mapa", LoadSceneMode.Single);
            //compara la etiqueta del player y carga de manera individual la siguiente escena
        }
    }
}
