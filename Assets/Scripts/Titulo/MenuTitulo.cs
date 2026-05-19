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
    [Tooltip("Escena a la que va una nueva partida (Ej: Inicio)")]
    public string escenaJuego = "Inicio";

    [Tooltip("Escena por defecto al continuar si no hay mapa guardado (Ej: Pueblo)")]
    public string escenaContinuar = "Pueblo"; // ¡AQUÍ ESTÁ LA NUEVA CASILLA!

    void Start()
    {
        if (panelNombre != null) panelNombre.SetActive(false);
        if (panelConfirmar != null) panelConfirmar.SetActive(false);
        StartCoroutine(ComprobarPartida());
    }

    System.Collections.IEnumerator ComprobarPartida()
    {
        // Espera varios frames para asegurar que SistemaGuardado esté listo
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
            // Ejecuta la carga de datos del JSON
            SistemaGuardado.instancia.Cargar();

            // Intenta extraer el nombre de la escena que se grabó físicamente al usar la estatua
            string escenaGuardadaFisica = SistemaGuardado.instancia.escenaCargadaAutomatica;

            // Si el JSON tiene un mapa válido grabado, viaja directo a él de forma inteligente
            if (!string.IsNullOrEmpty(escenaGuardadaFisica))
            {
                SceneManager.LoadScene(escenaGuardadaFisica);
                return; // Corta aquí para ignorar las casillas por defecto
            }
        }

        // Si es una partida vieja sin mapa guardado, usa la nueva casilla que me pediste
        SceneManager.LoadScene(escenaContinuar);
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