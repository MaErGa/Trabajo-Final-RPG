using UnityEngine;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    [Header("Referencias")]
    public DatosJugador datosRyo;
    public GameObject prefabSlot; // El botón que crearemos
    public Transform contenedorPadre; // El objeto 'Content' de tu ScrollView o Panel

    [Header("Información del Item Seleccionado")]
    public TextMeshProUGUI textoNombre;
    public TextMeshProUGUI textoDescripcion;

    private void OnEnable()
    {
        ActualizarInventarioUI();
    }

    public void ActualizarInventarioUI()
    {
        // 1. Limpiar el inventario visual anterior
        foreach (Transform hijo in contenedorPadre)
        {
            Destroy(hijo.gameObject);
        }

        // 2. Dibujar Items Consumibles (Plantas, etc)
        // Por ahora usamos tu sistema antiguo de enteros para las plantas
        if (datosRyo.plantasMedicinales > 0)
        {
            CrearBotonItem("Planta Medicinal", "Cura 30 HP", null);
        }

        // 3. Dibujar Equipo (Armas, Cascos, etc)
        foreach (EquipoBase equipo in datosRyo.armarioEquipo)
        {
            CrearBotonItem(equipo.nombre, "Def: " + equipo.bonoDefensa + " Atq: " + equipo.bonoAtaque, equipo);
        }
    }

    void CrearBotonItem(string nombre, string desc, EquipoBase itemData)
    {
        GameObject nuevoSlot = Instantiate(prefabSlot, contenedorPadre);
        
        // Configurar texto del botón
        nuevoSlot.GetComponentInChildren<TextMeshProUGUI>().text = nombre;

        // Configurar el evento del botón
        Button btn = nuevoSlot.GetComponent<Button>();
        btn.onClick.AddListener(() => SeleccionarItem(nombre, desc, itemData));
    }

    public void SeleccionarItem(string nombre, string desc, EquipoBase itemData)
    {
        textoNombre.text = nombre;
        textoDescripcion.text = desc;

        // Si es equipo, mostramos opción de equipar
        if (itemData != null)
        {
            // Aquí llamaríamos a datosRyo.EquiparObjeto(itemData);
            Debug.Log("Has seleccionado: " + itemData.nombre);
        }
    }
}