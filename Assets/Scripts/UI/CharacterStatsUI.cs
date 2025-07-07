using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class CharacterStatsUI : MonoBehaviour {
    [Header("UI References")]
    [SerializeField] private Transform characterStatsContainer;
    [SerializeField] private GameObject characterStatsPrefab;

    private Dictionary<Character, CharacterStatsDisplay> characterDisplays = new Dictionary<Character, CharacterStatsDisplay>();
    private GameManager gameManager;
    private TurnManager turnManager;

    private void Start() {
        gameManager = FindFirstObjectByType<GameManager>();
        turnManager = FindFirstObjectByType<TurnManager>();

        if (gameManager != null)
            gameManager.OnCharactersSpawned += InitializeDisplays;

        if (turnManager != null) {
            turnManager.OnTurnStart += OnTurnStart;
            turnManager.OnTurnEnd += OnTurnEnd;
        }
    }

    private void OnDestroy() {
        if (gameManager != null)
            gameManager.OnCharactersSpawned -= InitializeDisplays;

        if (turnManager != null) {
            turnManager.OnTurnStart -= OnTurnStart;
            turnManager.OnTurnEnd -= OnTurnEnd;
        }

        foreach (var kvp in characterDisplays) {
            if (kvp.Key != null) {
                kvp.Key.OnHealthChanged -= OnCharacterHealthChanged;
                kvp.Key.OnCharacterDeath -= OnCharacterDeath;
            }
        }
    }

    private void InitializeDisplays() {
        Assert.IsNotNull(characterStatsContainer, "CharacterStatsContainer is not assigned in CharacterStatsUI");
        Assert.IsNotNull(characterStatsPrefab, "CharacterStatsPrefab is not assigned in CharacterStatsUI");
        Assert.IsNotNull(gameManager, "GameManager is not found in CharacterStatsUI");

        var allCharacters = new List<Character>();
        allCharacters.AddRange(gameManager.GetAllPlayers());
        allCharacters.AddRange(gameManager.GetAllEnemies());

        foreach (var character in allCharacters)
            CreateCharacterDisplay(character);
    }

    private void CreateCharacterDisplay(Character character) {
        if (character == null) return;

        GameObject displayObject = Instantiate(characterStatsPrefab, characterStatsContainer);
        CharacterStatsDisplay display = displayObject.GetComponent<CharacterStatsDisplay>();

        if (display == null) return;

        display.Initialize(character);
        characterDisplays[character] = display;

        character.OnHealthChanged += OnCharacterHealthChanged;
        character.OnCharacterDeath += OnCharacterDeath;
    }

    private void OnTurnStart(Character character) {
        ClearActiveIndicators();

        if (characterDisplays.ContainsKey(character))
            characterDisplays[character].SetActiveIndicator(true);
    }

    private void OnTurnEnd(Character character) {
        if (characterDisplays.ContainsKey(character))
            characterDisplays[character].SetActiveIndicator(false);
    }

    private void ClearActiveIndicators() {
        foreach (var display in characterDisplays.Values)
            display.SetActiveIndicator(false);
    }

    private void OnCharacterHealthChanged(Character character, int newHealth) {
        if (characterDisplays.ContainsKey(character))
            characterDisplays[character].UpdateHealth(newHealth);
    }

    private void OnCharacterDeath(Character character) {
        if (characterDisplays.ContainsKey(character))
            characterDisplays[character].UpdateDeathStatus(true);
    }

    public void ClearDisplays() {
        foreach (var kvp in characterDisplays) {
            if (kvp.Key != null) {
                kvp.Key.OnHealthChanged -= OnCharacterHealthChanged;
                kvp.Key.OnCharacterDeath -= OnCharacterDeath;
            }

            if (kvp.Value != null)
                Destroy(kvp.Value.gameObject);
        }

        characterDisplays.Clear();
    }
}