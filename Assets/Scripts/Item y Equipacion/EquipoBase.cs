using UnityEngine;

[CreateAssetMenu(fileName = "NuevoEquipo", menuName = "RPG/Equipo")]
public class EquipoBase : ScriptableObject
{
    public string nombre;
    public Sprite icono;
    public int bonoAtaque;
    public int bonoDefensa;
    public int bonoAgilidad;
    public int precio;
     public int precioCompra; 
public int precioVenta; 
}