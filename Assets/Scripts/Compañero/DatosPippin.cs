using UnityEngine;

[CreateAssetMenu(fileName = "DatosPippin", menuName = "RPG/Compañero Pippin")]
public class DatosPippin : ScriptableObject
{
    [Header("Info")]
    public string nombre = "Pippin";
    public Sprite spritePersonaje;

    [Header("Stats Base")]
    public int nivel = 1;
    public int hpMax = 35;
    public int hpActual = 35;
    public int mpMax = 12;
    public int mpActual = 12;
    public int fuerza = 10;
    public int defensa = 6;       // Tanque moderado
    public int fuerzaMagica = 4;
    public int terapeucidad = 5;

    [Header("Equipo fijo (Espada de Cobre, Escudo de Cuero, Armadura de Cuero)")]
    public EquipoBase armaEquipada;
    public EquipoBase escudoEquipado;
    public EquipoBase armaduraEquipada;

    [Header("Conjuros (los mismos que el jugador)")]
    public ConjuroBase conjuroMinicuracion;    // nivel 3
    public ConjuroBase conjuroFortalecimiento; // nivel 5
    public ConjuroBase conjuroMinihelada;      // nivel 8

    [Header("Bonos Temporales")]
    public int bonoDefensaTemporal;

    // Propiedades calculadas
    public int AtaqueTotal => fuerza + (armaEquipada != null ? armaEquipada.bonoAtaque : 0);
    public int DefensaTotal => defensa + bonoDefensaTemporal +
                               (armaduraEquipada != null ? armaduraEquipada.bonoDefensa : 0) +
                               (escudoEquipado != null ? escudoEquipado.bonoDefensa : 0);

    public void ResetearBonos()
    {
        bonoDefensaTemporal = 0;
    }

    // Al terminar combate: si está vivo o caído, se recupera a 1 HP mínimo
    public void RecuperarPostCombate()
    {
        if (hpActual <= 0) hpActual = 1;
        // Recupera algo de MP también
        mpActual = Mathf.Min(mpActual + 3, mpMax);
        ResetearBonos();
#if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(this);
#endif
    }
}
