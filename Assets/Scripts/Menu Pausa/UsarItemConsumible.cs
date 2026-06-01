using UnityEngine;

public static class UsarItemConsumible
{
    public static bool UsarItem(ItemConsumible item, DatosJugador datos)
    {
        if (item == null || datos == null) return false;

        switch (item.queCura)
        {
            case TipoEfecto.Vida:
                if (datos.hpActual >= datos.hpMax) return false;
                datos.hpActual = Mathf.Min(datos.hpActual + item.potencia, datos.hpMax);
                return true;

            case TipoEfecto.Mana:
                if (datos.mpActual >= datos.mpMax) return false;
                datos.mpActual = Mathf.Min(datos.mpActual + item.potencia, datos.mpMax);
                return true;

            case TipoEfecto.Antidoto:
                // Aquí puedes añadir lógica para curar veneno cuando lo implementes
                // Por ahora devuelve true para que se consuma el item
                return true;

            default:
                Debug.LogWarning("[UsarItemConsumible] TipoEfecto no reconocido: " + item.queCura);
                return false;
        }
    }
}