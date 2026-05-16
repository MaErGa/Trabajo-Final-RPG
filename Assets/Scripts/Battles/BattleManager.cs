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

    [Header("Paneles de Interfaz")]
    public GameObject panelMagia;
    public GameObject panelObjetos;

    [Header("Botones de Conjuros")]
    public GameObject botonMinicuracion;
    public GameObject botonFortalecimiento;
    public GameObject botonMinihelada;

    [Header("Botones de Objetos")]
    public GameObject botonPlanta;
    public GameObject botonColaDeConejo;

    [Header("Transición")]
    public CanvasGroup panelTransicion;

    private int hpSesion;
    private int mpSesion;
    private int vidaEnemigo;
    private bool turnoActivo = false;
    private bool estaDefendiendoManual = false;

    // Turnos restantes del Fortalecimiento
    private int turnosFortalecimiento = 0;

    void Start()
    {
        if (panelTransicion != null) panelTransicion.alpha = 0;
        if (textoMensajes != null) textoMensajes.text = "";

        if (datosRyo != null)
        {
            if (datosRyo.hpActual <= 0) datosRyo.hpActual = datosRyo.hpMax;
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

        if (panelMagia != null) panelMagia.SetActive(false);
        if (panelObjetos != null) panelObjetos.SetActive(false);

        ActualizarConjurosAprendidos();
        ActualizarObjetosDisponibles();
    }

    void ActualizarInterfaz()
    {
        if (textoNombreJugador != null) textoNombreJugador.text = datosRyo.nombre;
        textoHPJugador.text = "HP: " + hpSesion;
        textoMPJugador.text = "MP: " + mpSesion;
        textoLVJugador.text = "LV: " + datosRyo.nivel;
    }

    public void AbrirMenuObjetos()
    {
        if (!turnoActivo) return;
        CerrarMenus();
        if (panelObjetos != null)
        {
            panelObjetos.SetActive(true);
            ActualizarObjetosDisponibles();
        }
    }

    public void CerrarMenus()
    {
        if (panelMagia != null) panelMagia.SetActive(false);
        if (panelObjetos != null) panelObjetos.SetActive(false);
    }

    public void ActualizarConjurosAprendidos()
    {
        // Usa la lista de conjurosAprendidos del ScriptableObject
        if (botonMinicuracion != null)
            botonMinicuracion.SetActive(
                datosRyo.conjurosAprendidos.Contains(datosRyo.conjuroNivel3) && datosRyo.conjuroNivel3 != null);

        if (botonFortalecimiento != null)
            botonFortalecimiento.SetActive(
                datosRyo.conjurosAprendidos.Contains(datosRyo.conjuroNivel5) && datosRyo.conjuroNivel5 != null);

        if (botonMinihelada != null)
            botonMinihelada.SetActive(
                datosRyo.conjurosAprendidos.Contains(datosRyo.conjuroNivel8) && datosRyo.conjuroNivel8 != null);
    }

    public void ActualizarObjetosDisponibles()
    {
        if (botonPlanta != null)
            botonPlanta.SetActive(datosRyo.plantasMedicinales > 0);
        if (botonColaDeConejo != null)
            botonColaDeConejo.SetActive(datosRyo.colaDeConejo > 0);
    }

    public void AccionUsarPlanta()
    {
        if (!turnoActivo || datosRyo.plantasMedicinales <= 0) return;
        datosRyo.plantasMedicinales--;
        hpSesion = Mathf.Min(hpSesion + 30, datosRyo.hpMax);
        textoMensajes.text = "¡" + datosRyo.nombre + " usa una Planta Medicinal!";
        CerrarMenus();
        ActualizarInterfaz();
        ActualizarObjetosDisponibles();
        StartCoroutine(TurnoDelEnemigo());
    }

    public void AccionEquiparCola()
    {
        if (!turnoActivo || datosRyo.colaDeConejo <= 0) return;
        datosRyo.EquiparColaDeConejo();
        string estado = (datosRyo.accesorioEquipado == "Cola de Conejo") ? "equipa" : "desequipa";
        textoMensajes.text = "¡" + datosRyo.nombre + " se " + estado + " la Cola de Conejo!";
        CerrarMenus();
        ActualizarInterfaz();
        StartCoroutine(TurnoDelEnemigo());
    }

    public void AccionAtacar()
    {
        if (!turnoActivo || vidaEnemigo <= 0) return;

        int dañoBase = Mathf.Max(1, datosRyo.AtaqueTotal - MovimientoMapa.enemigoSeleccionado.defensa);

        if (Random.Range(0, 100) < 5)
        {
            dañoBase *= 2;
            textoMensajes.text = "¡Golpe excelente! El " + MovimientoMapa.enemigoSeleccionado.nombreEnemigo + " recibe " + dañoBase + " puntos de daño.";
        }
        else
        {
            textoMensajes.text = "¡" + datosRyo.nombre + " ataca! El " + MovimientoMapa.enemigoSeleccionado.nombreEnemigo + " recibe " + dañoBase + " puntos de daño.";
        }

        vidaEnemigo -= dañoBase;
        if (vidaEnemigo <= 0) StartCoroutine(VictoriaAutomatica());
        else StartCoroutine(TurnoDelEnemigo());
    }

    // Llamado desde el botón con el nombre del conjuro como parámetro
    public void AccionMagia(string hechizo)
    {
        if (!turnoActivo || vidaEnemigo <= 0) return;

        if (hechizo == "Minicuracion")
        {
            ConjuroBase conjuro = datosRyo.conjuroNivel3;
            if (conjuro == null || mpSesion < conjuro.costeMP) { textoMensajes.text = "¡No tienes PM!"; return; }
            mpSesion -= conjuro.costeMP;
            hpSesion = Mathf.Min(hpSesion + conjuro.valorEfecto + datosRyo.terapeucidad, datosRyo.hpMax);
            textoMensajes.text = "¡" + datosRyo.nombre + " lanza " + conjuro.nombreConjuro + "!";
        }
        else if (hechizo == "Fortalecimiento")
        {
            ConjuroBase conjuro = datosRyo.conjuroNivel5;
            if (conjuro == null || mpSesion < conjuro.costeMP) { textoMensajes.text = "¡No tienes PM!"; return; }
            mpSesion -= conjuro.costeMP;
            datosRyo.bonoDefensaTemporal += conjuro.valorEfecto;
            turnosFortalecimiento = conjuro.duracionTurnos;
            textoMensajes.text = "¡" + datosRyo.nombre + " lanza " + conjuro.nombreConjuro + "! Defensa +" + conjuro.valorEfecto + " por " + conjuro.duracionTurnos + " turnos.";
        }
        else if (hechizo == "Minihelada")
        {
            ConjuroBase conjuro = datosRyo.conjuroNivel8;
            if (conjuro == null || mpSesion < conjuro.costeMP) { textoMensajes.text = "¡No tienes PM!"; return; }
            mpSesion -= conjuro.costeMP;
            int dañoM = conjuro.valorEfecto + datosRyo.fuerzaMagica;
            vidaEnemigo -= dañoM;
            textoMensajes.text = "¡" + datosRyo.nombre + " lanza " + conjuro.nombreConjuro + "! Daño: " + dañoM;
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
        if (mpSesion < datosRyo.mpMax) mpSesion += 1;
        ActualizarInterfaz();
        StartCoroutine(TurnoDelEnemigo());
    }

    public void AccionEscapar()
    {
        if (!turnoActivo) return;
        StartCoroutine(IntentarEscapar());
    }

    IEnumerator IntentarEscapar()
    {
        turnoActivo = false;
        textoMensajes.text = datosRyo.nombre + " intenta huir...";
        yield return new WaitForSeconds(1.2f);

        // Fórmula Dragon Quest: probabilidad basada en agilidad
        int agiJugador = datosRyo.AgilidadTotal;
        int agiEnemigo = MovimientoMapa.enemigoSeleccionado.agilidad;
        int probabilidad = Mathf.RoundToInt((float)agiJugador / (agiJugador + agiEnemigo) * 100);

        if (Random.Range(0, 100) < probabilidad)
        {
            textoMensajes.text = "¡" + datosRyo.nombre + " ha escapado!";
            yield return new WaitForSeconds(1.5f);
            GuardarEstadoRyo();
            StartCoroutine(CargarMapa());
        }
        else
        {
            textoMensajes.text = "¡No has podido huir!";
            yield return new WaitForSeconds(1.2f);
            StartCoroutine(TurnoDelEnemigo());
        }
    }

    IEnumerator CargarMapa()
    {
        if (panelTransicion != null)
        {
            while (panelTransicion.alpha < 1)
            {
                panelTransicion.alpha += Time.deltaTime * 2f;
                yield return null;
            }
        }
        SceneManager.LoadScene("Underworld");
    }

    IEnumerator VictoriaAutomatica()
    {
        turnoActivo = false;
        objetoImagenEnemigo.SetActive(false);
        int expGanada = MovimientoMapa.enemigoSeleccionado.expAlMorir;
        int oroGanado = MovimientoMapa.enemigoSeleccionado.oroAlMorir;
        datosRyo.experiencia += expGanada;
        datosRyo.oro += oroGanado;

        string mensajeVictoria = "¡Has derrotado al " + MovimientoMapa.enemigoSeleccionado.nombreEnemigo + "!";
        string mensajeItem = "";

        string nombreEnemigo = MovimientoMapa.enemigoSeleccionado.nombreEnemigo.ToLower();
        if (nombreEnemigo.Contains("slime") && Random.Range(0, 100) < 12)
        {
            datosRyo.plantasMedicinales++;
            mensajeItem = "\n¡El Slime ha soltado una Planta Medicinal!";
        }
        else if (nombreEnemigo.Contains("bunicornio") && Random.Range(0, 100) < 6)
        {
            datosRyo.colaDeConejo++;
            mensajeItem = "\n¡El Bunicornio ha soltado una Cola de Conejo!";
        }

        string levelUpTexto = "";
        while (datosRyo.experiencia >= datosRyo.expSiguienteNivel)
            levelUpTexto += ComprobarLevelUp();

        ActualizarConjurosAprendidos();
        GuardarEstadoRyo();
        textoMensajes.text = mensajeVictoria + "\nRecibes " + expGanada + " EXP y " + oroGanado + " monedas." + mensajeItem + levelUpTexto;
        yield return new WaitForSeconds(4f);
        StartCoroutine(CargarMapa());
    }

    string ComprobarLevelUp()
    {
        datosRyo.nivel++;
        datosRyo.hpMax += 10;
        datosRyo.mpMax += 5;
        datosRyo.fuerza += 3;
        hpSesion = datosRyo.hpMax;

        // Aprende conjuros automáticamente según nivel
        string mensajeConjuro = datosRyo.AprenderConjurosPorNivel();

        if (datosRyo.nivel - 1 < datosRyo.tablaExpPilgrim.Length)
            datosRyo.expSiguienteNivel = datosRyo.tablaExpPilgrim[datosRyo.nivel - 1];
        else
            datosRyo.expSiguienteNivel = Mathf.RoundToInt(datosRyo.expSiguienteNivel * 1.5f);

        return "\n¡" + datosRyo.nombre + " sube al nivel " + datosRyo.nivel + "!" + mensajeConjuro;
    }

    void GuardarEstadoRyo()
    {
        datosRyo.hpActual = hpSesion;
        datosRyo.mpActual = mpSesion;
        #if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(datosRyo);
        #endif
    }

    IEnumerator TurnoDelEnemigo()
    {
        turnoActivo = false;
        yield return new WaitForSeconds(1.2f);

        // Descontar turno de Fortalecimiento
        if (turnosFortalecimiento > 0)
        {
            turnosFortalecimiento--;
            if (turnosFortalecimiento <= 0)
            {
                datosRyo.bonoDefensaTemporal = 0;
                textoMensajes.text = "El efecto de Fortalecimiento ha terminado.";
                yield return new WaitForSeconds(0.8f);
            }
        }

        int defTotal = datosRyo.DefensaTotal;
        int daño = Mathf.Max(1, MovimientoMapa.enemigoSeleccionado.dañoAtaque - defTotal);

        if (Random.Range(0, 100) < 5)
        {
            daño = Mathf.RoundToInt(MovimientoMapa.enemigoSeleccionado.dañoAtaque * 1.5f);
            textoMensajes.text = "¡Golpe excelente! ¡" + datosRyo.nombre + " recibe " + daño + " puntos de daño!";
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
            int oroPerdido = datosRyo.oro / 2;
            datosRyo.oro -= oroPerdido;
            GuardarEstadoRyo();
            textoMensajes.text = "¡" + datosRyo.nombre + " ha perecido! Has perdido " + oroPerdido + " G.";
            yield return new WaitForSeconds(2f);
            StartCoroutine(CargarMapa());
        }
        else turnoActivo = true;
    }

    private void OnDestroy()
    {
        if (datosRyo != null) datosRyo.ResetearBonos();
    }
}