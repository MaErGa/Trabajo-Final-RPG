using UnityEngine;
using TMPro;

public class BotonTiendaUI : MonoBehaviour
{
    public EquipoBase itemAsignado;
    public TextMeshProUGUI textoDelBotones;

    void Start()
    {
        if (itemAsignado != null && textoDelBotones != null)
        {
            textoDelBotones.text = itemAsignado.nombre + " " + itemAsignado.precioCompra + "G";
        }
    }

    public void AlPulsarBoton()
    {
        StoreManager gestor = FindObjectOfType<StoreManager>();
        if (gestor != null)
        {
            gestor.AccionObjeto(itemAsignado);
        }
    }
}