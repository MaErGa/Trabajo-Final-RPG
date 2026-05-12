using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;

public class BattleManager : MonoBehaviour
{
    [Header("UI Jugador")]
    public TextMeshProUGUI textoNombreJugador; 
    public TextMeshProUGUI textoMensajes;
    public TextMeshProUGUI textoHPJugador;
    public TextMeshProUGUI textoMPJugador;
    public TextMeshProUGUI textoLVJugador;
    
    [Header("El Hueco del Enemigo")]
    public GameObject objetoImagenEnemigo; 

    [Header("Estadísticas de Ryo (Nivel 1)")]
    private string nombrePlayer = "Ryo"; 
    private int hpJugador = 20;    // PV: 20
    private int mpJugador = 5;     // PM: 5
    private int lvJugador = 1;   
    private int ataqueJugador = 8; // Fuerza: 8

    private int vidaEnemigo;
    private string nombreEnemigo;

    void Start()
    {
        // Asignamos el nombre al empezar
        if(textoNombreJugador != null) textoNombreJugador.text = nombrePlayer;

        if (MovimientoMapa.enemigoSeleccionado != null)
        {
            nombreEnemigo = MovimientoMapa.enemigoSeleccionado.nombreEnemigo;
            vidaEnemigo = MovimientoMapa.enemigoSeleccionado.vidaMaxima;
            
            SpriteRenderer sr = objetoImagenEnemigo.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                sr.sprite = MovimientoMapa.enemigoSeleccionado.imagenEnemigo;
                objetoImagenEnemigo.SetActive(true);
                sr.sortingOrder = 20; 
                objetoImagenEnemigo.transform.localScale = new Vector3(3f, 3f, 1f); 
            }
            textoMensajes.text = "¡Un " + nombreEnemigo + " aparece!";
        }
        ActualizarInterfaz();
    }

    public void AccionAtacar()
    {
        if (vidaEnemigo <= 0) return;
        
        vidaEnemigo -= ataqueJugador; 
        
        if (vidaEnemigo <= 0) 
        {
            StartCoroutine(VictoriaAutomatica());
        }
        else 
        {
            textoMensajes.text = "¡" + nombrePlayer + " ataca al " + nombreEnemigo + "!";
        }
        
        ActualizarInterfaz();
    }

    public void AccionDefender()
    {
        if (vidaEnemigo <= 0) return;

        // Ryo recupera 1 PM al defenderse, hasta un máximo de 5
        textoMensajes.text = "¡" + nombrePlayer + " se pone en guardia y recupera 1 PM!";
        
        if(mpJugador < 5) 
        {
            mpJugador += 1;
        }
        
        ActualizarInterfaz();
    }

    public void AccionEscapar()
    {
        // Volvemos al mapa (MovimientoMapa se encarga de la posición)
        SceneManager.LoadScene("Underworld"); 
    }

    IEnumerator VictoriaAutomatica()
    {
        vidaEnemigo = 0;
        if(objetoImagenEnemigo != null) objetoImagenEnemigo.SetActive(false);
        
        int exp = (MovimientoMapa.enemigoSeleccionado != null) ? MovimientoMapa.enemigoSeleccionado.expAlMorir : 10;
        int oro = (MovimientoMapa.enemigoSeleccionado != null) ? MovimientoMapa.enemigoSeleccionado.oroAlMorir : 5;
        
        textoMensajes.text = "¡" + nombreEnemigo + " derrotado!\nGanas " + exp + " EXP y " + oro + " monedas.";
        
        yield return new WaitForSeconds(3f); 
        SceneManager.LoadScene("Underworld"); 
    }

    void ActualizarInterfaz()
    {
        if(textoHPJugador != null) textoHPJugador.text = "HP: " + hpJugador;
        if(textoMPJugador != null) textoMPJugador.text = "MP: " + mpJugador;
        if(textoLVJugador != null) textoLVJugador.text = "LV: " + lvJugador;
    }
}