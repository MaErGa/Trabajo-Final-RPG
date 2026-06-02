using UnityEngine;
using UnityEngine.UI;
using TMPro;

// ─────────────────────────────────────────────────────────────────────────────
//  MenuController.cs  –  Menú principal estilo FF7
//  El script vive dentro del Canvas del menú.
//  Para no desactivarse a sí mismo (y perder el Update), usa CanvasGroup
//  para mostrar/ocultar toda la UI sin tocar SetActive en este GameObject.
//
//  SETUP (sin arrastrar nada al inspector para el menuRoot):
//    · Añade este script al GameObject raíz del Canvas del menú.
//    · El CanvasGroup se crea automáticamente en Start() si no existe.
//    · El resto de referencias (paneles, botones, TMP) se asignan en el inspector
//      como siempre.
// ─────────────────────────────────────────────────────────────────────────────
public class MenuController : MonoBehaviour
{
    // ── Control de visibilidad (sin SetActive sobre sí mismo) ─────────────────
    private CanvasGroup canvasGroup;
    private bool menuAbierto = false;

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
    public Button btnEstado;

    // ─────────────────────────────────────────────────────────────────────────
    //  STATS PANEL – referencias TMP
    // ─────────────────────────────────────────────────────────────────────────
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
    public TextMeshProUGUI tmpAtaque;
    public TextMeshProUGUI tmpDefensa;
    public TextMeshProUGUI tmpAgilidad;
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

    // ── Referencia al InventoryManager ────────────────────────────────────────
    [Header("Referencia al InventoryManager del panel inventario")]
    public InventoryManager inventoryManager;

    // ── Equipo Panel ──────────────────────────────────────────────────────────
    [Header("Equipo Panel – slots actuales")]
    public TextMeshProUGUI equipTmpArma;
    public TextMeshProUGUI equipTmpArmadura;
    public TextMeshProUGUI equipTmpEscudo;
    public TextMeshProUGUI equipTmpCasco;
    public TextMeshProUGUI equipTmpAccesorio;
    public TextMeshProUGUI equipTmpBonos;

    // ─────────────────────────────────────────────────────────────────────────
    void Start()
    {
        // ── CanvasGroup: se crea solo si no existe en el inspector ────────────
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = gameObject.AddComponent<CanvasGroup>();

        // Conectar botones
        btnItem?.onClick.AddListener(AbrirInventario);
        btnEquipo?.onClick.AddListener(AbrirEquipo);
        btnEstado?.onClick.AddListener(AbrirStats);

        // Menú oculto al arrancar (sin desactivar el GameObject)
        SetMenuVisible(false);
        menuAbierto = false;
    }

    void Update()
    {
        // El timer solo corre mientras el juego está activo
        if (!menuAbierto)
            segundosJugados += Time.deltaTime;

        ActualizarTiempo();

        // Abrir / cerrar menú con P
        if (Input.GetKeyDown(KeyCode.P))
        {
            menuAbierto = !menuAbierto;
            SetMenuVisible(menuAbierto);

            if (menuAbierto) AbrirStats();
        }
    }

    // ─────────────────────────────────────────────────────────────────────────
    //  VISIBILIDAD  –  CanvasGroup en lugar de SetActive
    // ─────────────────────────────────────────────────────────────────────────
    void SetMenuVisible(bool visible)
    {
        canvasGroup.alpha          = visible ? 1f : 0f;
        canvasGroup.interactable   = visible;
        canvasGroup.blocksRaycasts = visible;
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

        if (tmpNombreJugador) tmpNombreJugador.text = datosJugador.nombre;
        if (tmpNivel)         tmpNivel.text         = datosJugador.nivel.ToString();
        if (tmpExp)           tmpExp.text           = datosJugador.experiencia.ToString();
        if (tmpExpSiguiente)  tmpExpSiguiente.text  = datosJugador.expSiguienteNivel.ToString();

        if (tmpHpActual) tmpHpActual.text = datosJugador.hpActual.ToString();
        if (tmpHpMax)    tmpHpMax.text    = datosJugador.hpMax.ToString();
        if (tmpMpActual) tmpMpActual.text = datosJugador.mpActual.ToString();
        if (tmpMpMax)    tmpMpMax.text    = datosJugador.mpMax.ToString();

        if (sliderHP && datosJugador.hpMax > 0)
            sliderHP.value = (float)datosJugador.hpActual / datosJugador.hpMax;
        if (sliderMP && datosJugador.mpMax > 0)
            sliderMP.value = (float)datosJugador.mpActual / datosJugador.mpMax;

        if (tmpAtaque)       tmpAtaque.text       = datosJugador.AtaqueTotal.ToString();
        if (tmpDefensa)      tmpDefensa.text      = datosJugador.DefensaTotal.ToString();
        if (tmpAgilidad)     tmpAgilidad.text     = datosJugador.AgilidadTotal.ToString();
        if (tmpFuerzaMagica) tmpFuerzaMagica.text = datosJugador.fuerzaMagica.ToString();
        if (tmpTerapeucidad) tmpTerapeucidad.text = datosJugador.terapeucidad.ToString();

        if (tmpArma)      tmpArma.text      = datosJugador.armaEquipadaAsset      != null ? datosJugador.armaEquipadaAsset.nombre      : "Ninguno";
        if (tmpArmadura)  tmpArmadura.text  = datosJugador.armaduraEquipadaAsset  != null ? datosJugador.armaduraEquipadaAsset.nombre  : "Ninguno";
        if (tmpEscudo)    tmpEscudo.text    = datosJugador.escudoEquipadoAsset    != null ? datosJugador.escudoEquipadoAsset.nombre    : "Ninguno";
        if (tmpCasco)     tmpCasco.text     = datosJugador.cascoEquipadoAsset     != null ? datosJugador.cascoEquipadoAsset.nombre     : "Ninguno";
        if (tmpAccesorio) tmpAccesorio.text = datosJugador.accesorioEquipadoAsset != null ? datosJugador.accesorioEquipadoAsset.nombre : "Ninguno";

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