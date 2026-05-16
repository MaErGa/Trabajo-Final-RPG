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

    [Header("Bonos Temporales (Combate)")]
    public int bonoDefensaTemporal;
    public int bonoAtaqueTemporal;
    public int bonoAgilidadTemporal;

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

    [Header("Conjuros Aprendidos (se actualiza automático)")]
    public List<ConjuroBase> conjurosAprendidos = new List<ConjuroBase>();

    [Header("Conjuros por Nivel (asigna en el Inspector)")]
    public ConjuroBase conjuroNivel3;  // Minicuración
    public ConjuroBase conjuroNivel5;  // Fortalecimiento
    public ConjuroBase conjuroNivel8;  // Minihelada

    [Header("Inventario Dinámico")]
    public List<ItemConsumible> mochilaItems = new List<ItemConsumible>();
    public List<EquipoBase> armarioEquipo = new List<EquipoBase>();

    [Header("Inventario Antiguo")]
    public int plantasMedicinales;
    public int colaDeConejo;

    // --- PROPIEDADES CON CÁLCULO DE BONOS ---
    public int AtaqueTotal => fuerza + bonoAtaqueTemporal +
                               (armaEquipadaAsset != null ? armaEquipadaAsset.bonoAtaque : 0);

    public int DefensaTotal => defensa + bonoDefensaTemporal +
                               (armaduraEquipadaAsset != null ? armaduraEquipadaAsset.bonoDefensa : 0) +
                               (escudoEquipadoAsset != null ? escudoEquipadoAsset.bonoDefensa : 0) +
                               (cascoEquipadoAsset != null ? cascoEquipadoAsset.bonoDefensa : 0);

    public int AgilidadTotal => agilidad + bonoAgilidadTemporal +
                                (accesorioEquipadoAsset != null ? accesorioEquipadoAsset.bonoAgilidad : 0);

    // --- CONJUROS: se aprenden automáticamente al subir de nivel ---
    public string AprenderConjurosPorNivel()
    {
        string mensaje = "";

        if (nivel >= 3 && conjuroNivel3 != null && !conjurosAprendidos.Contains(conjuroNivel3))
        {
            conjurosAprendidos.Add(conjuroNivel3);
            mensaje += "\n¡Has aprendido " + conjuroNivel3.nombreConjuro + "!";
        }
        if (nivel >= 5 && conjuroNivel5 != null && !conjurosAprendidos.Contains(conjuroNivel5))
        {
            conjurosAprendidos.Add(conjuroNivel5);
            mensaje += "\n¡Has aprendido " + conjuroNivel5.nombreConjuro + "!";
        }
        if (nivel >= 8 && conjuroNivel8 != null && !conjurosAprendidos.Contains(conjuroNivel8))
        {
            conjurosAprendidos.Add(conjuroNivel8);
            mensaje += "\n¡Has aprendido " + conjuroNivel8.nombreConjuro + "!";
        }

        return mensaje;
    }

    // --- MÉTODOS DE CONTROL ---

    [ContextMenu("Limpiar Bonos Mágicos")]
    public void ResetearBonos()
    {
        bonoDefensaTemporal = 0;
        bonoAtaqueTemporal = 0;
        bonoAgilidadTemporal = 0;
    }

    private void OnEnable()
    {
        ResetearBonos();
    }

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
        switch (nuevoItem.tipoSlot)
        {
            case TipoSlot.Arma:      armaEquipadaAsset      = nuevoItem; break;
            case TipoSlot.Armadura:  armaduraEquipadaAsset  = nuevoItem; break;
            case TipoSlot.Escudo:    escudoEquipadoAsset    = nuevoItem; break;
            case TipoSlot.Casco:     cascoEquipadoAsset     = nuevoItem; break;
            case TipoSlot.Accesorio: accesorioEquipadoAsset = nuevoItem; break;
        }

        #if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(this);
        #endif
    }

    [ContextMenu("Reiniciar a Nivel 1")]
    public void ReiniciarPersonaje()
    {
        nivel = 1; experiencia = 0; oro = 50;
        hpMax = 20; hpActual = 20;
        mpMax = 5; mpActual = 5;
        fuerza = 8; defensa = 2; agilidad = 6;
        fuerzaMagica = 5; terapeucidad = 4;

        ResetearBonos();
        conjurosAprendidos.Clear();

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