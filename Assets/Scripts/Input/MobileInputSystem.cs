public class MobileInputSystem : BaseInputSystem {
    private bool enableMobileInput = true;
    private GridVisualFeedbackSystem visualFeedbackSystem;

    public override void Initialize(PlayerActionController controller) {
        base.Initialize(controller);
        GridCell.OnCellClicked += HandleCellClick;
    }

    public void InitializeWithVisualFeedback(PlayerActionController controller, GridVisualFeedbackSystem visualSystem) {
        Initialize(controller);
        visualFeedbackSystem = visualSystem;
    }

    public override void HandleInput() {
        // Mobile input is handled via events (GridCell.OnCellClicked)
        // so this method doesn't need to do anything in the Update loop
    }

    protected override void OnPlayerSet(BaseCharacter player) {
        if (visualFeedbackSystem == null) return;

        if (enableMobileInput && isPlayerTurn)
            visualFeedbackSystem.SetActiveCharacter(player);
        else
            visualFeedbackSystem.ClearAllHighlights();
    }

    protected override void OnPlayerTurnEnded() {
        if (visualFeedbackSystem != null)
            visualFeedbackSystem.EndTurn();
    }

    private void HandleCellClick(GridCell clickedCell) {
        if (clickedCell == null || !enableMobileInput || !isPlayerTurn || currentActivePlayer == null)
            return;

        TriggerCellClicked(clickedCell);

        if (visualFeedbackSystem != null && visualFeedbackSystem.IsValidMoveForCurrentCharacter(clickedCell)) {
            if (actionController.TryMovePlayerToPosition(currentActivePlayer, clickedCell)) {
                TriggerPlayerMoveRequested(currentActivePlayer, clickedCell);
                visualFeedbackSystem.UpdateAfterMovement();
            }
        }
    }

    public void Cleanup() {
        GridCell.OnCellClicked -= HandleCellClick;
    }
}