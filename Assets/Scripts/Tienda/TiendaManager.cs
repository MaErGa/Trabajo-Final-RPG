using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

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

    [Header("Botones Comprar/Vender")]
    public Button botonComprar;
    public Button botonVender;

    [Header("Panel Confirmacion")]
    public GameObject panelConfirmacion;
    public TextMeshProUGUI textoConfirmacion;
    public Button botonConfirmarSi;
    public Button botonConfirmarNo;

    private enum ModoTienda { Comprar, Vender }
    private ModoTienda modoActual = ModoTienda.Comprar;

    private ItemConsumible itemConsumibleSeleccionado;
    private EquipoBase equipoSeleccionado;

    void Start()
    {
        if (panelConfirmacion != null) panelConfirmacion.SetActive(false);
        ActualizarOro();
        MostrarModoComprar();
    }

    // ── Modos ─────────────────────────────────────────────────

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
            var itemRef = item;
            CrearBoton(item.nombre, item.precioCompra + "G", () => SeleccionarConsumible(itemRef));
        }

        foreach (var equipo in equipoEnVenta)
        {
            var equipoRef = equipo;
            CrearBoton(equipo.nombre, equipo.precioCompra + "G", () => SeleccionarEquipo(equipoRef));
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
            var itemRef = item;
            CrearBoton(item.nombre, item.precioVenta + "G", () => SeleccionarConsumibleVenta(itemRef));
        }

        foreach (var equipo in datosRyo.armarioEquipo)
        {
            if (equipo == null) continue;
            var equipoRef = equipo;
            CrearBoton(equipo.nombre, equipo.precioVenta + "G", () => SeleccionarEquipoVenta(equipoRef));
        }

        if (datosRyo.plantasMedicinales > 0)
            CrearBoton("Planta Medicinal x" + datosRyo.plantasMedicinales, "5G", () =>
            {
                MostrarInfo("Planta Medicinal", "Restaura 30 HP.", "Venta: 5G");
                itemConsumibleSeleccionado = null;
                equipoSeleccionado = null;
            });
    }

    // ── Selección (muestra descripción) ───────────────────────

    void SeleccionarConsumible(ItemConsumible item)
    {
        itemConsumibleSeleccionado = item;
        equipoSeleccionado = null;
        if (panelConfirmacion != null) panelConfirmacion.SetActive(false);
        MostrarInfo(item.nombre, item.descripcion, "Precio: " + item.precioCompra + "G");
    }

    void SeleccionarEquipo(EquipoBase equipo)
    {
        equipoSeleccionado = equipo;
        itemConsumibleSeleccionado = null;
        if (panelConfirmacion != null) panelConfirmacion.SetActive(false);
        string stats = "";
        if (equipo.bonoAtaque > 0)   stats += "ATQ +" + equipo.bonoAtaque + "  ";
        if (equipo.bonoDefensa > 0)  stats += "DEF +" + equipo.bonoDefensa + "  ";
        if (equipo.bonoAgilidad > 0) stats += "AGI +" + equipo.bonoAgilidad;
        MostrarInfo(equipo.nombre, equipo.descripcion + "\n" + stats, "Precio: " + equipo.precioCompra + "G");
    }

    void SeleccionarConsumibleVenta(ItemConsumible item)
    {
        itemConsumibleSeleccionado = item;
        equipoSeleccionado = null;
        if (panelConfirmacion != null) panelConfirmacion.SetActive(false);
        MostrarInfo(item.nombre, item.descripcion, "Venta: " + item.precioVenta + "G");
    }

    void SeleccionarEquipoVenta(EquipoBase equipo)
    {
        equipoSeleccionado = equipo;
        itemConsumibleSeleccionado = null;
        if (panelConfirmacion != null) panelConfirmacion.SetActive(false);
        string stats = "";
        if (equipo.bonoAtaque > 0)   stats += "ATQ +" + equipo.bonoAtaque + "  ";
        if (equipo.bonoDefensa > 0)  stats += "DEF +" + equipo.bonoDefensa + "  ";
        if (equipo.bonoAgilidad > 0) stats += "AGI +" + equipo.bonoAgilidad;
        MostrarInfo(equipo.nombre, equipo.descripcion + "\n" + stats, "Venta: " + equipo.precioVenta + "G");
    }

    // ── Botón Comprar → abre confirmación ─────────────────────

    public void AccionComprar()
    {
        if (modoActual != ModoTienda.Comprar) return;
        if (itemConsumibleSeleccionado == null && equipoSeleccionado == null) return;

        string nombreItem = itemConsumibleSeleccionado != null ? itemConsumibleSeleccionado.nombre : equipoSeleccionado.nombre;
        int precio = itemConsumibleSeleccionado != null ? itemConsumibleSeleccionado.precioCompra : equipoSeleccionado.precioCompra;

        if (datosRyo.oro < precio)
        {
            MostrarInfo(nombreItem, "No tienes suficiente oro.", "Necesitas: " + precio + "G");
            return;
        }

        // Mostrar panel confirmación
        if (panelConfirmacion != null)
        {
            panelConfirmacion.SetActive(true);
            if (textoConfirmacion != null)
                textoConfirmacion.text = "¿Comprar " + nombreItem + " por " + precio + "G?";

            botonConfirmarSi.onClick.RemoveAllListeners();
            botonConfirmarNo.onClick.RemoveAllListeners();
            botonConfirmarSi.onClick.AddListener(ConfirmarCompra);
            botonConfirmarNo.onClick.AddListener(CancelarConfirmacion);
        }
    }

    void ConfirmarCompra()
    {
        if (itemConsumibleSeleccionado != null)
        {
            datosRyo.oro -= itemConsumibleSeleccionado.precioCompra;
            datosRyo.mochilaItems.Add(itemConsumibleSeleccionado);
            MostrarInfo(itemConsumibleSeleccionado.nombre, "¡Comprado!", "Oro restante: " + datosRyo.oro + "G");
            ActualizarOro();
            if (panelConfirmacion != null) panelConfirmacion.SetActive(false);
        }
        else if (equipoSeleccionado != null)
        {
            datosRyo.oro -= equipoSeleccionado.precioCompra;
            ActualizarOro();
            if (panelConfirmacion != null) panelConfirmacion.SetActive(false);

            // Preguntar si quiere equipárselo
            var equipoComprado = equipoSeleccionado;
            panelConfirmacion.SetActive(true);
            if (textoConfirmacion != null)
                textoConfirmacion.text = "¿Quieres equiparte " + equipoComprado.nombre + " ahora?";

            botonConfirmarSi.onClick.RemoveAllListeners();
            botonConfirmarNo.onClick.RemoveAllListeners();

            botonConfirmarSi.onClick.AddListener(() =>
            {
                datosRyo.EquiparObjeto(equipoComprado);
                MostrarInfo(equipoComprado.nombre, "¡Equipado!", "Oro restante: " + datosRyo.oro + "G");
                if (panelConfirmacion != null) panelConfirmacion.SetActive(false);
            });

            botonConfirmarNo.onClick.AddListener(() =>
            {
                datosRyo.armarioEquipo.Add(equipoComprado);
                MostrarInfo(equipoComprado.nombre, "¡Guardado en el armario!", "Oro restante: " + datosRyo.oro + "G");
                if (panelConfirmacion != null) panelConfirmacion.SetActive(false);
            });
        }
    }

    // ── Botón Vender → abre confirmación ──────────────────────

    public void AccionVender()
    {
        if (modoActual != ModoTienda.Vender) return;
        if (itemConsumibleSeleccionado == null && equipoSeleccionado == null) return;

        string nombreItem = itemConsumibleSeleccionado != null ? itemConsumibleSeleccionado.nombre : equipoSeleccionado.nombre;
        int precio = itemConsumibleSeleccionado != null ? itemConsumibleSeleccionado.precioVenta : equipoSeleccionado.precioVenta;

        if (panelConfirmacion != null)
        {
            panelConfirmacion.SetActive(true);
            if (textoConfirmacion != null)
                textoConfirmacion.text = "¿Vender " + nombreItem + " por " + precio + "G?";

            botonConfirmarSi.onClick.RemoveAllListeners();
            botonConfirmarNo.onClick.RemoveAllListeners();
            botonConfirmarSi.onClick.AddListener(ConfirmarVenta);
            botonConfirmarNo.onClick.AddListener(CancelarConfirmacion);
        }
    }

    void ConfirmarVenta()
    {
        if (itemConsumibleSeleccionado != null)
        {
            datosRyo.oro += itemConsumibleSeleccionado.precioVenta;
            datosRyo.mochilaItems.Remove(itemConsumibleSeleccionado);
            MostrarInfo(itemConsumibleSeleccionado.nombre, "¡Vendido!", "Oro: " + datosRyo.oro + "G");
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

    // ── Utilidades ────────────────────────────────────────────

    void CrearBoton(string nombre, string precio, UnityEngine.Events.UnityAction accion)
    {
        GameObject boton = Instantiate(prefabBotonItem, contenedorLista);
        TextMeshProUGUI[] textos = boton.GetComponentsInChildren<TextMeshProUGUI>();
        if (textos.Length >= 2)
        {
            textos[0].text = nombre;
            textos[1].text = precio;
        }
        else if (textos.Length == 1)
        {
            textos[0].text = nombre + "  " + precio;
        }
        boton.GetComponent<Button>().onClick.AddListener(accion);
    }

    void LimpiarLista()
    {
        foreach (Transform hijo in contenedorLista)
            Destroy(hijo.gameObject);
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
}