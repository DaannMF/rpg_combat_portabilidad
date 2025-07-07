
public enum CharacterType {
    Player1,
    Player2,
    Player3,
    Enemy
}

public static class CharacterTypeExtensions {
    public static bool IsPlayer(this CharacterType type) {
        return type == CharacterType.Player1 || type == CharacterType.Player2 || type == CharacterType.Player3;
    }

    public static bool IsEnemy(this CharacterType type) {
        return type == CharacterType.Enemy;
    }
}