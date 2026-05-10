using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Enemys; // Esto conecta con tu script EnemyCore

[CreateAssetMenu(fileName = "NuevoEnemigo", menuName = "RPG/Enemigo para Encuentro")]
public class EncountEnemySO : ScriptableObject
{
    // Datos del enemigo que aparecerá
    public EnemyCore datosEnemigo;

    // Probabilidad de que aparezca este enemigo (ej: 20 para un 20%)
    [Range(0, 100)]
    public int probabilidad = 10;

    // Método sencillo para obtener una copia de los datos
    // Así cada enemigo en combate tendrá su propia vida
    public EnemyCore ObtenerEnemigo()
    {
        return new EnemyCore(datosEnemigo);
    }
}