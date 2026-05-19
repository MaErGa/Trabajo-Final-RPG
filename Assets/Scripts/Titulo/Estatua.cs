using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Estatua : MonoBehaviour
{
    [Header("Datos del Jugador")]
    public DatosJugador datosRyo;

    [Header("Distancia para interactuar")]
    public float distancia = 2f;

    [Header("Coste de Curación")]
    public int costeCuracion = 4;

    [Header("Panel de Opciones")]
    public GameObject panelOpciones;
    public TextMeshProUGUI textoOpciones;
    public Button botonSi;
    public Button botonNo;

    private Transform jugador;

    void Start()
    {
        GameObject obj = GameObject.FindGameObjectWithTag("Player");
        if (obj != null) jugador = obj.transform;

        if (panelOpciones != null) panelOpciones.SetActive(false);
    }

    void Update()
    {
        if (jugador == null) return;
        if (DialogoManager.instancia != null && DialogoManager.instancia.EstaActivo()) return;

        float dist = Vector2.Distance(transform.position, jugador.position);

        if (dist <= distancia && Input.GetKeyDown(KeyCode.X))
        {
            if (panelOpciones != null && panelOpciones.activeSelf)
            {
                CerrarOpciones();
                return;
            }
            MostrarMenuEstatua();
        }
    }

    void MostrarMenuEstatua()
    {
        // Primero muestra el diálogo de bienvenida
        string[] lineas = {
            "Soy la estatua del pueblo.",
            "Puedo restaurar tu salud por " + costeCuracion + " G.",
            "También puedo guardar tu aventura.",
            "¿Qué deseas hacer? (X para cerrar)"
        };
        DialogoManager.instancia.MostrarDialogo(lineas);
        // Cuando termine el diálogo mostramos las opciones
        StartCoroutine(EsperarDialogoYMostrarOpciones());
    }

    System.Collections.IEnumerator EsperarDialogoYMostrarOpciones()
    {
        yield return new WaitUntil(() => !DialogoManager.instancia.EstaActivo());
        MostrarOpcionesPrincipales();
    }

    void MostrarOpcionesPrincipales()
    {
        if (panelOpciones == null) return;
        panelOpciones.SetActive(true);
        textoOpciones.text = "¿Qué deseas hacer?\n\n[Curar por " + costeCuracion + " G]\n[Guardar partida]";

        botonSi.GetComponentInChildren<TextMeshProUGUI>().text = "Curar";
        botonNo.GetComponentInChildren<TextMeshProUGUI>().text = "Guardar";

        botonSi.onClick.RemoveAllListeners();
        botonNo.onClick.RemoveAllListeners();

        botonSi.onClick.AddListener(ConfirmarCurar);
        botonNo.onClick.AddListener(ConfirmarGuardar);
    }

    void ConfirmarCurar()
    {
        panelOpciones.SetActive(false);

        if (datosRyo.oro < costeCuracion)
        {
            string[] lineas = { "No tienes suficiente oro...\nNecesitas " + costeCuracion + " G." };
            DialogoManager.instancia.MostrarDialogo(lineas);
            return;
        }

        if (datosRyo.hpActual >= datosRyo.hpMax)
        {
            string[] lineas = { "Ya tienes la salud al máximo." };
            DialogoManager.instancia.MostrarDialogo(lineas);
            return;
        }

        // Mostrar confirmación
        panelOpciones.SetActive(true);
        textoOpciones.text = "Curar toda la salud por " + costeCuracion + " G.\n¿Estás seguro?";

        botonSi.GetComponentInChildren<TextMeshProUGUI>().text = "Sí";
        botonNo.GetComponentInChildren<TextMeshProUGUI>().text = "No";

        botonSi.onClick.RemoveAllListeners();
        botonNo.onClick.RemoveAllListeners();

        botonSi.onClick.AddListener(EjecutarCuracion);
        botonNo.onClick.AddListener(CerrarOpciones);
    }

    void EjecutarCuracion()
    {
        datosRyo.oro -= costeCuracion;
        datosRyo.hpActual = datosRyo.hpMax;
        datosRyo.mpActual = datosRyo.mpMax;
        CerrarOpciones();

        string[] lineas = { "¡Tu salud ha sido restaurada!\nQue la luz te guíe, aventurero." };
        DialogoManager.instancia.MostrarDialogo(lineas);
    }

    void ConfirmarGuardar()
    {
        panelOpciones.SetActive(false);

        // Mostrar confirmación
        panelOpciones.SetActive(true);
        textoOpciones.text = "¿Deseas guardar tu aventura?";

        botonSi.GetComponentInChildren<TextMeshProUGUI>().text = "Sí";
        botonNo.GetComponentInChildren<TextMeshProUGUI>().text = "No";

        botonSi.onClick.RemoveAllListeners();
        botonNo.onClick.RemoveAllListeners();

        botonSi.onClick.AddListener(EjecutarGuardado);
        botonNo.onClick.AddListener(CerrarOpciones);
    }

    void EjecutarGuardado()
    {
        if (SistemaGuardado.instancia != null)
            SistemaGuardado.instancia.Guardar();

        CerrarOpciones();

        string[] lineas = { "¡Aventura guardada!\nQue los dioses te protejan." };
        DialogoManager.instancia.MostrarDialogo(lineas);
    }

    void CerrarOpciones()
    {
        if (panelOpciones != null) panelOpciones.SetActive(false);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, distancia);
    }
}