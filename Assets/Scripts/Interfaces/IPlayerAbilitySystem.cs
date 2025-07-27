using System.Collections.Generic;

public interface IPlayerAbilitySystem {
    List<PlayerAction> GetAvailableActions(BaseCharacter character, List<BaseCharacter> allCharacters);
    bool CanPerformAction(BaseCharacter character, PlayerAction action);
    bool ExecuteAction(BaseCharacter character, PlayerAction action);
}