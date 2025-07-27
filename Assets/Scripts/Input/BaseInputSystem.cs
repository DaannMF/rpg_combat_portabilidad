using System;

public abstract class BaseInputSystem {
    protected PlayerActionController actionController;
    protected BaseCharacter currentActivePlayer;
    protected bool isPlayerTurn = false;

    public event Action<BaseCharacter, GridCell> OnPlayerMoveRequested;
    public event Action<GridCell> OnCellClicked;

    public virtual void Initialize(PlayerActionController controller) {
        actionController = controller;
    }

    public virtual void SetActivePlayer(BaseCharacter player) {
        currentActivePlayer = player;
        isPlayerTurn = player != null && player.CharacterType.IsPlayer();
        OnPlayerSet(player);
    }

    public virtual void EndPlayerTurn() {
        currentActivePlayer = null;
        isPlayerTurn = false;
        OnPlayerTurnEnded();
    }

    public abstract void HandleInput();

    protected virtual void OnPlayerSet(BaseCharacter player) { }

    protected virtual void OnPlayerTurnEnded() { }

    protected virtual void TriggerPlayerMoveRequested(BaseCharacter player, GridCell cell) {
        OnPlayerMoveRequested?.Invoke(player, cell);
    }

    protected virtual void TriggerCellClicked(GridCell cell) {
        OnCellClicked?.Invoke(cell);
    }
}