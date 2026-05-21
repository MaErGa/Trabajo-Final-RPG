using UnityEngine;
using TMPro;

public class StoreManager : MonoBehaviour
{
    [Header("CONFIGURACIÓN DE DATOS")]
    public DatosJugador datosRyo; 

    [Header("ELEMENTOS DE LA INTERFAZ (UI)")]
    public TextMeshProUGUI textoOroUI;   
    public TextMeshProUGUI textoInfoUI;  

    private bool modoComprar = true; 

    void OnEnable()
    {
        CambiarModoComprar(true);
    }

    public void ActualizarOroEnPantalla()
    {
        if (textoOroUI != null && datosRyo != null)
        {
            textoOroUI.text = "Oro: " + datosRyo.oro + " G";
        }
    }

    public void CambiarModoComprar(bool esComprar)
    {
        modoComprar = esComprar;
        ActualizarOroEnPantalla();

        if (modoComprar)
        {
            if (textoInfoUI != null) textoInfoUI.text = "Modo Comprar: Selecciona un artículo del tendero.";
        }
        else
        {
            if (textoInfoUI != null) textoInfoUI.text = "Modo Vender: Selecciona un artículo de tu mochila.";
        }
    }

    // El gestor procesa tu EquipoBase usando sus precios de compra y venta reales
    public void AccionObjeto(EquipoBase item)
    {
        if (datosRyo == null || item == null) return;

        if (modoComprar)
        {
            // --- LÓGICA DE COMPRA ---
            if (datosRyo.oro >= item.precioCompra)
            {
                datosRyo.oro -= item.precioCompra; // Restamos lo que cuesta comprarlo
                
                if (datosRyo.armarioEquipo != null)
                {
                    datosRyo.armarioEquipo.Add(item); // Se añade a tu mochila
                }

                ActualizarOroEnPantalla();
                if (textoInfoUI != null) textoInfoUI.text = "¡Comprado " + item.nombre + " por " + item.precioCompra + "G!";
            }
            else
            {
                if (textoInfoUI != null) textoInfoUI.text = "¡No tienes suficiente Oro para " + item.nombre + "!";
            }
        }
        else
        {
            // --- LÓGICA DE VENTA ---
            if (datosRyo.armarioEquipo != null && datosRyo.armarioEquipo.Contains(item))
            {
                datosRyo.oro += item.precioVenta; // ¡Te sumamos tu precio de venta real!
                datosRyo.armarioEquipo.Remove(item); // Se quita de tu mochila
                
                ActualizarOroEnPantalla();
                if (textoInfoUI != null) textoInfoUI.text = "¡Vendido " + item.nombre + " por " + item.precioVenta + "G!";
            }
        }
    }
}