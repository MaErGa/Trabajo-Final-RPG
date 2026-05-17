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
    public GameObject panelNombre;        // panel con el input del nombre
    public TMP_InputField inputNombre;    // campo de texto para escribir el nombre

    [Header("Panel Confirmar Borrar")]
    public GameObject panelConfirmar;

    [Header("Escenas")]
    public string escenaJuego = "Underworld";

    void Start()
    {
        bool hayPartida = SistemaGuardado.instancia != null && SistemaGuardado.instancia.ExistePartida();

        if (botonContinuar != null)    botonContinuar.SetActive(hayPartida);
        if (botonBorrarPartida != null) botonBorrarPartida.SetActive(hayPartida);
        if (panelNombre != null)       panelNombre.SetActive(false);
        if (panelConfirmar != null)    panelConfirmar.SetActive(false);
    }

    // ── Continuar ─────────────────────────────────────────────

    public void BotonContinuar()
    {
        if (SistemaGuardado.instancia != null)
            SistemaGuardado.instancia.Cargar();

        SceneManager.LoadScene(escenaJuego);
    }

    // ── Nueva Partida ─────────────────────────────────────────

    public void BotonNuevaPartida()
    {
        // Muestra el panel para escribir el nombre
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

        // Reinicia el personaje y asigna el nombre
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
}