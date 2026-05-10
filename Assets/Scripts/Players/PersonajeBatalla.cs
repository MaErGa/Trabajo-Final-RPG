using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PersonajeBatalla : MonoBehaviour
{
    public Image imagenCuerpo;

    public void ConfigurarImagen(Sprite dibujo)
    {
        if (imagenCuerpo == null || dibujo == null) return;
        
        // Solo cambiamos el dibujo, no tocamos el tamaño
        imagenCuerpo.sprite = dibujo;
        imagenCuerpo.enabled = true; // Nos aseguramos de que esté encendida
    }

    public void RecibirGolpe()
    {
        StopAllCoroutines();
        StartCoroutine(EfectoParpadeo());
    }

    public void Morir()
    {
        if (imagenCuerpo != null) imagenCuerpo.enabled = false;
    }

    IEnumerator EfectoParpadeo()
    {
        for (int i = 0; i < 6; i++)
        {
            imagenCuerpo.enabled = !imagenCuerpo.enabled;
            yield return new WaitForSeconds(0.05f);
        }
        imagenCuerpo.enabled = true;
    }
}