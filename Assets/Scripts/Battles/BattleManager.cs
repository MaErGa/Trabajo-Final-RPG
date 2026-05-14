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

    // --- AQUÍ TIENES LOS BOTONES DE LOS CONJUROS ---
    [Header("Botones de Conjuros (Magias)")]
    public GameObject botonMinicuracion;
    public GameObject botonMinihelada;

    private int hpSesion; 
    private int mpSesion; 
    private int vidaEnemigo;
    private bool turnoActivo = false;
    private bool estaDefendiendoManual = false;

    void Start()
    {
        if(textoMensajes != null) textoMensajes.text = ""; 

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
            textoMensajes.text = "¡Un " + MovimientoMapa.enemigoSeleccionado.nombreEnemigo + " aparece!";
        }
        
        ActualizarInterfaz();
        turnoActivo = true;

        // Ocultamos los conjuros al empezar por si acaso
        ActualizarConjurosAprendidos();
    }

    void ActualizarInterfaz()
    {
        if(textoNombreJugador != null) textoNombreJugador.text = datosRyo.nombre;
        textoHPJugador.text = "HP: " + hpSesion;
        textoMPJugador.text = "MP: " + mpSesion;
        textoLVJugador.text = "LV: " + datosRyo.nivel;
    }

    // --- ESTA FUNCIÓN OCULTA LOS BOTONES SI NO TIENES NIVEL ---
    public void ActualizarConjurosAprendidos()
    {
        if (botonMinicuracion != null)
            botonMinicuracion.SetActive(datosRyo.nivel >= 3);

        if (botonMinihelada != null)
            botonMinihelada.SetActive(datosRyo.nivel >= 8);
    }

    public void AccionAtacar()
    {
        if (!turnoActivo || vidaEnemigo <= 0) return;
        int dañoBase = (datosRyo.fuerza + datosRyo.poderArma) - MovimientoMapa.enemigoSeleccionado.defensa;
        dañoBase = Mathf.Max(1, dañoBase);
        
        if (Random.Range(0, 100) < 5) 
        {
            dañoBase *= 2; 
            textoMensajes.text = "¡Un golpe excelente! El " + MovimientoMapa.enemigoSeleccionado.nombreEnemigo + " recibe " + dañoBase + " puntos de daño.";
        }
        else 
        {
            textoMensajes.text = "¡" + datosRyo.nombre + " ataca! El " + MovimientoMapa.enemigoSeleccionado.nombreEnemigo + " recibe " + dañoBase + " puntos de daño.";
        }

        vidaEnemigo -= dañoBase;
        if (vidaEnemigo <= 0) StartCoroutine(VictoriaAutomatica());
        else StartCoroutine(TurnoDelEnemigo());
    }

    public void AccionMagia(string hechizo)
    {
        if (!turnoActivo || vidaEnemigo <= 0) return;
        
        if (hechizo == "Minicuracion")
        {
            if (mpSesion < 2) { textoMensajes.text = "¡No tienes PM!"; return; }
            mpSesion -= 2;
            hpSesion = Mathf.Min(hpSesion + 20 + datosRyo.terapeucidad, datosRyo.hpMax);
            textoMensajes.text = "¡" + datosRyo.nombre + " lanza Minicuración!";
        }
        else if (hechizo == "Minihelada")
        {
            if (mpSesion < 3) { textoMensajes.text = "¡No tienes PM!"; return; }
            mpSesion -= 3;
            int dañoM = 15 + datosRyo.fuerzaMagica;
            vidaEnemigo -= dañoM;
            textoMensajes.text = "¡" + datosRyo.nombre + " lanza Minihelada!";
        }

        ActualizarInterfaz();
        if (vidaEnemigo <= 0) StartCoroutine(VictoriaAutomatica());
        else StartCoroutine(TurnoDelEnemigo());
    }

    public void AccionDefender()
    {
        if (!turnoActivo || vidaEnemigo <= 0) return;
        estaDefendiendoManual = true; 
        textoMensajes.text = "¡" + datosRyo.nombre + " se defiende!";
        if(mpSesion < datosRyo.mpMax) mpSesion += 1;
        ActualizarInterfaz();
        StartCoroutine(TurnoDelEnemigo());
    }

    public void AccionEscapar() 
    { 
        if (!turnoActivo) return;
        textoMensajes.text = "¡" + datosRyo.nombre + " intenta escapar!";
        GuardarEstadoRyo(); 
        Invoke("CargarMapa", 1f);
    }

    void CargarMapa() { SceneManager.LoadScene("Underworld"); }

    IEnumerator VictoriaAutomatica()
    {
        turnoActivo = false;
        objetoImagenEnemigo.SetActive(false);
        int expGanada = MovimientoMapa.enemigoSeleccionado.expAlMorir;
        int oroGanado = MovimientoMapa.enemigoSeleccionado.oroAlMorir;
        datosRyo.experiencia += expGanada;
        datosRyo.oro += oroGanado;

        string mensajeVictoria = "¡Has derrotado al " + MovimientoMapa.enemigoSeleccionado.nombreEnemigo + "!";
        string levelUpTexto = "";
        while (datosRyo.experiencia >= datosRyo.expSiguienteNivel)
        {
            levelUpTexto += ComprobarLevelUp();
        }
        
        GuardarEstadoRyo(); 
        textoMensajes.text = mensajeVictoria + "\nRecibes " + expGanada + " EXP y " + oroGanado + " monedas." + levelUpTexto;
        yield return new WaitForSeconds(4f);
        CargarMapa();
    }

    string ComprobarLevelUp()
    {
        datosRyo.nivel++;
        datosRyo.hpMax += 10;
        datosRyo.mpMax += 5;
        datosRyo.fuerza += 3;
        hpSesion = datosRyo.hpMax;
        
        if (datosRyo.nivel - 1 < datosRyo.tablaExpPilgrim.Length)
            datosRyo.expSiguienteNivel = datosRyo.tablaExpPilgrim[datosRyo.nivel - 1];
        else
            datosRyo.expSiguienteNivel = Mathf.RoundToInt(datosRyo.expSiguienteNivel * 1.5f);
        
        return "\n¡" + datosRyo.nombre + " sube al nivel " + datosRyo.nivel + "!";
    }

    void GuardarEstadoRyo() 
    { 
        datosRyo.hpActual = hpSesion; 
        datosRyo.mpActual = mpSesion;
        #if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(datosRyo);
        #endif
    }

    IEnumerator TurnoDelEnemigo() { 
        turnoActivo = false; 
        yield return new WaitForSeconds(1.2f); 
        int defTotal = datosRyo.defensa + datosRyo.poderArmadura + datosRyo.poderEscudo;
        int daño = Mathf.Max(1, MovimientoMapa.enemigoSeleccionado.dañoAtaque - defTotal);
        
        if (Random.Range(0, 100) < 5) 
        {
            daño = Mathf.RoundToInt(MovimientoMapa.enemigoSeleccionado.dañoAtaque * 1.5f);
            textoMensajes.text = "¡Un golpe excelente! ¡" + datosRyo.nombre + " recibe " + daño + " puntos de daño!";
        }
        else 
        {
            if (estaDefendiendoManual) { daño = 1; estaDefendiendoManual = false; }
            textoMensajes.text = "¡El " + MovimientoMapa.enemigoSeleccionado.nombreEnemigo + " ataca! ¡" + datosRyo.nombre + " recibe " + daño + " puntos de daño!";
        }
        
        hpSesion -= daño;
        ActualizarInterfaz();
        if (hpSesion <= 0) 
        {
            textoMensajes.text = "¡" + datosRyo.nombre + " ha perecido!";
            yield return new WaitForSeconds(2f);
            SceneManager.LoadScene("Underworld");
        }
        else turnoActivo = true;
    }
}