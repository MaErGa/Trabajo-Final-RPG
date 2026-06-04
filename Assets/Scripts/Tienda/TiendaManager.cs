using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

/// <summary>
/// TiendaManager con layout fijado por código en Start().
/// Todas las referencias siguen arrastrándose en el Inspector igual que antes,
/// pero las posiciones y tamaños se aplican en tiempo de ejecución para que
/// nunca se descuadren en la build WebGL.
///
/// JERARQUÍA ESPERADA (la misma que tenías):
///   PanelTienda  (panelTienda)
///   ├── VentanaLista
///   │   └── ScrollView
///   │       └── Viewport/Content  ← contenedorLista
///   ├── VentanaInfo
///   │   ├── TextoNombre           ← textoNombreItem
///   │   ├── TextoDescripcion      ← textoDescripcion
///   │   └── TextoOro              ← textoOro
///   ├── BotonComprar              ← botonComprar
///   ├── BotonVender               ← botonVender
///   └── PanelConfirmacion         ← panelConfirmacion
///       ├── TextoConfirmacion     ← textoConfirmacion
///       ├── BotonSi               ← botonConfirmarSi
///       └── BotonNo               ← botonConfirmarNo
/// </summary>
public class TiendaManager : MonoBehaviour
{
    [Header("Datos del Jugador")]
    public DatosJugador datosRyo;

    [Header("Items en venta (arrastra los assets)")]
    public List<ItemConsumible> itemsConsumiblesVenta = new List<ItemConsumible>();
    public List<EquipoBase> equipoEnVenta = new List<EquipoBase>();

    [Header("Panel Principal")]
    public GameObject panelTienda;

    [Header("Ventana Lista")]
    public Transform contenedorLista;
    public GameObject prefabBotonItem;

    [Header("Ventana Info")]
    public TextMeshProUGUI textoNombreItem;
    public TextMeshProUGUI textoDescripcion;
    public TextMeshProUGUI textoOro;

    [Header("Botones Comprar / Vender")]
    public Button botonComprar;
    public Button botonVender;

    [Header("Panel Confirmación")]
    public GameObject panelConfirmacion;
    public TextMeshProUGUI textoConfirmacion;
    public Button botonConfirmarSi;
    public Button botonConfirmarNo;

    // ── Estado interno ────────────────────────────────────────────────────────
    private enum ModoTienda { Comprar, Vender }
    private ModoTienda modoActual = ModoTienda.Comprar;
    private ItemConsumible itemConsumibleSeleccionado;
    private EquipoBase equipoSeleccionado;

    // ══════════════════════════════════════════════════════════════════════════
    void Start()
    {
        AjustarLayoutUI();                                  // ← fija posiciones
        if (panelConfirmacion != null) panelConfirmacion.SetActive(false);
        ActualizarOro();
        MostrarModoComprar();
    }

    // ══════════════════════════════════════════════════════════════════════════
    // LAYOUT POR CÓDIGO
    // Todas las medidas están en píxeles de Canvas (resolución de referencia
    // 960 × 540). Si tu CanvasScaler usa otra resolución ajusta los valores.
    // ══════════════════════════════════════════════════════════════════════════
    void AjustarLayoutUI()
    {
        if (panelTienda == null) return;

        // ── Panel principal: mitad izquierda de la pantalla ───────────────────
        //    Ancla estirable de (0,0)→(0.55, 1), sin offset
        Anclar(panelTienda, new Vector2(0f, 0f), new Vector2(0.55f, 1f));

        // ── Ventana Lista (padre de contenedorLista) ───────────────────────────
        //    Ocupa el 60 % izquierdo del panel, dejando hueco abajo para botones
        Transform ventanaLista = contenedorLista != null
            ? contenedorLista.parent?.parent   // Content→Viewport→ScrollView
            : null;
        if (ventanaLista == null && contenedorLista != null)
            ventanaLista = contenedorLista.parent; // por si no hay Viewport

        if (ventanaLista != null)
            Anclar(ventanaLista.gameObject,
                new Vector2(0f, 0.12f), new Vector2(1f, 1f));

        // ── Botones Comprar / Vender (parte inferior del panel) ───────────────
        if (botonComprar != null)
            AnclarTamanio(botonComprar.gameObject,
                new Vector2(0.02f, 0f), new Vector2(0.49f, 0.11f));

        if (botonVender != null)
            AnclarTamanio(botonVender.gameObject,
                new Vector2(0.51f, 0f), new Vector2(0.98f, 0.11f));

        // ── Ventana Info: mitad derecha de la pantalla ────────────────────────
        //    Ancla estirable de (0.56, 0)→(1, 1)
        //    Buscamos el padre de los textos de info
        Transform ventanaInfo = textoNombreItem != null
            ? textoNombreItem.transform.parent
            : null;

        if (ventanaInfo != null)
        {
            // La ventana info está fuera del panelTienda o dentro — la posicionamos
            // relativa a su propio padre (que debe ser el Canvas o un panel raíz)
            Anclar(ventanaInfo.gameObject,
                new Vector2(0.56f, 0f), new Vector2(1f, 1f));

            // TextoNombre: franja superior
            if (textoNombreItem != null)
                AnclarTamanio(textoNombreItem.gameObject,
                    new Vector2(0f, 0.82f), new Vector2(1f, 1f));

            // TextoDescripcion: zona central
            if (textoDescripcion != null)
                AnclarTamanio(textoDescripcion.gameObject,
                    new Vector2(0f, 0.30f), new Vector2(1f, 0.80f));

            // TextoOro: esquina inferior
            if (textoOro != null)
                AnclarTamanio(textoOro.gameObject,
                    new Vector2(0f, 0.05f), new Vector2(1f, 0.18f));
        }

        // ── Panel Confirmación: centrado sobre todo ────────────────────────────
        if (panelConfirmacion != null)
        {
            // Relativo a su padre — lo centramos con tamaño fijo
            RectTransform rt = panelConfirmacion.GetComponent<RectTransform>();
            if (rt != null)
            {
                rt.anchorMin = new Vector2(0.5f, 0.5f);
                rt.anchorMax = new Vector2(0.5f, 0.5f);
                rt.pivot = new Vector2(0.5f, 0.5f);
                rt.sizeDelta = new Vector2(320, 160);
                rt.anchoredPosition = Vector2.zero;
            }

            // TextoConfirmacion: zona superior del panel
            if (textoConfirmacion != null)
                AnclarTamanio(textoConfirmacion.gameObject,
                    new Vector2(0.05f, 0.45f), new Vector2(0.95f, 0.95f));

            // Botón Sí: mitad izquierda inferior
            if (botonConfirmarSi != null)
                AnclarTamanio(botonConfirmarSi.gameObject,
                    new Vector2(0.05f, 0.05f), new Vector2(0.45f, 0.40f));

            // Botón No: mitad derecha inferior
            if (botonConfirmarNo != null)
                AnclarTamanio(botonConfirmarNo.gameObject,
                    new Vector2(0.55f, 0.05f), new Vector2(0.95f, 0.40f));
        }
    }

    // Ancla estirable pura (sin sizeDelta ni anchoredPosition)
    void Anclar(GameObject go, Vector2 min, Vector2 max)
    {
        RectTransform rt = go.GetComponent<RectTransform>();
        if (rt == null) return;
        rt.anchorMin = min;
        rt.anchorMax = max;
        rt.offsetMin = Vector2.zero;   // left / bottom padding = 0
        rt.offsetMax = Vector2.zero;   // right / top padding = 0
    }

    // Ancla proporcional dentro del padre, sin padding
    void AnclarTamanio(GameObject go, Vector2 min, Vector2 max)
    {
        Anclar(go, min, max);
    }

    // ══════════════════════════════════════════════════════════════════════════
    // MODOS
    // ══════════════════════════════════════════════════════════════════════════

    public void MostrarModoComprar()
    {
        modoActual = ModoTienda.Comprar;
        LimpiarLista();
        itemConsumibleSeleccionado = null;
        equipoSeleccionado = null;
        LimpiarInfo();
        if (panelConfirmacion != null) panelConfirmacion.SetActive(false);

        foreach (var item in itemsConsumiblesVenta)
        {
            var r = item;
            CrearBoton(item.nombre, item.precioCompra + "G", () => SeleccionarConsumible(r));
        }
        foreach (var equipo in equipoEnVenta)
        {
            var r = equipo;
            CrearBoton(equipo.nombre, equipo.precioCompra + "G", () => SeleccionarEquipo(r));
        }
    }

    public void MostrarModoVender()
    {
        modoActual = ModoTienda.Vender;
        LimpiarLista();
        itemConsumibleSeleccionado = null;
        equipoSeleccionado = null;
        LimpiarInfo();
        if (panelConfirmacion != null) panelConfirmacion.SetActive(false);

        foreach (var item in datosRyo.mochilaItems)
        {
            if (item == null) continue;
            var r = item;
            CrearBoton(item.nombre, item.precioVenta + "G", () => SeleccionarConsumibleVenta(r));
        }
        foreach (var equipo in datosRyo.armarioEquipo)
        {
            if (equipo == null) continue;
            var r = equipo;
            CrearBoton(equipo.nombre, equipo.precioVenta + "G", () => SeleccionarEquipoVenta(r));
        }
        if (datosRyo.plantasMedicinales > 0)
            CrearBoton("Planta Medicinal x" + datosRyo.plantasMedicinales, "5G", () =>
            {
                MostrarInfo("Planta Medicinal", "Restaura 30 HP.", "Venta: 5G");
                itemConsumibleSeleccionado = null;
                equipoSeleccionado = null;
            });
    }

    // ══════════════════════════════════════════════════════════════════════════
    // SELECCIÓN
    // ══════════════════════════════════════════════════════════════════════════

    void SeleccionarConsumible(ItemConsumible item)
    {
        itemConsumibleSeleccionado = item; equipoSeleccionado = null;
        if (panelConfirmacion != null) panelConfirmacion.SetActive(false);
        MostrarInfo(item.nombre, item.descripcion, "Precio: " + item.precioCompra + "G");
    }

    void SeleccionarEquipo(EquipoBase equipo)
    {
        equipoSeleccionado = equipo; itemConsumibleSeleccionado = null;
        if (panelConfirmacion != null) panelConfirmacion.SetActive(false);
        MostrarInfo(equipo.nombre, equipo.descripcion + "\n" + StatsEquipo(equipo),
                    "Precio: " + equipo.precioCompra + "G");
    }

    void SeleccionarConsumibleVenta(ItemConsumible item)
    {
        itemConsumibleSeleccionado = item; equipoSeleccionado = null;
        if (panelConfirmacion != null) panelConfirmacion.SetActive(false);
        MostrarInfo(item.nombre, item.descripcion, "Venta: " + item.precioVenta + "G");
    }

    void SeleccionarEquipoVenta(EquipoBase equipo)
    {
        equipoSeleccionado = equipo; itemConsumibleSeleccionado = null;
        if (panelConfirmacion != null) panelConfirmacion.SetActive(false);
        MostrarInfo(equipo.nombre, equipo.descripcion + "\n" + StatsEquipo(equipo),
                    "Venta: " + equipo.precioVenta + "G");
    }

    // ══════════════════════════════════════════════════════════════════════════
    // COMPRAR
    // ══════════════════════════════════════════════════════════════════════════

    public void AccionComprar()
    {
        if (modoActual != ModoTienda.Comprar) return;
        if (itemConsumibleSeleccionado == null && equipoSeleccionado == null) return;

        string nombre = itemConsumibleSeleccionado != null
            ? itemConsumibleSeleccionado.nombre : equipoSeleccionado.nombre;
        int precio = itemConsumibleSeleccionado != null
            ? itemConsumibleSeleccionado.precioCompra : equipoSeleccionado.precioCompra;

        if (datosRyo.oro < precio)
        {
            MostrarInfo(nombre, "No tienes suficiente oro.", "Necesitas: " + precio + "G");
            return;
        }

        AbrirConfirmacion("¿Comprar " + nombre + " por " + precio + "G?",
            ConfirmarCompra, CancelarConfirmacion);
    }

    void ConfirmarCompra()
    {
        if (itemConsumibleSeleccionado != null)
        {
            datosRyo.oro -= itemConsumibleSeleccionado.precioCompra;
            datosRyo.mochilaItems.Add(itemConsumibleSeleccionado);
            MostrarInfo(itemConsumibleSeleccionado.nombre, "¡Comprado!",
                        "Oro restante: " + datosRyo.oro + "G");
            ActualizarOro();
            if (panelConfirmacion != null) panelConfirmacion.SetActive(false);
        }
        else if (equipoSeleccionado != null)
        {
            datosRyo.oro -= equipoSeleccionado.precioCompra;
            ActualizarOro();
            if (panelConfirmacion != null) panelConfirmacion.SetActive(false);

            var eq = equipoSeleccionado;
            AbrirConfirmacion("¿Equiparte " + eq.nombre + " ahora?",
                () =>
                {
                    datosRyo.EquiparObjeto(eq);
                    MostrarInfo(eq.nombre, "¡Equipado!", "Oro restante: " + datosRyo.oro + "G");
                    if (panelConfirmacion != null) panelConfirmacion.SetActive(false);
                },
                () =>
                {
                    datosRyo.armarioEquipo.Add(eq);
                    MostrarInfo(eq.nombre, "¡Guardado en el armario!",
                                "Oro restante: " + datosRyo.oro + "G");
                    if (panelConfirmacion != null) panelConfirmacion.SetActive(false);
                });
        }
    }

    // ══════════════════════════════════════════════════════════════════════════
    // VENDER
    // ══════════════════════════════════════════════════════════════════════════

    public void AccionVender()
    {
        if (modoActual != ModoTienda.Vender) return;
        if (itemConsumibleSeleccionado == null && equipoSeleccionado == null) return;

        string nombre = itemConsumibleSeleccionado != null
            ? itemConsumibleSeleccionado.nombre : equipoSeleccionado.nombre;
        int precio = itemConsumibleSeleccionado != null
            ? itemConsumibleSeleccionado.precioVenta : equipoSeleccionado.precioVenta;

        AbrirConfirmacion("¿Vender " + nombre + " por " + precio + "G?",
            ConfirmarVenta, CancelarConfirmacion);
    }

    void ConfirmarVenta()
    {
        if (itemConsumibleSeleccionado != null)
        {
            datosRyo.oro += itemConsumibleSeleccionado.precioVenta;
            datosRyo.mochilaItems.Remove(itemConsumibleSeleccionado);
            MostrarInfo(itemConsumibleSeleccionado.nombre, "¡Vendido!",
                        "Oro: " + datosRyo.oro + "G");
            itemConsumibleSeleccionado = null;
        }
        else if (equipoSeleccionado != null)
        {
            datosRyo.oro += equipoSeleccionado.precioVenta;
            datosRyo.armarioEquipo.Remove(equipoSeleccionado);
            MostrarInfo(equipoSeleccionado.nombre, "¡Vendido!", "Oro: " + datosRyo.oro + "G");
            equipoSeleccionado = null;
        }
        ActualizarOro();
        if (panelConfirmacion != null) panelConfirmacion.SetActive(false);
        MostrarModoVender();
    }

    void CancelarConfirmacion()
    {
        if (panelConfirmacion != null) panelConfirmacion.SetActive(false);
    }

    // ══════════════════════════════════════════════════════════════════════════
    // UTILIDADES
    // ══════════════════════════════════════════════════════════════════════════

    void AbrirConfirmacion(string texto,
        UnityEngine.Events.UnityAction accionSi,
        UnityEngine.Events.UnityAction accionNo)
    {
        if (panelConfirmacion == null) return;
        panelConfirmacion.SetActive(true);
        if (textoConfirmacion != null) textoConfirmacion.text = texto;
        botonConfirmarSi.onClick.RemoveAllListeners();
        botonConfirmarNo.onClick.RemoveAllListeners();
        botonConfirmarSi.onClick.AddListener(accionSi);
        botonConfirmarNo.onClick.AddListener(accionNo);
    }

    void CrearBoton(string nombre, string precio, UnityEngine.Events.UnityAction accion)
    {
        GameObject boton = Instantiate(prefabBotonItem, contenedorLista);
        TextMeshProUGUI[] textos = boton.GetComponentsInChildren<TextMeshProUGUI>();
        if (textos.Length >= 2) { textos[0].text = nombre; textos[1].text = precio; }
        else if (textos.Length == 1) textos[0].text = nombre + "  " + precio;
        boton.GetComponent<Button>().onClick.AddListener(accion);
    }

    void LimpiarLista()
    {
        foreach (Transform hijo in contenedorLista) Destroy(hijo.gameObject);
    }

    void MostrarInfo(string nombre, string descripcion, string precio)
    {
        if (textoNombreItem != null) textoNombreItem.text = nombre;
        if (textoDescripcion != null) textoDescripcion.text = descripcion + "\n" + precio;
    }

    void LimpiarInfo()
    {
        if (textoNombreItem != null) textoNombreItem.text = "";
        if (textoDescripcion != null) textoDescripcion.text = "";
    }

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
}