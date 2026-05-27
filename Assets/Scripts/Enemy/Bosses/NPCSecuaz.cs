using UnityEngine;

public class NPCSecuaz : MonoBehaviour
{
    [Header("Distancia para interactuar")]
    public float distancia = 2f;

    [Header("Enemigo del combate")]
    public DatosEnemigo datosEnemigo;

    private Transform jugador;

    // Static: persiste entre escenas
    public static bool secuazDerrotado = false;

    private string[] dialogoEncuentro = new string[]
    {
        "Eh, tu! Para el carro, forastero!",
        "Pensabas que ibas a llegar hasta el jefe sin pasar por mi? Que ingenuo!",
        "Soy Grock, el guardaespaldas personal del gran Robbin Odd.",
        "Y tengo ordenes estrictas de no dejar pasar a nadie... especialmente a los que tienen pinta de heroe!",
        "Quieres ver al jefe? Primero tendras que derrotarme a mi. Preparate!"
    };

    private string[] dialogoYaDerrotado = new string[]
    {
        "...",
        "Ya me derrotaste. El jefe esta ahi dentro. No te envidio."
    };

    void Start()
    {
        GameObject obj = GameObject.FindGameObjectWithTag("Player");
        if (obj != null) jugador = obj.transform;

        // Si ya fue derrotado, desaparecer directamente sin resetear
        if (secuazDerrotado)
        {
            gameObject.SetActive(false);
        }
    }

    void Update()
    {
        if (jugador == null) return;
        if (secuazDerrotado) return;

        float dist = Vector2.Distance(transform.position, jugador.position);
        if (dist > distancia) return;
        if (!Input.GetKeyDown(KeyCode.X)) return;
        if (DialogoManagerBoss.instancia != null && DialogoManagerBoss.instancia.EstaActivo()) return;

        DialogoManagerBoss.instancia.MostrarDialogo(dialogoEncuentro, IniciarCombate);
    }

    void IniciarCombate()
    {
        if (datosEnemigo == null) return;
        MovimientoMapa.enemigoSeleccionado = datosEnemigo;
        MovimientoMapa.posicionRetorno = jugador.position;
        MovimientoMapa.vieneDeCombate = true;
        MovimientoMapa.escenaOrigen = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        MovimientoMapa.combateBoss = true;
        MovimientoMapa.combateSecuaz = true;
        UnityEngine.SceneManagement.SceneManager.LoadScene("Battle");
    }

    // Llamado desde BattleManager al vencer
    public static void MarcarDerrotado()
    {
        secuazDerrotado = true;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, distancia);
    }
}