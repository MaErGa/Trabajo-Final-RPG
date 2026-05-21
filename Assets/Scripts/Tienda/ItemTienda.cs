using UnityEngine;

[CreateAssetMenu(fileName = "NuevoObjeto", menuName = "Tienda/Objeto")]
public class ItemTienda : ScriptableObject
{
    public string nombreObjeto;
    public Sprite icono;
    public int precio;
    [TextArea] public string descripcion;
}