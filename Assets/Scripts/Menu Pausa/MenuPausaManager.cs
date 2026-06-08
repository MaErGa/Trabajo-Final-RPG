using UnityEngine;
using UnityEngine.UI;
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

    // txtInventario y txtEquipo ya NO se usan para mostrar listas —
    // se mantienen por si los tienes asignados en el Inspector (no romperá nada).
    [Header("Texto Legacy (opcional, no se usa)")]
    public TextMeshProUGUI txtInventario;
    public TextMeshProUGUI txtEquipo;

    [Header("Texto de Conjuros")]
    public TextMeshProUGUI txtConjuros;

    [Header("Texto de Oro (siempre visible)")]
    public TextMeshProUGUI txtOro;

    // ── Panel Inventario Dinámico ─────────────────────────────
    [Header("Inventario Dinámico")]
    /// <summary>El Transform del Content dentro del ScrollView del panelInventario.</summary>
    public Transform contenedorInventario;
    /// <summary>Prefab con el script FilaInventario.</summary>
    public GameObject prefabFilaInventario;
    /// <summary>Texto de feedback ("HP ya al máximo", etc.).</summary>
    public TextMeshProUGUI txtFeedbackInventario;

    // ── Panel Equipo Dinámico ─────────────────────────────────
    [Header("Equipo Dinámico")]
    /// <summary>El Transform del Content dentro del ScrollView del panelEquipo.</summary>
    public Transform contenedorEquipo;
    /// <summary>Prefab con el script FilaEquipo.</summary>
    public GameObject prefabFilaEquipo;
    /// <summary>Texto de feedback ("Equipado: Espada +5", etc.).</summary>
    public TextMeshProUGUI txtFeedbackEquipo;

    // ─────────────────────────────────────────────────────────

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
        if (Input.GetKeyDown(KeyCode.P))
            ToggleMenu();
    }

    public bool MenuActivo()
    {
        return objetoMenu != null && objetoMenu.activeSelf;
    }

    public void SetPausa(bool activa)
    {
        if (objetoMenu == null) return;
        objetoMenu.SetActive(activa);
        if (activa)
        {
            OcultarTodosLosPaneles();
            ActualizarOro();
        }
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
        if (panelInventario == null) return;
        bool nuevoEstado = !panelInventario.activeSelf;
        OcultarTodosLosPaneles();
        panelInventario.SetActive(nuevoEstado);
        if (nuevoEstado) RefrescarPanelInventario();
    }

    public void BotonPresionadoEquipo()
    {
        if (panelEquipo == null) return;
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
        if (contenedorInventario == null || prefabFilaInventario == null || datosRyo == null)
        {
            Debug.LogWarning("[MenuPausa] Faltan referencias en el panel de inventario.");
            return;
        }

        // Limpiar filas anteriores
        foreach (Transform hijo in contenedorInventario)
            Destroy(hijo.gameObject);

        LimpiarFeedback(txtFeedbackInventario);

        bool hayItems = false;

        // Items consumibles de la mochila
        foreach (var item in datosRyo.mochilaItems)
        {
            if (item == null) continue;
            hayItems = true;
            GameObject fila = Instantiate(prefabFilaInventario, contenedorInventario);
            FilaInventario filaScript = fila.GetComponent<FilaInventario>();
            if (filaScript != null)
                filaScript.Inicializar(item, OnUsarItem);
        }

        if (!hayItems)
            MostrarFeedback(txtFeedbackInventario, "La mochila está vacía.");
    }

    void OnUsarItem(ItemConsumible item)
    {
        bool exito = UsarItemConsumible.UsarItem(item, datosRyo);

        if (exito)
        {
            // Quitar UNA unidad del item de la mochila
            datosRyo.mochilaItems.Remove(item);
            MostrarFeedback(txtFeedbackInventario,
                "Usaste " + item.nombre + ". " + item.queCura + " +" + item.potencia);
            ActualizarOro();
            RefrescarPanelInventario(); // Recargar la lista actualizada
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
        if (contenedorEquipo == null || prefabFilaEquipo == null || datosRyo == null)
        {
            Debug.LogWarning("[MenuPausa] Faltan referencias en el panel de equipo.");
            return;
        }

        // Limpiar filas anteriores
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

            GameObject fila = Instantiate(prefabFilaEquipo, contenedorEquipo);
            FilaEquipo filaScript = fila.GetComponent<FilaEquipo>();
            if (filaScript != null)
                filaScript.Inicializar(equipo, yaEquipado, OnEquiparItem);
        }
    }

    void OnEquiparItem(EquipoBase equipo)
    {
        datosRyo.EquiparObjeto(equipo);
        MostrarFeedback(txtFeedbackEquipo, "Equipado: " + equipo.nombre);
        RefrescarPanelEquipo(); // Recargar para actualizar estado "Equipado"
    }

    /// <summary>Comprueba si un equipo ya está en alguno de los slots equipados.</summary>
    bool EstaEquipado(EquipoBase equipo)
    {
        return equipo == datosRyo.armaEquipadaAsset
            || equipo == datosRyo.armaduraEquipadaAsset
            || equipo == datosRyo.escudoEquipadoAsset
            || equipo == datosRyo.cascoEquipadoAsset
            || equipo == datosRyo.accesorioEquipadoAsset;
    }

    // ── Actualización de datos (Stats, Conjuros, Oro) ─────────

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