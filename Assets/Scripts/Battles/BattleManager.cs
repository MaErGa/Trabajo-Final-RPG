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

    [Header("Fondo de Batalla")]
    public UnityEngine.UI.Image imagenFondo;
    public Sprite fondoUnderworld;
    public Sprite fondoBosque;
    public Sprite fondoDefecto;

    [Header("Sonidos de Combate")]
    public AudioClip sonidoAtaqueJugador;       // golpe espada
    public AudioClip sonidoAtaqueEnemigo;       // golpe enemigo
    public AudioClip sonidoGolpeCritico;        // golpe crítico
    public AudioClip sonidoFallo;               // fallo de ambos
    public AudioClip sonidoCuracionObjeto;      // usar planta/poción
    public AudioClip sonidoCuracionMagia;       // Minicuración
    public AudioClip sonidoMagiaAtaque;         // Minihelada / magia de daño
    public AudioClip sonidoMagiaDefensa;        // Fortalecimiento
    public AudioClip sonidoDefender;            // defenderse
    public AudioClip sonidoEscapeExito;         // escapar con éxito
    public AudioClip sonidoEscapeFallo;         // no pudo huir
    public AudioClip sonidoVictoria;            // derrota enemigo
    public AudioClip sonidoLevelUp;             // subir de nivel
    public AudioClip sonidoMuerte;              // jugador muere

    [Header("Música de Batalla")]
    public AudioClip musicaBatalla;
    private AudioSource musicaSource;

    private AudioSource audioSource;
    private int hpSesion;
    private int mpSesion;
    private int vidaEnemigo;
    private bool turnoActivo = false;
    private bool estaDefendiendoManual = false;
    private int turnosFortalecimiento = 0;

    void Start()
    {
        // Sonidos de efectos
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null) audioSource = gameObject.AddComponent<AudioSource>();

        // Música de batalla en loop
        musicaSource = gameObject.AddComponent<AudioSource>();
        musicaSource.loop = true;
        musicaSource.volume = 0.7f;
        if (musicaBatalla != null)
        {
            musicaSource.clip = musicaBatalla;
            musicaSource.Play();
        }

        if (panelTransicion != null) panelTransicion.alpha = 0;

        // Asignar fondo según la escena de origen
        Debug.Log("Escena origen: '" + MovimientoMapa.escenaOrigen + "'");
        if (imagenFondo != null)
        {
            switch (MovimientoMapa.escenaOrigen)
            {
                case "Underworld":
                case "UnderWorld":
                    if (fondoUnderworld != null) imagenFondo.sprite = fondoUnderworld;
                    break;
                case "Bosque":
                    if (fondoBosque != null) imagenFondo.sprite = fondoBosque;
                    break;
                default:
                    if (fondoDefecto != null) imagenFondo.sprite = fondoDefecto;
                    break;
            }
        }
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

    // ── Utilidad de sonido ────────────────────────────────────

    void ReproducirSonido(AudioClip clip)
    {
        if (audioSource != null && clip != null)
            audioSource.PlayOneShot(clip);
    }

    // ── Interfaz ──────────────────────────────────────────────

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

    // ── Acciones del jugador ──────────────────────────────────

    public void AccionUsarPlanta()
    {
        if (!turnoActivo || datosRyo.plantasMedicinales <= 0) return;
        datosRyo.plantasMedicinales--;
        hpSesion = Mathf.Min(hpSesion + 30, datosRyo.hpMax);
        ReproducirSonido(sonidoCuracionObjeto);
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
            ReproducirSonido(sonidoGolpeCritico);
            textoMensajes.text = "¡Golpe excelente! El " + MovimientoMapa.enemigoSeleccionado.nombreEnemigo + " recibe " + dañoBase + " puntos de daño.";
        }
        else
        {
            ReproducirSonido(sonidoAtaqueJugador);
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
            ConjuroBase conjuro = datosRyo.conjuroNivel3;
            if (conjuro == null || mpSesion < conjuro.costeMP) { textoMensajes.text = "¡No tienes PM!"; return; }
            mpSesion -= conjuro.costeMP;
            hpSesion = Mathf.Min(hpSesion + conjuro.valorEfecto + datosRyo.terapeucidad, datosRyo.hpMax);
            ReproducirSonido(sonidoCuracionMagia);
            textoMensajes.text = "¡" + datosRyo.nombre + " lanza " + conjuro.nombreConjuro + "!";
        }
        else if (hechizo == "Fortalecimiento")
        {
            ConjuroBase conjuro = datosRyo.conjuroNivel5;
            if (conjuro == null || mpSesion < conjuro.costeMP) { textoMensajes.text = "¡No tienes PM!"; return; }
            mpSesion -= conjuro.costeMP;
            datosRyo.bonoDefensaTemporal += conjuro.valorEfecto;
            turnosFortalecimiento = conjuro.duracionTurnos;
            ReproducirSonido(sonidoMagiaDefensa);
            textoMensajes.text = "¡" + datosRyo.nombre + " lanza " + conjuro.nombreConjuro + "! Defensa +" + conjuro.valorEfecto + " por " + conjuro.duracionTurnos + " turnos.";
        }
        else if (hechizo == "Minihelada")
        {
            ConjuroBase conjuro = datosRyo.conjuroNivel8;
            if (conjuro == null || mpSesion < conjuro.costeMP) { textoMensajes.text = "¡No tienes PM!"; return; }
            mpSesion -= conjuro.costeMP;
            int dañoM = conjuro.valorEfecto + datosRyo.fuerzaMagica;
            vidaEnemigo -= dañoM;
            ReproducirSonido(sonidoMagiaAtaque);
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
        ReproducirSonido(sonidoDefender);
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

    // ── Corrutinas ────────────────────────────────────────────

    IEnumerator IntentarEscapar()
    {
        turnoActivo = false;
        textoMensajes.text = datosRyo.nombre + " intenta huir...";
        yield return new WaitForSeconds(1.2f);

        int agiJugador = datosRyo.AgilidadTotal;
        int agiEnemigo = MovimientoMapa.enemigoSeleccionado.agilidad;
        int probabilidad = Mathf.RoundToInt((float)agiJugador / (agiJugador + agiEnemigo) * 100);

        if (Random.Range(0, 100) < probabilidad)
        {
            if (musicaSource != null) musicaSource.Stop();
            ReproducirSonido(sonidoEscapeExito);
            textoMensajes.text = "¡" + datosRyo.nombre + " ha escapado!";
            yield return new WaitForSeconds(1.5f);
            GuardarEstadoRyo();
            StartCoroutine(CargarMapa());
        }
        else
        {
            ReproducirSonido(sonidoEscapeFallo);
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
        if (musicaSource != null) musicaSource.Stop();
        ReproducirSonido(sonidoVictoria);

        int expGanada = MovimientoMapa.enemigoSeleccionado.expAlMorir;
        int oroGanado = MovimientoMapa.enemigoSeleccionado.oroAlMorir;
        datosRyo.experiencia += expGanada;
        datosRyo.oro += oroGanado;

        string mensajeVictoria = "¡Has derrotado al " + MovimientoMapa.enemigoSeleccionado.nombreEnemigo + "!";
        string mensajeItem = "";

        // Looteo de items consumibles
        foreach (var entrada in MovimientoMapa.enemigoSeleccionado.tablaLoot)
        {
            if (entrada.item != null && Random.Range(0, 100) < entrada.probabilidad)
            {
                datosRyo.mochilaItems.Add(entrada.item);
                mensajeItem += "\n¡" + MovimientoMapa.enemigoSeleccionado.nombreEnemigo + " ha soltado " + entrada.item.nombre + "!";
            }
        }

        // Looteo de equipo y accesorios
        foreach (var entrada in MovimientoMapa.enemigoSeleccionado.tablaLootEquipo)
        {
            if (entrada.equipo != null && Random.Range(0, 100) < entrada.probabilidad)
            {
                datosRyo.armarioEquipo.Add(entrada.equipo);
                mensajeItem += "\n¡" + MovimientoMapa.enemigoSeleccionado.nombreEnemigo + " ha soltado " + entrada.equipo.nombre + "!";
            }
        }

        string levelUpTexto = "";
        bool subioNivel = false;
        while (datosRyo.experiencia >= datosRyo.expSiguienteNivel)
        {
            levelUpTexto += ComprobarLevelUp();
            subioNivel = true;
        }

        if (subioNivel) ReproducirSonido(sonidoLevelUp);

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
            ReproducirSonido(sonidoGolpeCritico);
            textoMensajes.text = "¡Golpe excelente! ¡" + datosRyo.nombre + " recibe " + daño + " puntos de daño!";
        }
        else
        {
            if (estaDefendiendoManual)
            {
                daño = 1;
                estaDefendiendoManual = false;
                ReproducirSonido(sonidoDefender);
            }
            else
            {
                ReproducirSonido(sonidoAtaqueEnemigo);
            }
            textoMensajes.text = "¡El " + MovimientoMapa.enemigoSeleccionado.nombreEnemigo + " ataca! ¡" + datosRyo.nombre + " recibe " + daño + " puntos de daño!";
        }

        hpSesion -= daño;
        ActualizarInterfaz();

        if (hpSesion <= 0)
        {
            if (musicaSource != null) musicaSource.Stop();
            ReproducirSonido(sonidoMuerte);
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