using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Enemy : BaseCharacter {
    const short IDEAL_DISTANCE_TO_PLAYER = 1;

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

        if (!hasMovedThisTurn)
            ExecuteRandomMove();

        if (!hasActedThisTurn)
            ExecuteAttackAction();

        isExecutingTurn = false;
        EndTurn();
    }

    private void ExecuteRandomMove() {
        if (gridSystem == null) return;

        List<GridCell> validMoves = gridSystem.GetValidMovePositions(currentPosition, stats.speed);

        if (validMoves.Count > 0) {
            GridCell bestMove = FindBestMovePosition(validMoves);
            if (bestMove != null) {
                gridSystem.SetCharacterPosition(this, bestMove);
                MoveTo(bestMove);
                transform.position = gridSystem.GetCharacterWorldPosition(bestMove);
            }
            else
                hasMovedThisTurn = true;

        }
        else
            hasMovedThisTurn = true;
    }

    private GridCell FindBestMovePosition(List<GridCell> validMoves) {
        if (gameManager == null) return validMoves[Random.Range(0, validMoves.Count)];

        var players = gameManager.GetAllPlayers().Where(p => !p.IsDead).ToList();
        if (players.Count == 0) return validMoves[Random.Range(0, validMoves.Count)];

        GridCell bestMove = null;
        int shortestDistance = int.MaxValue;

        foreach (var move in validMoves) {
            foreach (var player in players) {
                int distance = move.GetChebyshevDistance(player.CurrentPosition);

                if (distance == IDEAL_DISTANCE_TO_PLAYER) return move;

                if (distance < shortestDistance) {
                    shortestDistance = distance;
                    bestMove = move;
                }
            }
        }

        return bestMove ?? validMoves[Random.Range(0, validMoves.Count)];
    }

    private void ExecuteAttackAction() {
        BaseCharacter targetPlayer = FindNearestPlayer();

        if (targetPlayer != null && CanAttack(targetPlayer)) {
            int distance = currentPosition.GetChebyshevDistance(targetPlayer.CurrentPosition);
            int damage = stats.GetAttackDamage(distance);
            targetPlayer.TakeDamage(damage);
            hasActedThisTurn = true;
        }
        else {
            hasActedThisTurn = true;
        }
    }

    private BaseCharacter FindNearestPlayer() {
        if (gameManager == null) return null;

        List<BaseCharacter> alivePlayers = gameManager.GetAllPlayers().Where(p => !p.IsDead).ToList();
        if (alivePlayers.Count == 0) return null;

        // First, try to find players in attack range
        var attackableTargets = new List<BaseCharacter>();
        foreach (var player in alivePlayers)
            if (CanAttack(player))
                attackableTargets.Add(player);

        if (attackableTargets.Count > 0)
            // Prioritize targets by health (attack weakest first)
            return attackableTargets.OrderBy(p => p.CurrentHealth).First();


        // If no attackable targets, find nearest player
        BaseCharacter nearestPlayer = null;
        int minDistance = int.MaxValue;

        foreach (var player in alivePlayers) {
            int distance = currentPosition.GetChebyshevDistance(player.CurrentPosition);
            if (distance < minDistance) {
                minDistance = distance;
                nearestPlayer = player;
            }
        }

        return nearestPlayer;
    }

    public override void EndTurn() {
        base.EndTurn();
        isExecutingTurn = false;
    }
}