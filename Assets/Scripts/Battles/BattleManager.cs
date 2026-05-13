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
    private int hpJugador = 20;    
    private int mpJugador = 5;     
    private int lvJugador = 1;   
    private int ataqueJugador = 8; 

    private int vidaEnemigo;
    private string nombreEnemigo;
    private bool turnoActivo = true; // Para bloquear botones durante el turno enemigo

    void Start()
    {
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
        if (!turnoActivo || vidaEnemigo <= 0) return;
        
        vidaEnemigo -= ataqueJugador; 
        
        if (vidaEnemigo <= 0) 
        {
            StartCoroutine(VictoriaAutomatica());
        }
        else 
        {
            textoMensajes.text = "¡" + nombrePlayer + " ataca al " + nombreEnemigo + "!";
            StartCoroutine(TurnoDelEnemigo());
        }
        
        ActualizarInterfaz();
    }

    public void AccionDefender()
    {
        if (!turnoActivo || vidaEnemigo <= 0) return;

        textoMensajes.text = "¡" + nombrePlayer + " se pone en guardia y recupera 1 PM!";
        
        if(mpJugador < 5) 
        {
            mpJugador += 1;
        }
        
        ActualizarInterfaz();
        StartCoroutine(TurnoDelEnemigo());
    }

    IEnumerator TurnoDelEnemigo()
    {
        turnoActivo = false; // Bloquea la entrada del jugador
        yield return new WaitForSeconds(1.5f);

        if (vidaEnemigo > 0)
        {
            // El daño puede venir de tu ScriptableObject si tienes un campo de ataque
            int daño = Random.Range(2, 5); 
            hpJugador -= daño;
            textoMensajes.text = "¡El " + nombreEnemigo + " ataca y te quita " + daño + " HP!";
            
            if (hpJugador <= 0)
            {
                hpJugador = 0;
                ActualizarInterfaz();
                textoMensajes.text = "Ryo ha caído en combate...";
                yield return new WaitForSeconds(2f);
                SceneManager.LoadScene("MenuPrincipal"); // O tu escena de Game Over
            }
        }

        ActualizarInterfaz();
        if (hpJugador > 0) turnoActivo = true; // Devuelve el control al jugador
    }

    public void AccionEscapar()
    {
        if (!turnoActivo) return;
        SceneManager.LoadScene("Underworld"); 
    }

    IEnumerator VictoriaAutomatica()
    {
        turnoActivo = false;
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