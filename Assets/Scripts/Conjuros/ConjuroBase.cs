using UnityEngine;

// Esto es para el desplegable de los tipos
public enum TipoEfecto { Daño, Curacion, AumentoDefensa }

[CreateAssetMenu(fileName = "NuevoConjuro", menuName = "RPG/Conjuro")]
public class ConjuroBase : ScriptableObject
{
    [Header("Información Básica")]
    public string nombreConjuro;
    [TextArea] public string descripcion;
    public int costeMP;

    [Header("Configuración del Efecto")]
    public TipoEfecto tipo; // El desplegable que te faltaba
    public int valorEfecto; // El número de daño, cura o defensa
    public int duracionTurnos; // Para el Fortalecimiento (ej: 3 turnos)

    [Header("Visual")]
    public Sprite icono;
}