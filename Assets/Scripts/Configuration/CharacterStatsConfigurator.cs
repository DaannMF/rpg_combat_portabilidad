using UnityEngine;

[System.Serializable]
public static class CharacterStatsConfigurator {
    public static BaseCharacterStats CreatePlayer1Stats() {
        var stats = ScriptableObject.CreateInstance<BaseCharacterStats>();
        stats.characterName = "Fighter";
        stats.maxHealth = 20;
        stats.speed = 3;
        stats.meleeAttackDamage = 5;
        stats.rangedAttackDamage = 0;
        stats.maxRangedDistance = 0;
        stats.canUseRangedAttack = false;
        stats.healingAmount = 2;
        stats.canHeal = true;
        stats.canHealOthers = false;
        stats.maxHealingDistance = 0;
        return stats;
    }

    public static BaseCharacterStats CreatePlayer2Stats() {
        var stats = ScriptableObject.CreateInstance<BaseCharacterStats>();
        stats.characterName = "Healer";
        stats.maxHealth = 15;
        stats.speed = 2;
        stats.meleeAttackDamage = 2;
        stats.rangedAttackDamage = 2;
        stats.maxRangedDistance = 3;
        stats.canUseRangedAttack = true;
        stats.healingAmount = 5;
        stats.canHeal = true;
        stats.canHealOthers = true;
        stats.maxHealingDistance = 2;
        return stats;
    }

    public static BaseCharacterStats CreatePlayer3Stats() {
        var stats = ScriptableObject.CreateInstance<BaseCharacterStats>();
        stats.characterName = "Ranger";
        stats.maxHealth = 15;
        stats.speed = 4;
        stats.meleeAttackDamage = 1;
        stats.rangedAttackDamage = 3;
        stats.maxRangedDistance = 99;
        stats.canUseRangedAttack = true;
        stats.healingAmount = 2;
        stats.canHeal = true;
        stats.canHealOthers = false;
        stats.maxHealingDistance = 0;
        return stats;
    }

    public static BaseCharacterStats CreateEnemyStats() {
        var stats = ScriptableObject.CreateInstance<BaseCharacterStats>();
        stats.characterName = "Enemy";
        stats.maxHealth = 10;
        stats.speed = 1;
        stats.meleeAttackDamage = 3;
        stats.rangedAttackDamage = 1;
        stats.maxRangedDistance = 3;
        stats.canUseRangedAttack = true;
        stats.healingAmount = 0;
        stats.canHeal = false;
        stats.canHealOthers = false;
        stats.maxHealingDistance = 0;
        return stats;
    }
}