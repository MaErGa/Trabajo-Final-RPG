using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Text;

public class MenuPausaManager : MonoBehaviour
{
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

    [Header("Texto Legacy (opcional, no se usa)")]
    public TextMeshProUGUI txtInventario;
    public TextMeshProUGUI txtEquipo;

    [Header("Texto de Conjuros")]
    public TextMeshProUGUI txtConjuros;

    [Header("Texto de Oro (siempre visible)")]
    public TextMeshProUGUI txtOro;

    [Header("Inventario Dinámico")]
    public Transform contenedorInventario;
    public GameObject prefabFilaInventario;
    public TextMeshProUGUI txtFeedbackInventario;

    [Header("Equipo Dinámico")]
    public Transform contenedorEquipo;
    public GameObject prefabFilaEquipo;
    public TextMeshProUGUI txtFeedbackEquipo;

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
        OcultarTodosLosPaneles();
        panelStats.SetActive(nuevoEstado);
        if (nuevoEstado) ActualizarStats();
    }

    public void BotonPresionadoInventario()
    {
        Debug.Log("[MenuPausa] BotonPresionadoInventario llamado");
        if (panelInventario == null) { Debug.LogError("[MenuPausa] panelInventario es NULL"); return; }
        bool nuevoEstado = !panelInventario.activeSelf;
        OcultarTodosLosPaneles();
        panelInventario.SetActive(nuevoEstado);
        if (nuevoEstado) RefrescarPanelInventario();
    }

    public void BotonPresionadoEquipo()
    {
        Debug.Log("[MenuPausa] BotonPresionadoEquipo llamado");
        if (panelEquipo == null) { Debug.LogError("[MenuPausa] panelEquipo es NULL"); return; }
        bool nuevoEstado = !panelEquipo.activeSelf;
        OcultarTodosLosPaneles();
        panelEquipo.SetActive(nuevoEstado);
        if (nuevoEstado) RefrescarPanelEquipo();
    }

    public void BotonPresionadoConjuros()
    {
        if (panelConjuros == null) return;
        bool nuevoEstado = !panelConjuros.activeSelf;
        OcultarTodosLosPaneles();
        panelConjuros.SetActive(nuevoEstado);
        if (nuevoEstado) ActualizarConjuros();
    }

    // ── Panel Inventario ──────────────────────────────────────

    void RefrescarPanelInventario()
    {
        Debug.Log("[Inventario] RefrescarPanelInventario llamado");

        if (contenedorInventario == null) { Debug.LogError("[Inventario] contenedorInventario es NULL"); return; }
        if (prefabFilaInventario == null) { Debug.LogError("[Inventario] prefabFilaInventario es NULL"); return; }
        if (datosRyo == null)             { Debug.LogError("[Inventario] datosRyo es NULL"); return; }

        Debug.Log("[Inventario] Items en mochila: " + datosRyo.mochilaItems.Count);

        foreach (Transform hijo in contenedorInventario)
            Destroy(hijo.gameObject);

        LimpiarFeedback(txtFeedbackInventario);

        bool hayItems = false;

        foreach (var item in datosRyo.mochilaItems)
        {
            if (item == null) { Debug.LogWarning("[Inventario] Item null encontrado"); continue; }
            hayItems = true;
            Debug.Log("[Inventario] Instanciando fila para: " + item.nombre);
            GameObject fila = Instantiate(prefabFilaInventario, contenedorInventario);
            FilaInventario filaScript = fila.GetComponent<FilaInventario>();
            if (filaScript != null)
                filaScript.Inicializar(item, OnUsarItem);
            else
                Debug.LogError("[Inventario] El prefab NO tiene el componente FilaInventario");
        }

        if (!hayItems)
            MostrarFeedback(txtFeedbackInventario, "La mochila está vacía.");
    }

    void OnUsarItem(ItemConsumible item)
    {
        bool exito = UsarItemConsumible.UsarItem(item, datosRyo);
        if (exito)
        {
            datosRyo.mochilaItems.Remove(item);
            MostrarFeedback(txtFeedbackInventario,
                "Usaste " + item.nombre + ". " + item.queCura + " +" + item.potencia);
            ActualizarOro();
            RefrescarPanelInventario();
        }
        else
        {
            MostrarFeedback(txtFeedbackInventario,
                "No puedes usar " + item.nombre + " ahora mismo.");
        }
    }

    // ── Panel Equipo ──────────────────────────────────────────

    void RefrescarPanelEquipo()
    {
        Debug.Log("[Equipo] RefrescarPanelEquipo llamado");

        if (contenedorEquipo == null) { Debug.LogError("[Equipo] contenedorEquipo es NULL"); return; }
        if (prefabFilaEquipo == null) { Debug.LogError("[Equipo] prefabFilaEquipo es NULL"); return; }
        if (datosRyo == null)         { Debug.LogError("[Equipo] datosRyo es NULL"); return; }

        Debug.Log("[Equipo] Items en armario: " + (datosRyo.armarioEquipo != null ? datosRyo.armarioEquipo.Count : 0));

        foreach (Transform hijo in contenedorEquipo)
            Destroy(hijo.gameObject);

        LimpiarFeedback(txtFeedbackEquipo);

        if (datosRyo.armarioEquipo == null || datosRyo.armarioEquipo.Count == 0)
        {
            MostrarFeedback(txtFeedbackEquipo, "El armario está vacío.");
            return;
        }

        foreach (var equipo in datosRyo.armarioEquipo)
        {
            if (equipo == null) continue;
            bool yaEquipado = EstaEquipado(equipo);
            Debug.Log("[Equipo] Instanciando fila para: " + equipo.nombre + " equipado=" + yaEquipado);
            GameObject fila = Instantiate(prefabFilaEquipo, contenedorEquipo);
            FilaEquipo filaScript = fila.GetComponent<FilaEquipo>();
            if (filaScript != null)
                filaScript.Inicializar(equipo, yaEquipado, OnEquiparItem);
            else
                Debug.LogError("[Equipo] El prefab NO tiene el componente FilaEquipo");
        }
    }

    void OnEquiparItem(EquipoBase equipo)
    {
        datosRyo.EquiparObjeto(equipo);
        MostrarFeedback(txtFeedbackEquipo, "Equipado: " + equipo.nombre);
        RefrescarPanelEquipo();
    }

    bool EstaEquipado(EquipoBase equipo)
    {
        return equipo == datosRyo.armaEquipadaAsset
            || equipo == datosRyo.armaduraEquipadaAsset
            || equipo == datosRyo.escudoEquipadoAsset
            || equipo == datosRyo.cascoEquipadoAsset
            || equipo == datosRyo.accesorioEquipadoAsset;
    }

    // ── Stats / Conjuros / Oro ────────────────────────────────

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

    void ActualizarConjuros()
    {
        if (txtConjuros == null || datosRyo == null) return;
        StringBuilder sb = new StringBuilder();
        bool tieneConjuros = false;
        if (datosRyo.nivel >= 3) { sb.AppendLine("• Minicuración (MP: 2) — Cura 20+" + datosRyo.terapeucidad + " HP"); tieneConjuros = true; }
        if (datosRyo.nivel >= 8) { sb.AppendLine("• Minihelada (MP: 3) — Daño 15+" + datosRyo.fuerzaMagica); tieneConjuros = true; }
        if (!tieneConjuros) sb.AppendLine("Aún no conoces ningún conjuro.");
        txtConjuros.text = sb.ToString();
    }

    // ── Feedback ──────────────────────────────────────────────

    void MostrarFeedback(TextMeshProUGUI txt, string mensaje)
    {
        if (txt != null) txt.text = mensaje;
    }

    void LimpiarFeedback(TextMeshProUGUI txt)
    {
        if (txt != null) txt.text = "";
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