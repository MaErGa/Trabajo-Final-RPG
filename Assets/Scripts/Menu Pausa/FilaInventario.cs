using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

/// <summary>
/// Componente que va en el prefab "FilaInventario".
/// Estructura del prefab:
///   FilaInventario (este script aquí)
///   ├── ImgIcono        (Image)
///   ├── TxtNombre       (TextMeshProUGUI)
///   ├── TxtDescripcion  (TextMeshProUGUI)
///   └── BtnUsar         (Button)
///         └── TxtBoton  (TextMeshProUGUI) → texto "Usar"
/// </summary>
public class FilaInventario : MonoBehaviour
{
    [Header("Referencias UI del prefab")]
    public Image       imgIcono;
    public TextMeshProUGUI txtNombre;
    public TextMeshProUGUI txtDescripcion;
    public Button      btnUsar;

    private ItemConsumible _item;
    private Action<ItemConsumible> _onUsar;

    /// <summary>
    /// Inicializa la fila con los datos del item y el callback a ejecutar al pulsar "Usar".
    /// </summary>
    public void Inicializar(ItemConsumible item, Action<ItemConsumible> callbackUsar)
    {
        _item   = item;
        _onUsar = callbackUsar;

        if (item == null)
        {
            gameObject.SetActive(false);
            return;
        }

        // Icono
        if (imgIcono != null)
        {
            imgIcono.sprite  = item.icono;
            imgIcono.enabled = item.icono != null;
        }

        // Nombre
        if (txtNombre != null)
            txtNombre.text = item.nombre;

        // Descripción corta: efecto + potencia
        if (txtDescripcion != null)
            txtDescripcion.text = item.queCura + " +" + item.potencia;

        // Botón
        if (btnUsar != null)
        {
            btnUsar.onClick.RemoveAllListeners();
            btnUsar.onClick.AddListener(OnPulsarUsar);
        }
    }

    private void OnPulsarUsar()
    {
        _onUsar?.Invoke(_item);
    }
}
