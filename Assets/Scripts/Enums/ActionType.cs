public enum ActionType {
    MeleeAttack,
    RangedAttack,
    HealSelf,
    HealOther,
    Move,
    EndTurn
}

public static class ActionTypeExtensions {
    public static string GetDisplayName(this ActionType actionType) {
        return actionType switch {
            ActionType.MeleeAttack => "Melee Attack",
            ActionType.RangedAttack => "Ranged Attack",
            ActionType.HealSelf => "Heal Self",
            ActionType.HealOther => "Heal Other",
            ActionType.Move => "Move",
            ActionType.EndTurn => "End Turn",
            _ => actionType.ToString()
        };
    }
}