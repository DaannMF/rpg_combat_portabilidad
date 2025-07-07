using UnityEngine;

[CreateAssetMenu(fileName = "BaseCharacterStats", menuName = "RPG Combat/Base Character Stats")]
public class BaseCharacterStats : ScriptableObject {
    [Header("Basic Stats")]
    public string characterName;
    public Sprite characterSprite;
    public int maxHealth;
    public int speed;

    [Header("Combat Stats")]
    public int meleeAttackDamage;
    public int rangedAttackDamage;
    public int maxRangedDistance;
    public bool canUseRangedAttack;

    [Header("Healing")]
    public int healingAmount;
    public bool canHeal;
    public bool canHealOthers;
    public int maxHealingDistance;

    public virtual bool CanAttackAtDistance(int distance) {
        return distance == 1 || (canUseRangedAttack && distance > 1 && distance <= maxRangedDistance);
    }

    public virtual int GetAttackDamage(int distance) {
        if (distance == 1)
            return meleeAttackDamage;
        else if (canUseRangedAttack && distance > 1 && distance <= maxRangedDistance)
            return rangedAttackDamage;
        else
            return 0;
    }
}