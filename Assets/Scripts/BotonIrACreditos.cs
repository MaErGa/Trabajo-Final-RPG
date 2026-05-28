using UnityEngine;
using UnityEngine.SceneManagement;

public class BotonIrACreditos : MonoBehaviour
{
    // Función pública para asignar al botón en el Inspector
    public void CargarEscenaCreditos2()
    {
        // Carga exactamente la escena con el nombre que me has pedido
        SceneManager.LoadScene("Creditos2");
    }
}