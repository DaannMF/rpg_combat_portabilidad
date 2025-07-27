using System;
using System.Collections.Generic;
using System.Linq;

public class PlayerActionController {
    private IPlayerAbilitySystem abilitySystem;
    private MovementSystem movementSystem;
    private List<BaseCharacter> allCharacters;

    public event Action<List<PlayerAction>> OnActionsUpdated;
    public event Action<List<ActionGroup>> OnActionGroupsUpdated;
    public event Action<int> OnMovementUpdated;
    public event Action<PlayerAction> OnActionExecuted;

    public PlayerActionController(IPlayerAbilitySystem abilitySystem, MovementSystem movementSystem) {
        this.abilitySystem = abilitySystem;
        this.movementSystem = movementSystem;
    }

    public void Initialize(List<BaseCharacter> allCharacters) {
        this.allCharacters = allCharacters;
    }

    public void StartPlayerTurn(BaseCharacter player) {
        if (!player.CharacterType.IsPlayer()) return;

        movementSystem.StartCharacterTurn(player);
        OnMovementUpdated?.Invoke(movementSystem.GetRemainingMovement(player));
        RefreshAvailableActions(player);
    }

    public void RefreshAvailableActions(BaseCharacter player) {
        if (!player.CharacterType.IsPlayer()) return;

        var availableActions = abilitySystem.GetAvailableActions(player, allCharacters);

        var validActions = new List<PlayerAction>();
        foreach (var action in availableActions) {
            action.isAvailable = abilitySystem.CanPerformAction(player, action);
            validActions.Add(action);
        }

        OnActionsUpdated?.Invoke(validActions);

        var groupedActions = GroupActionsByType(validActions);
        OnActionGroupsUpdated?.Invoke(groupedActions);
    }

    private List<ActionGroup> GroupActionsByType(List<PlayerAction> actions) {
        var groups = new Dictionary<ActionType, ActionGroup>();

        foreach (var action in actions) {
            if (action.actionType == ActionType.EndTurn) continue;

            if (!groups.ContainsKey(action.actionType))
                groups[action.actionType] = new ActionGroup(action.actionType);


            if (action.isAvailable) {
                if (action.actionType == ActionType.HealSelf)
                    groups[action.actionType].isAvailable = true;

                else if (action.target != null)
                    groups[action.actionType].AddTarget(action.target);
            }
        }

        return groups.Values.ToList();
    }

    public bool TryExecuteAction(BaseCharacter player, PlayerAction action) {
        if (!player.CharacterType.IsPlayer()) return false;

        if (abilitySystem.ExecuteAction(player, action)) {
            OnActionExecuted?.Invoke(action);

            if (action.actionType == ActionType.EndTurn) {
                player.EndTurn();
                movementSystem.EndCharacterTurn(player);
                OnMovementUpdated?.Invoke(0);
            }
            else {
                player.EndTurn();
                movementSystem.EndCharacterTurn(player);
                OnMovementUpdated?.Invoke(0);
            }

            return true;
        }

        return false;
    }

    public bool TryExecuteActionOnTarget(BaseCharacter player, ActionType actionType, BaseCharacter target) {
        if (!player.CharacterType.IsPlayer()) return false;

        var action = new PlayerAction(actionType, target);
        return TryExecuteAction(player, action);
    }

    public bool TryMovePlayer(BaseCharacter player, GridCell targetPosition) {
        if (!player.CharacterType.IsPlayer()) return false;

        if (movementSystem.MoveCharacter(player, targetPosition)) {
            OnMovementUpdated?.Invoke(movementSystem.GetRemainingMovement(player));
            RefreshAvailableActions(player);
            return true;
        }

        return false;
    }

    public bool TryMovePlayerToPosition(BaseCharacter player, GridCell targetPosition) {
        if (!player.CharacterType.IsPlayer()) return false;

        if (movementSystem.MoveCharacterToPosition(player, targetPosition)) {
            OnMovementUpdated?.Invoke(movementSystem.GetRemainingMovement(player));
            RefreshAvailableActions(player);
            return true;
        }

        return false;
    }
}