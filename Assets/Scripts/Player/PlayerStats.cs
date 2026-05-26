using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [Header("Configuración")]
    public string Nombre = "Retro"; 
    public int Nivel = 1;
    public int Experiencia = 1847; // Seteado por defecto como tu captura
    public int Oro = 58;           // Seteado por defecto como tu captura

    [Header("Vida y Magia")]
    public int HpActual;
    public int HpMax;
    public int MpActual; 
    public int MpMax;
    
    [Header("Combate Base (Nivel 1)")]
    [Tooltip("Fuerza a nivel 1")] public int fuerzaBase = 9;
    [Tooltip("Agilidad a nivel 1")] public int agilidadBase = 6;
    [Tooltip("Defensa a nivel 1")] public int defensaBase = 2;

    [Header("Atributos Actuales (Calculados)")]
    public int Fuerza;
    public int Agilidad;
    public int Defensa;

    private void Awake()
    {
        ActualizarEstadisticasPorNivel();
        
        // Al empezar, el personaje está completamente curado
        HpActual = HpMax;
        MpActual = MpMax;
    }

    /// <summary>
    /// Calcula las estadísticas del personaje basándose en su nivel actual.
    /// Escalado lineal diseñado para alcanzar los objetivos exactos de tu captura a Nivel 10.
    /// </summary>
    public void ActualizarEstadisticasPorNivel()
    {
        // Forzamos que el nivel esté en el rango de tu diseño actual
        Nivel = Mathf.Clamp(Nivel, 1, 10);

        // Factor de interpolación (0 a nivel 1, 1 a nivel 10)
        float t = (Nivel - 1) / 9f;

        // Escalado de Vida: de 20 (Nv 1) a 110 (Nv 10)
        HpMax = Mathf.RoundToInt(Mathf.Lerp(20, 110, t));

        // Escalado de Magia: de 10 (Nv 1) a 50 (Nv 10)
        MpMax = Mathf.RoundToInt(Mathf.Lerp(10, 50, t));

        // Escalado de Atributos de Nivel 1 a Nivel 10
        Fuerza = Mathf.RoundToInt(Mathf.Lerp(fuerzaBase, 35, t));     // Llega a 35
        Agilidad = Mathf.RoundToInt(Mathf.Lerp(agilidadBase, 24, t)); // Llega a 24 (más rápido que los 19 del secuaz)
        Defensa = Mathf.RoundToInt(Mathf.Lerp(defensaBase, 22, t));   // ¡Corregido! Sube de 2 hasta 22 para aguantar los golpes
    }

    // Método para cuando ganes experiencia y subas de nivel en el juego
    public void SubirNivel()
    {
        if (Nivel < 10)
        {
            Nivel++;
            ActualizarEstadisticasPorNivel();
            
            // Cura al jugador al subir de nivel
            HpActual = HpMax;
            MpActual = MpMax;
            
            Debug.Log($"¡Subiste de nivel! Ahora eres nivel {Nivel}");
        }
    }
}