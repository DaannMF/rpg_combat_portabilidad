using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerActionUI : MonoBehaviour {
    [Header("UI References")]
    [SerializeField] private GameObject actionGridPrefab;
    [SerializeField] private Transform actionGridContainer;
    [SerializeField] private TMP_Text movementText;
    [SerializeField] private TMP_Text currentPlayerText;
    [SerializeField] private Button endTurnButton;
    [SerializeField] private Button healSelfButton;

    private List<ActionTargetGrid> actionGrids = new List<ActionTargetGrid>();
    private PlayerActionController actionController;
    private BaseCharacter currentPlayer;
    private TurnManager turnManager;

    private void OnDestroy() {
        if (actionController != null) {
            actionController.OnActionsUpdated -= UpdateActionButtons;
            actionController.OnActionGroupsUpdated -= UpdateActionGrids;
            actionController.OnMovementUpdated -= UpdateMovementDisplay;
            actionController.OnActionExecuted -= OnActionExecuted;
        }

        if (turnManager != null) {
            turnManager.OnTurnStart -= OnTurnStart;
            turnManager.OnTurnEnd -= OnTurnEnd;
        }

        if (endTurnButton != null)
            endTurnButton.onClick.RemoveListener(EndTurn);

        if (healSelfButton != null)
            healSelfButton.onClick.RemoveListener(HealSelf);
    }

    public void Initialize(PlayerActionController controller) {
        actionController = controller;
        turnManager = FindFirstObjectByType<TurnManager>();

        if (actionController != null) {
            actionController.OnActionsUpdated += UpdateActionButtons;
            actionController.OnActionGroupsUpdated += UpdateActionGrids;
            actionController.OnMovementUpdated += UpdateMovementDisplay;
            actionController.OnActionExecuted += OnActionExecuted;
        }

        if (turnManager != null) {
            turnManager.OnTurnStart += OnTurnStart;
            turnManager.OnTurnEnd += OnTurnEnd;
        }

        if (endTurnButton != null)
            endTurnButton.onClick.AddListener(EndTurn);

        if (healSelfButton != null)
            healSelfButton.onClick.AddListener(HealSelf);

    }

    public void SetCurrentPlayer(BaseCharacter player) {
        currentPlayer = player;
        UpdateCurrentPlayerText(player);
        SetUIActive(player != null && player.CharacterType.IsPlayer());
    }

    private void SetUIActive(bool active) {
        gameObject.SetActive(active);
    }

    private void UpdateActionButtons(List<PlayerAction> availableActions) {
        if (healSelfButton != null) {
            var healSelfAction = availableActions.Find(a => a.actionType == ActionType.HealSelf);
            healSelfButton.interactable = healSelfAction != null && healSelfAction.isAvailable;
        }
    }

    private void UpdateActionGrids(List<ActionGroup> actionGroups) {
        ClearActionGrids();

        foreach (var group in actionGroups) {
            if (group.actionType == ActionType.HealSelf)
                continue;

            CreateActionGrid(group);
        }
    }

    private void CreateActionGrid(ActionGroup actionGroup) {
        if (actionGridPrefab == null || actionGridContainer == null) return;

        GameObject gridObj = Instantiate(actionGridPrefab, actionGridContainer);
        var actionGrid = gridObj.GetComponent<ActionTargetGrid>();

        if (actionGrid != null) {
            actionGrid.Setup(actionGroup.actionType, OnTargetSelected);
            actionGrid.UpdateTargets(actionGroup.availableTargets);
            actionGrids.Add(actionGrid);
        }
    }

    private void ClearActionGrids() {
        foreach (var grid in actionGrids)
            if (grid != null)
                Destroy(grid.gameObject);

        actionGrids.Clear();
    }

    private void OnTargetSelected(ActionType actionType, BaseCharacter target) {
        if (currentPlayer == null || actionController == null) return;
        actionController.TryExecuteActionOnTarget(currentPlayer, actionType, target);
    }

    private void EndTurn() {
        if (currentPlayer != null && actionController != null) {
            var endTurnAction = new PlayerAction(ActionType.EndTurn);
            actionController.TryExecuteAction(currentPlayer, endTurnAction);
        }
    }

    private void HealSelf() {
        if (currentPlayer != null && actionController != null) {
            var healSelfAction = new PlayerAction(ActionType.HealSelf);
            actionController.TryExecuteAction(currentPlayer, healSelfAction);
        }
    }

    private void UpdateMovementDisplay(int remainingMovement) {
        if (movementText != null)
            movementText.text = $"Movement: {remainingMovement}";
    }

    private void OnActionExecuted(PlayerAction action) {
        if (action.actionType == ActionType.EndTurn)
            SetUIActive(false);
    }

    private void OnTurnStart(BaseCharacter character) {
        UpdateCurrentPlayerText(character);

        if (character != null && character.CharacterType.IsPlayer())
            SetCurrentPlayer(character);
        else {
            currentPlayer = null;
            SetUIActive(false);
        }
    }

    private void OnTurnEnd(BaseCharacter character) {
        if (character != null && character.CharacterType.IsPlayer()) {
            currentPlayer = null;
            SetUIActive(false);
        }
    }

    private void UpdateCurrentPlayerText(BaseCharacter character) {
        if (currentPlayerText != null)
            if (character != null)
                currentPlayerText.text = $"Current Turn: {character.CharacterType}";
            else
                currentPlayerText.text = "Current Turn: None";
    }
}