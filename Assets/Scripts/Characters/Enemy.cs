using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Enemy : Character {
    [Header("Enemy Settings")]
    [SerializeField] private float turnDelay = 1f;

    private GridSystem gridSystem;
    private GameManager gameManager;
    private bool isExecutingTurn;

    protected override void Start() {
        base.Start();
        gridSystem = FindFirstObjectByType<GridSystem>();
        gameManager = FindFirstObjectByType<GameManager>();
    }

    public override void ExecuteTurn() {
        if (isDead || isExecutingTurn) return;

        isExecutingTurn = true;
        Invoke(nameof(ExecuteAITurn), turnDelay);
    }

    private void ExecuteAITurn() {
        if (isDead) {
            isExecutingTurn = false;
            return;
        }

        if (!hasMovedThisTurn) {
            ExecuteRandomMove();
        }

        if (!hasActedThisTurn) {
            ExecuteAttackAction();
        }

        isExecutingTurn = false;
        EndTurn();
    }

    private void ExecuteRandomMove() {
        if (gridSystem == null) return;

        List<GridCell> validMoves = gridSystem.GetValidMovePositions(currentPosition, stats.speed);

        if (validMoves.Count > 0) {
            GridCell randomMove = validMoves[Random.Range(0, validMoves.Count)];
            gridSystem.SetCharacterPosition(this, randomMove);
            MoveTo(randomMove);
            transform.position = gridSystem.GetWorldPosition(randomMove);
        }
        else {
            hasMovedThisTurn = true;
        }
    }

    private void ExecuteAttackAction() {
        Character targetPlayer = FindNearestPlayer();

        if (targetPlayer != null && CanAttack(targetPlayer)) {
            Attack(targetPlayer);
        }
        else {
            hasActedThisTurn = true;
        }
    }

    private Character FindNearestPlayer() {
        if (gameManager == null) return null;

        List<Character> alivePlayers = gameManager.GetAllPlayers().Where(p => !p.IsDead).ToList();

        if (alivePlayers.Count == 0) return null;

        List<Character> nearestPlayers = new List<Character>();
        int minDistance = int.MaxValue;

        foreach (var player in alivePlayers) {
            int distance = currentPosition.GetManhattanDistance(player.CurrentPosition);

            if (distance < minDistance) {
                minDistance = distance;
                nearestPlayers.Clear();
                nearestPlayers.Add(player);
            }
            else if (distance == minDistance) {
                nearestPlayers.Add(player);
            }
        }

        return nearestPlayers.Count > 0 ? nearestPlayers[Random.Range(0, nearestPlayers.Count)] : null;
    }

    public override void EndTurn() {
        base.EndTurn();
        isExecutingTurn = false;
    }
}