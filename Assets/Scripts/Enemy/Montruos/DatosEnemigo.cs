using UnityEngine;

[CreateAssetMenu(fileName = "NuevoEnemigo", menuName = "RPG/Enemigo")]
public class DatosEnemigo : ScriptableObject
{
    public string nombreEnemigo;
    public int vidaMaxima;
    public int dañoAtaque;
    public int agilidad; // <-- NUEVO
    public int defensa;  // <-- NUEVO
    public Sprite imagenEnemigo;
    
    [Header("Recompensas")]
    public int expAlMorir;
    public int oroAlMorir;
}