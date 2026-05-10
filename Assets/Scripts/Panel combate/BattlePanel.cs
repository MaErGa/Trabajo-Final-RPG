using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattlePanel : MonoBehaviour
{
    // La imagen donde aparecerá el monstruo en la interfaz
    public Image imagenMonstruo;

    // Este método se llama al empezar el combate para poner la foto del enemigo
    public void ConfigurarMonstruo(Sprite fotoEnemigo)
    {
        // Asignamos el dibujo del enemigo
        imagenMonstruo.sprite = fotoEnemigo;

        // Ajustamos el tamaño para que no se vea deformado
        // Multiplicamos por 200 para que tenga un tamaño visible en el Canvas
        float tamañoFinal = fotoEnemigo.bounds.size.x * 200;
        imagenMonstruo.rectTransform.sizeDelta = new Vector2(tamañoFinal, tamañoFinal);

        Debug.Log("Aparece un enemigo en el panel de batalla.");
    }

    // Método para ocultar al enemigo si muere
    public void OcultarEnemigo()
    {
        imagenMonstruo.enabled = false;
    }
}