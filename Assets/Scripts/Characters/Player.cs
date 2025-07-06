using UnityEngine;

public class Player : Character {
    [Header("Player Settings")]
    [SerializeField] private bool isCurrentPlayer;

    private GridSystem gridSystem;
    private bool isWaitingForInput;
    private GameManager gameManager;

    public bool IsCurrentPlayer => isCurrentPlayer;
    public bool IsWaitingForInput => isWaitingForInput;

    protected override void Start() {
        base.Start();
        gridSystem = FindFirstObjectByType<GridSystem>();
        gameManager = FindFirstObjectByType<GameManager>();
    }

    private void Update() {
        if (isCurrentPlayer && isWaitingForInput && !isDead) {
            HandleInput();
        }
    }

    private void HandleInput() {
        if (!hasMovedThisTurn) {
            HandleMovementInput();
        }
        else if (!hasActedThisTurn) {
            HandleActionInput();
        }
    }

    private void HandleMovementInput() {
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
        else if (Input.GetKeyDown(KeyCode.Space)) {
            hasMovedThisTurn = true;
            return;
        }

        if (targetPosition != null && !targetPosition.Equals(currentPosition) && CanMoveTo(targetPosition) &&
            gridSystem.CanMoveToPosition(targetPosition)) {
            gridSystem.SetCharacterPosition(this, targetPosition);
            MoveTo(targetPosition);
            transform.position = gridSystem.GetWorldPosition(targetPosition);
        }
    }

    private void HandleActionInput() {
        if (Input.GetKeyDown(KeyCode.Alpha1)) {
            TryAttackNearestEnemy();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2) && stats.canHeal) {
            TryHealSelf();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3) && stats.canHealOthers) {
            TryHealNearestPlayer();
        }
        else if (Input.GetKeyDown(KeyCode.Space)) {
            hasActedThisTurn = true;
        }
    }

    private void TryAttackNearestEnemy() {
        Character nearestEnemy = FindNearestEnemy();
        if (nearestEnemy != null && CanAttack(nearestEnemy)) {
            Attack(nearestEnemy);
        }
    }

    private void TryHealSelf() {
        if (CanHeal(this)) {
            Heal(this);
        }
    }

    private void TryHealNearestPlayer() {
        Character nearestPlayer = FindNearestPlayer();
        if (nearestPlayer != null && CanHeal(nearestPlayer)) {
            Heal(nearestPlayer);
        }
    }

    private Character FindNearestEnemy() {
        if (gameManager == null) return null;

        Character nearestEnemy = null;
        int minDistance = int.MaxValue;

        foreach (var enemy in gameManager.GetAllEnemies()) {
            if (enemy.IsDead) continue;

            int distance = currentPosition.GetManhattanDistance(enemy.CurrentPosition);
            if (distance < minDistance) {
                minDistance = distance;
                nearestEnemy = enemy;
            }
        }

        return nearestEnemy;
    }

    private Character FindNearestPlayer() {
        if (gameManager == null) return null;

        Character nearestPlayer = null;
        int minDistance = int.MaxValue;

        foreach (var player in gameManager.GetAllPlayers()) {
            if (player.IsDead || player == this) continue;

            int distance = currentPosition.GetManhattanDistance(player.CurrentPosition);
            if (distance < minDistance) {
                minDistance = distance;
                nearestPlayer = player;
            }
        }

        return nearestPlayer;
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