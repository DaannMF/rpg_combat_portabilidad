using System.Collections.Generic;
using UnityEngine;

public class MovementSystem {
    private GridSystem gridSystem;
    private Dictionary<BaseCharacter, int> remainingMovement;

    public MovementSystem(GridSystem gridSystem) {
        this.gridSystem = gridSystem;
        remainingMovement = new Dictionary<BaseCharacter, int>();
    }

    public void StartCharacterTurn(BaseCharacter character) {
        remainingMovement[character] = character.Stats.speed;
    }

    public bool CanMove(BaseCharacter character, GridCell targetPosition) {
        if (!remainingMovement.ContainsKey(character)) return false;
        if (remainingMovement[character] <= 0) return false;
        if (character.IsDead) return false;

        int distance = character.CurrentPosition.GetManhattanDistance(targetPosition);
        if (distance != 1) return false;

        return gridSystem.CanMoveToPosition(targetPosition);
    }

    public bool CanMoveToPosition(BaseCharacter character, GridCell targetPosition) {
        if (!remainingMovement.ContainsKey(character)) return false;
        if (remainingMovement[character] <= 0) return false;
        if (character.IsDead) return false;
        if (targetPosition == null) return false;

        if (gridSystem.IsPositionOccupied(targetPosition)) return false;

        return Pathfinding.HasValidPath(gridSystem, character.CurrentPosition, targetPosition, remainingMovement[character]);
    }

    public bool MoveCharacter(BaseCharacter character, GridCell targetPosition) {
        if (!CanMove(character, targetPosition)) return false;

        gridSystem.SetCharacterPosition(character, targetPosition);
        character.SetPosition(targetPosition);
        character.transform.position = gridSystem.GetCharacterWorldPosition(targetPosition);

        remainingMovement[character]--;
        return true;
    }

    public bool MoveCharacterToPosition(BaseCharacter character, GridCell targetPosition) {
        if (!CanMoveToPosition(character, targetPosition)) return false;

        List<GridCell> path = Pathfinding.FindPath(gridSystem, character.CurrentPosition, targetPosition, remainingMovement[character]);

        if (path == null || path.Count == 0) return false;

        gridSystem.SetCharacterPosition(character, targetPosition);
        character.SetPosition(targetPosition);
        character.transform.position = gridSystem.GetCharacterWorldPosition(targetPosition);

        int movementCost = path.Count;
        remainingMovement[character] = Mathf.Max(0, remainingMovement[character] - movementCost);

        return true;
    }

    public List<GridCell> GetValidMovePositions(BaseCharacter character) {
        if (!remainingMovement.ContainsKey(character) || remainingMovement[character] <= 0) {
            return new List<GridCell>();
        }

        return Pathfinding.GetValidMovePositions(gridSystem, character.CurrentPosition, remainingMovement[character]);
    }

    public int GetMovementCost(BaseCharacter character, GridCell targetPosition) {
        if (character == null || targetPosition == null) return -1;

        List<GridCell> path = Pathfinding.FindPath(gridSystem, character.CurrentPosition, targetPosition, remainingMovement[character]);
        return path?.Count ?? -1;
    }

    public int GetRemainingMovement(BaseCharacter character) {
        return remainingMovement.ContainsKey(character) ? remainingMovement[character] : 0;
    }

    public void EndCharacterTurn(BaseCharacter character) {
        if (remainingMovement.ContainsKey(character))
            remainingMovement[character] = 0;
    }
}