using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class Estatua : MonoBehaviour
{
    [Header("Datos del Jugador")]
    public DatosJugador datosRyo;

    [Header("Distancia para interactuar")]
    public float distancia = 2f;

    [Header("Coste de Curación")]
    public int costeCuracion = 4;

    private Transform jugador;

    private GameObject canvasGO;
    private GameObject panelOpciones;
    private GameObject panelMensaje;
    private TextMeshProUGUI textoMensaje;
    private List<GameObject> botones = new List<GameObject>();

    private bool panelCreado = false;

    void Start()
    {
        GameObject obj = GameObject.FindGameObjectWithTag("Player");
        if (obj != null) jugador = obj.transform;
        CrearUI();
    }

    // ── Construcción UI ───────────────────────────────────────

    void CrearUI()
    {
        canvasGO = new GameObject("CanvasEstatua");
        Canvas canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 50;
        canvasGO.AddComponent<CanvasScaler>();
        canvasGO.AddComponent<GraphicRaycaster>();

        // Panel mensaje — franja inferior con el texto de contexto
        panelMensaje = new GameObject("PanelMensaje");
        panelMensaje.transform.SetParent(canvasGO.transform, false);
        RectTransform rmsg = panelMensaje.AddComponent<RectTransform>();
        rmsg.anchorMin = new Vector2(0f, 0f);
        rmsg.anchorMax = new Vector2(1f, 0f);
        rmsg.pivot = new Vector2(0.5f, 0f);
        rmsg.anchoredPosition = new Vector2(0, 52f);
        rmsg.sizeDelta = new Vector2(0, 36f);
        panelMensaje.AddComponent<Image>().color = new Color(0f, 0f, 0f, 0.82f);

        GameObject textoGO = new GameObject("Texto");
        textoGO.transform.SetParent(panelMensaje.transform, false);
        RectTransform rtxt = textoGO.AddComponent<RectTransform>();
        rtxt.anchorMin = Vector2.zero; rtxt.anchorMax = Vector2.one;
        rtxt.offsetMin = new Vector2(20, 0); rtxt.offsetMax = new Vector2(-20, 0);
        textoMensaje = textoGO.AddComponent<TextMeshProUGUI>();
        textoMensaje.fontSize = 15;
        textoMensaje.color = Color.white;
        textoMensaje.alignment = TextAlignmentOptions.MidlineLeft;

        // Panel opciones — franja inferior con botones horizontales
        panelOpciones = new GameObject("PanelOpciones");
        panelOpciones.transform.SetParent(canvasGO.transform, false);
        RectTransform ropc = panelOpciones.AddComponent<RectTransform>();
        ropc.anchorMin = new Vector2(0f, 0f);
        ropc.anchorMax = new Vector2(1f, 0f);
        ropc.pivot = new Vector2(0.5f, 0f);
        ropc.anchoredPosition = new Vector2(0, 0f);
        ropc.sizeDelta = new Vector2(0, 52f);
        panelOpciones.AddComponent<Image>().color = new Color(0f, 0f, 0f, 0.82f);

        // Layout horizontal automático
        HorizontalLayoutGroup hlg = panelOpciones.AddComponent<HorizontalLayoutGroup>();
        hlg.childAlignment = TextAnchor.MiddleCenter;
        hlg.spacing = 4f;
        hlg.padding = new RectOffset(12, 12, 8, 8);
        hlg.childForceExpandWidth = false;
        hlg.childForceExpandHeight = true;

        panelOpciones.SetActive(false);
        panelMensaje.SetActive(false);
        panelCreado = true;
    }

    void MostrarOpciones(string mensaje, string[] opciones, System.Action<int> alSeleccionar)
    {
        // Limpiar botones anteriores
        foreach (var b in botones) Destroy(b);
        botones.Clear();

        textoMensaje.text = mensaje;

        for (int i = 0; i < opciones.Length; i++)
        {
            int indice = i;

            GameObject btnGO = new GameObject("Btn_" + i);
            btnGO.transform.SetParent(panelOpciones.transform, false);

            // Tamaño fijo por botón
            LayoutElement le = btnGO.AddComponent<LayoutElement>();
            le.preferredWidth = 140f;
            le.preferredHeight = 36f;

            Image imgBtn = btnGO.AddComponent<Image>();
            imgBtn.color = new Color(0.12f, 0.12f, 0.12f, 1f);

            Button btn = btnGO.AddComponent<Button>();
            ColorBlock cb = btn.colors;
            cb.normalColor = Color.white;
            cb.highlightedColor = new Color(0.75f, 0.75f, 0.75f, 1f);
            cb.pressedColor = new Color(0.5f, 0.5f, 0.5f, 1f);
            cb.selectedColor = Color.white;
            btn.colors = cb;

            // Texto del botón
            GameObject textoGO = new GameObject("Texto");
            textoGO.transform.SetParent(btnGO.transform, false);
            RectTransform rt = textoGO.AddComponent<RectTransform>();
            rt.anchorMin = Vector2.zero; rt.anchorMax = Vector2.one;
            rt.offsetMin = Vector2.zero; rt.offsetMax = Vector2.zero;
            TextMeshProUGUI txt = textoGO.AddComponent<TextMeshProUGUI>();
            txt.text = opciones[i];
            txt.fontSize = 15;
            txt.color = Color.white;
            txt.alignment = TextAlignmentOptions.Center;

            // Hover: texto gris claro
            EventTrigger trigger = btnGO.AddComponent<EventTrigger>();
            var enter = new EventTrigger.Entry { eventID = EventTriggerType.PointerEnter };
            enter.callback.AddListener((_) => txt.color = new Color(0.7f, 0.7f, 0.7f));
            trigger.triggers.Add(enter);
            var exit = new EventTrigger.Entry { eventID = EventTriggerType.PointerExit };
            exit.callback.AddListener((_) => txt.color = Color.white);
            trigger.triggers.Add(exit);

            btn.onClick.AddListener(() => alSeleccionar(indice));
            botones.Add(btnGO);
        }

        panelOpciones.SetActive(true);
        panelMensaje.SetActive(true);
    }

    void OcultarOpciones()
    {
        foreach (var b in botones) Destroy(b);
        botones.Clear();
        panelOpciones.SetActive(false);
        panelMensaje.SetActive(false);
    }

    // ── Lógica ────────────────────────────────────────────────

    void Update()
    {
        if (!panelCreado) return;
        if (jugador == null) return;
        if (DialogoManager.instancia != null && DialogoManager.instancia.EstaActivo()) return;

        float dist = Vector2.Distance(transform.position, jugador.position);

        if (dist <= distancia && Input.GetKeyDown(KeyCode.X))
        {
            if (panelOpciones.activeSelf)
            {
                OcultarOpciones();
                return;
            }
            MostrarMenuEstatua();
        }
    }

    void MostrarMenuEstatua()
    {
        string[] lineas = {
            "Soy la estatua del pueblo.",
            "Puedo restaurar tu salud por " + costeCuracion + " G.",
            "También puedo guardar tu aventura."
        };
        DialogoManager.instancia.MostrarDialogo(lineas, MostrarOpcionesPrincipales);
    }

    void MostrarOpcionesPrincipales()
    {
        MostrarOpciones(
            "¿Qué deseas hacer?",
            new string[] { "Curar (" + costeCuracion + " G)", "Guardar", "Salir" },
            (indice) => {
                switch (indice)
                {
                    case 0: ConfirmarCurar(); break;
                    case 1: ConfirmarGuardar(); break;
                    case 2: OcultarOpciones(); break;
                }
            });
    }

    void ConfirmarCurar()
    {
        if (datosRyo.oro < costeCuracion)
        {
            OcultarOpciones();
            DialogoManager.instancia.MostrarDialogo(new string[]{
                "No tienes suficiente oro...\nNecesitas " + costeCuracion + " G."
            });
            return;
        }
        if (datosRyo.hpActual >= datosRyo.hpMax)
        {
            OcultarOpciones();
            DialogoManager.instancia.MostrarDialogo(new string[]{
                "Ya tienes la salud al máximo."
            });
            return;
        }

        MostrarOpciones(
            "Curar toda la salud por " + costeCuracion + " G. ¿Seguro?",
            new string[] { "Sí", "No" },
            (indice) => {
                if (indice == 0) EjecutarCuracion();
                else MostrarOpcionesPrincipales();
            });
    }

    void EjecutarCuracion()
    {
        datosRyo.oro -= costeCuracion;
        datosRyo.hpActual = datosRyo.hpMax;
        datosRyo.mpActual = datosRyo.mpMax;
        OcultarOpciones();
        DialogoManager.instancia.MostrarDialogo(new string[]{
            "¡Tu salud ha sido restaurada!\nQue la luz te guíe, aventurero."
        });
    }

    void ConfirmarGuardar()
    {
        MostrarOpciones(
            "¿Deseas guardar tu aventura?",
            new string[] { "Sí", "No" },
            (indice) => {
                if (indice == 0) EjecutarGuardado();
                else MostrarOpcionesPrincipales();
            });
    }

    void EjecutarGuardado()
    {
        if (SistemaGuardado.instancia != null)
            SistemaGuardado.instancia.Guardar();
        OcultarOpciones();
        DialogoManager.instancia.MostrarDialogo(new string[]{
            "¡Aventura guardada!\nQue los dioses te protejan."
        });
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, distancia);
    }
}