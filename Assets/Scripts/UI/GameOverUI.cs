using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameOverUI : MonoBehaviour {
    [Header("UI Elements")]
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text messageText;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button quitButton;

    private GameManager gameManager;

    private void Start() {
        gameManager = FindFirstObjectByType<GameManager>();

        if (restartButton != null)
            restartButton.onClick.AddListener(RestartGame);

        if (quitButton != null) {
            quitButton.onClick.AddListener(() => {
                UIEvents.OnGameQuit?.Invoke();
            });
        }
    }

    public void SetTileAndMessage(string title, string message) {
        if (titleText != null)
            titleText.text = title;

        if (messageText != null)
            messageText.text = message;
    }


    private void RestartGame() {
        if (gameManager != null && (gameManager.CurrentGameState == GameState.Won || gameManager.CurrentGameState == GameState.Lost)) {
            gameManager.RestartGame();
        }
    }
}