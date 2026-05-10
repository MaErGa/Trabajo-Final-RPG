using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class LogBatalla : MonoBehaviour
{
    public Text[] lineasDeTexto; 
    private int lineaActual = 0;

    public void EscribirMensaje(string mensaje)
    {
        if (lineaActual >= lineasDeTexto.Length)
        {
            foreach (Text t in lineasDeTexto) t.text = "";
            lineaActual = 0;
        }

        lineasDeTexto[lineaActual].text = mensaje;
        lineaActual++;
    }
}