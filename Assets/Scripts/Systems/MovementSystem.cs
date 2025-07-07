using System.Collections.Generic;
using UnityEngine;

public class MovementSystem {
    private GridSystem gridSystem;
    private Dictionary<Character, int> remainingMovement;

    public MovementSystem(GridSystem gridSystem) {
        this.gridSystem = gridSystem;
        remainingMovement = new Dictionary<Character, int>();
    }

    public void StartCharacterTurn(Character character) {
        remainingMovement[character] = character.Stats.speed;
    }

    public bool CanMove(Character character, GridCell targetPosition) {
        if (!remainingMovement.ContainsKey(character)) return false;
        if (remainingMovement[character] <= 0) return false;
        if (character.IsDead) return false;

        int distance = character.CurrentPosition.GetChebyshevDistance(targetPosition);
        if (distance != 1) return false;

        return gridSystem.CanMoveToPosition(targetPosition);
    }

    public bool MoveCharacter(Character character, GridCell targetPosition) {
        if (!CanMove(character, targetPosition)) return false;

        gridSystem.SetCharacterPosition(character, targetPosition);
        character.SetPosition(targetPosition);
        character.transform.position = gridSystem.GetWorldPosition(targetPosition);

        remainingMovement[character]--;
        return true;
    }

    public List<GridCell> GetValidMovePositions(Character character) {
        var validPositions = new List<GridCell>();

        if (!remainingMovement.ContainsKey(character) || remainingMovement[character] <= 0)
            return validPositions;

        var currentPos = character.CurrentPosition;
        var directions = new Vector2Int[]
        {
            Vector2Int.left,
            Vector2Int.right,
            Vector2Int.up,
            Vector2Int.down,
        };

        foreach (var direction in directions) {
            var newPos = new Vector2Int(currentPos.x + direction.x, currentPos.y + direction.y);
            var targetCell = gridSystem.GetGridCell(newPos.x, newPos.y);

            if (targetCell != null && gridSystem.CanMoveToPosition(targetCell)) {
                validPositions.Add(targetCell);
            }
        }

        return validPositions;
    }

    public int GetRemainingMovement(Character character) {
        return remainingMovement.ContainsKey(character) ? remainingMovement[character] : 0;
    }

    public bool HasMovementLeft(Character character) {
        return GetRemainingMovement(character) > 0;
    }

    public void EndCharacterTurn(Character character) {
        if (remainingMovement.ContainsKey(character))
            remainingMovement[character] = 0;
    }
}