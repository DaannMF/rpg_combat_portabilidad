public class Player : BaseCharacter {
    public override void ExecuteTurn() {
        if (isDead) return;
    }

    public override void EndTurn() {
        base.EndTurn();
    }
}