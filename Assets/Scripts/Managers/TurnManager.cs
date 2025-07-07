using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TurnManager : MonoBehaviour {
    [Header("Turn Settings")]
    [SerializeField] private float turnTransitionDelay = 0.5f;

    private List<Character> allCharacters = new List<Character>();
    private int currentTurnIndex = 0;
    private bool isProcessingTurn = false;
    private bool isGameStopped = false;
    private PlayerActionController actionController;

    public Character CurrentCharacter { get; private set; }

    public event Action<Character> OnTurnStart;
    public event Action<Character> OnTurnEnd;

    private void Update() {
        if (isGameStopped) return;

        if (isProcessingTurn && CurrentCharacter != null) {
            if (CurrentCharacter.HasFinishedTurn) {
                EndCurrentTurn();
            }
            else if (CurrentCharacter is Player player && Input.GetKeyDown(KeyCode.Return)) {
                ForceEndTurn();
            }
        }
    }

    private void OnDestroy() {
        foreach (var character in allCharacters) {
            if (character != null) {
                character.OnCharacterDeath -= OnCharacterDeath;
            }
        }
    }

    public void Initialize(List<Character> characters) {
        allCharacters = characters.ToList();
        currentTurnIndex = -1;
        isProcessingTurn = false;
        isGameStopped = false;

        foreach (var character in allCharacters) {
            character.OnCharacterDeath += OnCharacterDeath;
        }
    }

    public void SetActionController(PlayerActionController controller) {
        actionController = controller;
    }

    public void ResetGame() {
        isGameStopped = false;
        isProcessingTurn = false;
        CurrentCharacter = null;
        currentTurnIndex = -1;

        CancelInvoke(nameof(StartNextTurn));
    }

    public void StartGame() {
        if (allCharacters.Count == 0 || isGameStopped) return;

        StartNextTurn();
    }

    public void StartNextTurn() {
        if (isProcessingTurn || isGameStopped) return;

        FindNextValidCharacter();

        if (CurrentCharacter == null) {
            EndRound();
            return;
        }

        isProcessingTurn = true;
        CurrentCharacter.StartTurn();
        OnTurnStart?.Invoke(CurrentCharacter);

        if (CurrentCharacter is Player player) {
            if (actionController != null)
                actionController.StartPlayerTurn(player);

            player.ExecuteTurn();
        }
        else if (CurrentCharacter is Enemy enemy) {
            enemy.ExecuteTurn();
        }
    }

    public void EndCurrentTurn() {
        if (!isProcessingTurn || CurrentCharacter == null || isGameStopped) return;

        Character finishedCharacter = CurrentCharacter;
        finishedCharacter.EndTurn();
        OnTurnEnd?.Invoke(finishedCharacter);

        isProcessingTurn = false;

        if (finishedCharacter is Player player)
            player.SetAsCurrentPlayer(false);

        if (!isGameStopped)
            Invoke(nameof(StartNextTurn), turnTransitionDelay);
    }

    private void FindNextValidCharacter() {
        int attempts = 0;
        int maxAttempts = allCharacters.Count;

        while (attempts < maxAttempts) {
            currentTurnIndex = (currentTurnIndex + 1) % allCharacters.Count;
            Character character = allCharacters[currentTurnIndex];

            if (!character.IsDead) {
                CurrentCharacter = character;
                return;
            }

            attempts++;
        }

        CurrentCharacter = null;
    }

    private void OnCharacterDeath(Character deadCharacter) {
        if (deadCharacter == CurrentCharacter && isProcessingTurn)
            EndCurrentTurn();
    }

    private void EndRound() {
        if (isGameStopped) return;

        bool anyPlayersAlive = allCharacters.Any(c => c.CharacterType.IsPlayer() && !c.IsDead);
        bool anyEnemiesAlive = allCharacters.Any(c => c.CharacterType.IsEnemy() && !c.IsDead);

        if (!anyPlayersAlive || !anyEnemiesAlive)
            return;

        currentTurnIndex = -1;
        StartNextTurn();
    }

    public void ForceEndTurn() {
        if (isGameStopped) return;

        if (CurrentCharacter is Player player)
            player.EndTurn();

        EndCurrentTurn();
    }

    public void StopGame() {
        isGameStopped = true;
        isProcessingTurn = false;
        CurrentCharacter = null;

        CancelInvoke(nameof(StartNextTurn));

        foreach (var character in allCharacters)
            if (character is Player player)
                player.SetAsCurrentPlayer(false);
    }
}