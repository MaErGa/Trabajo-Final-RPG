using UnityEngine;

public class NPCRobbinOdd : MonoBehaviour
{
    [Header("Distancia para interactuar")]
    public float distancia = 2f;

    [Header("Enemigo del combate")]
    public DatosEnemigo datosEnemigo;

    private Transform jugador;
    private bool derrotado = false;

    public static bool robbinDerrotado = false;

    private string[] dialogoAntesCombate = new string[]
    {
        "Robbin Odd: Alto ahi, renacuajo! Quien te da permiso para entrar en la guarida del mismisimo, ilustre y elegantisimo... Robbin Odd!?",
        "Robbin Odd: Que vienes de parte de los pueblerinos a recuperar sus tesoros? Por favor! Que insulto!",
        "Robbin Odd: Esos tacanos andan diciendo por ahi que les robe oro y joyas... Mentira! Calumnias! Yo tengo estandares! Lo que me lleve de ese pueblo fue su bien mas preciado...",
        "Robbin Odd: Su cargamento secreto de ropa interior de seda real con encaje dorado! Una obra de arte textil!",
        "Robbin Odd: Como se atreven a ocultar semejante botin historico diciendo que solo eran simples monedas? Eso hiere mi reputacion de saqueador refinado!",
        "Robbin Odd: Ya que estas aqui, te dare una leccion por escuchar los chismes de esa gente sin estilo! Prepara los punos!"
    };

    private string[] dialogoDerrota = new string[]
    {
        "Robbin Odd: Ay, ay, ay! Mis costillas imperiales! Vale, vale, tu ganas! Retiro lo de renacuajo... eres mas bien un lobo con piel de heroe.",
        "Robbin Odd: La ropa interior de seda? Estoo... veras, mi querido y fuerte amigo... Hay un pequeno, diminuto y logistico problema.",
        "Robbin Odd: Ya no la tengo conmigo! Y no me mires asi, que no me la he puesto!",
        "Robbin Odd: Es que... uno de mis secuaces (el muy traidor incompetente) vio que la cosa se ponia fea y salio corriendo con el botin metido en un saco.",
        "Robbin Odd: Dijo algo como: Me llevo esto a la gran ciudad para revenderlo en el mercado negro, jefe, ahi pagan el triple! Y pfff... desaparecio!",
        "Robbin Odd: Si quieres recuperar esa seda tan... comoda y transpirable, vas a tener que buscar a mi esbirro en el proximo pueblo.",
        "Robbin Odd: Y ahora, si me disculpas... un verdadero caballero de la delincuencia sabe cuando retirarse a llorar a un rincon. Hasta la vista!"
    };

    private string[] dialogoBloqueado = new string[]
    {
        "Robbin Odd: Ey, ey! A donde vas tan rapido?",
        "Robbin Odd: Si quieres hablar conmigo, primero supera a mi guardaespaldas Grock. Ese chico necesita motivacion!"
    };

    void Start()
    {
        GameObject obj = GameObject.FindGameObjectWithTag("Player");
        if (obj != null) jugador = obj.transform;

        if (robbinDerrotado)
        {
            derrotado = true;
            gameObject.SetActive(false);
        }
    }

    void Update()
    {
        if (jugador == null || derrotado) return;

        float dist = Vector2.Distance(transform.position, jugador.position);
        if (dist > distancia) return;
        if (!Input.GetKeyDown(KeyCode.X)) return;
        if (DialogoManagerBoss.instancia != null && DialogoManagerBoss.instancia.EstaActivo()) return;

        if (!NPCSecuaz.secuazDerrotado)
        {
            DialogoManagerBoss.instancia.MostrarDialogo(dialogoBloqueado, null);
        }
        else
        {
            DialogoManagerBoss.instancia.MostrarDialogo(dialogoAntesCombate, IniciarCombate);
        }
    }

    void IniciarCombate()
    {
        if (datosEnemigo == null) return;
        MovimientoMapa.enemigoSeleccionado = datosEnemigo;
        MovimientoMapa.posicionRetorno = jugador.position;
        MovimientoMapa.vieneDeCombate = true;
        MovimientoMapa.escenaOrigen = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        MovimientoMapa.combateBoss = true;
        MovimientoMapa.combateSecuaz = false;
        UnityEngine.SceneManagement.SceneManager.LoadScene("Battle");
    }

    public static void MarcarDerrotado()
    {
        robbinDerrotado = true;
    }

    public void MostrarDialogoDerrota()
    {
        DialogoManagerBoss.instancia.MostrarDialogo(dialogoDerrota, () =>
        {
            derrotado = true;
            gameObject.SetActive(false);
        });
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, distancia);
    }
}