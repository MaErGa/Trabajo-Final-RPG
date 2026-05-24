using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class CambioEscena : MonoBehaviour
{
    [Header("Escena destino")]
    public string escenaDestino;

    [Header("Transición")]
    public CanvasGroup panelTransicion;
    public float velocidadFade = 2f;

    private bool cargando = false;

    void OnTriggerStay2D(Collider2D otro)
    {
        if (cargando) return;
        if (!otro.CompareTag("Player")) return;

        cargando = true;
        MovimientoMapa.escenaOrigen = SceneManager.GetActiveScene().name;
        StartCoroutine(CargarEscena());
    }

    IEnumerator CargarEscena()
    {
        if (panelTransicion != null)
        {
            while (panelTransicion.alpha < 1f)
            {
                panelTransicion.alpha += Time.deltaTime * velocidadFade;
                yield return null;
            }
        }
        SceneManager.LoadScene(escenaDestino);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        if (GetComponent<Collider2D>() != null)
            Gizmos.DrawWireCube(transform.position, GetComponent<Collider2D>().bounds.size);
    }
}