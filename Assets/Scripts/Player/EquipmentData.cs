using UnityEngine;

/// <summary>
/// ScriptableObject que representa una pieza de equipamiento.
/// Crea un asset: clic derecho en Project → Create → DQ3 / Equipment
/// </summary>
[CreateAssetMenu(fileName = "NewEquipment", menuName = "DQ3/Equipment")]
public class EquipmentData : ScriptableObject
{
    [Header("Información")]
    public string equipmentName;
    public EquipmentSlot slot;

    [Header("Bonificadores de Combate")]
    [Tooltip("Potencia de ataque del arma. Ej: Espada de Cobre = 7")]
    public int bonusAttack;

    [Tooltip("Bonus de defensa de la pieza. Ej: Escudo de Cuero = 4")]
    public int bonusDefense;

    [Tooltip("Bonus directo de Agilidad. Ej: Sandalias = 1")]
    public int bonusAgility;

    [Header("Bonificadores de Stats Primarios")]
    public int bonusStrength;
    public int bonusVitality;    // Resistencia
    public int bonusWisdom;      // Fuerza Mágica
    public int bonusLuck;        // Encanto
    public int bonusSkill;       // Pericia
    public int bonusTherapeutics;// Terapeucidad
    public int bonusStyle;       // Estilo

    public override string ToString() => $"{equipmentName} [{slot}]";
}

public enum EquipmentSlot
{
    Weapon,
    Shield,
    Head,
    Body,
    Footwear
}
