using UnityEngine;

// Esto nos permite elegir en el Inspector si cura Vida o Maná
public enum TipoEfecto { Vida, Mana, Antidoto }

[CreateAssetMenu(fileName = "NuevoConsumible", menuName = "RPG/Item Consumible")]
public class ItemConsumible : ScriptableObject
{
    [Header("Información Básica")]
    public string nombre;
    public Sprite icono;
    [TextArea] public string descripcion;

    [Header("Efecto del Item")]
    public TipoEfecto queCura; 
    public int potencia; // Ejemplo: 20 para una poción pequeña

    [Header("Economía")]
    public int precioCompra;
    public int precioVenta;
}