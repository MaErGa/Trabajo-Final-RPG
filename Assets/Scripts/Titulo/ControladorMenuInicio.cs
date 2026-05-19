using UnityEngine;
using UnityEngine.SceneManagement;

public class ControladorMenuInicio : MonoBehaviour
{
    [Header("Componente de Audio")]
    [SerializeField] private AudioSource musicaMenu;

    [Header("Nombres de las Escenas")]
    [SerializeField] private string escenaNuevaPartida = "Inicio";
    [SerializeField] private string escenaContinuar = "Pueblo";

    private void Start()
    {
        if (musicaMenu != null && !musicaMenu.isPlaying)
            musicaMenu.Play();
    }

    // ── Nueva Partida ────────────────────────────────────────
    public void BotonNuevaPartida()
    {
        ApagarMusica();

        if (SistemaGuardado.instancia != null)
        {
            SistemaGuardado.instancia.hayPosicionGuardada = false;
            SistemaGuardado.instancia.escenaCargadaAutomatica = "";
        }

        if (SceneLoadManager.Instance != null)
            SceneLoadManager.Instance.ResetSpawnFlag();

        SceneManager.LoadScene(escenaNuevaPartida);
    }

    // ── Continuar ────────────────────────────────────────────
    public void BotonContinuar()
    {
        ApagarMusica();

        // DEBUG TEMPORAL
        Debug.Log("INSTANCIA: " + SistemaGuardado.instancia);
        Debug.Log("EXISTE PARTIDA: " + SistemaGuardado.instancia.ExistePartida());
        Debug.Log("RUTA JSON: " + Application.persistentDataPath + "/partida.json");

        if (SceneLoadManager.Instance != null)
            SceneLoadManager.Instance.ResetSpawnFlag();

        if (SistemaGuardado.instancia != null && SistemaGuardado.instancia.ExistePartida())
        {
            SistemaGuardado.instancia.Cargar();

            string escenaParaCargar = SistemaGuardado.instancia.escenaCargadaAutomatica;

            if (!string.IsNullOrEmpty(escenaParaCargar))
            {
                SceneManager.LoadScene(escenaParaCargar);
                return;
            }
        }

        SceneManager.LoadScene(escenaContinuar);
    }

    // ── Borrar Partida ───────────────────────────────────────
    public void BotonBorrarPartida()
    {
        if (SistemaGuardado.instancia != null)
        {
            SistemaGuardado.instancia.BorrarPartida();
            SistemaGuardado.instancia.hayPosicionGuardada = false;
            SistemaGuardado.instancia.escenaCargadaAutomatica = "";
        }

        if (SceneLoadManager.Instance != null)
            SceneLoadManager.Instance.ResetSpawnFlag();

        Debug.Log("Partida borrada desde el menú.");
    }

    // ── Salir ────────────────────────────────────────────────
    public void BotonSalir()
    {
        Application.Quit();
    }

    // ── Privados ─────────────────────────────────────────────
    private void ApagarMusica()
    {
        if (musicaMenu != null)
            musicaMenu.Stop();
    }
}