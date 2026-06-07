using UnityEngine;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;

// ─────────────────────────────────────────────────────────────────────────────
//  InventoryManager.cs  –  Gestiona el inventario del jugador en la UI.
//  Trabaja con tus ScriptableObjects: DatosJugador, EquipoBase, ItemConsumible.
// ─────────────────────────────────────────────────────────────────────────────
public class InventoryManager : MonoBehaviour
{
    [Header("Datos del Jugador")]
    public DatosJugador datosJugador;

    [Header("Prefab de fila (un botón con TMP)")]
    public GameObject prefabSlot;

    [Header("Contenedor donde se instancian las filas")]
    public Transform contenedorPadre;

    [Header("Panel de información del item seleccionado")]
    public TextMeshProUGUI textoNombre;
    public TextMeshProUGUI textoDescripcion;

    [Header("Botón Usar (se activa al seleccionar un consumible)")]
    public Button botonUsar;
    public TextMeshProUGUI textoBotonUsar;

    // ── Estado interno ────────────────────────────────────────────────────────
    private ItemConsumible itemConsumibleSeleccionado;
    private EquipoBase     equipoSeleccionado;

    private void OnEnable()
    {
        ActualizarInventarioUI();
        OcultarBotonUsar();
    }

    // ─────────────────────────────────────────────────────────────────────────
    public void ActualizarInventarioUI()
    {
        foreach (Transform hijo in contenedorPadre)
            Destroy(hijo.gameObject);

        itemConsumibleSeleccionado = null;
        equipoSeleccionado         = null;
        OcultarBotonUsar();

        // ── Consumibles (mochilaItems) ────────────────────────────────────────
        foreach (ItemConsumible item in datosJugador.mochilaItems)
        {
            ItemConsumible captura = item;
            string descCorta = $"{ObtenerTextoEfecto(item.queCura)} +{item.potencia}";
            CrearFila(item.nombre, descCorta, () => SeleccionarConsumible(captura));
        }

        // ── Plantas medicinales (sistema antiguo) ─────────────────────────────
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

    void CrearFila(string nombre, string desc, System.Action alHacerClick)
    {
        GameObject slot = Instantiate(prefabSlot, contenedorPadre);
        slot.GetComponentInChildren<TextMeshProUGUI>().text = nombre;

        Button btn = slot.GetComponent<Button>();
        btn.onClick.AddListener(() =>
        {
            if (textoNombre)      textoNombre.text      = nombre;
            if (textoDescripcion) textoDescripcion.text = desc;
            alHacerClick?.Invoke();
        });
    }

    void SeleccionarConsumible(ItemConsumible item)
    {
        itemConsumibleSeleccionado = item;
        equipoSeleccionado         = null;

        if (textoDescripcion)
            textoDescripcion.text = item.descripcion;

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

        if (botonUsar)
        {
            botonUsar.gameObject.SetActive(true);
            botonUsar.onClick.RemoveAllListeners();
            botonUsar.onClick.AddListener(EquiparSeleccionado);
            if (textoBotonUsar) textoBotonUsar.text = "Equipar";
        }
    }

    public void UsarItemSeleccionado()
    {
        if (itemConsumibleSeleccionado == null) return;

        AplicarEfectoConsumible(itemConsumibleSeleccionado, datosJugador);

        datosJugador.mochilaItems.Remove(itemConsumibleSeleccionado);
        itemConsumibleSeleccionado = null;

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
        datosJugador.armarioEquipo.Remove(equipoSeleccionado);
        equipoSeleccionado = null;
        ActualizarInventarioUI();
        if (textoNombre)      textoNombre.text      = "";
        if (textoDescripcion) textoDescripcion.text  = "";
        Debug.Log("Equipo cambiado.");
    }

    // ─────────────────────────────────────────────────────────────────────────
    //  LÓGICA DE EFECTO CONSUMIBLE
    // ─────────────────────────────────────────────────────────────────────────
    public static void AplicarEfectoConsumible(ItemConsumible item, DatosJugador jugador)
    {
        switch (item.queCura)
        {
            case ItemConsumible.TipoEfecto.Vida:
                jugador.hpActual = Mathf.Min(jugador.hpMax, jugador.hpActual + item.potencia);
                Debug.Log($"HP restaurado: {jugador.hpActual}/{jugador.hpMax}");
                break;

            case ItemConsumible.TipoEfecto.Mana:
                jugador.mpActual = Mathf.Min(jugador.mpMax, jugador.mpActual + item.potencia);
                Debug.Log($"MP restaurado: {jugador.mpActual}/{jugador.mpMax}");
                break;

            case ItemConsumible.TipoEfecto.Antidoto:
                if (jugador.CurarEstadoEspecifico(EstadoAlterado.Envenenado))
                    Debug.Log("Veneno curado con Antídoto.");
                else
                    Debug.Log("El jugador no estaba envenenado.");
                break;

            case ItemConsumible.TipoEfecto.Antiparalisis:
                if (jugador.CurarEstadoEspecifico(EstadoAlterado.Paralizado))
                    Debug.Log("Parálisis curada con Antiparálisis.");
                else
                    Debug.Log("El jugador no estaba paralizado.");
                break;

            case ItemConsumible.TipoEfecto.Despertar:
                if (jugador.CurarEstadoEspecifico(EstadoAlterado.Dormido))
                    Debug.Log("Sueño curado con Despertador.");
                else
                    Debug.Log("El jugador no estaba dormido.");
                break;
        }
    }

    void OcultarBotonUsar()
    {
        if (botonUsar) botonUsar.gameObject.SetActive(false);
    }

    static string ObtenerTextoEfecto(ItemConsumible.TipoEfecto efecto)
    {
        return efecto switch
        {
            ItemConsumible.TipoEfecto.Vida           => "HP",
            ItemConsumible.TipoEfecto.Mana           => "MP",
            ItemConsumible.TipoEfecto.Antidoto       => "Cura veneno",
            ItemConsumible.TipoEfecto.Antiparalisis  => "Cura parálisis",
            ItemConsumible.TipoEfecto.Despertar      => "Despierta",
            _                         => "?"
        };
    }
}