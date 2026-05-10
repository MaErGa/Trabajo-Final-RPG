using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PersonajeBatalla : MonoBehaviour
{
    [Header("Configuracion Visual")]
    public Image imagenCuerpo;
    public bool debeParpadear = true;

    [Header("Ajustes del Parpadeo")]
    public int totalParpadeos = 8;
    public float velocidadParpadeo = 0.05f;

    // Se llama cuando el personaje recibe un golpe en el combate
    public void RecibirGolpe()
    {
        // Detenemos cualquier parpadeo que se este ejecutando antes
        StopAllCoroutines();

        if (debeParpadear)
        {
            StartCoroutine(EfectoParpadeo());
        }
        
        Debug.Log("¡El personaje ha recibido un impacto!");
    }

    // Se llama cuando los puntos de vida llegan a cero
    public void Morir()
    {
        // Desactivamos la imagen para que el enemigo "desaparezca"
        if (imagenCuerpo != null)
        {
            imagenCuerpo.enabled = false;
        }
        Debug.Log("El personaje ha sido derrotado.");
    }

    // Metodo para asignar la imagen del monstruo (usado por el BattlePanel)
    public void ConfigurarImagen(Sprite dibujo)
    {
        if (imagenCuerpo == null) return;

        imagenCuerpo.sprite = dibujo;
        
        // Ajustamos el tamaño para que se vea bien en el Canvas (escala de 200)
        float nuevoTamaño = dibujo.bounds.size.x * 200;
        imagenCuerpo.rectTransform.sizeDelta = new Vector2(nuevoTamaño, nuevoTamaño);
    }

    // Corrutina para el efecto visual de daño
    IEnumerator EfectoParpadeo()
    {
        for (int i = 0; i < totalParpadeos; i++)
        {
            // Esperamos un tiempo muy corto
            yield return new WaitForSeconds(velocidadParpadeo);
            
            // Si la imagen esta encendida, la apaga. Si esta apagada, la enciende.
            imagenCuerpo.enabled = !imagenCuerpo.enabled;
        }

        // Al final, nos aseguramos de que la imagen sea visible siempre
        imagenCuerpo.enabled = true;
    }
}