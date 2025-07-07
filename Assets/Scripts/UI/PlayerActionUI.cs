using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerActionUI : MonoBehaviour {
    [Header("UI References")]
    [SerializeField] private GameObject actionButtonPrefab;
    [SerializeField] private Transform actionButtonContainer;
    [SerializeField] private TMP_Text movementText;
    [SerializeField] private TMP_Text currentPlayerText;
    [SerializeField] private Button endTurnButton;

    private List<Button> actionButtons = new List<Button>();
    private PlayerActionController actionController;
    private Character currentPlayer;
    private TurnManager turnManager;

    private void OnDestroy() {
        if (actionController != null) {
            actionController.OnActionsUpdated -= UpdateActionButtons;
            actionController.OnMovementUpdated -= UpdateMovementDisplay;
            actionController.OnActionExecuted -= OnActionExecuted;
        }

        if (turnManager != null) {
            turnManager.OnTurnStart -= OnTurnStart;
            turnManager.OnTurnEnd -= OnTurnEnd;
        }

        if (endTurnButton != null)
            endTurnButton.onClick.RemoveListener(EndTurn);
    }

    public void Initialize(PlayerActionController controller) {
        actionController = controller;
        turnManager = FindFirstObjectByType<TurnManager>();

        if (actionController != null) {
            actionController.OnActionsUpdated += UpdateActionButtons;
            actionController.OnMovementUpdated += UpdateMovementDisplay;
            actionController.OnActionExecuted += OnActionExecuted;
        }

        if (turnManager != null) {
            turnManager.OnTurnStart += OnTurnStart;
            turnManager.OnTurnEnd += OnTurnEnd;
        }

        if (endTurnButton != null) {
            endTurnButton.onClick.AddListener(EndTurn);
        }
    }

    public void SetCurrentPlayer(Character player) {
        currentPlayer = player;
        UpdateCurrentPlayerText(player);
        SetUIActive(player != null && player.CharacterType.IsPlayer());
    }

    private void SetUIActive(bool active) {
        gameObject.SetActive(active);
    }

    private void UpdateActionButtons(List<PlayerAction> availableActions) {
        ClearActionButtons();

        foreach (var action in availableActions) {
            if (action.actionType == ActionType.EndTurn) continue;

            CreateActionButton(action);
        }
    }

    private void CreateActionButton(PlayerAction action) {
        if (actionButtonPrefab == null) return;

        GameObject buttonObj = Instantiate(actionButtonPrefab, actionButtonContainer);

        var actionButtonComponent = buttonObj.GetComponent<ActionButton>();
        if (actionButtonComponent != null)
            actionButtonComponent.Setup(action, ExecuteAction);

        actionButtons.Add(actionButtonComponent.GetComponent<Button>());
    }

    private void ClearActionButtons() {
        foreach (var button in actionButtons)
            if (button != null) {
                button.onClick.RemoveAllListeners();
                Destroy(button.gameObject);
            }

        actionButtons.Clear();
    }

    private void ExecuteAction(PlayerAction action) {
        if (currentPlayer != null && actionController != null)
            actionController.TryExecuteAction(currentPlayer, action);
    }

    private void EndTurn() {
        if (currentPlayer != null && actionController != null) {
            var endTurnAction = new PlayerAction(ActionType.EndTurn);
            actionController.TryExecuteAction(currentPlayer, endTurnAction);
        }
    }

    private void UpdateMovementDisplay(int remainingMovement) {
        if (movementText != null)
            movementText.text = $"Movement: {remainingMovement}";
    }

    private void OnActionExecuted(PlayerAction action) {
        Debug.Log($"Action executed: {action.displayName}");

        if (action.actionType == ActionType.EndTurn)
            SetUIActive(false);
    }

    private void OnTurnStart(Character character) {
        UpdateCurrentPlayerText(character);
    }

    private void OnTurnEnd(Character character) {
        // Optional: handle turn end if needed
    }

    private void UpdateCurrentPlayerText(Character character) {
        if (currentPlayerText != null) {
            if (character != null) {
                currentPlayerText.text = $"Current Turn: {character.CharacterType}";
            }
            else {
                currentPlayerText.text = "Current Turn: None";
            }
        }
    }
}