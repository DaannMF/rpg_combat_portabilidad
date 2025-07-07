using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerActionController {
    private IPlayerAbilitySystem abilitySystem;
    private MovementSystem movementSystem;
    private List<Character> allCharacters;

    public event Action<List<PlayerAction>> OnActionsUpdated;
    public event Action<int> OnMovementUpdated;
    public event Action<PlayerAction> OnActionExecuted;

    public PlayerActionController(IPlayerAbilitySystem abilitySystem, MovementSystem movementSystem) {
        this.abilitySystem = abilitySystem;
        this.movementSystem = movementSystem;
    }

    public void Initialize(List<Character> allCharacters) {
        this.allCharacters = allCharacters;
    }

    public void StartPlayerTurn(Character player) {
        if (!player.CharacterType.IsPlayer()) return;

        movementSystem.StartCharacterTurn(player);
        OnMovementUpdated?.Invoke(movementSystem.GetRemainingMovement(player));
        RefreshAvailableActions(player);
    }

    public void RefreshAvailableActions(Character player) {
        if (!player.CharacterType.IsPlayer()) return;

        var availableActions = abilitySystem.GetAvailableActions(player, allCharacters);

        var validActions = new List<PlayerAction>();
        foreach (var action in availableActions) {
            action.isAvailable = abilitySystem.CanPerformAction(player, action);
            validActions.Add(action);
        }

        OnActionsUpdated?.Invoke(validActions);
    }

    public bool TryExecuteAction(Character player, PlayerAction action) {
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

    public bool TryMovePlayer(Character player, GridCell targetPosition) {
        if (!player.CharacterType.IsPlayer()) return false;

        if (movementSystem.MoveCharacter(player, targetPosition)) {
            OnMovementUpdated?.Invoke(movementSystem.GetRemainingMovement(player));
            RefreshAvailableActions(player);
            return true;
        }

        return false;
    }
}