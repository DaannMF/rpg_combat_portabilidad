using System;
using System.Collections.Generic;

[Serializable]
public class ActionGroup {
    public ActionType actionType;
    public string displayName;
    public List<BaseCharacter> availableTargets;
    public bool isAvailable;

    public ActionGroup(ActionType type) {
        actionType = type;
        displayName = type.GetDisplayName();
        availableTargets = new List<BaseCharacter>();
        isAvailable = false;
    }

    public void AddTarget(BaseCharacter target) {
        if (target != null && !availableTargets.Contains(target)) {
            availableTargets.Add(target);
            isAvailable = true;
        }
    }
}