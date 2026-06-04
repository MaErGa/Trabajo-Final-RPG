using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

/// <summary>
/// Genera y gestiona TODA la UI de la tienda por código.
/// Reemplaza TiendaManager + prefabs posicionados a mano.
/// 
/// SETUP mínimo en el Inspector:
///   - datosRyo              → tu DatosJugador ScriptableObject
///   - itemsConsumiblesVenta → lista de ItemConsumible
///   - equipoEnVenta         → lista de EquipoBase
///   - (opcional) fuentePixel → fuente TMP para estética pixel
/// 
/// Llama a AbrirTienda() / CerrarTienda() desde NPCTienda.
/// </summary>
public class TiendaUI : MonoBehaviour
{
    // ── Datos ──────────────────────────────────────────────────────────────────
    [Header("Datos del jugador")]
    public DatosJugador datosRyo;

    [Header("Catálogo")]
    public List<ItemConsumible> itemsConsumiblesVenta = new List<ItemConsumible>();
    public List<EquipoBase> equipoEnVenta = new List<EquipoBase>();

    [Header("Estética (opcional)")]
    public TMP_FontAsset fuentePixel;   // arrastra tu fuente pixel; si es null usa la default

    // ── Paleta ─────────────────────────────────────────────────────────────────
    static readonly Color COL_FONDO_PANEL = new Color(0.05f, 0.05f, 0.12f, 0.97f);
    static readonly Color COL_HEADER = new Color(0.10f, 0.10f, 0.22f, 1f);
    static readonly Color COL_BOTON_NORMAL = new Color(0.12f, 0.18f, 0.30f, 1f);
    static readonly Color COL_BOTON_HOVER = new Color(0.20f, 0.35f, 0.55f, 1f);
    static readonly Color COL_BOTON_ACCION = new Color(0.18f, 0.45f, 0.22f, 1f);
    static readonly Color COL_BOTON_VENDER = new Color(0.50f, 0.28f, 0.08f, 1f);
    static readonly Color COL_BOTON_CANCEL = new Color(0.45f, 0.10f, 0.10f, 1f);
    static readonly Color COL_ITEM_SEL = new Color(0.25f, 0.55f, 0.85f, 1f);
    static readonly Color COL_SEPARADOR = new Color(0.30f, 0.30f, 0.50f, 0.5f);
    static readonly Color COL_ORO = new Color(1f, 0.85f, 0.20f, 1f);
    static readonly Color COL_TEXTO = Color.white;

    // ── Referencias UI generadas ───────────────────────────────────────────────
    private Canvas canvas;
    private GameObject panelRaiz;
    private Transform contenedorLista;
    private TextMeshProUGUI textoNombre;
    private TextMeshProUGUI textoDescripcion;
    private TextMeshProUGUI textoOro;
    private GameObject panelConfirmacion;
    private TextMeshProUGUI textoConfirmacion;
    private Button btnConfirmarSi;
    private Button btnConfirmarNo;
    private Button btnComprar;
    private Button btnVender;

    // ── Estado ─────────────────────────────────────────────────────────────────
    private enum Modo { Comprar, Vender }
    private Modo modoActual = Modo.Comprar;
    private ItemConsumible itemConsumibleSel;
    private EquipoBase equipoSel;
    private GameObject botonSeleccionadoGO;

    // ══════════════════════════════════════════════════════════════════════════
    // INICIALIZACIÓN
    // ══════════════════════════════════════════════════════════════════════════

    void Awake()
    {
        ConstruirUI();
        panelRaiz.SetActive(false);
    }

    // ══════════════════════════════════════════════════════════════════════════
    // API PÚBLICA
    // ══════════════════════════════════════════════════════════════════════════

    public void AbrirTienda()
    {
        panelRaiz.SetActive(true);
        MostrarModoComprar();
    }

    public void CerrarTienda()
    {
        panelRaiz.SetActive(false);
    }

    // ══════════════════════════════════════════════════════════════════════════
    // CONSTRUCCIÓN DE LA UI
    // ══════════════════════════════════════════════════════════════════════════

    void ConstruirUI()
    {
        // Canvas propio (Screen Space Overlay) para no depender del de la escena
        GameObject canvasGO = new GameObject("TiendaCanvas");
        canvasGO.transform.SetParent(transform);
        canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 50;
        canvasGO.AddComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        ((CanvasScaler)canvasGO.GetComponent<CanvasScaler>()).referenceResolution = new Vector2(960, 540);
        canvasGO.AddComponent<GraphicRaycaster>();

        // ── Panel raíz (pantalla completa, fondo semitransparente) ─────────────
        panelRaiz = CrearPanel(canvasGO.transform, "PanelTienda",
            Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero,
            new Color(0, 0, 0, 0.55f));

        // ── Contenedor centrado (780 × 420) ────────────────────────────────────
        GameObject ventana = CrearPanel(panelRaiz.transform, "Ventana",
            new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f),
            new Vector2(780, 420), Vector2.zero,
            COL_FONDO_PANEL);
        AgregarContorno(ventana, new Color(0.40f, 0.40f, 0.70f, 0.8f), 2);

        // ── Header ─────────────────────────────────────────────────────────────
        GameObject header = CrearPanel(ventana.transform, "Header",
            new Vector2(0, 1), new Vector2(1, 1),
            new Vector2(0, 40), new Vector2(0, -20),
            COL_HEADER);
        CrearTexto(header.transform, "TituloTienda", "⚔  TIENDA  ⚔",
            new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero, Vector2.zero,
            18, TextAlignmentOptions.Center, COL_ORO, true);

        // ── Texto oro (esquina superior derecha) ───────────────────────────────
        textoOro = CrearTexto(header.transform, "TextoOro", "Oro: 0G",
            new Vector2(1, 0.5f), new Vector2(1, 0.5f),
            new Vector2(160, 30), new Vector2(-85, 0),
            13, TextAlignmentOptions.Right, COL_ORO);

        // ── Columna izquierda: lista de items ──────────────────────────────────
        GameObject colIzq = CrearPanel(ventana.transform, "ColIzquierda",
            new Vector2(0, 0), new Vector2(0.52f, 1),
            new Vector2(0, -40), new Vector2(0, -20),
            Color.clear);

        // Botones modo (Comprar / Vender)
        GameObject baraModo = CrearPanel(colIzq.transform, "BaraModo",
            new Vector2(0, 1), new Vector2(1, 1),
            new Vector2(0, 32), new Vector2(0, -16),
            Color.clear);
        btnComprar = CrearBotonAccion(baraModo.transform, "BtnModoComprar", "COMPRAR",
            new Vector2(0, 0.5f), new Vector2(0.48f, 0.5f), Vector2.zero, Vector2.zero,
            COL_BOTON_ACCION, 12, MostrarModoComprar);
        btnVender = CrearBotonAccion(baraModo.transform, "BtnModoVender", "VENDER",
            new Vector2(0.52f, 0.5f), new Vector2(1, 0.5f), Vector2.zero, Vector2.zero,
            COL_BOTON_VENDER, 12, MostrarModoVender);

        // Scroll de lista
        GameObject scroll = CrearScrollView(colIzq.transform, "ListaScroll",
            new Vector2(0, 0), new Vector2(1, 1),
            new Vector2(0, -32), new Vector2(0, -16));
        contenedorLista = scroll.GetComponentInChildren<VerticalLayoutGroup>().transform;

        // ── Columna derecha: info + acciones ───────────────────────────────────
        GameObject colDer = CrearPanel(ventana.transform, "ColDerecha",
            new Vector2(0.54f, 0), new Vector2(1, 1),
            new Vector2(0, -40), new Vector2(0, -20),
            Color.clear);

        // Panel info
        GameObject panelInfo = CrearPanel(colDer.transform, "PanelInfo",
            new Vector2(0, 1), new Vector2(1, 1),
            new Vector2(0, 200), new Vector2(0, -100),
            new Color(0.08f, 0.08f, 0.18f, 1f));
        AgregarContorno(panelInfo, COL_SEPARADOR, 1);

        textoNombre = CrearTexto(panelInfo.transform, "TextoNombre", "",
            new Vector2(0, 1), new Vector2(1, 1),
            new Vector2(0, 36), new Vector2(0, -18),
            14, TextAlignmentOptions.Center, COL_ORO, true);

        textoDescripcion = CrearTexto(panelInfo.transform, "TextoDesc", "",
            new Vector2(0, 0), new Vector2(1, 1),
            new Vector2(-16, -44), new Vector2(8, -10),
            11, TextAlignmentOptions.TopLeft, COL_TEXTO);
        textoDescripcion.enableWordWrapping = true;

        // Separador
        GameObject sep = CrearPanel(colDer.transform, "Separador",
            new Vector2(0, 1), new Vector2(1, 1),
            new Vector2(0, 2), new Vector2(0, -208),
            COL_SEPARADOR);

        // Botón acción principal (Comprar/Vender según modo)
        // Lo guardamos como btnAccion y cambiamos su texto+color al cambiar modo
        GameObject btnAccionGO = CrearPanel(colDer.transform, "BtnAccion",
            new Vector2(0, 0), new Vector2(1, 0),
            new Vector2(0, 38), new Vector2(0, 60),
            COL_BOTON_ACCION);
        Button btnAccion = btnAccionGO.AddComponent<Button>();
        AgregarColoresBoton(btnAccion, COL_BOTON_ACCION);
        TextMeshProUGUI txtAccion = CrearTexto(btnAccionGO.transform, "Txt", "COMPRAR",
            new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero, Vector2.zero,
            13, TextAlignmentOptions.Center, COL_TEXTO, true);
        AgregarRoundedCorners(btnAccionGO);

        btnAccion.onClick.AddListener(() =>
        {
            if (modoActual == Modo.Comprar) AccionComprar();
            else AccionVender();
        });

        // Botón cerrar
        CrearBotonAccion(colDer.transform, "BtnCerrar", "✕  CERRAR",
            new Vector2(0, 0), new Vector2(1, 0),
            new Vector2(0, 32), new Vector2(0, 16),
            COL_BOTON_CANCEL, 12, CerrarTienda);

        // ── Panel confirmación (overlay dentro de la ventana) ──────────────────
        panelConfirmacion = CrearPanel(ventana.transform, "PanelConfirmacion",
            new Vector2(0.15f, 0.2f), new Vector2(0.85f, 0.8f),
            Vector2.zero, Vector2.zero,
            new Color(0.05f, 0.05f, 0.15f, 0.98f));
        AgregarContorno(panelConfirmacion, COL_ORO, 2);

        textoConfirmacion = CrearTexto(panelConfirmacion.transform, "TextoConf", "",
            new Vector2(0, 0.4f), new Vector2(1, 1),
            new Vector2(-20, 0), new Vector2(10, -10),
            13, TextAlignmentOptions.Center, COL_TEXTO);
        textoConfirmacion.enableWordWrapping = true;

        btnConfirmarSi = CrearBotonAccion(panelConfirmacion.transform, "BtnSi", "✔  SÍ",
            new Vector2(0.05f, 0.05f), new Vector2(0.45f, 0.38f),
            Vector2.zero, Vector2.zero,
            COL_BOTON_ACCION, 12, null).GetComponent<Button>();

        btnConfirmarNo = CrearBotonAccion(panelConfirmacion.transform, "BtnNo", "✕  NO",
            new Vector2(0.55f, 0.05f), new Vector2(0.95f, 0.38f),
            Vector2.zero, Vector2.zero,
            COL_BOTON_CANCEL, 12, CancelarConfirmacion).GetComponent<Button>();

        panelConfirmacion.SetActive(false);

        // Guardar referencia al botón acción para cambiar texto/color según modo
        _btnAccion = btnAccion;
        _txtBtnAccion = txtAccion;
    }

    // referencia guardada para cambiar texto del botón de acción
    private Button _btnAccion;
    private TextMeshProUGUI _txtBtnAccion;

    // ══════════════════════════════════════════════════════════════════════════
    // LÓGICA DE TIENDA (igual que TiendaManager original)
    // ══════════════════════════════════════════════════════════════════════════

    public void MostrarModoComprar()
    {
        modoActual = Modo.Comprar;
        if (_txtBtnAccion != null) _txtBtnAccion.text = "COMPRAR";
        if (_btnAccion != null) AgregarColoresBoton(_btnAccion, COL_BOTON_ACCION);
        LimpiarLista(); LimpiarInfo(); LimpiarSeleccion();
        if (panelConfirmacion != null) panelConfirmacion.SetActive(false);

        foreach (var item in itemsConsumiblesVenta)
        {
            var r = item;
            CrearItemLista(item.nombre, item.precioCompra + "G", () => SeleccionarConsumible(r));
        }
        foreach (var eq in equipoEnVenta)
        {
            var r = eq;
            CrearItemLista(eq.nombre, eq.precioCompra + "G", () => SeleccionarEquipo(r));
        }
    }

    public void MostrarModoVender()
    {
        modoActual = Modo.Vender;
        if (_txtBtnAccion != null) _txtBtnAccion.text = "VENDER";
        if (_btnAccion != null) AgregarColoresBoton(_btnAccion, COL_BOTON_VENDER);
        LimpiarLista(); LimpiarInfo(); LimpiarSeleccion();
        if (panelConfirmacion != null) panelConfirmacion.SetActive(false);

        foreach (var item in datosRyo.mochilaItems)
        {
            if (item == null) continue;
            var r = item;
            CrearItemLista(item.nombre, item.precioVenta + "G", () => SeleccionarConsumibleVenta(r));
        }
        foreach (var eq in datosRyo.armarioEquipo)
        {
            if (eq == null) continue;
            var r = eq;
            CrearItemLista(eq.nombre, eq.precioVenta + "G", () => SeleccionarEquipoVenta(r));
        }
        if (datosRyo.plantasMedicinales > 0)
            CrearItemLista("Planta Medicinal x" + datosRyo.plantasMedicinales, "5G", () =>
                MostrarInfo("Planta Medicinal", "Restaura 30 HP.\nVenta: 5G"));
    }

    // ── Selección ─────────────────────────────────────────────────────────────

    void SeleccionarConsumible(ItemConsumible item)
    {
        itemConsumibleSel = item; equipoSel = null;
        panelConfirmacion.SetActive(false);
        MostrarInfo(item.nombre, item.descripcion + "\nPrecio: " + item.precioCompra + "G");
    }
    void SeleccionarEquipo(EquipoBase eq)
    {
        equipoSel = eq; itemConsumibleSel = null;
        panelConfirmacion.SetActive(false);
        MostrarInfo(eq.nombre, eq.descripcion + "\n" + StatsEquipo(eq) + "\nPrecio: " + eq.precioCompra + "G");
    }
    void SeleccionarConsumibleVenta(ItemConsumible item)
    {
        itemConsumibleSel = item; equipoSel = null;
        panelConfirmacion.SetActive(false);
        MostrarInfo(item.nombre, item.descripcion + "\nVenta: " + item.precioVenta + "G");
    }
    void SeleccionarEquipoVenta(EquipoBase eq)
    {
        equipoSel = eq; itemConsumibleSel = null;
        panelConfirmacion.SetActive(false);
        MostrarInfo(eq.nombre, eq.descripcion + "\n" + StatsEquipo(eq) + "\nVenta: " + eq.precioVenta + "G");
    }

    // ── Comprar ───────────────────────────────────────────────────────────────

    void AccionComprar()
    {
        if (itemConsumibleSel == null && equipoSel == null) return;
        string nombre = itemConsumibleSel != null ? itemConsumibleSel.nombre : equipoSel.nombre;
        int precio = itemConsumibleSel != null ? itemConsumibleSel.precioCompra : equipoSel.precioCompra;

        if (datosRyo.oro < precio)
        {
            MostrarInfo(nombre, "No tienes suficiente oro.\nNecesitas: " + precio + "G");
            return;
        }

        AbrirConfirmacion("¿Comprar " + nombre + "\npor " + precio + "G?", ConfirmarCompra, CancelarConfirmacion);
    }

    void ConfirmarCompra()
    {
        if (itemConsumibleSel != null)
        {
            datosRyo.oro -= itemConsumibleSel.precioCompra;
            datosRyo.mochilaItems.Add(itemConsumibleSel);
            MostrarInfo(itemConsumibleSel.nombre, "¡Comprado!\nOro restante: " + datosRyo.oro + "G");
            ActualizarOro(); panelConfirmacion.SetActive(false);
        }
        else if (equipoSel != null)
        {
            datosRyo.oro -= equipoSel.precioCompra;
            ActualizarOro(); panelConfirmacion.SetActive(false);
            var eq = equipoSel;
            AbrirConfirmacion("¿Equiparte " + eq.nombre + " ahora?",
                () => { datosRyo.EquiparObjeto(eq); MostrarInfo(eq.nombre, "¡Equipado!\nOro: " + datosRyo.oro + "G"); panelConfirmacion.SetActive(false); },
                () => { datosRyo.armarioEquipo.Add(eq); MostrarInfo(eq.nombre, "¡Guardado!\nOro: " + datosRyo.oro + "G"); panelConfirmacion.SetActive(false); });
        }
    }

    // ── Vender ────────────────────────────────────────────────────────────────

    void AccionVender()
    {
        if (itemConsumibleSel == null && equipoSel == null) return;
        string nombre = itemConsumibleSel != null ? itemConsumibleSel.nombre : equipoSel.nombre;
        int precio = itemConsumibleSel != null ? itemConsumibleSel.precioVenta : equipoSel.precioVenta;
        AbrirConfirmacion("¿Vender " + nombre + "\npor " + precio + "G?", ConfirmarVenta, CancelarConfirmacion);
    }

    void ConfirmarVenta()
    {
        if (itemConsumibleSel != null)
        {
            datosRyo.oro += itemConsumibleSel.precioVenta;
            datosRyo.mochilaItems.Remove(itemConsumibleSel);
            MostrarInfo(itemConsumibleSel.nombre, "¡Vendido!\nOro: " + datosRyo.oro + "G");
            itemConsumibleSel = null;
        }
        else if (equipoSel != null)
        {
            datosRyo.oro += equipoSel.precioVenta;
            datosRyo.armarioEquipo.Remove(equipoSel);
            MostrarInfo(equipoSel.nombre, "¡Vendido!\nOro: " + datosRyo.oro + "G");
            equipoSel = null;
        }
        ActualizarOro(); panelConfirmacion.SetActive(false);
        MostrarModoVender();
    }

    void CancelarConfirmacion() => panelConfirmacion.SetActive(false);

    // ── Helpers de lógica ─────────────────────────────────────────────────────

    void AbrirConfirmacion(string texto, UnityEngine.Events.UnityAction accionSi, UnityEngine.Events.UnityAction accionNo)
    {
        textoConfirmacion.text = texto;
        btnConfirmarSi.onClick.RemoveAllListeners();
        btnConfirmarNo.onClick.RemoveAllListeners();
        btnConfirmarSi.onClick.AddListener(accionSi);
        btnConfirmarNo.onClick.AddListener(accionNo);
        panelConfirmacion.SetActive(true);
    }

    void MostrarInfo(string nombre, string desc)
    {
        if (textoNombre != null) textoNombre.text = nombre;
        if (textoDescripcion != null) textoDescripcion.text = desc;
    }

    void LimpiarInfo()
    {
        if (textoNombre != null) textoNombre.text = "";
        if (textoDescripcion != null) textoDescripcion.text = "";
    }

    void LimpiarLista()
    {
        foreach (Transform h in contenedorLista) Destroy(h.gameObject);
        botonSeleccionadoGO = null;
    }

    void LimpiarSeleccion() { itemConsumibleSel = null; equipoSel = null; }

    void ActualizarOro()
    {
        if (textoOro != null) textoOro.text = "Oro: " + datosRyo.oro + "G";
    }

    string StatsEquipo(EquipoBase eq)
    {
        string s = "";
        if (eq.bonoAtaque > 0) s += "ATQ +" + eq.bonoAtaque + "  ";
        if (eq.bonoDefensa > 0) s += "DEF +" + eq.bonoDefensa + "  ";
        if (eq.bonoAgilidad > 0) s += "AGI +" + eq.bonoAgilidad;
        return s;
    }

    // ══════════════════════════════════════════════════════════════════════════
    // CONSTRUCTORES DE WIDGETS UI
    // ══════════════════════════════════════════════════════════════════════════

    void CrearItemLista(string nombre, string precio, UnityEngine.Events.UnityAction accion)
    {
        GameObject fila = new GameObject("Item_" + nombre, typeof(RectTransform));
        fila.transform.SetParent(contenedorLista, false);
        var rt = fila.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(0, 30);

        var img = fila.AddComponent<Image>();
        img.color = COL_BOTON_NORMAL;

        var btn = fila.AddComponent<Button>();
        var cols = btn.colors;
        cols.normalColor = COL_BOTON_NORMAL;
        cols.highlightedColor = COL_BOTON_HOVER;
        cols.pressedColor = COL_ITEM_SEL;
        btn.colors = cols;

        // Texto nombre
        var tNom = CrearTexto(fila.transform, "Nombre", nombre,
            new Vector2(0, 0.5f), new Vector2(0.72f, 0.5f),
            new Vector2(0, 26), new Vector2(6, 0),
            11, TextAlignmentOptions.Left, COL_TEXTO);

        // Texto precio
        CrearTexto(fila.transform, "Precio", precio,
            new Vector2(0.73f, 0.5f), new Vector2(1, 0.5f),
            new Vector2(0, 26), new Vector2(-4, 0),
            11, TextAlignmentOptions.Right, COL_ORO);

        btn.onClick.AddListener(() =>
        {
            // Desmarcar anterior
            if (botonSeleccionadoGO != null)
            {
                var imgAnterior = botonSeleccionadoGO.GetComponent<Image>();
                if (imgAnterior != null) imgAnterior.color = COL_BOTON_NORMAL;
            }
            img.color = COL_ITEM_SEL;
            botonSeleccionadoGO = fila;
            accion?.Invoke();
        });
    }

    // Crea un panel con anclas estirables O de tamaño fijo
    // anchorMin/Max: esquinas del ancla. Si sizeDelta != zero, se usa como tamaño fijo con anchoredPosition como pivot.
    GameObject CrearPanel(Transform padre, string nombre,
        Vector2 anchorMin, Vector2 anchorMax,
        Vector2 sizeDelta, Vector2 anchoredPos,
        Color color)
    {
        var go = new GameObject(nombre, typeof(RectTransform));
        go.transform.SetParent(padre, false);
        var rt = go.GetComponent<RectTransform>();
        rt.anchorMin = anchorMin;
        rt.anchorMax = anchorMax;
        rt.sizeDelta = sizeDelta;
        rt.anchoredPosition = anchoredPos;
        if (color.a > 0)
        {
            var img = go.AddComponent<Image>();
            img.color = color;
        }
        return go;
    }

    TextMeshProUGUI CrearTexto(Transform padre, string nombre, string contenido,
        Vector2 anchorMin, Vector2 anchorMax,
        Vector2 sizeDelta, Vector2 anchoredPos,
        int fontSize, TextAlignmentOptions alineacion, Color color, bool bold = false)
    {
        var go = new GameObject(nombre, typeof(RectTransform));
        go.transform.SetParent(padre, false);
        var rt = go.GetComponent<RectTransform>();
        rt.anchorMin = anchorMin;
        rt.anchorMax = anchorMax;
        rt.sizeDelta = sizeDelta;
        rt.anchoredPosition = anchoredPos;

        var tmp = go.AddComponent<TextMeshProUGUI>();
        tmp.text = contenido;
        tmp.fontSize = fontSize;
        tmp.alignment = alineacion;
        tmp.color = color;
        if (bold) tmp.fontStyle = FontStyles.Bold;
        if (fuentePixel != null) tmp.font = fuentePixel;
        tmp.overflowMode = TextOverflowModes.Ellipsis;
        return tmp;
    }

    Button CrearBotonAccion(Transform padre, string nombre, string etiqueta,
        Vector2 anchorMin, Vector2 anchorMax,
        Vector2 sizeDelta, Vector2 anchoredPos,
        Color colorFondo, int fontSize,
        UnityEngine.Events.UnityAction accion)
    {
        var go = CrearPanel(padre, nombre, anchorMin, anchorMax, sizeDelta, anchoredPos, colorFondo);
        AgregarRoundedCorners(go);
        var btn = go.AddComponent<Button>();
        AgregarColoresBoton(btn, colorFondo);
        CrearTexto(go.transform, "Txt", etiqueta,
            new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f),
            Vector2.zero, Vector2.zero,
            fontSize, TextAlignmentOptions.Center, COL_TEXTO, true);
        if (accion != null) btn.onClick.AddListener(accion);
        return btn;
    }

    GameObject CrearScrollView(Transform padre, string nombre,
        Vector2 anchorMin, Vector2 anchorMax,
        Vector2 sizeDelta, Vector2 anchoredPos)
    {
        // Viewport
        var scrollGO = new GameObject(nombre, typeof(RectTransform));
        scrollGO.transform.SetParent(padre, false);
        var rtScroll = scrollGO.GetComponent<RectTransform>();
        rtScroll.anchorMin = anchorMin;
        rtScroll.anchorMax = anchorMax;
        rtScroll.sizeDelta = sizeDelta;
        rtScroll.anchoredPosition = anchoredPos;
        var imgScroll = scrollGO.AddComponent<Image>();
        imgScroll.color = new Color(0, 0, 0, 0.25f);
        var sr = scrollGO.AddComponent<ScrollRect>();
        sr.horizontal = false;
        sr.scrollSensitivity = 30;

        // Viewport mask
        var viewport = new GameObject("Viewport", typeof(RectTransform));
        viewport.transform.SetParent(scrollGO.transform, false);
        var rtVP = viewport.GetComponent<RectTransform>();
        rtVP.anchorMin = Vector2.zero; rtVP.anchorMax = Vector2.one;
        rtVP.sizeDelta = Vector2.zero; rtVP.anchoredPosition = Vector2.zero;
        viewport.AddComponent<Image>().color = Color.clear;
        viewport.AddComponent<Mask>().showMaskGraphic = false;
        sr.viewport = rtVP;

        // Contenido
        var content = new GameObject("Content", typeof(RectTransform));
        content.transform.SetParent(viewport.transform, false);
        var rtC = content.GetComponent<RectTransform>();
        rtC.anchorMin = new Vector2(0, 1); rtC.anchorMax = new Vector2(1, 1);
        rtC.pivot = new Vector2(0.5f, 1);
        rtC.sizeDelta = new Vector2(0, 0);
        rtC.anchoredPosition = Vector2.zero;

        var vlg = content.AddComponent<VerticalLayoutGroup>();
        vlg.spacing = 3;
        vlg.childForceExpandWidth = true;
        vlg.childForceExpandHeight = false;
        vlg.padding = new RectOffset(4, 4, 4, 4);

        var csf = content.AddComponent<ContentSizeFitter>();
        csf.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

        sr.content = rtC;
        return scrollGO;
    }

    // ── Utilidades de estilo ───────────────────────────────────────────────────

    void AgregarColoresBoton(Button btn, Color base_)
    {
        var c = btn.colors;
        c.normalColor = base_;
        c.highlightedColor = Color.Lerp(base_, Color.white, 0.25f);
        c.pressedColor = Color.Lerp(base_, Color.black, 0.25f);
        c.selectedColor = base_;
        c.fadeDuration = 0.1f;
        btn.colors = c;
    }

    void AgregarContorno(GameObject go, Color color, float grosor)
    {
        // Outline via Outline component si existe, o borde manual con 4 imágenes
        // Usamos un segundo Image de contorno como hijo
        var borde = new GameObject("Borde", typeof(RectTransform));
        borde.transform.SetParent(go.transform, false);
        borde.transform.SetAsFirstSibling();
        var rt = borde.GetComponent<RectTransform>();
        rt.anchorMin = Vector2.zero; rt.anchorMax = Vector2.one;
        rt.sizeDelta = new Vector2(grosor * 2, grosor * 2);
        rt.anchoredPosition = Vector2.zero;
        var img = borde.AddComponent<Image>();
        img.color = color;
        img.raycastTarget = false;
    }

    void AgregarRoundedCorners(GameObject go)
    {
        // Sin librerías extra: simplemente dejamos la Image cuadrada.
        // Si usas una sprite de 9-slice redondeada puedes asignarla aquí.
    }
}