using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class DialogoManager : MonoBehaviour
{
    public static DialogoManager instancia;

    [Header("UI")]
    public GameObject panelDialogo;
    public TextMeshProUGUI textoDialogo;
    public TextMeshProUGUI textoContinuar;

    [Header("Configuracion")]
    public float velocidadTexto = 0.05f;

    private string[] lineasActuales;
    private int lineaActual = 0;
    private bool escribiendo = false;
    private bool dialogoActivo = false;

    // ── NUEVO: callback opcional al cerrar ──
    private System.Action onDialogoTerminado;

    void Awake()
    {
        instancia = this;
        panelDialogo.SetActive(false);
    }

    void Update()
    {
        if (!dialogoActivo) return;

        if (Input.GetKeyDown(KeyCode.X))
        {
            if (escribiendo)
            {
                StopAllCoroutines();
                textoDialogo.text = lineasActuales[lineaActual];
                escribiendo = false;
                textoContinuar.gameObject.SetActive(true);
            }
            else
            {
                SiguienteLinea();
            }
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            CerrarDialogo();
        }
    }

    // ── Método original intacto (NPC, tienda, etc. siguen funcionando) ──
    public void MostrarDialogo(string[] lineas)
    {
        MostrarDialogo(lineas, null);
    }

    // ── NUEVO: versión con callback ──
    public void MostrarDialogo(string[] lineas, System.Action alTerminar)
    {
        lineasActuales = lineas;
        lineaActual = 0;
        dialogoActivo = true;
        onDialogoTerminado = alTerminar;
        panelDialogo.SetActive(true);
        textoContinuar.gameObject.SetActive(false);
        StartCoroutine(EscribirTexto(lineasActuales[lineaActual]));
    }

    void SiguienteLinea()
    {
        lineaActual++;

        if (lineaActual < lineasActuales.Length)
        {
            textoContinuar.gameObject.SetActive(false);
            StartCoroutine(EscribirTexto(lineasActuales[lineaActual]));
        }
        else
        {
            CerrarDialogo();
        }
    }

    IEnumerator EscribirTexto(string texto)
    {
        escribiendo = true;
        textoDialogo.text = "";

        foreach (char letra in texto)
        {
            textoDialogo.text += letra;
            yield return new WaitForSeconds(velocidadTexto);
        }

        escribiendo = false;
        textoContinuar.gameObject.SetActive(true);
    }

    void CerrarDialogo()
    {
        dialogoActivo = false;
        panelDialogo.SetActive(false);
        textoDialogo.text = "";

        // ── NUEVO: ejecuta el callback si existe y lo limpia ──
        System.Action callback = onDialogoTerminado;
        onDialogoTerminado = null;
        callback?.Invoke();
    }

    public bool EstaActivo() => dialogoActivo;
}