using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class CreditosMovimiento : MonoBehaviour
{
    [Header("Configuración de Velocidad")]
    public float velocidad = 60f;

    [Header("Configuración de Cierre")]
    public string escenaAlTerminar = "Titulo";

    [Tooltip("Segundos que se queda la pantalla en negro al final antes de cambiar de escena.")]
    public float tiempoEsperaAlFinal = 3.0f;

    private RectTransform rectTransform;
    private Canvas canvasPadre;
    private bool haTerminado = false;
    private bool listoParaMover = false;
    private float posicionObjetivo;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasPadre = GetComponentInParent<Canvas>();
        StartCoroutine(InicializarConDelay());
    }

    IEnumerator InicializarConDelay()
    {
        // Espera dos frames para que Unity calcule bien el tamaño del TextMeshPro
        yield return null;
        yield return null;

        float alturaPantalla = 1080f;
        if (canvasPadre != null)
            alturaPantalla = canvasPadre.GetComponent<RectTransform>().rect.height;

        float alturaTexto = rectTransform.rect.height;
        float posInicial   = rectTransform.anchoredPosition.y;

        // El texto termina cuando su borde INFERIOR sale por el borde SUPERIOR de la pantalla.
        // Borde superior del canvas = alturaPantalla / 2
        // Borde inferior del texto  = anchoredPosition.y - alturaTexto  (pivot arriba-izquierda, Y crece hacia arriba)
        // Condición final: posY - alturaTexto >= alturaPantalla / 2
        // => posY >= alturaPantalla / 2 + alturaTexto
        posicionObjetivo = (alturaPantalla / 2f) + alturaTexto;

        Debug.Log($"[Creditos] alturaPantalla={alturaPantalla} alturaTexto={alturaTexto} posInicial={posInicial} objetivo={posicionObjetivo}");

        listoParaMover = true;
    }

    void Update()
    {
        if (!listoParaMover || haTerminado) return;

        rectTransform.anchoredPosition += Vector2.up * velocidad * Time.deltaTime;

        if (rectTransform.anchoredPosition.y >= posicionObjetivo)
        {
            haTerminado = true;
            StartCoroutine(EsperarYTerminar());
        }

        if (Input.GetKeyDown(KeyCode.X) || Input.GetKeyDown(KeyCode.Space))
        {
            haTerminado = true;
            TerminarCreditos();
        }
    }

    IEnumerator EsperarYTerminar()
    {
        yield return new WaitForSeconds(tiempoEsperaAlFinal);
        TerminarCreditos();
    }

    void TerminarCreditos()
    {
        SceneManager.LoadScene("Titulo");
    }
}