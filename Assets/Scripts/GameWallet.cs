using UnityEngine;

/// <summary>
/// ================================================================
///  GAME WALLET — Monedero del Grupo
///  El oro pertenece al grupo completo, no a cada personaje.
///  Igual que en Dragon Quest y Final Fantasy.
///
///  Uso:
///    GameWallet.Instancia.GanarOro(50);
///    GameWallet.Instancia.GastarOro(30);
///    GameWallet.Instancia.Oro  → cantidad actual
/// ================================================================
/// </summary>
public class GameWallet : MonoBehaviour
{
    // ── Singleton ────────────────────────────────────────────────
    public static GameWallet Instancia { get; private set; }

    private void Awake()
    {
        if (Instancia != null && Instancia != this)
        {
            Destroy(gameObject);
            return;
        }
        Instancia = this;
        DontDestroyOnLoad(gameObject); // persiste entre escenas
    }

    // ════════════════════════════════════════════════════════════
    //  DATOS
    // ════════════════════════════════════════════════════════════

    [Header("Oro del Grupo")]
    [SerializeField] private int oro = 0;

    public int Oro => oro;

    // ════════════════════════════════════════════════════════════
    //  MÉTODOS
    // ════════════════════════════════════════════════════════════

    /// <summary>El grupo gana oro (al vencer enemigos, abrir cofres, etc.).</summary>
    public void GanarOro(int cantidad)
    {
        if (cantidad <= 0) return;
        oro += cantidad;
        Debug.Log($"[GameWallet] +{cantidad} oro. Total: {oro}");
    }

    /// <summary>
    /// El grupo gasta oro (tienda, posada, etc.).
    /// Devuelve true si había suficiente oro.
    /// </summary>
    public bool GastarOro(int cantidad)
    {
        if (cantidad <= 0) return false;

        if (oro < cantidad)
        {
            Debug.LogWarning($"[GameWallet] Oro insuficiente. Tiene {oro}, necesita {cantidad}.");
            return false;
        }

        oro -= cantidad;
        Debug.Log($"[GameWallet] -{cantidad} oro. Total: {oro}");
        return true;
    }

    /// <summary>Comprueba si el grupo puede pagar sin gastar el oro.</summary>
    public bool PuedePagar(int cantidad) => oro >= cantidad;

    /// <summary>Establece el oro directamente (útil al cargar partida guardada).</summary>
    public void EstablecerOro(int cantidad)
    {
        oro = Mathf.Max(0, cantidad);
        Debug.Log($"[GameWallet] Oro establecido a {oro}.");
    }

    // ════════════════════════════════════════════════════════════
    //  DEBUG
    // ════════════════════════════════════════════════════════════

    [ContextMenu("Imprimir Oro")]
    public void ImprimirOro() =>
        Debug.Log($"[GameWallet] Oro actual del grupo: {oro}");
}
