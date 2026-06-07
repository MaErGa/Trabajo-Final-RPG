using UnityEngine;

[CreateAssetMenu(fileName = "NuevoConsumible", menuName = "RPG/Item Consumible")]
public class ItemConsumible : ScriptableObject
{
    public enum TipoEfecto
    {
        Vida,          // Restaura HP
        Mana,          // Restaura MP
        Antidoto,      // Cura el estado Envenenado
        Antiparalisis, // Cura el estado Paralizado
        Despertar      // Cura el estado Dormido
    }

    [Header("Información Básica")]
    public string nombre;
    public Sprite icono;
    [TextArea] public string descripcion;

    [Header("Efecto del Item")]
    public TipoEfecto queCura;
    public int potencia;

    [Header("Economía")]
    public int precioCompra;
    public int precioVenta;
}