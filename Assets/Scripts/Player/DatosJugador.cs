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
    [SerializeField] private int _oro;
    public int oro
    {
        get => _oro;
        set => _oro = Mathf.Clamp(value, 0, 99999);
    }
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

    [Header("Equipación Inicial (se asigna al reiniciar)")]
    public EquipoBase armaInicial;      // Porra de Cipres
    public EquipoBase armaduraInicial;  // Vestimenta de Viaje

    [Header("Equipación (Nombres Antiguos)")]
    public string armaEquipada = "Porra de Cipres";
    public string armaduraEquipada = "Vestimenta de Viaje";
    public string escudoEquipado = "Ninguno";
    public string cascoEquipado = "Ninguno";
    public string accesorioEquipado = "Ninguno";

    [Header("Sistema de Niveles")]
    public int expSiguienteNivel = 14;
    public int[] tablaExpPilgrim = {
        11, 22, 44, 66, 99, 148, 222, 334, 501, 751,
        1126, 1408, 1760, 2200, 2750, 3437, 4296, 5370, 6713, 7552,
        8496, 9557, 10751, 12095, 13608, 15306, 17221, 19373, 21795, 24520,
        27582, 31031, 34910, 39274, 43969, 48920, 54230, 59880, 65890, 65535,
        65535, 65535, 65535, 65535, 65535, 65535, 65535, 65535, 65535, 65535,
        65535, 65535, 65535, 65535, 65535, 65535, 65535, 65535, 65535, 65535,
        65535, 65535, 65535, 65535, 65535, 65535, 65535, 65535, 65535, 65535,
        65535, 65535, 65535, 65535, 65535, 65535, 65535, 65535, 65535, 65535,
        65535, 65535, 65535, 65535, 65535, 65535, 65535, 65535, 65535, 65535,
        65535, 65535, 65535, 65535, 65535, 65535, 65535, 65535, 65535
    };

    [Header("Conjuros Aprendidos (se actualiza automático)")]
    public List<ConjuroBase> conjurosAprendidos = new List<ConjuroBase>();

    [Header("Conjuros por Nivel (asigna en el Inspector)")]
    public ConjuroBase conjuroNivel3;
    public ConjuroBase conjuroNivel5;
    public ConjuroBase conjuroNivel8;
    public ConjuroBase conjuroNivel10; // ¡AÑADIDO! Ranura para Miniincendio

    [Header("Inventario Dinámico")]
    public List<ItemConsumible> mochilaItems = new List<ItemConsumible>();
    public List<EquipoBase> armarioEquipo = new List<EquipoBase>();

    [Header("Inventario Antiguo")]
    public int plantasMedicinales;
    public int colaDeConejo;
    public int eter;

    // --- PROPIEDADES CON CÁLCULO DE BONOS ---
    public int AtaqueTotal => fuerza + bonoAtaqueTemporal +
                               (armaEquipadaAsset != null ? armaEquipadaAsset.bonoAtaque : 0);

    public int DefensaTotal => defensa + bonoDefensaTemporal +
                               (armaduraEquipadaAsset != null ? armaduraEquipadaAsset.bonoDefensa : 0) +
                               (escudoEquipadoAsset != null ? escudoEquipadoAsset.bonoDefensa : 0) +
                               (cascoEquipadoAsset != null ? cascoEquipadoAsset.bonoDefensa : 0);

    public int AgilidadTotal => agilidad + bonoAgilidadTemporal +
                                (accesorioEquipadoAsset != null ? accesorioEquipadoAsset.bonoAgilidad : 0);

    
    // --- SISTEMA DE ESCALADO AUTOMÁTICO ---
    private void OnValidate()
    {
        ActualizarEstadisticasPorNivel();
    }

    /// <summary>
    /// Modifica los atributos BASE del jugador usando interpolación matemática lineal (Lerp)
    /// </summary>
    public void ActualizarEstadisticasPorNivel()
    {
        nivel = Mathf.Clamp(nivel, 1, 99);
        float t = (nivel - 1) / 98f;   // 0 en nivel 1, 1 en nivel 99

        hpMax = Mathf.RoundToInt(Mathf.Lerp(20, 999, t));
        mpMax = Mathf.RoundToInt(Mathf.Lerp(5, 500, t));

        fuerza   = Mathf.RoundToInt(Mathf.Lerp(8,  255, t));
        agilidad = Mathf.RoundToInt(Mathf.Lerp(6,  200, t));
        defensa  = Mathf.RoundToInt(Mathf.Lerp(2,  200, t));

        if (tablaExpPilgrim != null && (nivel - 1) < tablaExpPilgrim.Length)
            expSiguienteNivel = tablaExpPilgrim[nivel - 1];

        AprenderConjurosPorNivel();
    }

    // --- CONJUROS ---
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
        // ¡AÑADIDO! Condición para aprender el conjuro de nivel 10
        if (nivel >= 10 && conjuroNivel10 != null && !conjurosAprendidos.Contains(conjuroNivel10))
        {
            conjurosAprendidos.Add(conjuroNivel10);
            mensaje += "\n¡Has aprendido " + conjuroNivel10.nombreConjuro + "!";
        }

        return mensaje;
    }

    // --- MÉTODOS DE CONTROL ---

    [ContextMenu("Resetear Tabla EXP a valores correctos")]
    public void ResetearTablaExp()
    {
        tablaExpPilgrim = new int[] {
            11, 22, 44, 66, 99, 148, 222, 334, 501, 751,
            1126, 1408, 1760, 2200, 2750, 3437, 4296, 5370, 6713, 7552,
            8496, 9557, 10751, 12095, 13608, 15306, 17221, 19373, 21795, 24520,
            27582, 31031, 34910, 39274, 43969, 48920, 54230, 59880, 65890, 65535,
            65535, 65535, 65535, 65535, 65535, 65535, 65535, 65535, 65535, 65535,
            65535, 65535, 65535, 65535, 65535, 65535, 65535, 65535, 65535, 65535,
            65535, 65535, 65535, 65535, 65535, 65535, 65535, 65535, 65535, 65535,
            65535, 65535, 65535, 65535, 65535, 65535, 65535, 65535, 65535, 65535,
            65535, 65535, 65535, 65535, 65535, 65535, 65535, 65535, 65535, 65535,
            65535, 65535, 65535, 65535, 65535, 65535, 65535, 65535, 65535
        };
        ActualizarEstadisticasPorNivel();
        #if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(this);
        #endif
        Debug.Log("Tabla EXP actualizada a 99 niveles.");
    }

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

    public void SubirNivelYCurar()
    {
        if (nivel < 99)
        {
            nivel++;
            ActualizarEstadisticasPorNivel();
            hpActual = hpMax;
            mpActual = mpMax;
            #if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(this);
            #endif
        }
    }

    [ContextMenu("DEBUG - Subir a Nivel 99")]
    public void SubirANivel99()
    {
        nivel = 99;
        experiencia = 4463783;
        ActualizarEstadisticasPorNivel();
        hpActual = hpMax;
        mpActual = mpMax;
        #if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(this);
        #endif
        Debug.Log("¡Nivel 99 alcanzado!");
    }

    [ContextMenu("DEBUG - Subir a Nivel 50")]
    public void SubirANivel50()
    {
        nivel = 50;
        experiencia = 1252568;
        ActualizarEstadisticasPorNivel();
        hpActual = hpMax;
        mpActual = mpMax;
        #if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(this);
        #endif
        Debug.Log("¡Nivel 50 alcanzado!");
    }

    [ContextMenu("DEBUG - Subir a Nivel 20")]
    public void SubirANivel20()
    {
        nivel = 20;
        experiencia = 31258;
        ActualizarEstadisticasPorNivel();
        hpActual = hpMax;
        mpActual = mpMax;
        #if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(this);
        #endif
        Debug.Log("¡Nivel 20 alcanzado!");
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

        armaEquipadaAsset      = armaInicial;
        armaduraEquipadaAsset  = armaduraInicial;
        escudoEquipadoAsset    = null;
        cascoEquipadoAsset     = null;
        accesorioEquipadoAsset = null;

        armaEquipada     = armaInicial     != null ? armaInicial.nombre     : "Ninguno";
        armaduraEquipada = armaduraInicial != null ? armaduraInicial.nombre : "Ninguno";
        escudoEquipado   = "Ninguno";

        mochilaItems.Clear();
        armarioEquipo.Clear();

        if (tablaExpPilgrim.Length > 0) expSiguienteNivel = tablaExpPilgrim[0];

        #if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(this);
        #endif
    }
}