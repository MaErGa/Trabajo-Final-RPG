using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Text;
using System.Collections.Generic;

public class MenuPausaManager : MonoBehaviour
{
    public static MenuPausaManager instancia;

    [Header("Datos del Jugador")]
    public DatosJugador datosRyo;
    public GameObject objetoMenu;

    [Header("Panel Superior")]
    public TextMeshProUGUI txtNombre;
    public TextMeshProUGUI txtHP;
    public TextMeshProUGUI txtMP;
    public TextMeshProUGUI txtOro;

    [Header("Botones Menu Izquierdo")]
    public Button btnStats;
    public Button btnInventario;
    public Button btnEquipo;
    public Button btnConjuros;
    public Button btnSalir;

    [Header("Paneles de Contenido")]
    public GameObject panelStats;
    public GameObject panelInventario;
    public GameObject panelEquipo;
    public GameObject panelConjuros;

    [Header("Stats")]
    public TextMeshProUGUI txtStats;

    [Header("Conjuros")]
    public TextMeshProUGUI txtConjuros;

    [Header("Inventario")]
    public Transform contenedorItems;
    public GameObject prefabBotonItem;
    public GameObject panelDetalleInventario;
    public TextMeshProUGUI txtNombreItem;
    public TextMeshProUGUI txtDescItem;
    public Button btnUsar;

    [Header("Equipo")]
    public Transform contenedorEquipo;
    public GameObject prefabBotonEquipo;
    public GameObject panelDetalleEquipo;
    public TextMeshProUGUI txtNombreEquipo;
    public TextMeshProUGUI txtDescEquipo;
    public Button btnEquipar;

    private ItemConsumible itemSeleccionado;
    private EquipoBase equipoSeleccionado;

    void Awake()
    {
        instancia = this;
    }

    void Start()
    {
        if (objetoMenu != null) objetoMenu.SetActive(false);

        if (btnStats != null)      btnStats.onClick.AddListener(MostrarStats);
        if (btnInventario != null) btnInventario.onClick.AddListener(MostrarInventario);
        if (btnEquipo != null)     btnEquipo.onClick.AddListener(MostrarEquipo);
        if (btnConjuros != null)   btnConjuros.onClick.AddListener(MostrarConjuros);
        if (btnSalir != null)      btnSalir.onClick.AddListener(SalirAlTitulo);

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

    public void ToggleMenu()
    {
        if (objetoMenu == null) return;
        bool abriendo = !objetoMenu.activeSelf;
        objetoMenu.SetActive(abriendo);
        if (abriendo)
        {
            OcultarTodosLosPaneles();
            ActualizarPanelSuperior();
            MostrarStats();
        }
    }

    // ── Navegación ────────────────────────────────────────────

    public void MostrarStats()
    {
        Debug.Log("[Menu] MostrarStats");
        OcultarTodosLosPaneles();
        if (panelStats != null) panelStats.SetActive(true);
        else Debug.LogWarning("[Menu] panelStats es null!");
        ActualizarStats();
    }

    public void MostrarInventario()
    {
        Debug.Log("[Menu] MostrarInventario");
        OcultarTodosLosPaneles();
        if (panelInventario != null) panelInventario.SetActive(true);
        else Debug.LogWarning("[Menu] panelInventario es null!");
        if (panelDetalleInventario != null) panelDetalleInventario.SetActive(false);
        CargarInventario();
    }

    public void MostrarEquipo()
    {
        Debug.Log("[Menu] MostrarEquipo");
        OcultarTodosLosPaneles();
        if (panelEquipo != null)
        {
            panelEquipo.SetActive(true);
            Debug.Log("[Menu] PanelEquipo activado: " + panelEquipo.activeSelf);
        }
        else Debug.LogWarning("[Menu] panelEquipo es null!");
        if (panelDetalleEquipo != null) panelDetalleEquipo.SetActive(false);
        CargarEquipo();
    }

    public void MostrarConjuros()
    {
        Debug.Log("[Menu] MostrarConjuros");
        OcultarTodosLosPaneles();
        if (panelConjuros != null) panelConjuros.SetActive(true);
        else Debug.LogWarning("[Menu] panelConjuros es null!");
        ActualizarConjuros();
    }

    public void SalirAlTitulo()
    {
        SceneManager.LoadScene("Titulo");
    }

    // ── Panel Superior ────────────────────────────────────────

    void ActualizarPanelSuperior()
    {
        if (datosRyo == null) return;
        if (txtNombre != null) txtNombre.text = datosRyo.nombre;
        if (txtHP != null)     txtHP.text = "HP: " + datosRyo.hpActual + "/" + datosRyo.hpMax;
        if (txtMP != null)     txtMP.text = "MP: " + datosRyo.mpActual + "/" + datosRyo.mpMax;
        if (txtOro != null)    txtOro.text = "Oro: " + datosRyo.oro + "G";
    }

    // ── Stats ─────────────────────────────────────────────────

    void ActualizarStats()
    {
        if (txtStats == null || datosRyo == null) return;
        StringBuilder sb = new StringBuilder();
        sb.AppendLine("Nombre:   " + datosRyo.nombre);
        sb.AppendLine("Nivel:    " + datosRyo.nivel);
        sb.AppendLine("HP:       " + datosRyo.hpActual + " / " + datosRyo.hpMax);
        sb.AppendLine("MP:       " + datosRyo.mpActual + " / " + datosRyo.mpMax);
        sb.AppendLine("Ataque:   " + datosRyo.AtaqueTotal);
        sb.AppendLine("Defensa:  " + datosRyo.DefensaTotal);
        sb.AppendLine("Agilidad: " + datosRyo.AgilidadTotal);
        sb.AppendLine("EXP:      " + datosRyo.experiencia);
        sb.AppendLine("Sig. NV:  " + datosRyo.expSiguienteNivel);
        txtStats.text = sb.ToString();
    }

    // ── Inventario ────────────────────────────────────────────

    void CargarInventario()
    {
        if (contenedorItems == null) { Debug.LogWarning("[Menu] contenedorItems es null!"); return; }
        foreach (Transform hijo in contenedorItems)
            Destroy(hijo.gameObject);

        bool vacio = true;

        foreach (var item in datosRyo.mochilaItems)
        {
            if (item == null) continue;
            vacio = false;
            var itemRef = item;
            CrearBoton(contenedorItems, prefabBotonItem, item.nombre, () => SeleccionarItem(itemRef));
        }

        if (datosRyo.plantasMedicinales > 0)
        {
            vacio = false;
            CrearBoton(contenedorItems, prefabBotonItem,
                "Planta Medicinal x" + datosRyo.plantasMedicinales, () => SeleccionarPlanta());
        }

        if (datosRyo.colaDeConejo > 0)
        {
            vacio = false;
            CrearBoton(contenedorItems, prefabBotonItem,
                "Cola de Conejo x" + datosRyo.colaDeConejo, null);
        }

        if (vacio)
            CrearBoton(contenedorItems, prefabBotonItem, "Mochila vacía.", null);
    }

    void SeleccionarItem(ItemConsumible item)
    {
        itemSeleccionado = item;
        if (panelDetalleInventario != null) panelDetalleInventario.SetActive(true);
        if (txtNombreItem != null) txtNombreItem.text = item.nombre;
        if (txtDescItem != null)   txtDescItem.text = item.descripcion + "\nCura: " + item.queCura + " +" + item.potencia;
        if (btnUsar != null)
        {
            btnUsar.onClick.RemoveAllListeners();
            btnUsar.onClick.AddListener(UsarItem);
        }
    }

    void SeleccionarPlanta()
    {
        itemSeleccionado = null;
        if (panelDetalleInventario != null) panelDetalleInventario.SetActive(true);
        if (txtNombreItem != null) txtNombreItem.text = "Planta Medicinal";
        if (txtDescItem != null)   txtDescItem.text = "Restaura 30 HP.";
        if (btnUsar != null)
        {
            btnUsar.onClick.RemoveAllListeners();
            btnUsar.onClick.AddListener(UsarPlanta);
        }
    }

    void UsarItem()
    {
        if (itemSeleccionado == null) return;
        datosRyo.hpActual = Mathf.Min(datosRyo.hpActual + itemSeleccionado.potencia, datosRyo.hpMax);
        datosRyo.mochilaItems.Remove(itemSeleccionado);
        itemSeleccionado = null;
        if (panelDetalleInventario != null) panelDetalleInventario.SetActive(false);
        ActualizarPanelSuperior();
        CargarInventario();
    }

    void UsarPlanta()
    {
        if (datosRyo.plantasMedicinales <= 0) return;
        datosRyo.hpActual = Mathf.Min(datosRyo.hpActual + 30, datosRyo.hpMax);
        datosRyo.plantasMedicinales--;
        if (panelDetalleInventario != null) panelDetalleInventario.SetActive(false);
        ActualizarPanelSuperior();
        CargarInventario();
    }

    // ── Equipo ────────────────────────────────────────────────

    void CargarEquipo()
    {
        if (contenedorEquipo == null) { Debug.LogWarning("[Menu] contenedorEquipo es null!"); return; }
        foreach (Transform hijo in contenedorEquipo)
            Destroy(hijo.gameObject);

        CrearBoton(contenedorEquipo, prefabBotonEquipo,
            "Arma: " + NombreEquipo(datosRyo.armaEquipadaAsset, datosRyo.armaEquipada), null);
        CrearBoton(contenedorEquipo, prefabBotonEquipo,
            "Armadura: " + NombreEquipo(datosRyo.armaduraEquipadaAsset, datosRyo.armaduraEquipada), null);
        CrearBoton(contenedorEquipo, prefabBotonEquipo,
            "Escudo: " + NombreEquipo(datosRyo.escudoEquipadoAsset, datosRyo.escudoEquipado), null);
        CrearBoton(contenedorEquipo, prefabBotonEquipo,
            "Casco: " + NombreEquipo(datosRyo.cascoEquipadoAsset, datosRyo.cascoEquipado), null);
        CrearBoton(contenedorEquipo, prefabBotonEquipo,
            "Accesorio: " + NombreEquipo(datosRyo.accesorioEquipadoAsset, datosRyo.accesorioEquipado), null);

        if (datosRyo.armarioEquipo != null)
        {
            foreach (var equipo in datosRyo.armarioEquipo)
            {
                if (equipo == null) continue;
                var equipoRef = equipo;
                CrearBoton(contenedorEquipo, prefabBotonEquipo,
                    "▶ " + equipo.nombre, () => SeleccionarEquipo(equipoRef));
            }
        }

        Debug.Log("[Menu] CargarEquipo completado. Botones: " + contenedorEquipo.childCount);
    }

    void SeleccionarEquipo(EquipoBase equipo)
    {
        equipoSeleccionado = equipo;
        if (panelDetalleEquipo != null) panelDetalleEquipo.SetActive(true);
        if (txtNombreEquipo != null) txtNombreEquipo.text = equipo.nombre;
        string stats = equipo.descripcion;
        if (equipo.bonoAtaque > 0)   stats += "\nATQ +" + equipo.bonoAtaque;
        if (equipo.bonoDefensa > 0)  stats += "\nDEF +" + equipo.bonoDefensa;
        if (equipo.bonoAgilidad > 0) stats += "\nAGI +" + equipo.bonoAgilidad;
        if (txtDescEquipo != null) txtDescEquipo.text = stats;
        if (btnEquipar != null)
        {
            btnEquipar.onClick.RemoveAllListeners();
            btnEquipar.onClick.AddListener(EquiparSeleccionado);
        }
    }

    void EquiparSeleccionado()
    {
        if (equipoSeleccionado == null) return;
        datosRyo.EquiparObjeto(equipoSeleccionado);
        equipoSeleccionado = null;
        if (panelDetalleEquipo != null) panelDetalleEquipo.SetActive(false);
        CargarEquipo();
    }

    // ── Conjuros ──────────────────────────────────────────────

    void ActualizarConjuros()
    {
        if (txtConjuros == null || datosRyo == null) return;
        StringBuilder sb = new StringBuilder();
        bool tieneConjuros = false;
        if (datosRyo.nivel >= 3) { sb.AppendLine("• Minicuración (MP:2) — Cura 20+" + datosRyo.terapeucidad + " HP"); tieneConjuros = true; }
        if (datosRyo.nivel >= 8) { sb.AppendLine("• Minihelada (MP:3) — Daño 15+" + datosRyo.fuerzaMagica); tieneConjuros = true; }
        if (!tieneConjuros) sb.AppendLine("Aún no conoces ningún conjuro.");
        txtConjuros.text = sb.ToString();
    }

    // ── Utilidades ────────────────────────────────────────────

    void CrearBoton(Transform contenedor, GameObject prefab, string etiqueta, UnityEngine.Events.UnityAction accion)
    {
        if (prefab == null || contenedor == null) return;
        GameObject boton = Instantiate(prefab, contenedor);
        var tmp = boton.GetComponentInChildren<TextMeshProUGUI>();
        if (tmp != null) tmp.text = etiqueta;
        var btn = boton.GetComponent<Button>();
        if (btn != null)
        {
            if (accion != null)
                btn.onClick.AddListener(accion);
            else
                btn.interactable = false;
        }
    }

    string NombreEquipo(EquipoBase asset, string nombreAntiguo)
    {
        if (asset != null) return asset.nombre;
        if (!string.IsNullOrEmpty(nombreAntiguo)) return nombreAntiguo;
        return "Ninguno";
    }

    void OcultarTodosLosPaneles()
    {
        if (panelStats != null)      panelStats.SetActive(false);
        if (panelInventario != null) panelInventario.SetActive(false);
        if (panelEquipo != null)     panelEquipo.SetActive(false);
        if (panelConjuros != null)   panelConjuros.SetActive(false);
    }
}