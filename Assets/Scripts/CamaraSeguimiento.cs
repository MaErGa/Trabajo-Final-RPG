using UnityEngine;

public class CamaraSigue : MonoBehaviour
{
    public Transform objetivo;

    void Start()
    {
        Debug.Log("¡El script de la cámara se ha iniciado correctamente!");
    }

    void LateUpdate()
    {
        if (objetivo != null)
        {
            transform.position = new Vector3(objetivo.position.x, objetivo.position.y, -10f);
        }
        else
        {
            // Si el objetivo es nulo, esto saldrá en amarillo en tu consola
            Debug.LogWarning("La cámara no tiene a quién seguir. Arrastra al jugador al script.");
        }
    }
}