using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ================================================================
///  PLAYER STATS — JUGLAR
///  Motor de combate: Dragon Quest III (SNES)
///  Clase y stats:    Dragon Quest IX (Juglar / Minstrel)
/// ================================================================
///
///  FUENTES DE LAS FÓRMULAS
///  ──────────────────────────────────────────────────────────────
///  [DQ3-SNES] HP máx     = Resistencia × 2  (DQ3: 195%–205% de VIT)
///  [DQ3-SNES] MP máx     = F.Mágica × 2     (DQ3: 195%–205% de SAB)
///  [DQ3-SNES] Ataque     = (Fuerza + bonusAtaque_Arma) / 2
///  [DQ3-SNES] Defensa    = (Agilidad / 2) + suma bonusDefensa armaduras
///  [DQ3-SNES] Daño fís.  = (Ataque×2 − DEF_enemigo) × rand(0.90–1.10), mín 1
///  [DQ3-SNES] Crítico    = Ataque × rand(0.84–1.00), ignora DEF, base 1/64
///  [DQ3-SNES] Iniciativa = Agilidad × rand(25%–100%)
///  [DQ3-SNES] Estado alt.= (384 − Encanto) × MOD / 65536
///
///  [DQ9]      Crítico    = Pericia/100 + 3%  (base DQ9 con arma equipada)
///             El mayor valor entre [DQ3] y [DQ9] se usa como probabilidad final
///  [DQ9]      Curación   = base_hechizo × (1 + Terapeucidad / 200)
///             (inspirado en Magical Mending de DQ9: escala desde 200 en adelante)
/// </summary>
public class PlayerStats : MonoBehaviour
{
    // ════════════════════════════════════════════════════════════════
    //  STATS PRIMARIOS BASE (Juglar, Nivel 1)
    // ════════════════════════════════════════════════════════════════

    [Header("Stats Primarios Base — Nivel 1")]
    [Tooltip("Fuerza. Determina el daño físico: (Fuerza + Arma) / 2 = Ataque")]
    [SerializeField] private int baseFuerza        = 9;

    [Tooltip("Agilidad. Contribuye a la Defensa (AGI/2) y al orden de turno.")]
    [SerializeField] private int baseAgilidad      = 8;

    [Tooltip("Resistencia. HP máximo ≈ Resistencia × 2")]
    [SerializeField] private int baseResistencia   = 8;

    [Tooltip("Pericia. Aumenta la probabilidad de golpe crítico: Pericia/100 + 3%")]
    [SerializeField] private int basePericia       = 12;

    [Tooltip("Encanto. Reduce la probabilidad de sufrir estados alterados.")]
    [SerializeField] private int baseEncanto       = 9;

    [Tooltip("Terapeucidad. Aumenta la potencia de los hechizos curativos.")]
    [SerializeField] private int baseTerapeucidad  = 7;

    [Tooltip("Fuerza Mágica. MP máximo ≈ F.Mágica × 2")]
    [SerializeField] private int baseFuerzaMagica  = 6;

    [Tooltip("Estilo. Usado en concursos de estilo, no afecta combate.")]
    [SerializeField] private int baseEstilo        = 9;

    // ════════════════════════════════════════════════════════════════
    //  NIVEL Y EXPERIENCIA
    // ════════════════════════════════════════════════════════════════

    [Header("Nivel")]
    [SerializeField] private int nivelActual       = 1;
    [SerializeField] private int expActual         = 0;

    /// <summary>XP necesaria para el siguiente nivel.</summary>
    private int ExpSiguienteNivel => 100 * nivelActual * nivelActual;

    // ════════════════════════════════════════════════════════════════
    //  STATS ACTUALES (crecen con el nivel)
    // ════════════════════════════════════════════════════════════════

    private int fuerza;
    private int agilidad;
    private int resistencia;
    private int pericia;
    private int encanto;
    private int terapeucidad;
    private int fuerzaMagica;
    private int estilo;

    // ════════════════════════════════════════════════════════════════
    //  EQUIPAMIENTO
    // ════════════════════════════════════════════════════════════════

    [Header("Equipamiento")]
    [SerializeField] private EquipmentData ranuraArma;
    [SerializeField] private EquipmentData ranuraEscudo;
    [SerializeField] private EquipmentData ranuraCabeza;
    [SerializeField] private EquipmentData ranuraCuerpo;
    [SerializeField] private EquipmentData ranuraCalzado;

    // ════════════════════════════════════════════════════════════════
    //  ESTADO EN COMBATE
    // ════════════════════════════════════════════════════════════════

    private int pvActuales;
    private int pmActuales;

    // ════════════════════════════════════════════════════════════════
    //  PROPIEDADES PÚBLICAS
    // ════════════════════════════════════════════════════════════════

    public int Nivel            => nivelActual;
    public int Experiencia      => expActual;
    public int ExpParaSiguiente => ExpSiguienteNivel;

    // Stats primarios con bonus de equipo
    public int Fuerza        => fuerza       + BonusEquipo(e => e.bonusStrength);
    public int Agilidad      => agilidad     + BonusEquipo(e => e.bonusAgility);
    public int Resistencia   => resistencia  + BonusEquipo(e => e.bonusVitality);
    public int Pericia       => pericia      + BonusEquipo(e => e.bonusSkill);
    public int Encanto       => encanto      + BonusEquipo(e => e.bonusLuck);
    public int Terapeucidad  => terapeucidad + BonusEquipo(e => e.bonusTherapeutics);
    public int FuerzaMagica  => fuerzaMagica + BonusEquipo(e => e.bonusWisdom);
    public int Estilo        => estilo       + BonusEquipo(e => e.bonusStyle);

    // ── Stats derivados [fórmulas DQ3 SNES] ──────────────────────────

    /// <summary>[DQ3] HP máximo = Resistencia × 2</summary>
    public int PVMaximos => Resistencia * 2;

    /// <summary>[DQ3] MP máximo = Fuerza Mágica × 2</summary>
    public int PMMaximos => FuerzaMagica * 2;

    /// <summary>[DQ3] Ataque = (Fuerza + potencia arma equipada) / 2</summary>
    public int Ataque => (Fuerza + BonusEquipo(e => e.bonusAttack)) / 2;

    /// <summary>
    /// [DQ3] Defensa = (Agilidad / 2) + suma de bonusDefensa de toda la armadura.
    /// La Agilidad contribuye directamente a la defensa base en DQ3 SNES.
    /// </summary>
    public int Defensa => (Agilidad / 2) + BonusEquipo(e => e.bonusDefense);

    /// <summary>
    /// [DQ9] Probabilidad de golpe crítico = Pericia/100 + 3%
    /// Se compara con la probabilidad base DQ3 (1/64 ≈ 1.56%) y se usa la mayor.
    /// </summary>
    public float ProbabilidadCritico
    {
        get
        {
            float critDQ9 = (Pericia / 100f) + 0.03f;   // Pericia/100 + 3%  [DQ9]
            float critDQ3 = 1f / 64f;                    // ~1.56%            [DQ3 base]
            return Mathf.Max(critDQ9, critDQ3);
        }
    }

    public int PVActuales => pvActuales;
    public int PMActuales => pmActuales;
    public bool EstaVivo  => pvActuales > 0;

    // ════════════════════════════════════════════════════════════════
    //  UNITY LIFECYCLE
    // ════════════════════════════════════════════════════════════════

    private void Awake()
    {
        InicializarStats();
        pvActuales = PVMaximos;
        pmActuales = PMMaximos;
    }

    private void InicializarStats()
    {
        fuerza       = baseFuerza;
        agilidad     = baseAgilidad;
        resistencia  = baseResistencia;
        pericia      = basePericia;
        encanto      = baseEncanto;
        terapeucidad = baseTerapeucidad;
        fuerzaMagica = baseFuerzaMagica;
        estilo       = baseEstilo;
    }

    // ════════════════════════════════════════════════════════════════
    //  COMBATE — ATAQUE
    // ════════════════════════════════════════════════════════════════

    /// <summary>
    /// Calcula el daño físico de este personaje contra un enemigo.
    ///
    /// [DQ3 SNES] base = Ataque×2 − DEF_enemigo
    ///            varianza ±10%,  mínimo 1
    /// [DQ9]      probabilidad crítico = Pericia/100 + 3%
    /// </summary>
    public int CalcularDañoFisico(int defensaEnemigo)
    {
        // ── Comprueba golpe crítico con probabilidad basada en Pericia [DQ9] ──
        if (Random.value < ProbabilidadCritico)
            return CalcularDañoCritico();

        // ── Daño normal [DQ3 SNES] ──────────────────────────────────────────
        int dañoBase  = Mathf.Max(0, Ataque * 2 - defensaEnemigo);
        float factor  = Random.Range(0.90f, 1.10f);
        int dañoFinal = Mathf.Max(1, Mathf.FloorToInt(dañoBase * factor));

        Debug.Log($"[PlayerStats] Ataque normal → base:{dañoBase} × {factor:F2} = {dañoFinal}");
        return dañoFinal;
    }

    /// <summary>
    /// [DQ3 SNES] Golpe crítico: Ataque × rand(84%–100%), ignora DEF enemigo.
    /// La probabilidad de activarse usa la fórmula DQ9 basada en Pericia.
    /// </summary>
    public int CalcularDañoCritico()
    {
        float factor  = Random.Range(0.84f, 1.00f);
        int   critico = Mathf.Max(1, Mathf.FloorToInt(Ataque * factor));
        Debug.Log($"[PlayerStats] ¡CRÍTICO! ({ProbabilidadCritico*100:F1}% prob) " +
                  $"Ataque:{Ataque} × {factor:F2} = {critico} (ignora DEF)");
        return critico;
    }

    // ════════════════════════════════════════════════════════════════
    //  COMBATE — RECIBIR DAÑO
    // ════════════════════════════════════════════════════════════════

    /// <summary>
    /// Recibe daño físico de un enemigo.
    /// [DQ3 SNES] base = ATQ_enemigo×2 − Defensa, varianza ±10%, mínimo 1
    /// </summary>
    public void RecibirDaño(int ataqueEnemigo)
    {
        int   dañoBase  = Mathf.Max(0, ataqueEnemigo * 2 - Defensa);
        float factor    = Random.Range(0.90f, 1.10f);
        int   daño      = Mathf.Max(1, Mathf.FloorToInt(dañoBase * factor));

        pvActuales = Mathf.Max(0, pvActuales - daño);
        Debug.Log($"[PlayerStats] Recibió {daño} daño físico. PV: {pvActuales}/{PVMaximos}");

        if (!EstaVivo) AlMorir();
    }

    /// <summary>
    /// Recibe daño mágico (ignora Defensa física).
    /// </summary>
    public void RecibirDañoMagico(int daño)
    {
        int tomado = Mathf.Max(1, daño);
        pvActuales = Mathf.Max(0, pvActuales - tomado);
        Debug.Log($"[PlayerStats] Recibió {tomado} daño mágico. PV: {pvActuales}/{PVMaximos}");

        if (!EstaVivo) AlMorir();
    }

    // ════════════════════════════════════════════════════════════════
    //  CURACIÓN — CON TERAPEUCIDAD [DQ9 Magical Mending]
    // ════════════════════════════════════════════════════════════════

    /// <summary>
    /// Cura PV con un hechizo.
    /// [DQ9] potencia real = baseCuración × (1 + Terapeucidad / 200)
    ///       La Terapeucidad empieza a escalar desde 1 y va subiendo linealmente.
    ///       A Terapeucidad 200 la curación es el doble del valor base.
    /// </summary>
    public int CurarConHechizo(int baseCuracion)
    {
        float multiplicador = 1f + (Terapeucidad / 200f);
        int   curacionFinal = Mathf.FloorToInt(baseCuracion * multiplicador);

        pvActuales = Mathf.Min(PVMaximos, pvActuales + curacionFinal);
        Debug.Log($"[PlayerStats] Curó {curacionFinal} PV con hechizo " +
                  $"(base:{baseCuracion} × {multiplicador:F2} por Terapeucidad:{Terapeucidad}). " +
                  $"PV: {pvActuales}/{PVMaximos}");
        return curacionFinal;
    }

    /// <summary>Restaura PV directamente (pociones, descanso, etc.). No usa Terapeucidad.</summary>
    public void CurarPV(int cantidad)
    {
        pvActuales = Mathf.Min(PVMaximos, pvActuales + cantidad);
        Debug.Log($"[PlayerStats] Curó {cantidad} PV. PV: {pvActuales}/{PVMaximos}");
    }

    /// <summary>Restaura PM directamente.</summary>
    public void CurarPM(int cantidad)
    {
        pmActuales = Mathf.Min(PMMaximos, pmActuales + cantidad);
        Debug.Log($"[PlayerStats] Restauró {cantidad} PM. PM: {pmActuales}/{PMMaximos}");
    }

    /// <summary>Consume PM para un hechizo. Devuelve true si hay PM suficientes.</summary>
    public bool GastarPM(int coste)
    {
        if (pmActuales < coste)
        {
            Debug.LogWarning("[PlayerStats] PM insuficientes.");
            return false;
        }
        pmActuales -= coste;
        Debug.Log($"[PlayerStats] Gastó {coste} PM. PM: {pmActuales}/{PMMaximos}");
        return true;
    }

    // ════════════════════════════════════════════════════════════════
    //  INICIATIVA / ORDEN DE TURNO [DQ3 SNES]
    // ════════════════════════════════════════════════════════════════

    /// <summary>
    /// [DQ3 SNES] Iniciativa = Agilidad × rand(25%–100%)
    /// Quien tenga mayor iniciativa actúa primero en el turno.
    /// </summary>
    public int TirarIniciativa()
    {
        float factor = Random.Range(0.25f, 1.00f);
        return Mathf.FloorToInt(Agilidad * factor);
    }

    // ════════════════════════════════════════════════════════════════
    //  RESISTENCIA A ESTADOS ALTERADOS [DQ3 SNES + Encanto]
    // ════════════════════════════════════════════════════════════════

    /// <summary>
    /// [DQ3 SNES] Probabilidad de sufrir un estado alterado:
    ///   P = (384 − Encanto) × MOD / 65536
    ///   MOD varía por tipo de estado (valores típicos: 160–240).
    /// Devuelve true si el estado TIENE EFECTO (no fue resistido).
    /// </summary>
    public bool ComprobarEstadoAlterado(int mod = 192)
    {
        float probabilidad = Mathf.Clamp01((384f - Encanto) * mod / 65536f);
        bool  afectado     = Random.value < probabilidad;
        Debug.Log($"[PlayerStats] Resistencia estado: P={probabilidad:F3} " +
                  $"(Encanto:{Encanto}) → {(afectado ? "AFECTADO" : "RESISTIDO")}");
        return afectado;
    }

    // ════════════════════════════════════════════════════════════════
    //  NIVEL Y EXPERIENCIA
    // ════════════════════════════════════════════════════════════════

    /// <summary>Añade XP y sube de nivel si corresponde.</summary>
    public void GanarExperiencia(int cantidad)
    {
        expActual += cantidad;
        Debug.Log($"[PlayerStats] +{cantidad} XP → {expActual}/{ExpSiguienteNivel}");

        while (expActual >= ExpSiguienteNivel)
        {
            expActual -= ExpSiguienteNivel;
            SubirNivel();
        }
    }

    private void SubirNivel()
    {
        nivelActual++;

        // ── Crecimiento de stats [DQ9 Juglar: clase equilibrada, todos los ──
        // ── stats suben cada nivel con rangos ajustados a su perfil]        ──
        int ganFuerza       = Random.Range(1, 4);   // +1~3  (combate moderado)
        int ganAgilidad     = Random.Range(2, 5);   // +2~4  (clase ágil)
        int ganResistencia  = Random.Range(1, 4);   // +1~3
        int ganPericia      = Random.Range(2, 5);   // +2~4  (alta destreza)
        int ganEncanto      = Random.Range(1, 4);   // +1~3
        int ganTerapeucidad = Random.Range(1, 3);   // +1~2  (soporte ligero)
        int ganFuerzaMagica = Random.Range(1, 3);   // +1~2
        int ganEstilo       = Random.Range(1, 4);   // +1~3

        fuerza       += ganFuerza;
        agilidad     += ganAgilidad;
        resistencia  += ganResistencia;
        pericia      += ganPericia;
        encanto      += ganEncanto;
        terapeucidad += ganTerapeucidad;
        fuerzaMagica += ganFuerzaMagica;
        estilo       += ganEstilo;

        // PV y PM se recalculan automáticamente desde Resistencia y F.Mágica
        pvActuales = PVMaximos;
        pmActuales = PMMaximos;

        Debug.Log($"[PlayerStats] ¡NIVEL {nivelActual}! PV restaurados: {pvActuales}/{PVMaximos}");
        ImprimirStats();
    }

    // ════════════════════════════════════════════════════════════════
    //  GESTIÓN DE EQUIPAMIENTO
    // ════════════════════════════════════════════════════════════════

    /// <summary>Equipa una pieza en su ranura correspondiente.</summary>
    public void Equipar(EquipmentData equipo)
    {
        if (equipo == null) return;
        switch (equipo.slot)
        {
            case EquipmentSlot.Weapon:   ranuraArma    = equipo; break;
            case EquipmentSlot.Shield:   ranuraEscudo  = equipo; break;
            case EquipmentSlot.Head:     ranuraCabeza  = equipo; break;
            case EquipmentSlot.Body:     ranuraCuerpo  = equipo; break;
            case EquipmentSlot.Footwear: ranuraCalzado = equipo; break;
        }
        Debug.Log($"[PlayerStats] Equipado: {equipo}  ATQ:{Ataque}  DEF:{Defensa}");
    }

    /// <summary>Desequipa la ranura indicada.</summary>
    public void Desequipar(EquipmentSlot ranura)
    {
        switch (ranura)
        {
            case EquipmentSlot.Weapon:   ranuraArma    = null; break;
            case EquipmentSlot.Shield:   ranuraEscudo  = null; break;
            case EquipmentSlot.Head:     ranuraCabeza  = null; break;
            case EquipmentSlot.Body:     ranuraCuerpo  = null; break;
            case EquipmentSlot.Footwear: ranuraCalzado = null; break;
        }
        Debug.Log($"[PlayerStats] Ranura {ranura} vaciada.");
    }

    // ════════════════════════════════════════════════════════════════
    //  HELPERS PRIVADOS
    // ════════════════════════════════════════════════════════════════

    private int BonusEquipo(System.Func<EquipmentData, int> selector)
    {
        int total = 0;
        if (ranuraArma    != null) total += selector(ranuraArma);
        if (ranuraEscudo  != null) total += selector(ranuraEscudo);
        if (ranuraCabeza  != null) total += selector(ranuraCabeza);
        if (ranuraCuerpo  != null) total += selector(ranuraCuerpo);
        if (ranuraCalzado != null) total += selector(ranuraCalzado);
        return total;
    }

    private void AlMorir()
    {
        Debug.Log("[PlayerStats] El Juglar ha caído en batalla.");
        // Aquí: animación de muerte, pantalla de game over, etc.
    }

    // ════════════════════════════════════════════════════════════════
    //  DEBUG
    // ════════════════════════════════════════════════════════════════

    [ContextMenu("Imprimir Stats")]
    public void ImprimirStats()
    {
        Debug.Log(
            $"══════════════════════════════════════\n" +
            $"  JUGLAR  |  Nivel {nivelActual}\n" +
            $"  XP: {expActual}/{ExpSiguienteNivel}\n" +
            $"══════════════════════════════════════\n" +
            $"  Fuerza:        {Fuerza}\n" +
            $"  Agilidad:      {Agilidad}\n" +
            $"  Resistencia:   {Resistencia}\n" +
            $"  Pericia:       {Pericia}  (crítico: {ProbabilidadCritico*100:F1}%)\n" +
            $"  Encanto:       {Encanto}\n" +
            $"  Terapeucidad:  {Terapeucidad}  (×{1f + Terapeucidad/200f:F2} en curación)\n" +
            $"  Fuerza Mágica: {FuerzaMagica}\n" +
            $"  Estilo:        {Estilo}\n" +
            $"──────────────────────────────────────\n" +
            $"  ATQ: {Ataque}   DEF: {Defensa}\n" +
            $"  PV:  {pvActuales}/{PVMaximos}\n" +
            $"  PM:  {pmActuales}/{PMMaximos}\n" +
            $"══════════════════════════════════════"
        );
    }

    [ContextMenu("Simular Ataque al Enemigo (DEF=10)")]
    private void DebugAtaque() =>
        Debug.Log($"[DEBUG] Daño al enemigo con DEF 10: {CalcularDañoFisico(10)}");

    [ContextMenu("Simular Hechizo de Curación (base 20 PV)")]
    private void DebugCuracion() =>
        Debug.Log($"[DEBUG] Curación con base 20: {Mathf.FloorToInt(20 * (1f + Terapeucidad / 200f))} PV");
}
