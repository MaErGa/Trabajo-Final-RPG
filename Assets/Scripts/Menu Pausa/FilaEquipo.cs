using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

/// <summary>
/// Componente que va en el prefab "FilaEquipo".
/// Estructura del prefab:
///   FilaEquipo (este script aquí)
///   ├── ImgIcono          (Image)
///   ├── TxtNombre         (TextMeshProUGUI)
///   ├── TxtStats          (TextMeshProUGUI)  → "+X ATK / +Y DEF / +Z AGI"
///   └── BtnEquipar        (Button)
///         └── TxtBoton    (TextMeshProUGUI)  → "Equipar" o "Equipado"
/// </summary>
public class FilaEquipo : MonoBehaviour
{
    [Header("Referencias UI del prefab")]
    public Image           imgIcono;
    public TextMeshProUGUI txtNombre;
    public TextMeshProUGUI txtStats;
    public Button          btnEquipar;
    public TextMeshProUGUI txtBoton;   // texto dentro del botón

    private EquipoBase _equipo;
    private Action<EquipoBase> _onEquipar;

    /// <summary>
    /// Inicializa la fila con los datos del equipo y el callback al pulsar "Equipar".
    /// </summary>
    public void Inicializar(EquipoBase equipo, bool yaEquipado, Action<EquipoBase> callbackEquipar)
    {
        _equipo    = equipo;
        _onEquipar = callbackEquipar;

        if (equipo == null)
        {
            gameObject.SetActive(false);
            return;
        }

        // Icono
        if (imgIcono != null)
        {
            imgIcono.sprite  = equipo.icono;
            imgIcono.enabled = equipo.icono != null;
        }

        // Nombre
        if (txtNombre != null)
            txtNombre.text = equipo.nombre + "  [" + equipo.tipoSlot + "]";

        // Stats del equipo
        if (txtStats != null)
        {
            string stats = "";
            if (equipo.bonoAtaque   != 0) stats += "+ATK " + equipo.bonoAtaque   + "  ";
            if (equipo.bonoDefensa  != 0) stats += "+DEF " + equipo.bonoDefensa  + "  ";
            if (equipo.bonoAgilidad != 0) stats += "+AGI " + equipo.bonoAgilidad;
            txtStats.text = stats.Trim();
        }

        // Botón
        if (btnEquipar != null)
        {
            btnEquipar.onClick.RemoveAllListeners();

            if (yaEquipado)
            {
                // Ya equipado: deshabilitar el botón visualmente
                btnEquipar.interactable = false;
                if (txtBoton != null) txtBoton.text = "Equipado";
            }
            else
            {
                btnEquipar.interactable = true;
                if (txtBoton != null) txtBoton.text = "Equipar";
                btnEquipar.onClick.AddListener(OnPulsarEquipar);
            }
        }
    }

    private void OnPulsarEquipar()
    {
        _onEquipar?.Invoke(_equipo);
    }
}
