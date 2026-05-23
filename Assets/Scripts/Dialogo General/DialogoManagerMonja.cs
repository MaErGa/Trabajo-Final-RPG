using UnityEngine;
using TMPro;
using System.Collections;

public class DialogoManagerMonja : MonoBehaviour
{
    public static DialogoManagerMonja instancia;

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

    void Awake()
    {
        instancia = this;
        if (panelDialogo != null) panelDialogo.SetActive(false);
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
                if (textoContinuar != null) textoContinuar.gameObject.SetActive(true);
            }
            else
            {
                SiguienteLinea();
            }
        }

        if (Input.GetKeyDown(KeyCode.C))
            CerrarDialogo();
    }

    public void MostrarDialogo(string[] lineas)
    {
        lineasActuales = lineas;
        lineaActual = 0;
        dialogoActivo = true;
        if (panelDialogo != null) panelDialogo.SetActive(true);
        if (textoContinuar != null) textoContinuar.gameObject.SetActive(false);
        StartCoroutine(EscribirTexto(lineasActuales[lineaActual]));
    }

    void SiguienteLinea()
    {
        lineaActual++;
        if (lineaActual < lineasActuales.Length)
        {
            if (textoContinuar != null) textoContinuar.gameObject.SetActive(false);
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
        if (textoContinuar != null) textoContinuar.gameObject.SetActive(true);
    }

    void CerrarDialogo()
    {
        dialogoActivo = false;
        if (panelDialogo != null) panelDialogo.SetActive(false);
        if (textoDialogo != null) textoDialogo.text = "";
    }

    public bool EstaActivo() => dialogoActivo;
}
