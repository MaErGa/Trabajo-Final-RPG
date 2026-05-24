using UnityEngine;

// Coloca este script en un GameObject vacío en el punto donde debe aparecer
// el jugador al llegar desde una escena concreta.
// Ejemplo: en Underworld, pon uno llamado "EntradaDesdeBosque" con escenaOrigen = "Bosque"
public class EntradaEscena : MonoBehaviour
{
    [Tooltip("Nombre exacto de la escena desde la que viene el jugador")]
    public string escenaOrigen;

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, 0.5f);
        Gizmos.DrawIcon(transform.position, "sv_label_1");
    }
}
