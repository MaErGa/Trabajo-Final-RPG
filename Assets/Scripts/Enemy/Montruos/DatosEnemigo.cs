using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class EntradaLoot
{
    public ItemConsumible item;
    [Range(0, 100)]
    public int probabilidad;
}

[System.Serializable]
public class EntradaLootEquipo
{
    public EquipoBase equipo;
    [Range(0, 100)]
    public int probabilidad;
}

/// <summary>
/// Ataque especial que puede causar un estado alterado al jugador.
/// Configúralo desde el Inspector de cada asset de enemigo.
/// </summary>
[System.Serializable]
public class AtaqueEspecial
{
    [Tooltip("Nombre del ataque que aparecerá en los mensajes de combate.\nEj: 'Golpe Venenoso', 'Golpe Somnífero', 'Golpe Paralizante'")]
    public string nombreAtaque = "Golpe Especial";

    [Tooltip("Daño base del ataque (0 = mismo daño que el ataque normal)")]
    public int dañoBase = 0;

    [Tooltip("Estado alterado que aplica este ataque al jugador")]
    public EstadoAlterado estadoQueAplica = EstadoAlterado.Normal;

    [Range(0, 100)]
    [Tooltip("Probabilidad (0-100) de que el ataque cause el estado alterado")]
    public int probabilidadEstado = 35;

    [Tooltip("Duración en turnos del estado alterado (ignorado si el estado es Envenenado, que es persistente)")]
    [Range(1, 8)]
    public int duracionTurnos = 3;

    [Tooltip("Probabilidad (0-100) de que el enemigo use este ataque especial en su turno, en lugar del ataque normal")]
    [Range(0, 100)]
    public int probabilidadUso = 25;
}

[CreateAssetMenu(fileName = "NuevoEnemigo", menuName = "RPG/Enemigo")]
public class DatosEnemigo : ScriptableObject
{
    public string nombreEnemigo;
    public int vidaMaxima;
    public int dañoAtaque;
    public int agilidad;
    public int defensa;
    public Sprite imagenEnemigo;

    [Header("Recompensas")]
    public int expAlMorir;
    public int oroAlMorir;

    [Header("Loot - Items Consumibles")]
    public List<EntradaLoot> tablaLoot = new List<EntradaLoot>();

    [Header("Loot - Equipo y Accesorios")]
    public List<EntradaLootEquipo> tablaLootEquipo = new List<EntradaLootEquipo>();

    // ── ATAQUES ESPECIALES ────────────────────────────────────────────────────
    [Header("Ataques Especiales con Estado Alterado")]
    [Tooltip("Lista de ataques especiales que puede usar este enemigo.\n" +
             "Cada ataque tiene su propia probabilidad de uso y puede aplicar un estado alterado.\n\n" +
             "Ejemplos predefinidos del Excel:\n" +
             "· Golpe Paralizante → Paralizado (35%, 3-5 turnos)\n" +
             "· Golpe Somnífero   → Dormido    (40%, 2-5 turnos)\n" +
             "· Golpe Venenoso    → Envenenado (50%, persistente)")]
    public List<AtaqueEspecial> ataquesEspeciales = new List<AtaqueEspecial>();

    // ── HELPERS ───────────────────────────────────────────────────────────────

    /// <summary>
    /// Elige al azar qué ataque realizará el enemigo este turno.
    /// Devuelve null si usa el ataque normal.
    /// </summary>
    public AtaqueEspecial ElegirAtaque()
    {
        foreach (var ataque in ataquesEspeciales)
        {
            if (ataque.probabilidadUso > 0 && Random.Range(0, 100) < ataque.probabilidadUso)
                return ataque;
        }
        return null;
    }
}