using UnityEngine;
using TMPro; 
using UnityEngine.SceneManagement; 

public class BattleManager : MonoBehaviour
{
    [Header("Referencias Jugador")]
    public PlayerStats misStats;      
    public TextMeshProUGUI textoNombre; // NUEVO: Hueco para el nombre
    public TextMeshProUGUI textoVida; 
    public TextMeshProUGUI textoNivel; 
    public TextMeshProUGUI textoMP;    

    [Header("Referencias Enemigo")]
    public GameObject elSlime;        
    public int vidaEnemigo = 20; 

    [Header("Interfaz")]
    public TextMeshProUGUI textoMensajes; 

    private bool esTurnoDelJugador = true;
    private bool estaDefendiendo = false; // Añadido para que funcionen los botones de antes

    void Start()
    {
        ActualizarInterfaz();
        // Usamos el nombre dinámico en el mensaje inicial
        textoMensajes.text = "¡Un Slime salvaje aparece ante " + misStats.nombreJugador + "!";
    }

    public void AccionAtacar()
    {
        if (!esTurnoDelJugador || vidaEnemigo <= 0) return;

        vidaEnemigo -= misStats.ataque; 
        textoMensajes.text = "¡Atacas al Slime!";
        
        elSlime.transform.position += new Vector3(0.2f, 0, 0);
        Invoke("ResetearPosicionSlime", 0.1f);

        if (vidaEnemigo <= 0)
        {
            GanarCombate();
        }
        else
        {
            esTurnoDelJugador = false;
            Invoke("TurnoDelEnemigo", 1.2f);
        }
    }

    // Funciones de Defensa y Escapar para que no dejen de funcionar
    public void AccionDefensa()
    {
        if (!esTurnoDelJugador || vidaEnemigo <= 0) return;
        estaDefendiendo = true;
        textoMensajes.text = misStats.nombreJugador + " se pone en guardia.";
        esTurnoDelJugador = false;
        Invoke("TurnoDelEnemigo", 1.2f);
    }

    public void AccionEscapar()
    {
        if (!esTurnoDelJugador || vidaEnemigo <= 0) return;
        textoMensajes.text = "¡Has escapado con éxito!";
        Invoke("VolverAlMapa", 1.2f);
    }

    void GanarCombate()
    {
        vidaEnemigo = 0;
        elSlime.SetActive(false); 

        int expGanada = 20;
        int oroGanado = 10;
        misStats.experiencia += expGanada;
        misStats.oro += oroGanado;

        textoMensajes.text = "¡Slime derrotado! Ganas " + expGanada + " EXP y " + oroGanado + " monedas.";
        
        CancelInvoke("TurnoDelEnemigo");
        Invoke("VolverAlMapa", 2.5f); 
    }

    void TurnoDelEnemigo()
    {
        if (vidaEnemigo <= 0) return;

        int dañoFinal = 4;
        if (estaDefendiendo)
        {
            dañoFinal = 1;
            estaDefendiendo = false;
            textoMensajes.text = "¡Te defiendes del golpe!";
        }
        else
        {
            textoMensajes.text = "¡El Slime embiste!";
        }

        misStats.pvActuales -= dañoFinal;
        ActualizarInterfaz();

        if (misStats.pvActuales <= 0)
        {
            textoMensajes.text = "Has caído...";
        }
        else
        {
            esTurnoDelJugador = true;
            Invoke("MensajeTurno", 1f);
        }
    }

    void MensajeTurno() { textoMensajes.text = "¿Qué harás ahora?"; }

    public void ActualizarInterfaz()
    {
        // Actualizamos el nombre en la pantalla
        if (textoNombre != null) textoNombre.text = misStats.nombreJugador;
        
        if (textoVida != null) textoVida.text = "HP " + misStats.pvActuales;
        if (textoNivel != null) textoNivel.text = "NV " + misStats.nivel;
        if (textoMP != null) textoMP.text = "MP " + misStats.mpActual;
    }

    void ResetearPosicionSlime() => elSlime.transform.position -= new Vector3(0.2f, 0, 0);
    void VolverAlMapa() => SceneManager.LoadScene("Underworld");
}