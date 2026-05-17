using UnityEngine;

public class BotonSalir : MonoBehaviour
{
    public void SalirDelJuego()
    {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }
}
