using UnityEngine;

public class KeyboardInputSystem : BaseInputSystem {
    const short MOVEMENT_SPEED = 1;
    private GridSystem gridSystem;
    private bool isWaitingForInput;

    public override void Initialize(PlayerActionController controller) {
        base.Initialize(controller);
        gridSystem = Object.FindFirstObjectByType<GridSystem>();
    }

    public override void SetActivePlayer(BaseCharacter player) {
        base.SetActivePlayer(player);
        isWaitingForInput = isPlayerTurn;
    }

    public override void EndPlayerTurn() {
        base.EndPlayerTurn();
        isWaitingForInput = false;
    }

    public override void HandleInput() {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            UIEvents.OnGamePaused?.Invoke();
            return;
        }

        if (!isPlayerTurn || !isWaitingForInput || currentActivePlayer == null || currentActivePlayer.IsDead)
            return;

        if (actionController == null) return;

        GridCell targetPosition = null;
        GridCell currentPos = currentActivePlayer.CurrentPosition;

        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow)) {
            targetPosition = gridSystem.GetGridCell(currentPos.x, currentPos.y + MOVEMENT_SPEED);
        }
        else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow)) {
            targetPosition = gridSystem.GetGridCell(currentPos.x, currentPos.y - MOVEMENT_SPEED);
        }
        else if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow)) {
            targetPosition = gridSystem.GetGridCell(currentPos.x - MOVEMENT_SPEED, currentPos.y);
        }
        else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow)) {
            targetPosition = gridSystem.GetGridCell(currentPos.x + MOVEMENT_SPEED, currentPos.y);
        }

        if (targetPosition != null && !targetPosition.Equals(currentPos))
            actionController.TryMovePlayer(currentActivePlayer, targetPosition);
    }
}