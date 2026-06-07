using UnityEngine;

public class NPCRobbinOdd : MonoBehaviour
{
    [Header("Distancia para interactuar")]
    public float distancia = 2f;

    [Header("Enemigo del combate")]
    public DatosEnemigo datosEnemigo;

    [Header("Referencia directa al compañero")]
    public NPCCompañero companero;

    private Transform jugador;
    private bool derrotado = false;

    public static bool robbinDerrotado = false;

    private string[] dialogoAntesCombate = new string[]
    {
        "Alto ahi, renacuajo! Quien te da permiso para entrar en la guarida del mismisimo, ilustre y elegantisimo... Robbin Odd!?",
        "Que vienes de parte de los pueblerinos a recuperar sus tesoros? Por favor! Que insulto!",
        "Esos tacanos andan diciendo por ahi que les robe oro y joyas... Mentira! Calumnias! Yo tengo estandares! Lo que me lleve de ese pueblo fue su bien mas preciado...",
        "Su cargamento secreto de ropa interior de seda real con encaje dorado! Una obra de arte textil!",
        "Como se atreven a ocultar semejante botin historico diciendo que solo eran simples monedas? Eso hiere mi reputacion de saqueador refinado!",
        "Ya que estas aqui, te dare una leccion por escuchar los chismes de esa gente sin estilo! Prepara los punos!"
    };

    private string[] dialogoDerrota = new string[]
    {
        "Ay, ay, ay! Mis costillas imperiales! Vale, vale, tu ganas! Retiro lo de renacuajo... eres mas bien un lobo con piel de heroe.",
        "La ropa interior de seda? Estoo... veras, mi querido y fuerte amigo... Hay un pequeno, diminuto y logistico problema.",
        "Ya no la tengo conmigo! Y no me mires asi, que no me la he puesto!",
        "Es que... uno de mis secuaces (el muy traidor incompetente) vio que la cosa se ponia fea y salio corriendo con el botin metido en un saco.",
        "Dijo algo como: Me llevo esto a la gran ciudad para revenderlo en el mercado negro, jefe, ahi pagan el triple! Y pfff... desaparecio!",
        "Si quieres recuperar esa seda tan... comoda y transpirable, vas a tener que buscar a mi esbirro en el proximo pueblo.",
        "Y ahora, si me disculpas... un verdadero caballero de la delincuencia sabe cuando retirarse a llorar a un rincon. Hasta la vista!"
    };

    private string[] dialogoBloqueado = new string[]
    {
        "Ey, ey! A donde vas tan rapido?",
        "Si quieres hablar conmigo, primero supera a mi guardaespaldas Grock. Ese chico necesita motivacion!"
    };

    void Start()
    {
        GameObject obj = GameObject.FindGameObjectWithTag("Player");
        if (obj != null) jugador = obj.transform;

        if (robbinDerrotado && !derrotado)
        {
            derrotado = true;
            StartCoroutine(MostrarDialogoDerrota_Coroutine());
        }
        else if (robbinDerrotado && derrotado)
        {
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

    System.Collections.IEnumerator MostrarDialogoDerrota_Coroutine()
    {
        yield return null;

        Debug.Log("[RobbinOdd] Iniciando diálogo de derrota...");

        if (DialogoManagerBoss.instancia == null)
        {
            Debug.LogError("[RobbinOdd] DialogoManagerBoss.instancia es NULL");
            yield break;
        }

        if (companero == null)
        {
            Debug.LogError("[RobbinOdd] companero es NULL — arrastra el compañero al Inspector del boss");
            yield break;
        }

        // Capturar referencias locales ANTES de desactivar este objeto,
        // porque SetActive(false) mataría la corrutina si siguiéramos esperando aquí.
        NPCCompañero companeroLocal = companero;
        GameObject bossSelf = gameObject;

        // El callback lo ejecuta DialogoManagerBoss (que sigue vivo),
        // así que funciona aunque el boss se desactive dentro del propio callback.
        DialogoManagerBoss.instancia.MostrarDialogo(dialogoDerrota, () =>
        {
            Debug.Log("[RobbinOdd] Diálogo terminado — activando Pippin y ocultando boss.");

            // Primero activar a Pippin con los flags correctos
            companeroLocal.IniciarDespedida();
            companeroLocal.gameObject.SetActive(true);

            // Ahora sí desactivar el sprite del boss
            bossSelf.SetActive(false);
        });
    }

    public void MostrarDialogoDerrota()
    {
        StartCoroutine(MostrarDialogoDerrota_Coroutine());
    }

    public static void MarcarDerrotado()
    {
        robbinDerrotado = true;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, distancia);
    }
}