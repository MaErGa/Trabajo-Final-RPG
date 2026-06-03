using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;

public class BattleManager : MonoBehaviour
{
    [Header("Asset de Datos")]
    public DatosJugador datosRyo;
    public DatosPippin datosPippin;

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
    public GameObject botonMiniincendio;


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
    public AudioClip sonidoAtaqueJugador;
    public AudioClip sonidoAtaqueEnemigo;
    public AudioClip sonidoGolpeCritico;
    public AudioClip sonidoFallo;
    public AudioClip sonidoCuracionObjeto;
    public AudioClip sonidoCuracionMagia;
    public AudioClip sonidoMagiaAtaque;
    public AudioClip sonidoMagiaDefensa;
    public AudioClip sonidoDefender;
    public AudioClip sonidoEscapeExito;
    public AudioClip sonidoEscapeFallo;
    public AudioClip sonidoVictoria;
    public AudioClip sonidoLevelUp;
    public AudioClip sonidoMuerte;

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

    // Pippin
    private bool pippinActivo = false;
    private int hpPippin;
    private int mpPippin;
    private bool pippinCaido = false;
    private int turnosFortalecimientoPippin = 0;

    // ── Inspiración ───────────────────────────────────────────────────────────
    // +20% ataque, defensa y agilidad durante 3 turnos.
    // Se activa aleatoriamente al inicio del turno (15%) o al recibir daño (20%).
    [Header("Inspiración")]
    public AudioClip sonidoInspiracion;
    private bool inspiracionActiva = false;
    private int turnosInspiracion = 0;
    private int inspiracionBonoAtaque = 0;  // guardamos el valor exacto aplicado
    private int inspiracionBonoDefensa = 0;
    private int inspiracionBonoAgilidad = 0;
    private const float BONUS_INSPIRACION = 0.20f;
    private const int TURNOS_INSPIRACION = 3;
    private const int PROB_INICIO_TURNO = 15;
    private const int PROB_RECIBIR_DAÑO = 20;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null) audioSource = gameObject.AddComponent<AudioSource>();

        musicaSource = gameObject.AddComponent<AudioSource>();
        musicaSource.loop = true;
        musicaSource.volume = 0.7f;
        if (musicaBatalla != null) { musicaSource.clip = musicaBatalla; musicaSource.Play(); }

        if (panelTransicion != null) panelTransicion.alpha = 0;

        Debug.Log("Escena origen: '" + MovimientoMapa.escenaOrigen + "'");
        if (imagenFondo != null)
        {
            Sprite spriteElegido = fondoDefecto;
            switch (MovimientoMapa.escenaOrigen)
            {
                case "Underworld":
                case "UnderWorld":
                    if (fondoUnderworld != null) spriteElegido = fondoUnderworld; break;
                case "Bosque":
                    if (fondoBosque != null) spriteElegido = fondoBosque; break;
                default:
                    if (fondoDefecto != null) spriteElegido = fondoDefecto; break;
            }
            imagenFondo.sprite = spriteElegido;
            imagenFondo.color = Color.white;
            imagenFondo.enabled = true;
        }

        if (textoMensajes != null) textoMensajes.text = "";

        if (datosRyo != null)
        {
            if (datosRyo.hpActual <= 0) datosRyo.hpActual = datosRyo.hpMax;
            hpSesion = datosRyo.hpActual;
            mpSesion = datosRyo.mpActual;
        }

        // Pippin solo activo en el Bosque
        pippinActivo = MovimientoMapa.pippinUnido &&
                       MovimientoMapa.escenaOrigen == "Bosque" &&
                       datosPippin != null;

        if (pippinActivo)
        {
            hpPippin = datosPippin.hpActual > 0 ? datosPippin.hpActual : datosPippin.hpMax;
            mpPippin = datosPippin.mpActual;
            pippinCaido = false;
        }

        if (MovimientoMapa.enemigoSeleccionado != null)
        {
            vidaEnemigo = MovimientoMapa.enemigoSeleccionado.vidaMaxima;
            UnityEngine.UI.Image imgEnemigo = objetoImagenEnemigo.GetComponent<UnityEngine.UI.Image>();
            if (imgEnemigo != null) { imgEnemigo.sprite = MovimientoMapa.enemigoSeleccionado.imagenEnemigo; imgEnemigo.preserveAspect = true; }
            else
            {
                SpriteRenderer sr = objetoImagenEnemigo.GetComponent<SpriteRenderer>();
                if (sr != null) { sr.sprite = MovimientoMapa.enemigoSeleccionado.imagenEnemigo; sr.sortingOrder = 20; }
            }
            objetoImagenEnemigo.SetActive(true);
            textoMensajes.text = "¡Un " + MovimientoMapa.enemigoSeleccionado.nombreEnemigo + " aparece!";
            if (pippinActivo) textoMensajes.text += "\n¡Pippin está listo para combatir!";
        }

        ActualizarInterfaz();
        turnoActivo = true;
        if (panelMagia != null) panelMagia.SetActive(false);
        if (panelObjetos != null) panelObjetos.SetActive(false);
        ActualizarConjurosAprendidos();
        ActualizarObjetosDisponibles();
    }

    void ReproducirSonido(AudioClip clip)
    {
        if (audioSource != null && clip != null) audioSource.PlayOneShot(clip);
    }

    void ActualizarInterfaz()
    {
        if (textoNombreJugador != null) textoNombreJugador.text = datosRyo.nombre;
        textoHPJugador.text = "HP: " + hpSesion;
        textoMPJugador.text = "MP: " + mpSesion;
        textoLVJugador.text = "LV: " + datosRyo.nivel;
    }

    public void AbrirMenuMagia()
    {
        if (!turnoActivo) return;
        CerrarMenus();
        if (panelMagia != null) { panelMagia.SetActive(true); ActualizarConjurosAprendidos(); }
    }

    public void AbrirMenuObjetos()
    {
        if (!turnoActivo) return;
        CerrarMenus();
        if (panelObjetos != null) { panelObjetos.SetActive(true); ActualizarObjetosDisponibles(); }
    }

    public void CerrarMenus()
    {
        if (panelMagia != null) panelMagia.SetActive(false);
        if (panelObjetos != null) panelObjetos.SetActive(false);
    }

    public void ActualizarConjurosAprendidos()
    {
        if (botonMinicuracion != null)
            botonMinicuracion.SetActive(datosRyo.conjurosAprendidos.Contains(datosRyo.conjuroNivel3) && datosRyo.conjuroNivel3 != null);
        if (botonFortalecimiento != null)
            botonFortalecimiento.SetActive(datosRyo.conjurosAprendidos.Contains(datosRyo.conjuroNivel5) && datosRyo.conjuroNivel5 != null);
        if (botonMinihelada != null)
            botonMinihelada.SetActive(datosRyo.conjurosAprendidos.Contains(datosRyo.conjuroNivel8) && datosRyo.conjuroNivel8 != null);

        // CORREGIDO: Ahora verifica si el scriptable object contiene el conjuro de nivel 10
        if (botonMiniincendio != null)
            botonMiniincendio.SetActive(datosRyo.conjurosAprendidos.Contains(datosRyo.conjuroNivel10) && datosRyo.conjuroNivel10 != null);
    }

    public void ActualizarObjetosDisponibles()
    {
        if (botonPlanta != null) botonPlanta.SetActive(datosRyo.plantasMedicinales > 0);
        if (botonColaDeConejo != null) botonColaDeConejo.SetActive(datosRyo.colaDeConejo > 0);
    }

    public void AccionUsarPlanta()
    {
        if (!turnoActivo || datosRyo.plantasMedicinales <= 0) return;
        datosRyo.plantasMedicinales--;
        hpSesion = Mathf.Min(hpSesion + 30, datosRyo.hpMax);
        ReproducirSonido(sonidoCuracionObjeto);
        textoMensajes.text = "¡" + datosRyo.nombre + " usa una Planta Medicinal!";
        CerrarMenus(); ActualizarInterfaz(); ActualizarObjetosDisponibles();
        StartCoroutine(TurnoPippin());
    }

    public void AccionEquiparCola()
    {
        if (!turnoActivo || datosRyo.colaDeConejo <= 0) return;
        datosRyo.EquiparColaDeConejo();
        string estado = (datosRyo.accesorioEquipado == "Cola de Conejo") ? "equipa" : "desequipa";
        textoMensajes.text = "¡" + datosRyo.nombre + " se " + estado + " la Cola de Conejo!";
        CerrarMenus(); ActualizarInterfaz();
        StartCoroutine(TurnoPippin());
    }

    public void AccionAtacar()
    {
        if (!turnoActivo || vidaEnemigo <= 0) return;
        ChequearInspiracioInicio();
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
        else StartCoroutine(TurnoPippin());
    }

    public void AccionMagia(string hechizo)
    {
        if (!turnoActivo || vidaEnemigo <= 0) return;
        ChequearInspiracioInicio();
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
            textoMensajes.text = "¡" + datosRyo.nombre + " lanza " + conjuro.nombreConjuro + "!";
        }
        else if (hechizo == "Miniincendio")
        {
            // CORREGIDO: Ahora usa el sistema dinámico de ScriptableObjects igual que los otros hechizos
            ConjuroBase conjuro = datosRyo.conjuroNivel10;
            if (conjuro == null || mpSesion < conjuro.costeMP) { textoMensajes.text = "¡No tienes PM!"; return; }

            mpSesion -= conjuro.costeMP;
            int dañoM = conjuro.valorEfecto + datosRyo.fuerzaMagica;
            vidaEnemigo -= dañoM;

            ReproducirSonido(sonidoMagiaAtaque);
            textoMensajes.text = "¡" + datosRyo.nombre + " lanza " + conjuro.nombreConjuro + "! Daño: " + dañoM;
        }
        ActualizarInterfaz();
        if (vidaEnemigo <= 0) StartCoroutine(VictoriaAutomatica());
        else StartCoroutine(TurnoPippin());
    }

    public void AccionDefender()
    {
        if (!turnoActivo || vidaEnemigo <= 0) return;
        ChequearInspiracioInicio();
        estaDefendiendoManual = true;
        ReproducirSonido(sonidoDefender);
        textoMensajes.text = "¡" + datosRyo.nombre + " se defiende!";
        if (mpSesion < datosRyo.mpMax) mpSesion += 1;
        ActualizarInterfaz();
        StartCoroutine(TurnoPippin());
    }

    public void AccionEscapar()
    {
        if (!turnoActivo) return;
        StartCoroutine(IntentarEscapar());
    }

    // ── IA de Pippin ──────────────────────────────────────────

    IEnumerator TurnoPippin()
    {
        turnoActivo = false;
        yield return new WaitForSeconds(1.0f);

        if (!pippinActivo || pippinCaido || vidaEnemigo <= 0)
        {
            StartCoroutine(TurnoDelEnemigo());
            yield break;
        }

        string action = DecidirAccionPippin();
        yield return new WaitForSeconds(0.2f);

        switch (action)
        {
            case "curar_jugador":
                ConjuroBase cur = datosPippin.conjuroMinicuracion;
                mpPippin -= cur.costeMP;
                int curJ = cur.valorEfecto + datosPippin.terapeucidad;
                hpSesion = Mathf.Min(hpSesion + curJ, datosRyo.hpMax);
                ReproducirSonido(sonidoCuracionMagia);
                textoMensajes.text = "¡Pippin lanza Minicuración sobre " + datosRyo.nombre + "! +" + curJ + " HP";
                ActualizarInterfaz();
                break;

            case "curar_pippin":
                ConjuroBase curP = datosPippin.conjuroMinicuracion;
                mpPippin -= curP.costeMP;
                int curPP = curP.valorEfecto + datosPippin.terapeucidad;
                hpPippin = Mathf.Min(hpPippin + curPP, datosPippin.hpMax);
                ReproducirSonido(sonidoCuracionMagia);
                textoMensajes.text = "¡Pippin se lanza Minicuración! +" + curPP + " HP";
                break;

            case "fortalecer_jugador":
                ConjuroBase fort = datosPippin.conjuroFortalecimiento;
                mpPippin -= fort.costeMP;
                datosRyo.bonoDefensaTemporal += fort.valorEfecto;
                turnosFortalecimiento = Mathf.Max(turnosFortalecimiento, fort.duracionTurnos);
                ReproducirSonido(sonidoMagiaDefensa);
                textoMensajes.text = "¡Pippin lanza Fortalecimiento sobre " + datosRyo.nombre + "! Defensa +" + fort.valorEfecto;
                ActualizarInterfaz();
                break;

            case "fortalecer_pippin":
                ConjuroBase fortP = datosPippin.conjuroFortalecimiento;
                mpPippin -= fortP.costeMP;
                datosPippin.bonoDefensaTemporal += fortP.valorEfecto;
                turnosFortalecimientoPippin = fortP.duracionTurnos;
                ReproducirSonido(sonidoMagiaDefensa);
                textoMensajes.text = "¡Pippin se lanza Fortalecimiento! Defensa +" + fortP.valorEfecto;
                break;

            case "minihelada":
                ConjuroBase helada = datosPippin.conjuroMinihelada;
                mpPippin -= helada.costeMP;
                int dH = helada.valorEfecto + datosPippin.fuerzaMagica;
                vidaEnemigo -= dH;
                ReproducirSonido(sonidoMagiaAtaque);
                textoMensajes.text = "¡Pippin lanza Minihelada! El " + MovimientoMapa.enemigoSeleccionado.nombreEnemigo + " recibe " + dH + " de daño.";
                break;

            case "miniincendio":
                // CORREGIDO: Quitamos la redefinición de 'int dH' y usamos una nueva variable 'dI'
                int dI = 20 + datosPippin.fuerzaMagica;
                vidaEnemigo -= dI;
                ReproducirSonido(sonidoMagiaAtaque);
                textoMensajes.text = "¡Pippin lanza Miniincendio! El " + MovimientoMapa.enemigoSeleccionado.nombreEnemigo + " recibe " + dI + " de daño.";
                break;

            default: // atacar
                int dP = Mathf.Max(1, datosPippin.AtaqueTotal - MovimientoMapa.enemigoSeleccionado.defensa);
                vidaEnemigo -= dP;
                ReproducirSonido(sonidoAtaqueJugador);
                textoMensajes.text = "¡Pippin ataca! El " + MovimientoMapa.enemigoSeleccionado.nombreEnemigo + " recibe " + dP + " de daño.";
                break;
        }

        yield return new WaitForSeconds(1.2f);
        if (vidaEnemigo <= 0) StartCoroutine(VictoriaAutomatica());
        else StartCoroutine(TurnoDelEnemigo());
    }

    string DecidirAccionPippin()
    {
        float pctJugador = (float)hpSesion / datosRyo.hpMax;
        float pctPippin = (float)hpPippin / datosPippin.hpMax;
        bool tieneCur = datosPippin.conjuroMinicuracion != null;
        bool tieneFort = datosPippin.conjuroFortalecimiento != null;
        bool tieneHel = datosPippin.conjuroMinihelada != null;

        if (tieneCur && pctJugador < 0.35f && mpPippin >= datosPippin.conjuroMinicuracion.costeMP) return "curar_jugador";
        if (tieneCur && pctPippin < 0.30f && mpPippin >= datosPippin.conjuroMinicuracion.costeMP) return "curar_pippin";
        if (tieneFort && turnosFortalecimiento <= 0 && mpPippin >= datosPippin.conjuroFortalecimiento.costeMP) return "fortalecer_jugador";
        if (tieneFort && turnosFortalecimientoPippin <= 0 && mpPippin >= datosPippin.conjuroFortalecimiento.costeMP) return "fortalecer_pippin";
        if (tieneCur && pctJugador < 0.60f && mpPippin >= datosPippin.conjuroMinicuracion.costeMP) return "curar_jugador";
        if (tieneHel && mpPippin >= datosPippin.conjuroMinihelada.costeMP) return "minihelada";

        return "atacar";
    }

    // ── Corrutinas ────────────────────────────────────────────

    IEnumerator IntentarEscapar()
    {
        turnoActivo = false;
        textoMensajes.text = datosRyo.nombre + " intenta huir...";
        yield return new WaitForSeconds(1.2f);
        int agiJ = datosRyo.AgilidadTotal;
        int agiE = MovimientoMapa.enemigoSeleccionado.agilidad;
        int prob = Mathf.RoundToInt((float)agiJ / (agiJ + agiE) * 100);
        if (Random.Range(0, 100) < prob)
        {
            if (musicaSource != null) musicaSource.Stop();
            ReproducirSonido(sonidoEscapeExito);
            textoMensajes.text = "¡" + datosRyo.nombre + " ha escapado!";
            yield return new WaitForSeconds(1.5f);
            GuardarEstado();
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
            while (panelTransicion.alpha < 1) { panelTransicion.alpha += Time.deltaTime * 2f; yield return null; }
        }
        string escenaDestino = !string.IsNullOrEmpty(MovimientoMapa.escenaOrigen) ? MovimientoMapa.escenaOrigen : "Underworld";
        SceneManager.LoadScene(escenaDestino);
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

        foreach (var entrada in MovimientoMapa.enemigoSeleccionado.tablaLoot)
            if (entrada.item != null && Random.Range(0, 100) < entrada.probabilidad)
            { datosRyo.mochilaItems.Add(entrada.item); mensajeItem += "\n¡Soltó " + entrada.item.nombre + "!"; }

        foreach (var entrada in MovimientoMapa.enemigoSeleccionado.tablaLootEquipo)
            if (entrada.equipo != null && Random.Range(0, 100) < entrada.probabilidad)
            { datosRyo.armarioEquipo.Add(entrada.equipo); mensajeItem += "\n¡Soltó " + entrada.equipo.nombre + "!"; }

        string levelUpTexto = "";
        bool subioNivel = false;
        while (datosRyo.experiencia >= datosRyo.expSiguienteNivel) { levelUpTexto += ComprobarLevelUp(); subioNivel = true; }
        if (subioNivel) ReproducirSonido(sonidoLevelUp);

        ActualizarConjurosAprendidos();
        GuardarEstado();

        // Marcar boss o secuaz derrotado
        if (MovimientoMapa.combateBoss)
        {
            if (MovimientoMapa.combateSecuaz) NPCSecuaz.MarcarDerrotado();
            else NPCRobbinOdd.MarcarDerrotado();
            MovimientoMapa.combateBoss = false;
            MovimientoMapa.combateSecuaz = false;
        }

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

    void GuardarEstado()
    {
        datosRyo.hpActual = hpSesion;
        datosRyo.mpActual = mpSesion;
        if (pippinActivo && datosPippin != null)
        {
            datosPippin.hpActual = hpPippin;
            datosPippin.mpActual = mpPippin;
            datosPippin.RecuperarPostCombate();
        }
#if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(datosRyo);
        if (datosPippin != null) UnityEditor.EditorUtility.SetDirty(datosPippin);
#endif
    }

    IEnumerator TurnoDelEnemigo()
    {
        turnoActivo = false;
        yield return new WaitForSeconds(1.2f);

        if (turnosFortalecimiento > 0)
        {
            turnosFortalecimiento--;
            if (turnosFortalecimiento <= 0) { datosRyo.bonoDefensaTemporal = 0; textoMensajes.text = "El Fortalecimiento del jugador ha terminado."; yield return new WaitForSeconds(0.8f); }
        }
        if (turnosFortalecimientoPippin > 0)
        {
            turnosFortalecimientoPippin--;
            if (turnosFortalecimientoPippin <= 0) datosPippin.bonoDefensaTemporal = 0;
        }

        // Bajar contador de inspiración
        if (inspiracionActiva)
        {
            turnosInspiracion--;
            if (turnosInspiracion <= 0)
            {
                inspiracionActiva = false;
                datosRyo.bonoAtaqueTemporal -= inspiracionBonoAtaque;
                datosRyo.bonoDefensaTemporal -= inspiracionBonoDefensa;
                datosRyo.bonoAgilidadTemporal -= inspiracionBonoAgilidad;
                inspiracionBonoAtaque = inspiracionBonoDefensa = inspiracionBonoAgilidad = 0;
                textoMensajes.text = "La Inspiración de " + datosRyo.nombre + " ha terminado.";
                yield return new WaitForSeconds(0.8f);
            }
        }

        bool atacarPippin = pippinActivo && !pippinCaido && Random.Range(0, 2) == 0;

        if (atacarPippin)
        {
            int defP = datosPippin.DefensaTotal;
            int dañoP = Mathf.Max(1, MovimientoMapa.enemigoSeleccionado.dañoAtaque - defP);
            if (Random.Range(0, 100) < 5)
            {
                dañoP = Mathf.RoundToInt(MovimientoMapa.enemigoSeleccionado.dañoAtaque * 1.5f);
                ReproducirSonido(sonidoGolpeCritico);
                textoMensajes.text = "¡Golpe excelente a Pippin! Recibe " + dañoP + " de daño.";
            }
            else
            {
                ReproducirSonido(sonidoAtaqueEnemigo);
                textoMensajes.text = "¡El " + MovimientoMapa.enemigoSeleccionado.nombreEnemigo + " ataca a Pippin! Recibe " + dañoP + " de daño.";
            }
            hpPippin -= dañoP;
            if (hpPippin <= 0)
            {
                hpPippin = 0; pippinCaido = true;
                textoMensajes.text += "\n¡Pippin ha caído! Se recuperará tras el combate.";
                yield return new WaitForSeconds(1.5f);
            }
        }
        else
        {
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
                if (estaDefendiendoManual) { daño = 1; estaDefendiendoManual = false; ReproducirSonido(sonidoDefender); }
                else ReproducirSonido(sonidoAtaqueEnemigo);
                textoMensajes.text = "¡El " + MovimientoMapa.enemigoSeleccionado.nombreEnemigo + " ataca! ¡" + datosRyo.nombre + " recibe " + daño + " puntos de daño!";
            }
            hpSesion -= daño;
            ActualizarInterfaz();
            // Posible inspiración al recibir daño
            if (!inspiracionActiva && Random.Range(0, 100) < PROB_RECIBIR_DAÑO)
                ActivarInspiracion();
            if (hpSesion <= 0)
            {
                if (musicaSource != null) musicaSource.Stop();
                ReproducirSonido(sonidoMuerte);
                int oroPerdido = datosRyo.oro / 2;
                datosRyo.oro -= oroPerdido;
                GuardarEstado();
                textoMensajes.text = "¡" + datosRyo.nombre + " ha perecido! Has perdido " + oroPerdido + " G.";
                yield return new WaitForSeconds(2f);
                StartCoroutine(CargarMapa());
                yield break;
            }
        }
        turnoActivo = true;
    }

    // ── Inspiración ───────────────────────────────────────────────────────────

    void ChequearInspiracioInicio()
    {
        if (!inspiracionActiva && Random.Range(0, 100) < PROB_INICIO_TURNO)
            ActivarInspiracion();
    }

    void ActivarInspiracion()
    {
        inspiracionActiva = true;
        turnosInspiracion = TURNOS_INSPIRACION;

        inspiracionBonoAtaque = Mathf.RoundToInt(datosRyo.fuerza * BONUS_INSPIRACION);
        inspiracionBonoDefensa = Mathf.RoundToInt(datosRyo.defensa * BONUS_INSPIRACION);
        inspiracionBonoAgilidad = Mathf.RoundToInt(datosRyo.agilidad * BONUS_INSPIRACION);

        datosRyo.bonoAtaqueTemporal += inspiracionBonoAtaque;
        datosRyo.bonoDefensaTemporal += inspiracionBonoDefensa;
        datosRyo.bonoAgilidadTemporal += inspiracionBonoAgilidad;

        if (sonidoInspiracion != null) ReproducirSonido(sonidoInspiracion);

        textoMensajes.text = "✨ ¡" + datosRyo.nombre + " entra en estado de Inspiración!\n" +
                             "ATQ +" + inspiracionBonoAtaque + " | DEF +" + inspiracionBonoDefensa +
                             " | AGI +" + inspiracionBonoAgilidad +
                             " durante " + TURNOS_INSPIRACION + " turnos.";
    }

    // ── Wrappers para botones de magia (OnClick del Inspector) ───────────────

    public void BotonMiniincendio() => AccionMagia("Miniincendio");
    public void BotonMinihelada() => AccionMagia("Minihelada");
    public void BotonMinicuracion() => AccionMagia("Minicuracion");
    public void BotonFortalecimiento() => AccionMagia("Fortalecimiento");

    // ── Botón cerrar panel objetos ────────────────────────────────────────────

    public void CerrarPanelObjetos()
    {
        if (panelObjetos != null) panelObjetos.SetActive(false);
    }

    private void OnDestroy()
    {
        if (datosRyo != null) datosRyo.ResetearBonos();
        if (datosPippin != null) datosPippin.ResetearBonos();
    }
}