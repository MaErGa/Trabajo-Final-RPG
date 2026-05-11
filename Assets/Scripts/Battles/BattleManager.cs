using UnityEngine;
using TMPro; 
using UnityEngine.SceneManagement; 

public class BattleManager : MonoBehaviour
{
    [Header("Referencias Jugador")]
    public PlayerStats misStats;      
    public TextMeshProUGUI textoNombre; // NUEVO: Para mostrar el nombre (Ryo)
    public TextMeshProUGUI textoVida; 
    public TextMeshProUGUI textoNivel; 
    public TextMeshProUGUI textoMP;    

    [Header("Referencias Enemigo")]
    public GameObject elSlime;        
    public int vidaEnemigo = 20; 

    [Header("Interfaz")]
    public TextMeshProUGUI textoMensajes; 

    private bool esTurnoDelJugador = true;
    private bool estaDefendiendo = false;

    void Start()
    {
        ActualizarInterfaz();
        // Ahora el mensaje inicial también usa el nombre dinámico
        textoMensajes.text = "¡Un Slime salvaje aparece ante " + misStats.nombreJugador + "!";
    }

    // ... (Mantén las funciones AccionAtacar, AccionDefensa y AccionEscapar igual que antes)

    public void ActualizarInterfaz()
    {
        // Actualizamos el nombre que pusiste en PlayerStats
        if (textoNombre != null) textoNombre.text = misStats.nombreJugador;
        
        if (textoVida != null) textoVida.text = "HP " + misStats.pvActuales;
        if (textoNivel != null) textoNivel.text = "NV " + misStats.nivel;
        if (textoMP != null) textoMP.text = "MP " + misStats.mpActual;
    }

    // ... (Mantén el resto de funciones igual: GanarCombate, TurnoDelEnemigo, etc.)
}