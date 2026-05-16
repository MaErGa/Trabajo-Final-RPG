using UnityEngine;

public enum TipoSlot { Arma, Armadura, Escudo, Casco, Accesorio }

[CreateAssetMenu(fileName = "NuevoEquipo", menuName = "RPG/Equipo")]
public class EquipoBase : ScriptableObject
{
    [Header("Información")]
    public string nombre;
    public Sprite icono;
    public TipoSlot tipoSlot; // ← desplegable para elegir qué slot ocupa

    [Header("Bonificadores")]
    public int bonoAtaque;
    public int bonoDefensa;
    public int bonoAgilidad;

    [Header("Economía")]
    public int precioCompra;
    public int precioVenta;
}