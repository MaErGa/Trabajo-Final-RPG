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

    void Awake()
    {
        instancia = this;
        panelDialogo.SetActive(false);
    }

    void Update()
    {
        if (!dialogoActivo) return;

        // X para aceptar/avanzar
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

        // C para cancelar/cerrar
        if (Input.GetKeyDown(KeyCode.C))
        {
            CerrarDialogo();
        }
    }

    public void MostrarDialogo(string[] lineas)
    {
        lineasActuales = lineas;
        lineaActual = 0;
        dialogoActivo = true;
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
    }

    public bool EstaActivo() => dialogoActivo;
}
