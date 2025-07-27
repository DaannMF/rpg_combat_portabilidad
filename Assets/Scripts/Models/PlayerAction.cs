using System;

[Serializable]
public class PlayerAction {
    public ActionType actionType;
    public string displayName;
    public bool isAvailable;
    public BaseCharacter target;
    public int requiredDistance;
    public bool requiresTarget;

    public PlayerAction(ActionType type, bool requiresTarget = false, int requiredDistance = 0) {
        actionType = type;
        displayName = type.GetDisplayName();
        isAvailable = false;
        target = null;
        this.requiredDistance = requiredDistance;
        this.requiresTarget = requiresTarget;
    }

    public PlayerAction(ActionType type, BaseCharacter target, int requiredDistance = 0) {
        actionType = type;
        displayName = type.GetDisplayName();
        isAvailable = true;
        this.target = target;
        this.requiredDistance = requiredDistance;
        requiresTarget = true;
    }
}