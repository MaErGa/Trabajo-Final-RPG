using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;

public class BattleManager : MonoBehaviour
{
    [Header("Asset de Datos")]
    public DatosJugador datosRyo; 

    [Header("UI Jugador")]
    public TextMeshProUGUI textoNombreJugador; 
    public TextMeshProUGUI textoMensajes;
    public TextMeshProUGUI textoHPJugador;
    public TextMeshProUGUI textoMPJugador;
    public TextMeshProUGUI textoLVJugador;
    
    [Header("El Hueco del Enemigo")]
    public GameObject objetoImagenEnemigo; 

    private int hpSesion; 
    private int mpSesion; 
    private int vidaEnemigo;
    private int mpEnemigo = 10;
    private bool tieneHierbaBunicornio = true;
    private bool turnoActivo = false;
    private bool estaDefendiendoManual = false;
    private bool enemigoDefendiendoManual = false;

    void Start()
    {
        if(datosRyo != null)
        {
            if(datosRyo.hpActual <= 0) datosRyo.hpActual = datosRyo.hpMax;
            hpSesion = datosRyo.hpActual; 
            mpSesion = datosRyo.mpActual;
        }

        if (MovimientoMapa.enemigoSeleccionado != null)
        {
            vidaEnemigo = MovimientoMapa.enemigoSeleccionado.vidaMaxima;
            SpriteRenderer sr = objetoImagenEnemigo.GetComponent<SpriteRenderer>();
            if (sr != null) 
            {
                sr.sprite = MovimientoMapa.enemigoSeleccionado.imagenEnemigo; 
                sr.sortingOrder = 20; 
                objetoImagenEnemigo.transform.localScale = new Vector3(5f, 5f, 1f); 
            }
            objetoImagenEnemigo.SetActive(true); 
        }
        ActualizarInterfaz();
        DeterminarPrimerTurno();
    }

    void ActualizarInterfaz()
    {
        if(datosRyo != null)
        {
            if(textoNombreJugador != null) textoNombreJugador.text = datosRyo.nombre;
            if(textoHPJugador != null) textoHPJugador.text = "HP: " + hpSesion;
            if(textoMPJugador != null) textoMPJugador.text = "MP: " + mpSesion;
            if(textoLVJugador != null) textoLVJugador.text = "LV: " + datosRyo.nivel;
        }
    }

    public void AccionAtacar()
    {
        if (!turnoActivo || vidaEnemigo <= 0) return;
        int daño = (datosRyo.fuerza + datosRyo.poderArma) - MovimientoMapa.enemigoSeleccionado.defensa;
        if (daño < 1) daño = 1;
        if (enemigoDefendiendoManual) { daño /= 2; enemigoDefendiendoManual = false; }
        vidaEnemigo -= daño;
        textoMensajes.text = "¡" + datosRyo.nombre + " ataca con " + datosRyo.armaEquipada + " por " + daño + "!";
        if (vidaEnemigo <= 0) StartCoroutine(VictoriaAutomatica());
        else StartCoroutine(TurnoDelEnemigo());
    }

    public void AccionDefender()
    {
        if (!turnoActivo || vidaEnemigo <= 0) return;
        estaDefendiendoManual = true; 
        // CAMBIO: Ahora usa el nombre del jugador directamente
        textoMensajes.text = "¡" + datosRyo.nombre + " se defiende!";
        if(mpSesion < datosRyo.mpMax) mpSesion += 1;
        ActualizarInterfaz();
        StartCoroutine(TurnoDelEnemigo());
    }

    IEnumerator VictoriaAutomatica()
    {
        turnoActivo = false;
        if(objetoImagenEnemigo != null) objetoImagenEnemigo.SetActive(false);
        
        string itemTexto = "";
        float dado = Random.value;
        if (MovimientoMapa.enemigoSeleccionado.nombreEnemigo.Contains("Slime") && dado < 0.20f) itemTexto = "\n¡Obtienes Planta!";
        else if (MovimientoMapa.enemigoSeleccionado.nombreEnemigo.Contains("Bunicorn") && dado < 0.10f) itemTexto = "\n¡Obtienes Cola!";

        datosRyo.experiencia += MovimientoMapa.enemigoSeleccionado.expAlMorir;
        datosRyo.oro += MovimientoMapa.enemigoSeleccionado.oroAlMorir;

        // CORRECCIÓN NIVEL 2: Forzamos la comprobación inmediata tras ganar EXP
        string levelUpTexto = "";
        while (datosRyo.experiencia >= datosRyo.expSiguienteNivel && datosRyo.nivel < 99)
        {
            levelUpTexto += ComprobarLevelUp();
        }
        
        GuardarEstadoRyo(); 
        textoMensajes.text = "¡Enemigo derrotado!\nGanas " + MovimientoMapa.enemigoSeleccionado.expAlMorir + " EXP." + levelUpTexto + itemTexto;
        yield return new WaitForSeconds(3.5f);
        SceneManager.LoadScene("Underworld"); 
    }

    string ComprobarLevelUp()
    {
        datosRyo.nivel++;
        datosRyo.hpMax += 5;
        datosRyo.fuerza += 2;
        hpSesion = datosRyo.hpMax; 
        
        if (datosRyo.nivel - 1 < datosRyo.tablaExpPilgrim.Length)
            datosRyo.expSiguienteNivel = datosRyo.tablaExpPilgrim[datosRyo.nivel - 1];
        else
            datosRyo.expSiguienteNivel = Mathf.RoundToInt(datosRyo.expSiguienteNivel * 1.25f);

        return "\n¡¡" + datosRyo.nombre + " SUBE AL NIVEL " + datosRyo.nivel + "!!";
    }

    // El resto de funciones auxiliares se mantienen igual
    void GuardarEstadoRyo() { datosRyo.hpActual = hpSesion; datosRyo.mpActual = mpSesion; }
    void DeterminarPrimerTurno() { turnoActivo = true; }
    public void AccionEscapar() { GuardarEstadoRyo(); SceneManager.LoadScene("Underworld"); }
    IEnumerator TurnoDelEnemigo() { 
        turnoActivo = false; yield return new WaitForSeconds(1.5f); 
        if (vidaEnemigo > 0) AtacarARyo(); 
        ActualizarInterfaz(); turnoActivo = true; 
    }
    void AtacarARyo() {
        int def = datosRyo.defensa + datosRyo.poderArmadura;
        int daño = MovimientoMapa.enemigoSeleccionado.dañoAtaque - def;
        if (daño < 1) daño = 1;
        if (estaDefendiendoManual) { daño = 1; estaDefendiendoManual = false; }
        hpSesion -= daño;
        textoMensajes.text = "¡Recibes " + daño + " de daño!";
        if (hpSesion <= 0) SceneManager.LoadScene("Underworld");
    }
}