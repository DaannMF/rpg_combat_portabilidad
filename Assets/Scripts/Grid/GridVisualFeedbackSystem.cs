using System.Collections.Generic;
using UnityEngine;

public class GridVisualFeedbackSystem {
    private GridSystem gridSystem;
    private MovementSystem movementSystem;
    private BaseCharacter currentActiveCharacter;
    private List<GridCell> highlightedCells = new List<GridCell>();

    public GridVisualFeedbackSystem(GridSystem gridSystem, MovementSystem movementSystem) {
        this.gridSystem = gridSystem;
        this.movementSystem = movementSystem;
    }

    public void SetActiveCharacter(BaseCharacter character) {
        ClearAllHighlights();
        currentActiveCharacter = character;

        if (character != null && !character.IsDead) {
            HighlightValidMoves(character);
            HighlightCurrentPosition(character);
        }
    }

    private void HighlightValidMoves(BaseCharacter character) {
        List<GridCell> validPositions = movementSystem.GetValidMovePositions(character);

        foreach (GridCell cell in validPositions) {
            cell.SetAsValidMove();
            highlightedCells.Add(cell);
        }

        HighlightInvalidCells(character);
    }

    private void HighlightInvalidCells(BaseCharacter character) {
        int maxDistance = movementSystem.GetRemainingMovement(character);

        for (int x = character.CurrentPosition.x - maxDistance; x <= character.CurrentPosition.x + maxDistance; x++) {
            for (int y = character.CurrentPosition.y - maxDistance; y <= character.CurrentPosition.y + maxDistance; y++) {
                if (!gridSystem.IsValidPosition(x, y)) continue;

                GridCell cell = gridSystem.GetGridCell(x, y);
                if (cell == null || cell.Equals(character.CurrentPosition)) continue;

                if (gridSystem.IsPositionOccupied(cell)) {
                    int distance = character.CurrentPosition.GetChebyshevDistance(cell);
                    if (distance <= maxDistance) {
                        cell.SetAsInvalidMove();
                        highlightedCells.Add(cell);
                    }
                }
            }
        }
    }

    private void HighlightCurrentPosition(BaseCharacter character) {
        if (character.CurrentPosition != null) {
            character.CurrentPosition.SetAsCurrentPlayer();
            highlightedCells.Add(character.CurrentPosition);
        }
    }

    public void ClearAllHighlights() {
        foreach (GridCell cell in highlightedCells)
            if (cell != null) cell.ResetToDefault();

        highlightedCells.Clear();
    }

    public bool IsValidMoveForCurrentCharacter(GridCell targetCell) {
        if (currentActiveCharacter == null || targetCell == null) return false;

        return movementSystem.CanMoveToPosition(currentActiveCharacter, targetCell);
    }

    public void UpdateAfterMovement() {
        if (currentActiveCharacter != null) {
            SetActiveCharacter(currentActiveCharacter);
        }
    }

    public void EndTurn() {
        ClearAllHighlights();
        currentActiveCharacter = null;
    }
}