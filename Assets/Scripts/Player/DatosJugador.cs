using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NuevoJugador", menuName = "RPG/Jugador")]
public class DatosJugador : ScriptableObject
{
    public string nombre;
    public int nivel = 1;
    public int hpMax = 20;
    public int hpActual;
    public int mpMax = 5;
    public int mpActual;
    public int fuerza = 8;
    public int agilidad = 6;
    public int defensa = 2;
    public int oro;
    public int experiencia;

    [Header("Atributos Mágicos")]
    public int fuerzaMagica = 5;
    public int terapeucidad = 4;

    [Header("Equipación (Sistema de Assets)")]
    public EquipoBase armaEquipadaAsset;
    public EquipoBase armaduraEquipadaAsset;
    public EquipoBase escudoEquipadoAsset;
    public EquipoBase cascoEquipadoAsset; 
    public EquipoBase accesorioEquipadoAsset;

    [Header("Equipación (Nombres Antiguos)")]
    public string armaEquipada = "Espada de cobre";
    public string armaduraEquipada = "Ropa de viaje";
    public string escudoEquipado = "Escudo de cuero";
    public string cascoEquipado = "Casco de cuero"; 
    public string accesorioEquipado = "Ninguno";

    [Header("Sistema de Niveles")]
    public int expSiguienteNivel = 14;
    public int[] tablaExpPilgrim = { 14, 42, 98, 182, 308, 497, 780, 1205, 1842, 2798 };

    [Header("Inventario Dinámico")]
    public List<ItemConsumible> mochilaItems = new List<ItemConsumible>();
    public List<EquipoBase> armarioEquipo = new List<EquipoBase>();

    [Header("Inventario Antiguo")]
    public int plantasMedicinales;
    public int colaDeConejo;

    // --- PROPIEDADES AUTOMÁTICAS ---
    public int AtaqueTotal => fuerza + (armaEquipadaAsset != null ? armaEquipadaAsset.bonoAtaque : 0);
    
    public int DefensaTotal => defensa + 
                               (armaduraEquipadaAsset != null ? armaduraEquipadaAsset.bonoDefensa : 0) + 
                               (escudoEquipadoAsset != null ? escudoEquipadoAsset.bonoDefensa : 0) +
                               (cascoEquipadoAsset != null ? cascoEquipadoAsset.bonoDefensa : 0);

    public int AgilidadTotal => agilidad + (accesorioEquipadoAsset != null ? accesorioEquipadoAsset.bonoAgilidad : 0);

    public void EquiparColaDeConejo()
    {
        if (accesorioEquipado == "Cola de Conejo")
        {
            accesorioEquipado = "Ninguno";
            agilidad -= 2;
        }
        else 
        {
            accesorioEquipado = "Cola de Conejo";
            agilidad += 2;
        }
        #if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(this);
        #endif
    }

    public void EquiparObjeto(EquipoBase nuevoItem)
    {
        // Identifica qué tipo de equipo es y lo pone en su sitio
        if (nuevoItem.bonoAtaque > 0) armaEquipadaAsset = nuevoItem;
        else if (nuevoItem.bonoDefensa > 0) armaduraEquipadaAsset = nuevoItem;
        // Aquí podrías añadir más lógica para cascos o escudos
        
        #if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(this);
        #endif
    }

    [ContextMenu("Reiniciar a Nivel 1")]
    public void ReiniciarPersonaje()
    {
        nivel = 1; experiencia = 0; oro = 0;
        hpMax = 20; hpActual = 20;
        mpMax = 5; mpActual = 5;
        fuerza = 8; defensa = 2; agilidad = 6;
        fuerzaMagica = 5; terapeucidad = 4;
        plantasMedicinales = 0;
        colaDeConejo = 0;
        accesorioEquipado = "Ninguno";
        cascoEquipado = "Ninguno";
        
        armaEquipadaAsset = null;
        armaduraEquipadaAsset = null;
        escudoEquipadoAsset = null;
        cascoEquipadoAsset = null;
        accesorioEquipadoAsset = null;
        mochilaItems.Clear();
        armarioEquipo.Clear();
        
        if (tablaExpPilgrim.Length > 0) expSiguienteNivel = tablaExpPilgrim[0];

        #if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(this);
        #endif
    }
}