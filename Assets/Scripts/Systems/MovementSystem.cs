using System.Collections.Generic;

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
        character.transform.position = gridSystem.GetCharacterWorldPosition(targetPosition);

        remainingMovement[character]--;
        return true;
    }

    public int GetRemainingMovement(Character character) {
        return remainingMovement.ContainsKey(character) ? remainingMovement[character] : 0;
    }

    public void EndCharacterTurn(Character character) {
        if (remainingMovement.ContainsKey(character))
            remainingMovement[character] = 0;
    }
}