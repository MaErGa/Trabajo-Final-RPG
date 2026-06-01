using UnityEngine;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;

// ─────────────────────────────────────────────────────────────────────────────
//  InventoryManager.cs  –  Gestiona el inventario del jugador en la UI.
//  Trabaja con tus ScriptableObjects: DatosJugador, EquipoBase, ItemConsumible.
//  Adjunta este script al GameObject que contenga el panel de inventario.
// ─────────────────────────────────────────────────────────────────────────────
public class InventoryManager : MonoBehaviour
{
    [Header("Datos del Jugador")]
    public DatosJugador datosJugador;

    [Header("Prefab de fila (un botón con TMP)")]
    public GameObject prefabSlot;

    [Header("Contenedor donde se instancian las filas")]
    public Transform contenedorPadre;           // Content del ScrollView

    [Header("Panel de información del item seleccionado")]
    public TextMeshProUGUI textoNombre;
    public TextMeshProUGUI textoDescripcion;

    [Header("Botón Usar (se activa al seleccionar un consumible)")]
    public Button botonUsar;                    // Botón "Usar"
    public TextMeshProUGUI textoBotonUsar;      // Texto del botón (opcional)

    // ── Estado interno ────────────────────────────────────────────────────────
    private ItemConsumible itemConsumibleSeleccionado;
    private EquipoBase     equipoSeleccionado;

    // ─────────────────────────────────────────────────────────────────────────
    private void OnEnable()
    {
        ActualizarInventarioUI();
        OcultarBotonUsar();
    }

    // ─────────────────────────────────────────────────────────────────────────
    //  DIBUJAR INVENTARIO
    // ─────────────────────────────────────────────────────────────────────────
    public void ActualizarInventarioUI()
    {
        // Limpiar filas anteriores
        foreach (Transform hijo in contenedorPadre)
            Destroy(hijo.gameObject);

        itemConsumibleSeleccionado = null;
        equipoSeleccionado         = null;
        OcultarBotonUsar();

        // ── Consumibles (mochilaItems) ────────────────────────────────────────
        foreach (ItemConsumible item in datosJugador.mochilaItems)
        {
            ItemConsumible captura = item;  // necesario para la lambda

            string descCorta = $"{ObtenerTextoEfecto(item.queCura)} +{item.potencia}";
            CrearFila(item.nombre, descCorta, () => SeleccionarConsumible(captura));
        }

        // ── Plantas medicinales (sistema antiguo, curan 30 HP) ────────────────
        if (datosJugador.plantasMedicinales > 0)
        {
            CrearFila(
                $"Planta Medicinal  x{datosJugador.plantasMedicinales}",
                "Vida +30",
                UsarPlantaMedicinal
            );
        }

        // ── Equipo en armario ─────────────────────────────────────────────────
        foreach (EquipoBase equipo in datosJugador.armarioEquipo)
        {
            EquipoBase captura = equipo;
            string descCorta = $"[{equipo.tipoSlot}]  ATQ:{equipo.bonoAtaque}  DEF:{equipo.bonoDefensa}  AGI:{equipo.bonoAgilidad}";
            CrearFila(equipo.nombre, descCorta, () => SeleccionarEquipo(captura));
        }
    }

    // ─────────────────────────────────────────────────────────────────────────
    //  CREAR FILA
    // ─────────────────────────────────────────────────────────────────────────
    void CrearFila(string nombre, string desc, System.Action alHacerClick)
    {
        GameObject slot = Instantiate(prefabSlot, contenedorPadre);
        slot.GetComponentInChildren<TextMeshProUGUI>().text = nombre;

        Button btn = slot.GetComponent<Button>();
        btn.onClick.AddListener(() =>
        {
            // Mostrar info en el panel lateral
            if (textoNombre)      textoNombre.text      = nombre;
            if (textoDescripcion) textoDescripcion.text = desc;
            alHacerClick?.Invoke();
        });
    }

    // ─────────────────────────────────────────────────────────────────────────
    //  SELECCIÓN
    // ─────────────────────────────────────────────────────────────────────────
    void SeleccionarConsumible(ItemConsumible item)
    {
        itemConsumibleSeleccionado = item;
        equipoSeleccionado         = null;

        // Mostrar descripción completa
        if (textoDescripcion)
            textoDescripcion.text = item.descripcion;

        // Activar botón "Usar"
        if (botonUsar)
        {
            botonUsar.gameObject.SetActive(true);
            botonUsar.onClick.RemoveAllListeners();
            botonUsar.onClick.AddListener(UsarItemSeleccionado);
            if (textoBotonUsar) textoBotonUsar.text = "Usar";
        }
    }

    void SeleccionarEquipo(EquipoBase equipo)
    {
        equipoSeleccionado         = equipo;
        itemConsumibleSeleccionado = null;

        if (textoDescripcion)
            textoDescripcion.text = equipo.descripcion;

        // Activar botón como "Equipar"
        if (botonUsar)
        {
            botonUsar.gameObject.SetActive(true);
            botonUsar.onClick.RemoveAllListeners();
            botonUsar.onClick.AddListener(EquiparSeleccionado);
            if (textoBotonUsar) textoBotonUsar.text = "Equipar";
        }
    }

    // ─────────────────────────────────────────────────────────────────────────
    //  USAR / EQUIPAR
    // ─────────────────────────────────────────────────────────────────────────
    public void UsarItemSeleccionado()
    {
        if (itemConsumibleSeleccionado == null) return;

        AplicarEfectoConsumible(itemConsumibleSeleccionado, datosJugador);

        // Eliminar del inventario
        datosJugador.mochilaItems.Remove(itemConsumibleSeleccionado);
        itemConsumibleSeleccionado = null;

        // Refrescar UI
        ActualizarInventarioUI();

        if (textoNombre)      textoNombre.text      = "";
        if (textoDescripcion) textoDescripcion.text  = "";
    }

    void UsarPlantaMedicinal()
    {
        if (datosJugador.plantasMedicinales <= 0) return;

        datosJugador.hpActual = Mathf.Min(datosJugador.hpMax, datosJugador.hpActual + 30);
        datosJugador.plantasMedicinales--;

        Debug.Log($"Planta usada. HP: {datosJugador.hpActual}/{datosJugador.hpMax}");
        ActualizarInventarioUI();
    }

    public void EquiparSeleccionado()
    {
        if (equipoSeleccionado == null) return;

        datosJugador.EquiparObjeto(equipoSeleccionado);

        // Mover del armario al slot equipado (quitar del armario)
        datosJugador.armarioEquipo.Remove(equipoSeleccionado);
        equipoSeleccionado = null;

        ActualizarInventarioUI();

        if (textoNombre)      textoNombre.text      = "";
        if (textoDescripcion) textoDescripcion.text  = "";

        Debug.Log("Equipo cambiado.");
    }

    // ─────────────────────────────────────────────────────────────────────────
    //  LÓGICA DE EFECTO CONSUMIBLE
    //  Añade aquí nuevos TipoEfecto cuando los necesites.
    // ─────────────────────────────────────────────────────────────────────────
    public static void AplicarEfectoConsumible(ItemConsumible item, DatosJugador jugador)
    {
        switch (item.queCura)
        {
            case TipoEfecto.Vida:
                jugador.hpActual = Mathf.Min(jugador.hpMax, jugador.hpActual + item.potencia);
                Debug.Log($"HP restaurado: {jugador.hpActual}/{jugador.hpMax}");
                break;

            case TipoEfecto.Mana:
                jugador.mpActual = Mathf.Min(jugador.mpMax, jugador.mpActual + item.potencia);
                Debug.Log($"MP restaurado: {jugador.mpActual}/{jugador.mpMax}");
                break;

            case TipoEfecto.Antidoto:
                // Aquí conectarías con tu sistema de estados alterados cuando lo tengas
                Debug.Log("Veneno curado.");
                break;
        }
    }

    // ─────────────────────────────────────────────────────────────────────────
    //  UTILIDADES
    // ─────────────────────────────────────────────────────────────────────────
    void OcultarBotonUsar()
    {
        if (botonUsar) botonUsar.gameObject.SetActive(false);
    }

    static string ObtenerTextoEfecto(TipoEfecto efecto)
    {
        return efecto switch
        {
            TipoEfecto.Vida    => "HP",
            TipoEfecto.Mana    => "MP",
            TipoEfecto.Antidoto => "Cura veneno",
            _                  => "?"
        };
    }
}