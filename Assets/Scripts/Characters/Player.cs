using UnityEngine;

public class Player : Character {
    [Header("Player Settings")]
    [SerializeField] private bool isCurrentPlayer;

    private GridSystem gridSystem;
    private bool isWaitingForInput;
    private PlayerActionController actionController;

    protected override void Start() {
        base.Start();
        gridSystem = FindFirstObjectByType<GridSystem>();
    }

    public void SetActionController(PlayerActionController actionController) {
        this.actionController = actionController;
    }

    private void Update() {
        if (isCurrentPlayer && isWaitingForInput && !isDead)
            HandleMovementInput();
    }

    private void HandleMovementInput() {
        if (actionController == null) return;

        GridCell targetPosition = null;

        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow)) {
            targetPosition = gridSystem.GetGridCell(currentPosition.x, currentPosition.y + 1);
        }
        else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow)) {
            targetPosition = gridSystem.GetGridCell(currentPosition.x, currentPosition.y - 1);
        }
        else if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow)) {
            targetPosition = gridSystem.GetGridCell(currentPosition.x - 1, currentPosition.y);
        }
        else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow)) {
            targetPosition = gridSystem.GetGridCell(currentPosition.x + 1, currentPosition.y);
        }

        if (targetPosition != null && !targetPosition.Equals(currentPosition))
            actionController.TryMovePlayer(this, targetPosition);
    }

    public override void ExecuteTurn() {
        if (isDead) return;

        isCurrentPlayer = true;
        isWaitingForInput = true;
    }

    public void SetAsCurrentPlayer(bool current) {
        isCurrentPlayer = current;
        isWaitingForInput = current;
    }

    public override void EndTurn() {
        base.EndTurn();
        isCurrentPlayer = false;
        isWaitingForInput = false;
    }
}