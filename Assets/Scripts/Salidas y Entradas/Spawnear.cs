using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Coloca este script en el Player (o en un GameManager con DontDestroyOnLoad).
/// Se encarga de guardar y restaurar la posición entre escenas.
/// </summary>
public class Spawnear : MonoBehaviour
{
    // Clave usada en PlayerPrefs para persistir entre sesiones (opcional)
    private const string KeyX = "spawn_x";
    private const string KeyY = "spawn_y";
    private const string KeyZ = "spawn_z";

    // Posición guardada en memoria durante la sesión actual
    private static Vector3? _pendingSpawn = null;

    // -------------------------------------------------------
    // Llamado por SceneTransition ANTES de cargar la escena
    // -------------------------------------------------------
    public static void SaveSpawnPosition(Vector3 position)
    {
        _pendingSpawn = position;

        // (Opcional) También guardamos en PlayerPrefs para persistir
        // si el juego se cierra y reabre:
        PlayerPrefs.SetFloat(KeyX, position.x);
        PlayerPrefs.SetFloat(KeyY, position.y);
        PlayerPrefs.SetFloat(KeyZ, position.z);
        PlayerPrefs.Save();
    }

    // -------------------------------------------------------
    // Al iniciar la escena nueva, reposicionamos al player
    // -------------------------------------------------------
    private void Start()
    {
        if (_pendingSpawn.HasValue)
        {
            transform.position = _pendingSpawn.Value;
            _pendingSpawn = null; // limpiamos para la próxima transición
        }
        // Si no hay posición en memoria, intentamos desde PlayerPrefs
        else if (PlayerPrefs.HasKey(KeyX))
        {
            float x = PlayerPrefs.GetFloat(KeyX);
            float y = PlayerPrefs.GetFloat(KeyY);
            float z = PlayerPrefs.GetFloat(KeyZ);
            transform.position = new Vector3(x, y, z);
        }
        // Si no hay nada guardado, el player queda donde Unity lo colocó
    }

    // -------------------------------------------------------
    // NUEVO — Entrada a la tienda: guarda posición actual
    // -------------------------------------------------------
    public static void GuardarPosicionAntesDeTienda(Vector3 posicion)
    {
        SaveSpawnPosition(posicion);
    }
}