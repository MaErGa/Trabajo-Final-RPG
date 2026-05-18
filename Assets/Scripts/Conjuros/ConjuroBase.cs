using UnityEngine;

[CreateAssetMenu(fileName = "NuevoConjuro", menuName = "RPG/Conjuro")]
public class ConjuroBase : ScriptableObject
{
    [Header("Información Básica")]
    public string nombreConjuro;
    [TextArea] public string descripcion;
    public int costeMP;

    [Header("Configuración del Efecto")]
    public TipoEfectoConjuro tipo;
    public int valorEfecto;
    public int duracionTurnos;

    [Header("Visual")]
    public Sprite icono;
}