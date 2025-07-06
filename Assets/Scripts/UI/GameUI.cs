using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameUI : MonoBehaviour {
    [Header("UI Elements")]
    [SerializeField] private TMP_Text turnInfoText;
    [SerializeField] private TMP_Text gameStatusText;
    [SerializeField] private TMP_Text characterStatsText;
    [SerializeField] private Button restartButton;
    [SerializeField] private GameObject gameOverPanel;

    private GameManager gameManager;
    private TurnManager turnManager;

    private void Start() {
        gameManager = FindFirstObjectByType<GameManager>();
        turnManager = FindFirstObjectByType<TurnManager>();

        if (gameManager != null) {
            gameManager.OnGameStateChanged += OnGameStateChanged;
            gameManager.OnGameWon += OnGameWon;
            gameManager.OnGameLost += OnGameLost;
        }

        if (turnManager != null) {
            turnManager.OnTurnStart += OnTurnStart;
            turnManager.OnTurnEnd += OnTurnEnd;
        }

        if (restartButton != null) {
            restartButton.onClick.AddListener(RestartGame);
        }

        InitializeUI();
    }

    private void InitializeUI() {
        if (gameOverPanel != null) {
            gameOverPanel.SetActive(false);
        }

        UpdateGameStatusText("Game Starting...");
    }

    private void Update() {
        UpdateCharacterStatsText();
    }

    private void OnTurnStart(Character character) {
        UpdateTurnInfoText($"{character.CharacterType}'s Turn");

        if (character is Player) {
            UpdateGameStatusText("Your turn! Move with WASD, then choose an action.");
        }
        else {
            UpdateGameStatusText("Enemy is thinking...");
        }
    }

    private void OnTurnEnd(Character character) {
        UpdateTurnInfoText($"{character.CharacterType} finished turn");
    }

    private void OnGameStateChanged(GameState newState) {
        switch (newState) {
            case GameState.NotStarted:
                UpdateGameStatusText("Game Not Started");
                break;
            case GameState.Playing:
                UpdateGameStatusText("Game in Progress");
                break;
            case GameState.Won:
                UpdateGameStatusText("Victory!");
                break;
            case GameState.Lost:
                UpdateGameStatusText("Defeat!");
                break;
        }
    }

    private void OnGameWon() {
        ShowGameOverPanel("VICTORY!", "All enemies defeated!");
    }

    private void OnGameLost() {
        ShowGameOverPanel("DEFEAT!", "All players died!");
    }

    private void ShowGameOverPanel(string title, string message) {
        if (gameOverPanel != null) {
            gameOverPanel.SetActive(true);

            TMP_Text titleText = gameOverPanel.transform.Find("Title")?.GetComponent<TMP_Text>();
            if (titleText != null) {
                titleText.text = title;
            }

            TMP_Text messageText = gameOverPanel.transform.Find("Message")?.GetComponent<TMP_Text>();
            if (messageText != null) {
                messageText.text = message;
            }
        }
    }

    private void UpdateTurnInfoText(string text) {
        if (turnInfoText != null) {
            turnInfoText.text = text;
        }
    }

    private void UpdateGameStatusText(string text) {
        if (gameStatusText != null) {
            gameStatusText.text = text;
        }
    }

    private void UpdateCharacterStatsText() {
        if (characterStatsText == null || gameManager == null) return;

        string statsText = "Character Stats:\n\n";

        var players = gameManager.GetAllPlayers();
        var enemies = gameManager.GetAllEnemies();

        statsText += "PLAYERS:\n";
        foreach (var player in players) {
            if (player != null) {
                string status = player.IsDead ? " (DEAD)" : "";
                statsText += $"{player.CharacterType}: {player.CurrentHealth}/{player.Stats.maxHealth} HP{status}\n";
            }
        }

        statsText += "\nENEMIES:\n";
        foreach (var enemy in enemies) {
            if (enemy != null) {
                string status = enemy.IsDead ? " (DEAD)" : "";
                statsText += $"Enemy: {enemy.CurrentHealth}/{enemy.Stats.maxHealth} HP{status}\n";
            }
        }

        characterStatsText.text = statsText;
    }

    private void RestartGame() {
        if (gameManager != null) {
            gameManager.RestartGame();
        }

        if (gameOverPanel != null) {
            gameOverPanel.SetActive(false);
        }

        UpdateGameStatusText("Game Restarting...");
    }

    private void OnDestroy() {
        if (gameManager != null) {
            gameManager.OnGameStateChanged -= OnGameStateChanged;
            gameManager.OnGameWon -= OnGameWon;
            gameManager.OnGameLost -= OnGameLost;
        }

        if (turnManager != null) {
            turnManager.OnTurnStart -= OnTurnStart;
            turnManager.OnTurnEnd -= OnTurnEnd;
        }

        if (restartButton != null) {
            restartButton.onClick.RemoveListener(RestartGame);
        }
    }
}