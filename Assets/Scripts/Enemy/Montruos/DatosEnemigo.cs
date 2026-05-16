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
}