using UnityEngine;

[CreateAssetMenu(fileName = "NuevoConsumible", menuName = "RPG/Item Consumible")]
public class ItemConsumible : ScriptableObject
{
    [Header("Información Básica")]
    public string nombre;
    public Sprite icono;
    [TextArea] public string descripcion;

    [Header("Efecto del Item")]
    public TipoEfecto queCura; // Ahora usa el Enum Global sin errores
    public int potencia;

    [Header("Economía")]
    public int precioCompra;
    public int precioVenta;
}