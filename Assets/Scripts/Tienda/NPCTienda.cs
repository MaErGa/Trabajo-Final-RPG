using UnityEngine;

public class NPCTienda : MonoBehaviour
{
    [Header("Dialogo")]
    [TextArea(2, 5)]
    public string[] lineasBienvenida = {
        "¡Bienvenido a la tienda!",
        "Tengo los mejores artículos del reino.",
        "¿En qué puedo ayudarte?"
    };

    [TextArea(2, 5)]
    public string[] lineasDespedida = {
        "¡Vuelve cuando quieras!",
        "Que tengas un buen viaje, aventurero."
    };

    [Header("Distancia para interactuar")]
    public float distancia = 6f;

    [Header("Referencia a la Tienda")]
    public GameObject panelTienda;
    public TiendaManager tiendaManager;

    private Transform jugador;
    private bool tiendaAbierta = false;
    private bool conversacionIniciada = false;

    void Start()
    {
        GameObject obj = GameObject.FindGameObjectWithTag("Player");
        if (obj != null) jugador = obj.transform;

        if (panelTienda != null) panelTienda.SetActive(false);
    }

    void Update()
    {
        if (jugador == null) return;

        float dist = Vector2.Distance(transform.position, jugador.position);

        if (dist <= distancia && Input.GetKeyDown(KeyCode.X))
        {
            // Si el diálogo está activo no hacemos nada
            if (DialogoManager.instancia != null && DialogoManager.instancia.EstaActivo()) return;

            if (tiendaAbierta)
            {
                CerrarTienda();
                return;
            }

            if (!conversacionIniciada)
                StartCoroutine(IniciarConversacion());
        }
    }

    System.Collections.IEnumerator IniciarConversacion()
    {
        conversacionIniciada = true;

        DialogoManager.instancia.MostrarDialogo(lineasBienvenida);

        // Espera a que termine el diálogo
        yield return new WaitUntil(() => !DialogoManager.instancia.EstaActivo());

        Debug.Log("Dialogo terminado, abriendo tienda...");
        AbrirTienda();
        conversacionIniciada = false;
    }

    void AbrirTienda()
    {
        tiendaAbierta = true;
        if (panelTienda != null) panelTienda.SetActive(true);
        if (tiendaManager != null) tiendaManager.MostrarModoComprar();
    }

    void CerrarTienda()
    {
        tiendaAbierta = false;
        if (panelTienda != null) panelTienda.SetActive(false);
        StartCoroutine(MostrarDespedida());
    }

    System.Collections.IEnumerator MostrarDespedida()
    {
        DialogoManager.instancia.MostrarDialogo(lineasDespedida);
        yield return new WaitUntil(() => !DialogoManager.instancia.EstaActivo());
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, distancia);
    }
}