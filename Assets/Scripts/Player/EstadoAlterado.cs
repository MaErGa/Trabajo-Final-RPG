/// <summary>
/// Estados alterados que puede sufrir el jugador en combate.
/// Basados en el diseño DQ11 estilo Pokémon.
/// </summary>
public enum EstadoAlterado
{
    Normal,
    Envenenado,   // Resta 5-8 HP al final de cada turno del jugador
    Dormido,      // No puede actuar. 50% de despertar al recibir golpe físico
    Paralizado    // Agilidad reducida al 50%. Nunca pierde el turno
}
