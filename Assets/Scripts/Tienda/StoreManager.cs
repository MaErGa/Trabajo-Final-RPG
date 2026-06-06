using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class StoreManager : MonoBehaviour
{
    [Header("CONFIGURACIÓN DE DATOS")]
    public DatosJugador datosRyo;

    [Header("CATÁLOGO DEL TENDERO")]
    public List<EquipoBase>      equipoEnVenta      = new List<EquipoBase>();
    public List<ItemConsumible>  consumiblesEnVenta = new List<ItemConsumible>();

    [Header("ELEMENTOS DE LA INTERFAZ (UI)")]
    public TextMeshProUGUI textoOroUI;
    public TextMeshProUGUI textoInfoUI;

    private bool modoComprar = true;

    // ─────────────────────────────────────────────────────────────────────────
    void OnEnable()
    {
        CambiarModoComprar(true);
    }

    // ─────────────────────────────────────────────────────────────────────────
    public void ActualizarOroEnPantalla()
    {
        if (textoOroUI != null && datosRyo != null)
            textoOroUI.text = "Oro: " + datosRyo.oro + " G";
    }

    public void CambiarModoComprar(bool esComprar)
    {
        modoComprar = esComprar;
        ActualizarOroEnPantalla();
        if (textoInfoUI != null)
            textoInfoUI.text = modoComprar
                ? "Modo Comprar: Selecciona un artículo del tendero."
                : "Modo Vender: Selecciona un artículo de tu mochila.";
    }

    // ─────────────────────────────────────────────────────────────────────────
    // COMPRAR / VENDER  —  EQUIPO
    // ─────────────────────────────────────────────────────────────────────────
    public void AccionObjeto(EquipoBase item)
    {
        if (datosRyo == null || item == null) return;

        if (modoComprar)
        {
            if (datosRyo.oro >= item.precioCompra)
            {
                datosRyo.oro -= item.precioCompra;
                datosRyo.armarioEquipo?.Add(item);
                ActualizarOroEnPantalla();
                if (textoInfoUI != null)
                    textoInfoUI.text = "¡Comprado " + item.nombre + " por " + item.precioCompra + "G!";
            }
            else
            {
                if (textoInfoUI != null)
                    textoInfoUI.text = "¡No tienes suficiente Oro para " + item.nombre + "!";
            }
        }
        else
        {
            if (datosRyo.armarioEquipo != null && datosRyo.armarioEquipo.Contains(item))
            {
                datosRyo.oro += item.precioVenta;
                datosRyo.armarioEquipo.Remove(item);
                ActualizarOroEnPantalla();
                if (textoInfoUI != null)
                    textoInfoUI.text = "¡Vendido " + item.nombre + " por " + item.precioVenta + "G!";
            }
            else
            {
                if (textoInfoUI != null)
                    textoInfoUI.text = "Ese objeto no está en tu mochila.";
            }
        }
    }

    // ─────────────────────────────────────────────────────────────────────────
    // COMPRAR / VENDER  —  CONSUMIBLES  ← NUEVO
    // ─────────────────────────────────────────────────────────────────────────
    public void AccionConsumible(ItemConsumible item)
    {
        if (datosRyo == null || item == null) return;

        if (modoComprar)
        {
            if (datosRyo.oro >= item.precioCompra)
            {
                datosRyo.oro -= item.precioCompra;
                datosRyo.mochilaItems?.Add(item);   // va a la mochila, no al armario
                ActualizarOroEnPantalla();
                if (textoInfoUI != null)
                    textoInfoUI.text = "¡Comprado " + item.nombre + " por " + item.precioCompra + "G!";
            }
            else
            {
                if (textoInfoUI != null)
                    textoInfoUI.text = "¡No tienes suficiente Oro para " + item.nombre + "!";
            }
        }
        else
        {
            if (datosRyo.mochilaItems != null && datosRyo.mochilaItems.Contains(item))
            {
                datosRyo.oro += item.precioVenta;
                datosRyo.mochilaItems.Remove(item);
                ActualizarOroEnPantalla();
                if (textoInfoUI != null)
                    textoInfoUI.text = "¡Vendido " + item.nombre + " por " + item.precioVenta + "G!";
            }
            else
            {
                if (textoInfoUI != null)
                    textoInfoUI.text = "Ese objeto no está en tu mochila.";
            }
        }
    }
}