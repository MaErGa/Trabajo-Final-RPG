using UnityEngine;

[CreateAssetMenu(fileName = "NuevoJugador", menuName = "RPG/Jugador")]
public class DatosJugador : ScriptableObject
{
    public string nombre;
    public int nivel = 1;
    public int hpMax = 20;
    public int hpActual;
    public int mpMax = 5;
    public int mpActual;
    public int fuerza = 8;
    public int agilidad = 6;
    public int defensa = 2;
    public int oro;
    public int experiencia;

    [Header("Atributos Mágicos")]
    public int fuerzaMagica = 5;
    public int terapeucidad = 4;

    [Header("Equipación")]
    public string armaEquipada = "Espada de cobre";
    public int poderArma = 10;
    public string armaduraEquipada = "Ropa de viaje";
    public int poderArmadura = 4;
    public string escudoEquipado = "Escudo de cuero";
    public int poderEscudo = 2;
    public string accesorioEquipado = "Ninguno";
    public int poderAccesorio = 0;

    [Header("Sistema de Niveles")]
    public int expSiguienteNivel = 14;
    public int[] tablaExpPilgrim = { 14, 42, 98, 182, 308, 497, 780, 1205, 1842, 2798 };

    [Header("Inventario")]
    public int plantasMedicinales;
    public int colaDeConejo; // Corregido a singular

    [ContextMenu("Reiniciar a Nivel 1")]
    public void ReiniciarPersonaje()
    {
        nivel = 1; experiencia = 0; oro = 0;
        hpMax = 20; hpActual = 20;
        mpMax = 5; mpActual = 5;
        fuerza = 8; defensa = 2; agilidad = 6;
        fuerzaMagica = 5; terapeucidad = 4;
        plantasMedicinales = 0;
        colaDeConejo = 0;
        
        if (tablaExpPilgrim.Length > 0) expSiguienteNivel = tablaExpPilgrim[0];

        #if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(this);
        #endif
    }
}