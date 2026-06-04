using UnityEngine;

/// <summary>
/// Igual que NPCTienda original pero referencia TiendaUI en vez de TiendaManager.
/// Ya no necesita arrastrar panelTienda: TiendaUI crea su propio Canvas.
/// </summary>
public class NPCTienda : MonoBehaviour
{
    [Header("Diálogo")]
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

    [Header("Referencia a TiendaUI")]
    public TiendaUI tiendaUI;   // arrastra el GameObject que tiene TiendaUI

    private Transform jugador;
    private bool tiendaAbierta = false;
    private bool conversacionIniciada = false;

    void Start()
    {
        GameObject obj = GameObject.FindGameObjectWithTag("Player");
        if (obj != null) jugador = obj.transform;
    }

    void Update()
    {
        if (jugador == null) return;

        float dist = Vector2.Distance(transform.position, jugador.position);

        if (dist <= distancia && Input.GetKeyDown(KeyCode.X))
        {
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
        yield return new WaitUntil(() => !DialogoManager.instancia.EstaActivo());
        AbrirTienda();
        conversacionIniciada = false;
    }

    void AbrirTienda()
    {
        tiendaAbierta = true;
        if (tiendaUI != null) tiendaUI.AbrirTienda();
    }

    void CerrarTienda()
    {
        tiendaAbierta = false;
        if (tiendaUI != null) tiendaUI.CerrarTienda();
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