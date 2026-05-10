using System;
using UnityEngine;
using Battles; // Asegúrate de que este script exista

[Serializable]
public class EnemyCore
{
    public enum MonsterType
    {
        Slime,
        SlimeBeth,
        Drakey,
        Ghost,
        Wizard,
        Scorpion,
        ShimaGu,
    }

    public MonsterType tipoMonstruo = MonsterType.Slime;
    public Sprite imagenEnemigo;
    
    // Aquí es donde irían sus stats (vida, ataque, etc.)
    public BattlerBase stats;

    // Constructor para clonar al enemigo en combate
    public EnemyCore(EnemyCore plantilla)
    {
        this.stats = new BattlerBase(plantilla.stats);
        this.tipoMonstruo = plantilla.tipoMonstruo;
        this.imagenEnemigo = plantilla.imagenEnemigo;
    }
}