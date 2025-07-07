using System.Collections.Generic;
using System.Linq;

public class PlayerAbilitySystem : IPlayerAbilitySystem {
    public List<PlayerAction> GetAvailableActions(Character character, List<Character> allCharacters) {
        var availableActions = new List<PlayerAction>();

        availableActions.AddRange(GetPlayerActions(character, allCharacters));
        availableActions.Add(new PlayerAction(ActionType.EndTurn));

        return availableActions;
    }

    private List<PlayerAction> GetPlayerActions(Character character, List<Character> allCharacters) {
        var actions = new List<PlayerAction>();

        if (character.Stats.canHeal) {
            actions.Add(new PlayerAction(ActionType.HealSelf));
        }

        var otherCharacters = allCharacters.Where(c => !c.IsDead && c != character).ToList();
        foreach (var target in otherCharacters) {
            int distance = character.CurrentPosition.GetChebyshevDistance(target.CurrentPosition);

            if (character.Stats.canHealOthers && distance <= character.Stats.maxHealingDistance) {
                actions.Add(new PlayerAction(ActionType.HealOther, target, distance));
            }

            if (distance == 1 && character.Stats.meleeAttackDamage > 0) {
                actions.Add(new PlayerAction(ActionType.MeleeAttack, target, distance));
            }

            if (character.Stats.canUseRangedAttack && distance > 1 && distance <= character.Stats.maxRangedDistance) {
                actions.Add(new PlayerAction(ActionType.RangedAttack, target, distance));
            }
        }

        return actions;
    }

    public bool CanPerformAction(Character character, PlayerAction action) {
        if (character.IsDead || character.HasFinishedTurn) return false;

        switch (action.actionType) {
            case ActionType.HealSelf:
                return character.Stats.canHeal && character.CurrentHealth < character.Stats.maxHealth;

            case ActionType.HealOther:
                return character.Stats.canHealOthers && action.target != null &&
                       !action.target.IsDead && action.target.CurrentHealth < action.target.Stats.maxHealth;

            case ActionType.MeleeAttack:
                return action.target != null && !action.target.IsDead &&
                       character.Stats.meleeAttackDamage > 0;

            case ActionType.RangedAttack:
                return character.Stats.canUseRangedAttack && action.target != null &&
                       !action.target.IsDead && character.Stats.rangedAttackDamage > 0;

            case ActionType.EndTurn:
                return true;

            default:
                return false;
        }
    }

    public bool ExecuteAction(Character character, PlayerAction action) {
        if (!CanPerformAction(character, action)) return false;

        switch (action.actionType) {
            case ActionType.HealSelf:
                character.RestoreHealth(character.Stats.healingAmount);
                break;

            case ActionType.HealOther:
                if (action.target != null)
                    action.target.RestoreHealth(character.Stats.healingAmount);
                break;

            case ActionType.MeleeAttack:
                if (action.target != null)
                    action.target.TakeDamage(character.Stats.meleeAttackDamage);
                break;

            case ActionType.RangedAttack:
                if (action.target != null)
                    action.target.TakeDamage(character.Stats.rangedAttackDamage);
                break;

            case ActionType.EndTurn:
                return true;

            default:
                return false;
        }

        return true;
    }
}