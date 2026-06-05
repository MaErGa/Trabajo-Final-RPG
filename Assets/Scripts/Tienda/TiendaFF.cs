using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// ╔══════════════════════════════════════════════════════════╗
/// ║  TiendaFF  —  Script único de tienda estilo Final Fantasy ║
/// ╠══════════════════════════════════════════════════════════╣
/// ║  • Genera toda la UI por código (nunca se descuadra)     ║
/// ║  • Comprar / Vender consumibles y equipo                 ║
/// ║  • Diálogo de bienvenida y despedida del NPC integrado   ║
/// ║  • Confirmación de compra/venta                          ║
/// ╠══════════════════════════════════════════════════════════╣
/// ║  SETUP en el Inspector:                                  ║
/// ║    datosRyo              → DatosJugador ScriptableObject ║
/// ║    itemsConsumiblesVenta → tus ItemConsumible assets     ║
/// ║    equipoEnVenta         → tus EquipoBase assets         ║
/// ║    fuentePixel           → fuente TMP pixel (opcional)   ║
/// ║    distanciaInteraccion  → radio para abrir con tecla X  ║
/// ║                                                          ║
/// ║  Añade este script a un GameObject vacío en la escena.   ║
/// ║  El jugador pulsa X cerca del NPC para abrir la tienda.  ║
/// ╚══════════════════════════════════════════════════════════╝
/// </summary>
public class TiendaFF : MonoBehaviour
{
    // ─────────────────────────────────────────────────────────────────────────
    // INSPECTOR
    // ─────────────────────────────────────────────────────────────────────────

    [Header("── Datos ──────────────────────────────────────")]
    public DatosJugador datosRyo;

    [Header("Catálogo de la tienda")]
    public List<ItemConsumible> itemsConsumiblesVenta = new List<ItemConsumible>();
    public List<EquipoBase> equipoEnVenta = new List<EquipoBase>();

    [Header("── NPC ─────────────────────────────────────────")]
    [Tooltip("Tag del jugador para detectar proximidad")]
    public string tagJugador = "Player";
    public float distanciaInteraccion = 6f;

    [TextArea(2, 4)]
    public string[] lineasBienvenida = {
        "¡Bienvenido a la tienda!",
        "¿En qué puedo ayudarte?"
    };
    [TextArea(2, 4)]
    public string[] lineasDespedida = {
        "¡Hasta la próxima, aventurero!"
    };

    [Header("── Estética (opcional) ────────────────────────")]
    public TMP_FontAsset fuentePixel;

    // ─────────────────────────────────────────────────────────────────────────
    // PALETA Final Fantasy
    // ─────────────────────────────────────────────────────────────────────────
    static readonly Color C_FONDO = new Color(0.05f, 0.08f, 0.35f, 1f);
    static readonly Color C_MEDIO = new Color(0.10f, 0.15f, 0.55f, 1f);
    static readonly Color C_CLARO = new Color(0.20f, 0.30f, 0.75f, 1f);
    static readonly Color C_BORDE = new Color(0.55f, 0.65f, 1.00f, 1f);
    static readonly Color C_ORO = new Color(1.00f, 0.85f, 0.20f, 1f);
    static readonly Color C_BLANCO = Color.white;
    static readonly Color C_GRIS = new Color(0.70f, 0.70f, 0.70f, 1f);
    static readonly Color C_VERDE = new Color(0.15f, 0.65f, 0.25f, 1f);
    static readonly Color C_ROJO = new Color(0.70f, 0.12f, 0.12f, 1f);
    static readonly Color C_SEL = new Color(0.30f, 0.50f, 1.00f, 1f);

    // ─────────────────────────────────────────────────────────────────────────
    // REFERENCIAS UI (se generan en Awake)
    // ─────────────────────────────────────────────────────────────────────────
    Canvas _canvas;
    GameObject _raiz;

    // Panel izquierdo menú
    GameObject _panelMenu;
    Button _btnComprar, _btnVender, _btnSalir;

    // Panel Gil
    TextMeshProUGUI _txtGil;

    // Panel lista (derecha superior)
    GameObject _panelLista;
    Transform _headerLista;
    Transform _contenedor;

    // Panel descripción (inferior)
    GameObject _panelDesc;
    TextMeshProUGUI _txtDesc;

    // Panel confirmación (overlay)
    GameObject _panelConf;
    TextMeshProUGUI _txtConf;
    Button _btnSi, _btnNo;

    // Panel diálogo NPC (inferior, tapa desc mientras habla)
    GameObject _panelDialogo;
    TextMeshProUGUI _txtDialogo;

    // ─────────────────────────────────────────────────────────────────────────
    // ESTADO
    // ─────────────────────────────────────────────────────────────────────────
    enum Modo { Menu, Comprar, Vender }
    Modo _modo = Modo.Menu;
    ItemConsumible _consumibleSel;
    EquipoBase _equipoSel;
    GameObject _filaActiva;

    Transform _jugador;
    bool _abierta = false;
    bool _iniciando = false;

    // ═════════════════════════════════════════════════════════════════════════
    // UNITY
    // ═════════════════════════════════════════════════════════════════════════

    void Awake()
    {
        ConstruirUI();
        _raiz.SetActive(false);
    }

    void Start()
    {
        var obj = GameObject.FindGameObjectWithTag(tagJugador);
        if (obj != null) _jugador = obj.transform;
    }

    void Update()
    {
        if (_jugador == null || _iniciando) return;

        // Si el DialogoManager externo está mostrando algo, no procesar input de tienda
        if (DialogoManager.instancia != null && DialogoManager.instancia.EstaActivo()) return;

        float dist = Vector2.Distance(transform.position, _jugador.position);

        if (dist <= distanciaInteraccion && Input.GetKeyDown(KeyCode.X))
        {
            if (_abierta)
                StartCoroutine(CorCerrar());
            else
                StartCoroutine(CorAbrir());
        }
    }

    // ═════════════════════════════════════════════════════════════════════════
    // ABRIR / CERRAR
    // ═════════════════════════════════════════════════════════════════════════

    IEnumerator CorAbrir()
    {
        _iniciando = true;
        yield return null; // esperar un frame para que Update no consuma la X
        yield return StartCoroutine(MostrarDialogo(lineasBienvenida));
        _raiz.SetActive(true);
        ActualizarGil();
        MostrarMenu();
        _abierta = true;
        _iniciando = false;
    }

    IEnumerator CorCerrar()
    {
        _iniciando = true;
        _raiz.SetActive(false);
        _abierta = false;
        yield return null; // esperar un frame
        yield return StartCoroutine(MostrarDialogo(lineasDespedida));
        _iniciando = false;
    }

    // También llamable externamente (desde NPCTienda si lo prefieres)
    public void AbrirTienda() => StartCoroutine(CorAbrir());
    public void CerrarTienda() => StartCoroutine(CorCerrar());

    // ═════════════════════════════════════════════════════════════════════════
    // MODOS
    // ═════════════════════════════════════════════════════════════════════════

    void MostrarMenu()
    {
        _modo = Modo.Menu;
        LimpiarLista();
        LimpiarDesc();
        _panelConf.SetActive(false);
        ResaltarMenu(null);
    }

    void EntrarModoComprar()
    {
        _modo = Modo.Comprar;
        ResaltarMenu(_btnComprar.gameObject);
        LimpiarLista();
        LimpiarDesc();
        _panelConf.SetActive(false);
        ActualizarHeader("Nombre", "Costo", "Equipado");

        foreach (var item in itemsConsumiblesVenta)
        {
            var r = item;
            CrearFilaItem(item.nombre, item.precioCompra + "G", "—",
                () => AlSeleccionarCompra_Consumible(r));
        }
        foreach (var eq in equipoEnVenta)
        {
            var r = eq;
            string col3 = EstaEquipado(eq) ? "E" : "—";
            CrearFilaItem(eq.nombre, eq.precioCompra + "G", col3,
                () => AlSeleccionarCompra_Equipo(r));
        }
    }

    void EntrarModoVender()
    {
        _modo = Modo.Vender;
        ResaltarMenu(_btnVender.gameObject);
        LimpiarLista();
        LimpiarDesc();
        _panelConf.SetActive(false);
        ActualizarHeader("Nombre", "Precio", "Cant.");

        foreach (var item in datosRyo.mochilaItems)
        {
            if (item == null) continue;
            var r = item;
            CrearFilaItem(item.nombre, item.precioVenta + "G", "1",
                () => AlSeleccionarVenta_Consumible(r));
        }
        foreach (var eq in datosRyo.armarioEquipo)
        {
            if (eq == null) continue;
            var r = eq;
            string col3 = EstaEquipado(eq) ? "E" : "—";
            CrearFilaItem(eq.nombre, eq.precioVenta + "G", col3,
                () => AlSeleccionarVenta_Equipo(r));
        }
        if (datosRyo.plantasMedicinales > 0)
            CrearFilaItem("Planta Medicinal", "5G",
                datosRyo.plantasMedicinales.ToString(), () =>
                {
                    _consumibleSel = null; _equipoSel = null;
                    SetDesc("Planta Medicinal — Restaura 30 HP.  |  Venta: 5G");
                });
    }

    // ═════════════════════════════════════════════════════════════════════════
    // SELECCIÓN → abre confirmación directamente
    // ═════════════════════════════════════════════════════════════════════════

    void AlSeleccionarCompra_Consumible(ItemConsumible item)
    {
        _consumibleSel = item; _equipoSel = null;
        SetDesc(item.nombre + " — " + item.descripcion + "  |  Precio: " + item.precioCompra + "G");
        if (datosRyo.oro < item.precioCompra)
        { SetDesc(item.nombre + " — ¡Oro insuficiente! Necesitas " + item.precioCompra + "G"); return; }
        AbrirConf("¿Comprar " + item.nombre + "\npor " + item.precioCompra + "G?",
            ConfirmarCompra, () => _panelConf.SetActive(false));
    }

    void AlSeleccionarCompra_Equipo(EquipoBase eq)
    {
        _equipoSel = eq; _consumibleSel = null;
        SetDesc(eq.nombre + " — " + eq.descripcion + "  " + Stats(eq) + "  |  Precio: " + eq.precioCompra + "G");
        if (datosRyo.oro < eq.precioCompra)
        { SetDesc(eq.nombre + " — ¡Oro insuficiente! Necesitas " + eq.precioCompra + "G"); return; }
        AbrirConf("¿Comprar " + eq.nombre + "\npor " + eq.precioCompra + "G?",
            ConfirmarCompra, () => _panelConf.SetActive(false));
    }

    void AlSeleccionarVenta_Consumible(ItemConsumible item)
    {
        _consumibleSel = item; _equipoSel = null;
        SetDesc(item.nombre + " — " + item.descripcion + "  |  Venta: " + item.precioVenta + "G");
        AbrirConf("¿Vender " + item.nombre + "\npor " + item.precioVenta + "G?",
            ConfirmarVenta, () => _panelConf.SetActive(false));
    }

    void AlSeleccionarVenta_Equipo(EquipoBase eq)
    {
        _equipoSel = eq; _consumibleSel = null;
        SetDesc(eq.nombre + " — " + eq.descripcion + "  " + Stats(eq) + "  |  Venta: " + eq.precioVenta + "G");
        AbrirConf("¿Vender " + eq.nombre + "\npor " + eq.precioVenta + "G?",
            ConfirmarVenta, () => _panelConf.SetActive(false));
    }

    // ═════════════════════════════════════════════════════════════════════════
    // CONFIRMAR COMPRA
    // ═════════════════════════════════════════════════════════════════════════

    void ConfirmarCompra()
    {
        _panelConf.SetActive(false);

        if (_consumibleSel != null)
        {
            datosRyo.oro -= _consumibleSel.precioCompra;
            datosRyo.mochilaItems.Add(_consumibleSel);
            SetDesc("¡" + _consumibleSel.nombre + " comprado!  |  Oro: " + datosRyo.oro + "G");
            ActualizarGil();
            EntrarModoComprar();
        }
        else if (_equipoSel != null)
        {
            datosRyo.oro -= _equipoSel.precioCompra;
            ActualizarGil();
            var eq = _equipoSel;

            AbrirConf("¿Equiparte " + eq.nombre + " ahora?",
                () =>
                {
                    datosRyo.EquiparObjeto(eq);
                    _panelConf.SetActive(false);
                    SetDesc("¡" + eq.nombre + " equipado!  |  Oro: " + datosRyo.oro + "G");
                    EntrarModoComprar();
                },
                () =>
                {
                    datosRyo.armarioEquipo.Add(eq);
                    _panelConf.SetActive(false);
                    SetDesc("¡" + eq.nombre + " guardado!  |  Oro: " + datosRyo.oro + "G");
                    EntrarModoComprar();
                });
        }
    }

    // ═════════════════════════════════════════════════════════════════════════
    // CONFIRMAR VENTA
    // ═════════════════════════════════════════════════════════════════════════

    void ConfirmarVenta()
    {
        _panelConf.SetActive(false);

        if (_consumibleSel != null)
        {
            datosRyo.oro += _consumibleSel.precioVenta;
            datosRyo.mochilaItems.Remove(_consumibleSel);
            SetDesc("¡" + _consumibleSel.nombre + " vendido!  |  Oro: " + datosRyo.oro + "G");
            _consumibleSel = null;
        }
        else if (_equipoSel != null)
        {
            datosRyo.oro += _equipoSel.precioVenta;
            datosRyo.armarioEquipo.Remove(_equipoSel);
            // Si estaba equipado, desequipar
            if (_equipoSel == datosRyo.armaEquipadaAsset) datosRyo.armaEquipadaAsset = null;
            if (_equipoSel == datosRyo.armaduraEquipadaAsset) datosRyo.armaduraEquipadaAsset = null;
            if (_equipoSel == datosRyo.escudoEquipadoAsset) datosRyo.escudoEquipadoAsset = null;
            if (_equipoSel == datosRyo.cascoEquipadoAsset) datosRyo.cascoEquipadoAsset = null;
            if (_equipoSel == datosRyo.accesorioEquipadoAsset) datosRyo.accesorioEquipadoAsset = null;
            SetDesc("¡" + _equipoSel.nombre + " vendido!  |  Oro: " + datosRyo.oro + "G");
            _equipoSel = null;
        }

        ActualizarGil();
        EntrarModoVender();
    }

    // ═════════════════════════════════════════════════════════════════════════
    // DIÁLOGO NPC (no depende de DialogoManager)
    // ═════════════════════════════════════════════════════════════════════════

    IEnumerator MostrarDialogo(string[] lineas)
    {
        if (lineas == null || lineas.Length == 0) yield break;

        // Usar el DialogoManager global igual que el resto del juego
        if (DialogoManager.instancia != null)
        {
            DialogoManager.instancia.MostrarDialogo(lineas);
            yield return null; // esperar un frame para que dialogoActivo se ponga true
            yield return new WaitUntil(() => !DialogoManager.instancia.EstaActivo());
        }
        else
        {
            // Fallback: panel propio si no hay DialogoManager
            _panelDialogo.SetActive(true);
            foreach (var linea in lineas)
            {
                _txtDialogo.text = "";
                foreach (char c in linea)
                {
                    _txtDialogo.text += c;
                    yield return new WaitForSeconds(0.03f);
                }
                yield return new WaitUntil(() =>
                    Input.GetKeyDown(KeyCode.Z) ||
                    Input.GetKeyDown(KeyCode.X) ||
                    Input.GetKeyDown(KeyCode.Return) ||
                    Input.GetKeyDown(KeyCode.Space));
                yield return null;
            }
            _panelDialogo.SetActive(false);
        }
    }

    // ═════════════════════════════════════════════════════════════════════════
    // HELPERS LÓGICA
    // ═════════════════════════════════════════════════════════════════════════

    void AbrirConf(string texto,
        UnityEngine.Events.UnityAction accionSi,
        UnityEngine.Events.UnityAction accionNo)
    {
        _txtConf.text = texto;
        _btnSi.onClick.RemoveAllListeners();
        _btnNo.onClick.RemoveAllListeners();
        _btnSi.onClick.AddListener(accionSi);
        _btnNo.onClick.AddListener(accionNo);
        _panelConf.SetActive(true);
    }

    void ActualizarGil()
    {
        if (_txtGil != null && datosRyo != null)
            _txtGil.text = "Gil  " + datosRyo.oro;
    }

    void SetDesc(string texto)
    {
        if (_txtDesc != null) _txtDesc.text = texto;
    }

    void LimpiarDesc() => SetDesc("");

    void LimpiarLista()
    {
        if (_contenedor == null) return;
        foreach (Transform h in _contenedor) Destroy(h.gameObject);
        _filaActiva = null;
    }

    /// Comprueba si un EquipoBase está equipado en algún slot
    bool EstaEquipado(EquipoBase eq)
    {
        if (datosRyo == null || eq == null) return false;
        return eq == datosRyo.armaEquipadaAsset ||
               eq == datosRyo.armaduraEquipadaAsset ||
               eq == datosRyo.escudoEquipadoAsset ||
               eq == datosRyo.cascoEquipadoAsset ||
               eq == datosRyo.accesorioEquipadoAsset;
    }

    string Stats(EquipoBase eq)
    {
        string s = "";
        if (eq.bonoAtaque > 0) s += "ATQ+" + eq.bonoAtaque + " ";
        if (eq.bonoDefensa > 0) s += "DEF+" + eq.bonoDefensa + " ";
        if (eq.bonoAgilidad > 0) s += "AGI+" + eq.bonoAgilidad;
        return s.Trim();
    }

    void ResaltarMenu(GameObject activo)
    {
        ColorBtn(_btnComprar, activo == _btnComprar.gameObject);
        ColorBtn(_btnVender, activo == _btnVender.gameObject);
    }

    void ColorBtn(Button btn, bool seleccionado)
    {
        var img = btn.GetComponent<Image>();
        if (img) img.color = seleccionado ? C_CLARO : C_MEDIO;
    }

    void ActualizarHeader(string c1, string c2, string c3)
    {
        if (_headerLista == null) return;
        var txts = _headerLista.GetComponentsInChildren<TextMeshProUGUI>();
        if (txts.Length >= 3) { txts[0].text = c1; txts[1].text = c2; txts[2].text = c3; }
    }

    // ═════════════════════════════════════════════════════════════════════════
    // CONSTRUCCIÓN UI
    // Toda la UI se genera aquí. Resolución de referencia: 1366 × 768
    // Layout:
    //   ┌──────────┬──────────────────────────────────┐
    //   │  MENÚ    │  HEADER: Nombre | Costo | Equip  │
    //   │ Comprar  │  ─────────────────────────────── │
    //   │ Vender   │  item 1                          │
    //   │ Salir    │  item 2  ...  (scroll)           │
    //   ├──────────┤                                  │
    //   │  GIL     │                                  │
    //   └──────────┴──────────────────────────────────┘
    //   └──── DESCRIPCIÓN / FEEDBACK ───────────────────┘
    //   └──── DIÁLOGO NPC (solo mientras habla) ─────────┘
    // ═════════════════════════════════════════════════════════════════════════

    void ConstruirUI()
    {
        // Canvas
        var cgo = new GameObject("TiendaFF_Canvas");
        cgo.transform.SetParent(transform);
        _canvas = cgo.AddComponent<Canvas>();
        _canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        _canvas.sortingOrder = 60;
        var cs = cgo.AddComponent<CanvasScaler>();
        cs.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        cs.referenceResolution = new Vector2(1366, 768);
        cs.screenMatchMode = CanvasScaler.ScreenMatchMode.Expand;
        cgo.AddComponent<GraphicRaycaster>();

        // Raíz transparente pantalla completa
        _raiz = Nodo("Raiz", cgo.transform);
        Stretch(_raiz.GetComponent<RectTransform>(), Vector2.zero, Vector2.one);

        // ── Panel Menú (izquierda, 20-270, y desde arriba 20 hasta 420) ───────
        _panelMenu = PanelFF("PanelMenu", _raiz.transform, new Vector2(20, 20), new Vector2(250, 400));

        // ── Panel Gil (izquierda, debajo del menú) ────────────────────────────
        var panelGil = PanelFF("PanelGil", _raiz.transform, new Vector2(20, 430), new Vector2(250, 60));
        // Texto dentro del relleno del panel
        var rellenoGil = panelGil.transform.Find("Relleno");
        _txtGil = CrearTMP(rellenoGil ?? panelGil.transform, "TxtGil",
            "Gil  0", 14, TextAlignmentOptions.Left, C_ORO, bold: true);
        Stretch(_txtGil.rectTransform, new Vector2(10, 4), new Vector2(-10, -4));

        // ── Panel Lista (derecha, x 290, y 20, w 1056, h 490) ─────────────────
        _panelLista = PanelFF("PanelLista", _raiz.transform, new Vector2(290, 20), new Vector2(1056, 490));
        var rellenoLista = _panelLista.transform.Find("Relleno");
        Transform listaRoot = rellenoLista ?? _panelLista.transform;

        // Header fijo dentro del panel lista
        var headerGO = Nodo("Header", listaRoot);
        _headerLista = headerGO.transform;
        var rtH = headerGO.GetComponent<RectTransform>();
        rtH.anchorMin = new Vector2(0, 1); rtH.anchorMax = new Vector2(1, 1);
        rtH.pivot = new Vector2(0, 1);
        rtH.anchoredPosition = Vector2.zero;
        rtH.sizeDelta = new Vector2(0, 34);
        headerGO.AddComponent<Image>().color = C_CLARO;
        AgregarColumnas(headerGO.transform, "Nombre", "Costo", "Equipado",
                        C_BLANCO, C_BLANCO, C_BLANCO, esHeader: true);

        // ScrollView para los items
        var scrollGO = CrearScroll(listaRoot, new Vector2(0, 0), new Vector2(0, -34));
        _contenedor = scrollGO.GetComponentInChildren<VerticalLayoutGroup>().transform;

        // ── Panel Descripción (inferior, x 20, y 510, w 1326, h 68) ──────────
        _panelDesc = PanelFF("PanelDesc", _raiz.transform, new Vector2(20, 510), new Vector2(1326, 68));
        var rellenoDesc = _panelDesc.transform.Find("Relleno");
        _txtDesc = CrearTMP(rellenoDesc ?? _panelDesc.transform, "TxtDesc",
            "", 13, TextAlignmentOptions.Left, C_BLANCO);
        Stretch(_txtDesc.rectTransform, new Vector2(10, 4), new Vector2(-10, -4));
        _txtDesc.enableWordWrapping = true;

        // ── Panel Diálogo NPC (misma posición que desc, lo tapa mientras habla)
        _panelDialogo = PanelFF("PanelDialogo", _raiz.transform, new Vector2(20, 510), new Vector2(1326, 68));
        var rellenoDialogo = _panelDialogo.transform.Find("Relleno");
        _txtDialogo = CrearTMP(rellenoDialogo ?? _panelDialogo.transform, "TxtDialogo",
            "", 13, TextAlignmentOptions.Left, C_BLANCO);
        Stretch(_txtDialogo.rectTransform, new Vector2(10, 4), new Vector2(-10, -4));
        _txtDialogo.enableWordWrapping = true;
        _panelDialogo.SetActive(false);
        _panelDialogo.transform.SetAsLastSibling(); // siempre encima de panelDesc

        // ── Botones del menú ──────────────────────────────────────────────────
        var rellenoMenu = _panelMenu.transform.Find("Relleno");
        Transform menuRoot = rellenoMenu ?? _panelMenu.transform;

        _btnComprar = BotonMenu("Comprar", menuRoot, 0, () => EntrarModoComprar());
        _btnVender = BotonMenu("Vender", menuRoot, 56, () => EntrarModoVender());
        BotonMenu("Salir", menuRoot, 112, () => StartCoroutine(CorCerrar()));

        // ── Panel Confirmación (overlay centrado) ─────────────────────────────
        _panelConf = PanelFF("PanelConf", _raiz.transform, new Vector2(383, 234), new Vector2(600, 200));
        _panelConf.SetActive(false);
        var rellenoConf = _panelConf.transform.Find("Relleno");
        Transform confRoot = rellenoConf ?? _panelConf.transform;

        _txtConf = CrearTMP(confRoot, "TxtConf", "", 14, TextAlignmentOptions.Center, C_BLANCO);
        var rtTC = _txtConf.rectTransform;
        rtTC.anchorMin = new Vector2(0, 0.45f); rtTC.anchorMax = new Vector2(1, 1);
        rtTC.offsetMin = new Vector2(12, 0); rtTC.offsetMax = new Vector2(-12, -8);
        _txtConf.enableWordWrapping = true;

        _btnSi = BotonConf("BtnSi", "Sí", confRoot,
            new Vector2(0.05f, 0.05f), new Vector2(0.45f, 0.42f), C_VERDE);
        _btnNo = BotonConf("BtnNo", "No", confRoot,
            new Vector2(0.55f, 0.05f), new Vector2(0.95f, 0.42f), C_ROJO);
    }

    // ═════════════════════════════════════════════════════════════════════════
    // FÁBRICA DE WIDGETS
    // ═════════════════════════════════════════════════════════════════════════

    /// Panel azul oscuro con borde blanco y relleno. Pivot arriba-izquierda.
    GameObject PanelFF(string nombre, Transform padre, Vector2 posTopLeft, Vector2 tamano)
    {
        var go = Nodo(nombre, padre);
        var rt = go.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0, 1); rt.anchorMax = new Vector2(0, 1);
        rt.pivot = new Vector2(0, 1);
        rt.anchoredPosition = new Vector2(posTopLeft.x, -posTopLeft.y);
        rt.sizeDelta = tamano;
        go.AddComponent<Image>().color = C_FONDO;

        // Borde
        var b = Nodo("Borde", go.transform);
        Stretch(b.GetComponent<RectTransform>(), new Vector2(2, 2), new Vector2(-2, -2));
        b.AddComponent<Image>().color = C_BORDE;
        b.GetComponent<Image>().raycastTarget = false;

        // Relleno (tapa el borde dejando solo el contorno)
        var r = Nodo("Relleno", go.transform);
        Stretch(r.GetComponent<RectTransform>(), new Vector2(4, 4), new Vector2(-4, -4));
        r.AddComponent<Image>().color = C_FONDO;
        r.GetComponent<Image>().raycastTarget = false;

        return go;
    }

    /// Botón del menú lateral. yFromTop en píxeles desde el borde superior del relleno.
    Button BotonMenu(string etiqueta, Transform padre, float yFromTop,
        UnityEngine.Events.UnityAction accion)
    {
        var go = Nodo("Btn_" + etiqueta, padre);
        var rt = go.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0, 1); rt.anchorMax = new Vector2(1, 1);
        rt.pivot = new Vector2(0, 1);
        rt.anchoredPosition = new Vector2(0, -yFromTop);
        rt.sizeDelta = new Vector2(0, 48);

        var img = go.AddComponent<Image>();
        img.color = C_MEDIO;
        var btn = go.AddComponent<Button>();
        var cb = btn.colors;
        cb.normalColor = C_MEDIO;
        cb.highlightedColor = C_CLARO;
        cb.pressedColor = C_SEL;
        btn.colors = cb;
        btn.onClick.AddListener(accion);

        var tGO = Nodo("Txt", go.transform);
        Stretch(tGO.GetComponent<RectTransform>(), new Vector2(12, 0), new Vector2(-12, 0));
        var t = tGO.AddComponent<TextMeshProUGUI>();
        t.text = etiqueta; t.fontSize = 16; t.color = C_BLANCO;
        t.alignment = TextAlignmentOptions.Left;
        if (fuentePixel) t.font = fuentePixel;

        return btn;
    }

    /// Botón de confirmación con anclas proporcionales dentro del padre.
    Button BotonConf(string nombre, string etiqueta, Transform padre,
        Vector2 anchorMin, Vector2 anchorMax, Color color)
    {
        var go = Nodo(nombre, padre);
        var rt = go.GetComponent<RectTransform>();
        rt.anchorMin = anchorMin; rt.anchorMax = anchorMax;
        rt.offsetMin = new Vector2(4, 4); rt.offsetMax = new Vector2(-4, -4);

        var img = go.AddComponent<Image>();
        img.color = color;
        var btn = go.AddComponent<Button>();
        var cb = btn.colors;
        cb.normalColor = color;
        cb.highlightedColor = Color.Lerp(color, Color.white, 0.25f);
        cb.pressedColor = Color.Lerp(color, Color.black, 0.25f);
        btn.colors = cb;

        var tGO = Nodo("Txt", go.transform);
        Stretch(tGO.GetComponent<RectTransform>(), Vector2.zero, Vector2.zero);
        var t = tGO.AddComponent<TextMeshProUGUI>();
        t.text = etiqueta; t.fontSize = 16; t.color = C_BLANCO;
        t.fontStyle = FontStyles.Bold;
        t.alignment = TextAlignmentOptions.Center;
        if (fuentePixel) t.font = fuentePixel;

        return btn;
    }

    /// Fila de item clickable en la lista.
    void CrearFilaItem(string nombre, string costo, string col3,
        UnityEngine.Events.UnityAction accion)
    {
        var fila = Nodo("Fila_" + nombre, _contenedor);
        var rt = fila.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(0, 30);

        var img = fila.AddComponent<Image>();
        img.color = C_FONDO;
        var btn = fila.AddComponent<Button>();
        var cb = btn.colors;
        cb.normalColor = C_FONDO;
        cb.highlightedColor = C_MEDIO;
        cb.pressedColor = C_SEL;
        btn.colors = cb;

        AgregarColumnas(fila.transform, nombre, costo, col3,
                        C_BLANCO, C_ORO, C_GRIS, esHeader: false);

        btn.onClick.AddListener(() =>
        {
            if (_filaActiva != null)
            {
                var imgPrev = _filaActiva.GetComponent<Image>();
                if (imgPrev) imgPrev.color = C_FONDO;
            }
            img.color = C_SEL;
            _filaActiva = fila;
            accion?.Invoke();
        });
    }

    /// 3 columnas de texto dentro de una fila: nombre(50%) | costo(25%) | col3(25%)
    void AgregarColumnas(Transform padre,
        string c1, string c2, string c3,
        Color col1, Color col2, Color col3, bool esHeader)
    {
        float fs = esHeader ? 12f : 13f;
        bool bold = esHeader;

        TxtCol(padre, "C1", c1, new Vector2(0, 0), new Vector2(0.50f, 1), col1, fs, bold, TextAlignmentOptions.Left);
        TxtCol(padre, "C2", c2, new Vector2(0.50f, 0), new Vector2(0.75f, 1), col2, fs, bold, TextAlignmentOptions.Right);
        TxtCol(padre, "C3", c3, new Vector2(0.75f, 0), new Vector2(1f, 1), col3, fs, bold, TextAlignmentOptions.Right);
    }

    void TxtCol(Transform padre, string nombre, string texto,
        Vector2 aMin, Vector2 aMax, Color color, float fs, bool bold,
        TextAlignmentOptions align)
    {
        var go = Nodo(nombre, padre);
        var rt = go.GetComponent<RectTransform>();
        rt.anchorMin = aMin; rt.anchorMax = aMax;
        rt.offsetMin = new Vector2(8, 2); rt.offsetMax = new Vector2(-4, -2);
        var t = go.AddComponent<TextMeshProUGUI>();
        t.text = texto; t.fontSize = fs; t.color = color; t.alignment = align;
        t.overflowMode = TextOverflowModes.Ellipsis;
        if (bold) t.fontStyle = FontStyles.Bold;
        if (fuentePixel) t.font = fuentePixel;
    }

    /// ScrollView con Viewport + Content + VerticalLayoutGroup.
    GameObject CrearScroll(Transform padre, Vector2 offsetMin, Vector2 offsetMax)
    {
        var go = Nodo("Scroll", padre);
        Stretch(go.GetComponent<RectTransform>(), offsetMin, offsetMax);
        var imgScrollBg = go.AddComponent<Image>();
        imgScrollBg.color = Color.clear;
        imgScrollBg.raycastTarget = false;
        var sr = go.AddComponent<ScrollRect>();
        sr.horizontal = false;
        sr.scrollSensitivity = 30;

        var vp = Nodo("Viewport", go.transform);
        Stretch(vp.GetComponent<RectTransform>(), Vector2.zero, Vector2.zero);
        vp.AddComponent<RectMask2D>(); // RectMask2D funciona sin Image, mejor en Unity 2022
        sr.viewport = vp.GetComponent<RectTransform>();

        var ct = Nodo("Content", vp.transform);
        var rtC = ct.GetComponent<RectTransform>();
        rtC.anchorMin = new Vector2(0, 1); rtC.anchorMax = new Vector2(1, 1);
        rtC.pivot = new Vector2(0.5f, 1);
        rtC.sizeDelta = Vector2.zero; rtC.anchoredPosition = Vector2.zero;
        var vlg = ct.AddComponent<VerticalLayoutGroup>();
        vlg.spacing = 2;
        vlg.childControlWidth = true; vlg.childForceExpandWidth = true;
        vlg.childControlHeight = false; vlg.childForceExpandHeight = false;
        vlg.padding = new RectOffset(4, 4, 4, 4);
        var csf = ct.AddComponent<ContentSizeFitter>();
        csf.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        sr.content = rtC;

        return go;
    }

    TextMeshProUGUI CrearTMP(Transform padre, string nombre, string texto,
        float fs, TextAlignmentOptions align, Color color, bool bold = false)
    {
        var go = Nodo(nombre, padre);
        Stretch(go.GetComponent<RectTransform>(), Vector2.zero, Vector2.zero);
        var t = go.AddComponent<TextMeshProUGUI>();
        t.text = texto; t.fontSize = fs; t.color = color; t.alignment = align;
        t.overflowMode = TextOverflowModes.Ellipsis;
        if (bold) t.fontStyle = FontStyles.Bold;
        if (fuentePixel) t.font = fuentePixel;
        return t;
    }

    // ── Micro helpers ────────────────────────────────────────────────────────

    /// Crea un GameObject con RectTransform hijo del padre dado.
    GameObject Nodo(string nombre, Transform padre)
    {
        var go = new GameObject(nombre, typeof(RectTransform));
        go.transform.SetParent(padre, false);
        return go;
    }

    /// Ancla estirable (offsetMin/Max como padding).
    void Stretch(RectTransform rt, Vector2 offsetMin, Vector2 offsetMax)
    {
        rt.anchorMin = Vector2.zero; rt.anchorMax = Vector2.one;
        rt.offsetMin = offsetMin; rt.offsetMax = offsetMax;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, distanciaInteraccion);
    }
}