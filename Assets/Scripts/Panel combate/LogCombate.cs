using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LogCombate : MonoBehaviour
{
    public Text textoBatalla; // El componente de texto de la UI
    public GameObject flechaContinuar;
    
    private bool estaEscribiendo = false;

    void Start()
    {
        // Al empezar el combate, limpiamos el cuadro
        textoBatalla.text = "";
        flechaContinuar.SetActive(false);
    }

    // Este método lo llamarás desde tu script de combate
    // Ejemplo: log.EnviarMensaje("¡Un Slime aparece!");
    public void EnviarMensaje(string mensaje)
    {
        if (!estaEscribiendo)
        {
            StartCoroutine(EscribirPasoAPaso(mensaje));
        }
    }

    IEnumerator EscribirPasoAPaso(string linea)
    {
        estaEscribiendo = true;
        flechaContinuar.SetActive(false);
        textoBatalla.text = "";

        // Efecto máquina de escribir clásico de Dragon Quest
        foreach (char letra in linea.ToCharArray())
        {
            textoBatalla.text += letra;
            // Un pelín más rápido para que el combate no sea eterno
            yield return new WaitForSeconds(0.03f); 
        }

        estaEscribiendo = false;
        flechaContinuar.SetActive(true);
    }

    // Para limpiar el texto después de una acción
    public void LimpiarTexto()
    {
        textoBatalla.text = "";
        flechaContinuar.SetActive(false);
    }
}