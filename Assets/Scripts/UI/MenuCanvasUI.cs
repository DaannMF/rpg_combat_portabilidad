using UnityEngine;

public class MenuCanvasUI : MonoBehaviour {
    const string VICTORY_TITLE = "VICTORY!";
    const string DEFEAT_TITLE = "DEFEAT!";
    const string UNKNOWN_PLAYER = "Unknown Player";

    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameOverUI gameOverPanel;

    private InputManager inputManager;

    private void Awake() {
        inputManager = FindFirstObjectByType<InputManager>();
    }

    private void OnEnable() {
        inputManager.EnableInput(false);
    }

    private void OnDisable() {
        Hide();
        inputManager.EnableInput(true);
    }

    public void ShowMainMenu() {
        mainMenuPanel.SetActive(true);
        gameOverPanel.gameObject.SetActive(false);
    }

    public void ShowGameOverPanel(bool isVictory, BaseCharacter winner = null) {
        mainMenuPanel.SetActive(false);
        gameOverPanel.gameObject.SetActive(true);

        string winnerName = winner != null ? winner.Stats.characterName : UNKNOWN_PLAYER;
        if (isVictory)
            gameOverPanel.SetTileAndMessage(VICTORY_TITLE, $"{winnerName} wins!");
        else
            gameOverPanel.SetTileAndMessage(DEFEAT_TITLE, "All players LOST!");
    }

    public void Hide() {
        mainMenuPanel.SetActive(false);
        gameOverPanel.gameObject.SetActive(false);
        gameObject.SetActive(false);
    }
}