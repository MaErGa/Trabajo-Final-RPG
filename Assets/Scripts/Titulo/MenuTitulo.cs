using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class MenuTitulo : MonoBehaviour
{
    [Header("Botones")]
    public GameObject botonContinuar;
    public GameObject botonNuevaPartida;
    public GameObject botonBorrarPartida;

    [Header("Panel Nombre")]
    public GameObject panelNombre;
    public TMP_InputField inputNombre;

    [Header("Panel Confirmar Borrar")]
    public GameObject panelConfirmar;

    [Header("Escenas")]
    [Tooltip("Escena a la que va una nueva partida (Ej: Inicio)")]
    public string escenaJuego = "Inicio";

    [Tooltip("Escena por defecto al continuar si no hay mapa guardado (Ej: Pueblo)")]
    public string escenaContinuar = "Pueblo";

    [Tooltip("Escena de opciones")]
    public string escenaOpciones = "Opciones";

    [Tooltip("Escena de creditos")]
    public string escenaCreditos = "Creditos";

    void Start()
    {
        if (panelNombre != null) panelNombre.SetActive(false);
        if (panelConfirmar != null) panelConfirmar.SetActive(false);
        StartCoroutine(ComprobarPartida());
    }

    System.Collections.IEnumerator ComprobarPartida()
    {
        yield return null;
        yield return null;
        yield return null;

        bool hayPartida = SistemaGuardado.instancia != null && SistemaGuardado.instancia.ExistePartida();
        Debug.Log("Hay partida: " + hayPartida + " | Instancia: " + (SistemaGuardado.instancia != null));
        if (botonContinuar != null) botonContinuar.SetActive(hayPartida);
        if (botonBorrarPartida != null) botonBorrarPartida.SetActive(hayPartida);
    }

    // ── Continuar ─────────────────────────────────────────────

    public void BotonContinuar()
    {
        if (SistemaGuardado.instancia != null)
        {
            SistemaGuardado.instancia.Cargar();

            string escenaGuardadaFisica = SistemaGuardado.instancia.escenaCargadaAutomatica;

            if (!string.IsNullOrEmpty(escenaGuardadaFisica))
            {
                SceneManager.LoadScene(escenaGuardadaFisica);
                return;
            }
        }

        SceneManager.LoadScene(escenaContinuar);
    }

    // ── Nueva Partida ─────────────────────────────────────────

    public void BotonNuevaPartida()
    {
        if (panelNombre != null)
        {
            panelNombre.SetActive(true);
            if (inputNombre != null) inputNombre.text = "";
        }
    }

    public void ConfirmarNombre()
    {
        string nombre = inputNombre != null ? inputNombre.text.Trim() : "";

        if (string.IsNullOrEmpty(nombre))
        {
            Debug.LogWarning("El nombre no puede estar vacío.");
            return;
        }

        if (SistemaGuardado.instancia != null)
        {
            SistemaGuardado.instancia.BorrarPartida();
            SistemaGuardado.instancia.datosRyo.nombre = nombre;
        }

        SceneManager.LoadScene(escenaJuego);
    }

    public void CancelarNombre()
    {
        if (panelNombre != null) panelNombre.SetActive(false);
    }

    // ── Borrar Partida ────────────────────────────────────────

    public void BotonBorrarPartida()
    {
        if (panelConfirmar != null) panelConfirmar.SetActive(true);
    }

    public void ConfirmarBorrar()
    {
        if (SistemaGuardado.instancia != null)
            SistemaGuardado.instancia.BorrarPartida();

        if (panelConfirmar != null) panelConfirmar.SetActive(false);
        Start();
    }

    public void CancelarBorrar()
    {
        if (panelConfirmar != null) panelConfirmar.SetActive(false);
    }

    // ── Opciones ──────────────────────────────────────────────

    public void BotonOpciones()
    {
        SceneManager.LoadScene(escenaOpciones);
    }

    // ── Creditos ──────────────────────────────────────────────

    public void BotonCreditos()
    {
        SceneManager.LoadScene(escenaCreditos);
    }
}