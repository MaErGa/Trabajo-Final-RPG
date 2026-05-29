using UnityEngine;
using TMPro;
using System.Text;

public class MenuPausaManager : MonoBehaviour
{
    // Instancia estática para que PlayerController pueda consultarla
    public static MenuPausaManager instancia;

    [Header("Configuración de Datos")]
    public DatosJugador datosRyo;
    public GameObject objetoMenu;

    [Header("Paneles de Secciones")]
    public GameObject panelStats;
    public GameObject panelInventario;
    public GameObject panelEquipo;
    public GameObject panelConjuros;

    [Header("Texto de Stats")]
    public TextMeshProUGUI txtStats;

    [Header("Texto de Inventario")]
    public TextMeshProUGUI txtInventario;

    [Header("Texto de Equipo")]
    public TextMeshProUGUI txtEquipo;

    [Header("Texto de Conjuros")]
    public TextMeshProUGUI txtConjuros;

    [Header("Texto de Oro (siempre visible)")]
    public TextMeshProUGUI txtOro;

    void Awake()
    {
        instancia = this;
    }

    void Start()
    {
        OcultarTodosLosPaneles();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            ToggleMenu();
    }

    // Método que consulta PlayerController para saber si el menú está abierto
    public bool MenuActivo()
    {
        return objetoMenu != null && objetoMenu.activeSelf;
    }

    public void ToggleMenu()
    {
        if (objetoMenu == null) return;

        bool abriendo = !objetoMenu.activeSelf;
        objetoMenu.SetActive(abriendo);

        if (abriendo)
        {
            OcultarTodosLosPaneles();
            ActualizarOro();
        }
    }

    // ── Botones ──────────────────────────────────────────────

    public void BotonPresionadoStats()
    {
        if (panelStats == null) return;
        bool nuevoEstado = !panelStats.activeSelf;
        panelStats.SetActive(nuevoEstado);
        if (nuevoEstado) ActualizarStats();
    }

    public void BotonPresionadoInventario()
    {
        if (panelInventario == null) return;
        bool nuevoEstado = !panelInventario.activeSelf;
        panelInventario.SetActive(nuevoEstado);
        if (nuevoEstado) ActualizarInventario();
    }

    public void BotonPresionadoEquipo()
    {
        if (panelEquipo == null) return;
        bool nuevoEstado = !panelEquipo.activeSelf;
        panelEquipo.SetActive(nuevoEstado);
        if (nuevoEstado) ActualizarEquipo();
    }

    public void BotonPresionadoConjuros()
    {
        if (panelConjuros == null) return;
        bool nuevoEstado = !panelConjuros.activeSelf;
        panelConjuros.SetActive(nuevoEstado);
        if (nuevoEstado) ActualizarConjuros();
    }

    // ── Actualización de datos ────────────────────────────────

    void ActualizarOro()
    {
        if (txtOro && datosRyo != null)
            txtOro.text = "Oro: " + datosRyo.oro + " G";
    }

    void ActualizarStats()
    {
        if (txtStats == null || datosRyo == null) return;

        StringBuilder sb = new StringBuilder();
        sb.AppendLine("Nombre: "   + datosRyo.nombre);
        sb.AppendLine("Nivel: "    + datosRyo.nivel);
        sb.AppendLine("HP: "       + datosRyo.hpActual + " / " + datosRyo.hpMax);
        sb.AppendLine("MP: "       + datosRyo.mpActual + " / " + datosRyo.mpMax);
        sb.AppendLine("Ataque: "   + datosRyo.AtaqueTotal);
        sb.AppendLine("Defensa: "  + datosRyo.DefensaTotal);
        sb.AppendLine("Agilidad: " + datosRyo.AgilidadTotal);
        sb.AppendLine("EXP: "      + datosRyo.experiencia);
        sb.AppendLine("Sig. NV: "  + datosRyo.expSiguienteNivel);

        txtStats.text = sb.ToString();
    }

    void ActualizarInventario()
    {
        if (txtInventario == null || datosRyo == null) return;

        if ((datosRyo.mochilaItems == null || datosRyo.mochilaItems.Count == 0)
            && datosRyo.plantasMedicinales == 0 && datosRyo.colaDeConejo == 0)
        {
            txtInventario.text = "Mochila vacía.";
            return;
        }

        StringBuilder sb = new StringBuilder();
        foreach (var item in datosRyo.mochilaItems)
            if (item != null)
                sb.AppendLine("• " + item.nombre + " (" + item.queCura + " +" + item.potencia + ")");

        if (datosRyo.plantasMedicinales > 0)
            sb.AppendLine("• Planta Medicinal x" + datosRyo.plantasMedicinales);
        if (datosRyo.colaDeConejo > 0)
            sb.AppendLine("• Cola de Conejo x" + datosRyo.colaDeConejo);

        txtInventario.text = sb.ToString();
    }

    void ActualizarEquipo()
    {
        if (txtEquipo == null || datosRyo == null) return;

        StringBuilder sb = new StringBuilder();
        sb.AppendLine("Arma: "      + NombreEquipo(datosRyo.armaEquipadaAsset,      datosRyo.armaEquipada));
        sb.AppendLine("Armadura: "  + NombreEquipo(datosRyo.armaduraEquipadaAsset,  datosRyo.armaduraEquipada));
        sb.AppendLine("Escudo: "    + NombreEquipo(datosRyo.escudoEquipadoAsset,    datosRyo.escudoEquipado));
        sb.AppendLine("Casco: "     + NombreEquipo(datosRyo.cascoEquipadoAsset,     datosRyo.cascoEquipado));
        sb.AppendLine("Accesorio: " + NombreEquipo(datosRyo.accesorioEquipadoAsset, datosRyo.accesorioEquipado));

        if (datosRyo.armarioEquipo != null && datosRyo.armarioEquipo.Count > 0)
        {
            sb.AppendLine("");
            foreach (var equipo in datosRyo.armarioEquipo)
                if (equipo != null)
                    sb.AppendLine("• " + equipo.nombre);
        }

        txtEquipo.text = sb.ToString();
    }

    void ActualizarConjuros()
    {
        if (txtConjuros == null || datosRyo == null) return;

        StringBuilder sb = new StringBuilder();
        bool tieneConjuros = false;

        if (datosRyo.nivel >= 3)
        {
            sb.AppendLine("• Minicuración (MP: 2) — Cura 20+" + datosRyo.terapeucidad + " HP");
            tieneConjuros = true;
        }
        if (datosRyo.nivel >= 8)
        {
            sb.AppendLine("• Minihelada (MP: 3) — Daño 15+" + datosRyo.fuerzaMagica);
            tieneConjuros = true;
        }

        if (!tieneConjuros)
            sb.AppendLine("Aún no conoces ningún conjuro.");

        txtConjuros.text = sb.ToString();
    }

    // ── Utilidades ────────────────────────────────────────────

    string NombreEquipo(EquipoBase asset, string nombreAntiguo)
    {
        if (asset != null) return asset.nombre;
        if (!string.IsNullOrEmpty(nombreAntiguo)) return nombreAntiguo;
        return "Ninguno";
    }

    void OcultarTodosLosPaneles()
    {
        if (panelStats)      panelStats.SetActive(false);
        if (panelInventario) panelInventario.SetActive(false);
        if (panelEquipo)     panelEquipo.SetActive(false);
        if (panelConjuros)   panelConjuros.SetActive(false);
    }
}