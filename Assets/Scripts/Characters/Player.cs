using UnityEngine;

public class Player : BaseCharacter {
    private bool isCurrentPlayer;
    private PlayerActionController actionController;

    public void SetActionController(PlayerActionController actionController) {
        this.actionController = actionController;
    }

    public override void ExecuteTurn() {
        if (isDead) return;

        isCurrentPlayer = true;
    }

    public void SetAsCurrentPlayer(bool current) {
        isCurrentPlayer = current;
    }

    public override void EndTurn() {
        base.EndTurn();
        isCurrentPlayer = false;
    }
}