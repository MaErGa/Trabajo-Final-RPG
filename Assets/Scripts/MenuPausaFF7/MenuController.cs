using UnityEngine;
using UnityEngine.UI;
using TMPro;

// ─────────────────────────────────────────────────────────────────────────────
//  MenuController.cs  –  Menú principal estilo FF7
//  Un solo jugador. Usa tus scripts: DatosJugador, EquipoBase, ItemConsumible.
//
//  PANELES:
//    · StatsPanel     → Muestra HP, MP, nivel, stats, equipo puesto
//    · InventarioPanel → Lista de items y equipo del armario (usa InventoryManager)
//    · EquipoPanel    → Vista rápida de lo equipado actualmente
// ─────────────────────────────────────────────────────────────────────────────
public class MenuController : MonoBehaviour
{
    // ── Datos ─────────────────────────────────────────────────────────────────
    [Header("Datos del Jugador (ScriptableObject)")]
    public DatosJugador datosJugador;

    // ── Paneles ───────────────────────────────────────────────────────────────
    [Header("Paneles principales")]
    public GameObject statsPanel;
    public GameObject inventarioPanel;
    public GameObject equipoPanel;

    // ── Botones del menú lateral ──────────────────────────────────────────────
    [Header("Botones sidebar")]
    public Button btnItem;
    public Button btnEquipo;
    public Button btnEstado;     // abre statsPanel

    // ────────────────────────────────────────────────────────────────────────
    //  STATS PANEL – referencias TMP
    // ────────────────────────────────────────────────────────────────────────
    [Header("Stats Panel – nombre y nivel")]
    public TextMeshProUGUI tmpNombreJugador;
    public TextMeshProUGUI tmpNivel;
    public TextMeshProUGUI tmpExp;
    public TextMeshProUGUI tmpExpSiguiente;

    [Header("Stats Panel – HP / MP")]
    public TextMeshProUGUI tmpHpActual;
    public TextMeshProUGUI tmpHpMax;
    public TextMeshProUGUI tmpMpActual;
    public TextMeshProUGUI tmpMpMax;
    public Slider sliderHP;
    public Slider sliderMP;

    [Header("Stats Panel – atributos de combate")]
    public TextMeshProUGUI tmpAtaque;        // AtaqueTotal (con bonos)
    public TextMeshProUGUI tmpDefensa;       // DefensaTotal (con bonos)
    public TextMeshProUGUI tmpAgilidad;      // AgilidadTotal (con bonos)
    public TextMeshProUGUI tmpFuerzaMagica;
    public TextMeshProUGUI tmpTerapeucidad;

    [Header("Stats Panel – equipo puesto")]
    public TextMeshProUGUI tmpArma;
    public TextMeshProUGUI tmpArmadura;
    public TextMeshProUGUI tmpEscudo;
    public TextMeshProUGUI tmpCasco;
    public TextMeshProUGUI tmpAccesorio;

    [Header("Stats Panel – oro")]
    public TextMeshProUGUI tmpOro;

    // ── HUD de tiempo ─────────────────────────────────────────────────────────
    [Header("HUD – tiempo de juego")]
    public TextMeshProUGUI tmpTiempo;
    private float segundosJugados = 0f;

    // ── Referencia al InventoryManager (en el panel de inventario) ────────────
    [Header("Referencia al InventoryManager del panel inventario")]
    public InventoryManager inventoryManager;

    // ── Equipo Panel – referencias ─────────────────────────────────────────────
    [Header("Equipo Panel – slots actuales")]
    public TextMeshProUGUI equipTmpArma;
    public TextMeshProUGUI equipTmpArmadura;
    public TextMeshProUGUI equipTmpEscudo;
    public TextMeshProUGUI equipTmpCasco;
    public TextMeshProUGUI equipTmpAccesorio;
    public TextMeshProUGUI equipTmpBonos;    // resumen de bonos totales del equipo

    // ─────────────────────────────────────────────────────────────────────────
    void Start()
    {
        // Conectar botones
        btnItem?.onClick.AddListener(AbrirInventario);
        btnEquipo?.onClick.AddListener(AbrirEquipo);
        btnEstado?.onClick.AddListener(AbrirStats);

        // Arrancar en stats
        AbrirStats();
    }

    void Update()
    {
        segundosJugados += Time.deltaTime;
        ActualizarTiempo();
    }

    // ─────────────────────────────────────────────────────────────────────────
    //  NAVEGACIÓN
    // ─────────────────────────────────────────────────────────────────────────
    public void AbrirStats()
    {
        statsPanel?.SetActive(true);
        inventarioPanel?.SetActive(false);
        equipoPanel?.SetActive(false);
        RefrescarStats();
    }

    public void AbrirInventario()
    {
        statsPanel?.SetActive(false);
        inventarioPanel?.SetActive(true);
        equipoPanel?.SetActive(false);
        // InventoryManager se refresca solo en OnEnable al activar el panel
    }

    public void AbrirEquipo()
    {
        statsPanel?.SetActive(false);
        inventarioPanel?.SetActive(false);
        equipoPanel?.SetActive(true);
        RefrescarPanelEquipo();
    }

    // ─────────────────────────────────────────────────────────────────────────
    //  REFRESCAR STATS
    // ─────────────────────────────────────────────────────────────────────────
    public void RefrescarStats()
    {
        if (datosJugador == null) return;

        // Nombre y nivel
        if (tmpNombreJugador) tmpNombreJugador.text = datosJugador.nombre;
        if (tmpNivel)         tmpNivel.text         = datosJugador.nivel.ToString();
        if (tmpExp)           tmpExp.text           = datosJugador.experiencia.ToString();
        if (tmpExpSiguiente)  tmpExpSiguiente.text  = datosJugador.expSiguienteNivel.ToString();

        // HP / MP  con barras
        if (tmpHpActual) tmpHpActual.text = datosJugador.hpActual.ToString();
        if (tmpHpMax)    tmpHpMax.text    = datosJugador.hpMax.ToString();
        if (tmpMpActual) tmpMpActual.text = datosJugador.mpActual.ToString();
        if (tmpMpMax)    tmpMpMax.text    = datosJugador.mpMax.ToString();

        if (sliderHP && datosJugador.hpMax > 0)
            sliderHP.value = (float)datosJugador.hpActual / datosJugador.hpMax;
        if (sliderMP && datosJugador.mpMax > 0)
            sliderMP.value = (float)datosJugador.mpActual / datosJugador.mpMax;

        // Atributos (usan las propiedades calculadas de DatosJugador)
        if (tmpAtaque)       tmpAtaque.text       = datosJugador.AtaqueTotal.ToString();
        if (tmpDefensa)      tmpDefensa.text      = datosJugador.DefensaTotal.ToString();
        if (tmpAgilidad)     tmpAgilidad.text     = datosJugador.AgilidadTotal.ToString();
        if (tmpFuerzaMagica) tmpFuerzaMagica.text = datosJugador.fuerzaMagica.ToString();
        if (tmpTerapeucidad) tmpTerapeucidad.text = datosJugador.terapeucidad.ToString();

        // Equipo equipado (muestra nombre o "Ninguno")
        if (tmpArma)      tmpArma.text      = datosJugador.armaEquipadaAsset      != null ? datosJugador.armaEquipadaAsset.nombre      : "Ninguno";
        if (tmpArmadura)  tmpArmadura.text  = datosJugador.armaduraEquipadaAsset  != null ? datosJugador.armaduraEquipadaAsset.nombre  : "Ninguno";
        if (tmpEscudo)    tmpEscudo.text    = datosJugador.escudoEquipadoAsset    != null ? datosJugador.escudoEquipadoAsset.nombre    : "Ninguno";
        if (tmpCasco)     tmpCasco.text     = datosJugador.cascoEquipadoAsset     != null ? datosJugador.cascoEquipadoAsset.nombre     : "Ninguno";
        if (tmpAccesorio) tmpAccesorio.text = datosJugador.accesorioEquipadoAsset != null ? datosJugador.accesorioEquipadoAsset.nombre : "Ninguno";

        // Oro
        if (tmpOro) tmpOro.text = datosJugador.oro.ToString();
    }

    // ─────────────────────────────────────────────────────────────────────────
    //  REFRESCAR PANEL DE EQUIPO
    // ─────────────────────────────────────────────────────────────────────────
    void RefrescarPanelEquipo()
    {
        if (datosJugador == null) return;

        if (equipTmpArma)      equipTmpArma.text      = datosJugador.armaEquipadaAsset      != null ? datosJugador.armaEquipadaAsset.nombre      : "——";
        if (equipTmpArmadura)  equipTmpArmadura.text  = datosJugador.armaduraEquipadaAsset  != null ? datosJugador.armaduraEquipadaAsset.nombre  : "——";
        if (equipTmpEscudo)    equipTmpEscudo.text    = datosJugador.escudoEquipadoAsset    != null ? datosJugador.escudoEquipadoAsset.nombre    : "——";
        if (equipTmpCasco)     equipTmpCasco.text     = datosJugador.cascoEquipadoAsset     != null ? datosJugador.cascoEquipadoAsset.nombre     : "——";
        if (equipTmpAccesorio) equipTmpAccesorio.text = datosJugador.accesorioEquipadoAsset != null ? datosJugador.accesorioEquipadoAsset.nombre : "——";

        // Resumen de bonos totales del equipo actual
        if (equipTmpBonos)
        {
            equipTmpBonos.text =
                $"ATQ total: {datosJugador.AtaqueTotal}\n" +
                $"DEF total: {datosJugador.DefensaTotal}\n" +
                $"AGI total: {datosJugador.AgilidadTotal}";
        }
    }

    // ─────────────────────────────────────────────────────────────────────────
    //  TIEMPO DE JUEGO
    // ─────────────────────────────────────────────────────────────────────────
    void ActualizarTiempo()
    {
        if (tmpTiempo == null) return;
        int h = (int)(segundosJugados / 3600);
        int m = (int)((segundosJugados % 3600) / 60);
        int s = (int)(segundosJugados % 60);
        tmpTiempo.text = $"{h}:{m:D2}:{s:D2}";
    }
}