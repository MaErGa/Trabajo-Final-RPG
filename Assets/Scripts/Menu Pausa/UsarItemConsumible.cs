using UnityEngine;

/// <summary>
/// Lógica para aplicar el efecto de un ItemConsumible al jugador.
/// Llama a UsarItem(item, datos) desde el panel de inventario.
/// </summary>
public static class UsarItemConsumible
{
    /// <summary>
    /// Aplica el efecto del item a los datos del jugador.
    /// Devuelve true si se pudo usar (condición no al máximo, etc.)
    /// </summary>
    public static bool UsarItem(ItemConsumible item, DatosJugador datos)
    {
        if (item == null || datos == null) return false;

        switch (item.queCura)
        {
            case TipoEfecto.Vida:
                if (datos.hpActual >= datos.hpMax) return false;
                datos.hpActual = Mathf.Min(datos.hpActual + item.potencia, datos.hpMax);
                break;

            case TipoEfecto.Mana:
                if (datos.mpActual >= datos.mpMax) return false;
                datos.mpActual = Mathf.Min(datos.mpActual + item.potencia, datos.mpMax);
                break;

            case TipoEfecto.Antidoto:
                // Aquí puedes añadir lógica de curar veneno cuando la tengas
                // Por ahora simplemente devuelve true para que se consuma
                break;

            default:
                return false;
        }

#if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(datos);
#endif
        return true;
    }
}