using System.Collections.Generic;

public interface IPlayerAbilitySystem {
    List<PlayerAction> GetAvailableActions(Character character, List<Character> allCharacters);
    bool CanPerformAction(Character character, PlayerAction action);
    bool ExecuteAction(Character character, PlayerAction action);
}