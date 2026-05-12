using UnityEngine;

// Esto permite que crees archivos de enemigos haciendo clic derecho en Unity
[CreateAssetMenu(fileName = "NuevoEnemigo", menuName = "RPG/Enemigo")]
public class DatosEnemigo : ScriptableObject
{
    public string nombreEnemigo;
    public int vidaMaxima;
    public int dañoAtaque;
    public Sprite imagenEnemigo;
    
    [Header("Recompensas")]
    public int expAlMorir;
    public int oroAlMorir;
}