using UnityEngine;

[CreateAssetMenu(fileName = "NuevoEquipo", menuName = "RPG/Equipo")]
public class EquipoBase : ScriptableObject
{
    public string nombre;
    public Sprite icono;
    [TextArea] public string descripcion;

    public TipoSlot tipoSlot;

    public int precioCompra;
    public int precioVenta;
    public int bonoAtaque;
    public int bonoDefensa;
    public int bonoAgilidad;
}