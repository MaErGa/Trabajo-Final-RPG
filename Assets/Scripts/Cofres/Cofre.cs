using UnityEngine;
using TMPro;
using System.Collections;

public class Cofre : MonoBehaviour
{
    // ─── Flag estático: bloquea el movimiento del jugador ───────────────────
    public static bool dialogoActivo = false;

    [Header("Contenido del cofre")]
    public EquipoBase equipoContenido;
    public ItemConsumible itemContenido;

    [Header("Referencia al jugador")]
    public DatosJugador datosJugador;

    [Header("UI Diálogo")]
    public GameObject panelDialogo;          // ← Asignar: PanelDialogoCofres
    public TextMeshProUGUI textoDialogo;     // ← Asignar: Dialogo Cofres Texto
    public GameObject botonesEquipar;        // ← Asignar: Botones cofres

    [Header("Distancia para interactuar")]
    public float distanciaInteraccion = 2f;

    private bool abierto = false;
    private bool esperandoRespuesta = false;
    private Transform jugador;

    void Start()
    {
        // Ocultar UI al inicio
        if (panelDialogo != null)
            panelDialogo.SetActive(false);
        else
            Debug.LogError("[Cofre] panelDialogo NO asignado en el Inspector → " + gameObject.name);

        if (botonesEquipar != null)
            botonesEquipar.SetActive(false);
        else
            Debug.LogError("[Cofre] botonesEquipar NO asignado en el Inspector → " + gameObject.name);

        if (textoDialogo == null)
            Debug.LogError("[Cofre] textoDialogo NO asignado en el Inspector → " + gameObject.name);

        // Buscar jugador por tag
        GameObject obj = GameObject.FindGameObjectWithTag("Player");
        if (obj != null)
        {
            jugador = obj.transform;
            Debug.Log("[Cofre] Jugador encontrado: " + obj.name);
        }
        else
        {
            Debug.LogError("[Cofre] No se encontró ningún GameObject con tag 'Player'. " +
                           "Asegúrate de que el jugador tiene ese tag exacto.");
        }
    }

    void Update()
    {
        if (jugador == null || abierto || esperandoRespuesta) return;

        float dist = Vector2.Distance(transform.position, jugador.position);

        if (dist <= distanciaInteraccion && Input.GetKeyDown(KeyCode.X))
        {
            Debug.Log("[Cofre] Abriendo cofre. Distancia: " + dist);
            AbrirCofre();
        }
    }

    void AbrirCofre()
    {
        abierto = true;
        dialogoActivo = true;

        if (equipoContenido != null)
        {
            if (datosJugador != null)
                datosJugador.armarioEquipo.Add(equipoContenido);
            else
                Debug.LogWarning("[Cofre] datosJugador no asignado, no se guardó el equipo.");

            StartCoroutine(MostrarDialogoEquipo());
        }
        else if (itemContenido != null)
        {
            if (datosJugador != null)
                datosJugador.mochilaItems.Add(itemContenido);
            else
                Debug.LogWarning("[Cofre] datosJugador no asignado, no se guardó el item.");

            StartCoroutine(MostrarDialogoItem());
        }
        else
        {
            StartCoroutine(MostrarMensaje("El cofre está vacío..."));
        }
    }

    IEnumerator MostrarDialogoEquipo()
    {
        if (panelDialogo != null) panelDialogo.SetActive(true);
        else { Debug.LogError("[Cofre] panelDialogo es null"); yield break; }

        textoDialogo.text = "¡Encontraste " + equipoContenido.nombre + "!\n" + equipoContenido.descripcion;

        yield return StartCoroutine(EsperarInput());

        textoDialogo.text = "¿Deseas equipar " + equipoContenido.nombre + "?";

        if (botonesEquipar != null)
        {
            botonesEquipar.SetActive(true);
            Debug.Log("[Cofre] Botones equipar activados.");
        }
        else
        {
            Debug.LogError("[Cofre] botonesEquipar es null. Asígnalo en el Inspector.");
        }

        esperandoRespuesta = true;
    }

    IEnumerator MostrarDialogoItem()
    {
        if (panelDialogo != null) panelDialogo.SetActive(true);
        else { Debug.LogError("[Cofre] panelDialogo es null"); yield break; }

        textoDialogo.text = "¡Encontraste " + itemContenido.nombre + "!\n" + itemContenido.descripcion;
        yield return StartCoroutine(EsperarInput());
        CerrarDialogo();
    }

    IEnumerator MostrarMensaje(string mensaje)
    {
        if (panelDialogo != null) panelDialogo.SetActive(true);
        else { Debug.LogError("[Cofre] panelDialogo es null"); yield break; }

        textoDialogo.text = mensaje;
        yield return StartCoroutine(EsperarInput());
        CerrarDialogo();
    }

    IEnumerator EsperarInput()
    {
        yield return new WaitForSeconds(0.2f);
        while (!Input.GetKeyDown(KeyCode.X))
            yield return null;
    }

    // ─── Llamados desde los botones SI y NO en el Inspector ─────────────────

    public void EquiparSi()
    {
        esperandoRespuesta = false;
        if (botonesEquipar != null) botonesEquipar.SetActive(false);
        if (datosJugador != null) datosJugador.EquiparObjeto(equipoContenido);
        textoDialogo.text = "¡" + equipoContenido.nombre + " equipado!";
        StartCoroutine(CerrarTrasEspera());
    }

    public void EquiparNo()
    {
        esperandoRespuesta = false;
        if (botonesEquipar != null) botonesEquipar.SetActive(false);
        textoDialogo.text = equipoContenido.nombre + " guardado en el armario.";
        StartCoroutine(CerrarTrasEspera());
    }

    IEnumerator CerrarTrasEspera()
    {
        yield return new WaitForSeconds(1.5f);
        CerrarDialogo();
    }

    void CerrarDialogo()
    {
        if (panelDialogo != null)   panelDialogo.SetActive(false);
        if (botonesEquipar != null) botonesEquipar.SetActive(false);
        esperandoRespuesta = false;
        dialogoActivo = false;
        Debug.Log("[Cofre] Diálogo cerrado.");
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, distanciaInteraccion);
    }
}