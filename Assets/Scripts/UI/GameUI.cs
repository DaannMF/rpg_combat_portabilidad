using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameUI : MonoBehaviour {
    [Header("UI Elements")]
    [SerializeField] private Button restartButton;
    [SerializeField] private GameObject gameOverPanel;

    private GameManager gameManager;

    private void Start() {
        gameManager = FindFirstObjectByType<GameManager>();

        if (gameManager != null) {
            gameManager.OnGameWon += OnGameWon;
            gameManager.OnGameLost += OnGameLost;
        }

        if (restartButton != null) {
            restartButton.onClick.AddListener(RestartGame);
        }

        InitializeUI();
    }

    private void OnDestroy() {
        if (gameManager != null) {
            gameManager.OnGameWon -= OnGameWon;
            gameManager.OnGameLost -= OnGameLost;
        }

        if (restartButton != null) {
            restartButton.onClick.RemoveListener(RestartGame);
        }
    }

    private void InitializeUI() {
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);
    }

    private void OnGameWon(Character winner) {
        string winnerName = winner != null ? winner.Stats.characterName : "Unknown Player";
        ShowGameOverPanel("VICTORY!", $"{winnerName} wins!");
    }

    private void OnGameLost() {
        ShowGameOverPanel("DEFEAT!", "All players LOST!");
    }

    private void ShowGameOverPanel(string title, string message) {
        if (gameOverPanel != null) {
            gameOverPanel.SetActive(true);

            TMP_Text titleText = gameOverPanel.transform.Find("Title")?.GetComponent<TMP_Text>();
            if (titleText != null)
                titleText.text = title;

            TMP_Text messageText = gameOverPanel.transform.Find("Message")?.GetComponent<TMP_Text>();
            if (messageText != null)
                messageText.text = message;
        }
    }

    private void RestartGame() {
        if (gameManager != null && (gameManager.CurrentGameState == GameState.Won || gameManager.CurrentGameState == GameState.Lost)) {
            gameManager.RestartGame();

            if (gameOverPanel != null)
                gameOverPanel.SetActive(false);
        }
    }
}