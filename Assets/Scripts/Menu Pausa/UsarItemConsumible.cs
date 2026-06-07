using UnityEngine;

public static class UsarItemConsumible
{
    public static bool UsarItem(ItemConsumible item, DatosJugador datos)
    {
        if (item == null || datos == null) return false;

        switch (item.queCura)
        {
            case ItemConsumible.TipoEfecto.Vida:
                if (datos.hpActual >= datos.hpMax) return false;
                datos.hpActual = Mathf.Min(datos.hpActual + item.potencia, datos.hpMax);
                return true;

            case ItemConsumible.TipoEfecto.Mana:
                if (datos.mpActual >= datos.mpMax) return false;
                datos.mpActual = Mathf.Min(datos.mpActual + item.potencia, datos.mpMax);
                return true;

            case ItemConsumible.TipoEfecto.Antidoto:
                return datos.CurarEstadoEspecifico(EstadoAlterado.Envenenado);

            case ItemConsumible.TipoEfecto.Antiparalisis:
                return datos.CurarEstadoEspecifico(EstadoAlterado.Paralizado);

            case ItemConsumible.TipoEfecto.Despertar:
                return datos.CurarEstadoEspecifico(EstadoAlterado.Dormido);

            default:
                Debug.LogWarning("[UsarItemConsumible] TipoEfecto no reconocido: " + item.queCura);
                return false;
        }
    }
}