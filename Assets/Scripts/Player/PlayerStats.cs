using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [Header("Configuración")]
    public string nombreJugador = "Ryo"; 
    public int nivel = 1;
    public int experiencia = 0;
    public int oro = 0;

    [Header("Vida y Magia")]
    public int pvActuales = 20;
    public int pvMaximos = 20;
    public int mpActual = 10; 
    public int mpMaximo = 10;
    
    [Header("Combate")]
    public int ataque = 5;
    public int defensa = 3;

    private void Awake()
    {
        pvActuales = pvMaximos;
        mpActual = mpMaximo;
    }
}