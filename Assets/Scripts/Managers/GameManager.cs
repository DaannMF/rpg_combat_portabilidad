using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviour {
    [Header("Game Settings")]
    [SerializeField] private GridSystem gridSystem;
    [SerializeField] private TurnManager turnManager;

    [Header("Character Prefabs")]
    [SerializeField] private GameObject characterPrefab;

    [Header("Character Sprites")]
    [SerializeField] private Sprite fighterSprite;
    [SerializeField] private Sprite healerSprite;
    [SerializeField] private Sprite rangeSprite;
    [SerializeField] private Sprite[] enemySprites;

    [Header("UI References")]
    [SerializeField] private PlayerActionUI playerActionUI;
    [SerializeField] private CharacterStatsUI characterStatsUI;

    public event Action<Character> OnGameWon;
    public event Action OnGameLost;
    public event Action OnCharactersSpawned;

    private GameState currentGameState = GameState.NotStarted;
    private List<Character> allCharacters = new List<Character>();
    private List<Character> players = new List<Character>();
    private List<Character> enemies = new List<Character>();

    private PlayerActionController actionController;
    private MovementSystem movementSystem;
    private PlayerAbilitySystem abilitySystem;

    public GameState CurrentGameState => currentGameState;

    private void Start() {
        InitializeGame();
    }

    private void OnDestroy() {
        foreach (var player in players)
            if (player != null)
                player.OnCharacterDeath -= OnPlayerDeath;

        foreach (var enemy in enemies)
            if (enemy != null)
                enemy.OnCharacterDeath -= OnEnemyDeath;
    }

    private void InitializeGame() {
        if (gridSystem == null)
            gridSystem = GetComponentInChildren<GridSystem>();

        if (turnManager == null)
            turnManager = GetComponentInChildren<TurnManager>();

        if (characterStatsUI == null)
            characterStatsUI = FindFirstObjectByType<CharacterStatsUI>();

        InitializeSystems();
        SpawnCharacters();

        OnCharactersSpawned?.Invoke();

        SetupTurnManager();
        SetupUI();
        StartGame();
    }

    private void InitializeSystems() {
        movementSystem = new MovementSystem(gridSystem);
        abilitySystem = new PlayerAbilitySystem();
        actionController = new PlayerActionController(abilitySystem, movementSystem);
    }

    private void SetupUI() {
        if (playerActionUI != null && actionController != null)
            playerActionUI.Initialize(actionController);
    }

    private BaseCharacterStats CreatePlayerStats(CharacterType playerType) {
        BaseCharacterStats stats = playerType switch {
            CharacterType.Player1 => CharacterStatsConfigurator.CreatePlayer1Stats(),
            CharacterType.Player2 => CharacterStatsConfigurator.CreatePlayer2Stats(),
            CharacterType.Player3 => CharacterStatsConfigurator.CreatePlayer3Stats(),
            _ => throw new ArgumentException($"Invalid player type: {playerType}")
        };

        stats.characterSprite = playerType switch {
            CharacterType.Player1 => fighterSprite,
            CharacterType.Player2 => healerSprite,
            CharacterType.Player3 => rangeSprite,
            _ => null
        };

        return stats;
    }

    private void SpawnCharacters() {
        List<GridCell> availablePositions = gridSystem.GetAvailablePositions();

        if (availablePositions.Count < 5) return;

        List<GridCell> spawnPositions = GetRandomPositions(availablePositions, 5);

        SpawnPlayer(CharacterType.Player1, spawnPositions[0], CreatePlayerStats(CharacterType.Player1), fighterSprite);
        SpawnPlayer(CharacterType.Player2, spawnPositions[1], CreatePlayerStats(CharacterType.Player2), healerSprite);
        SpawnPlayer(CharacterType.Player3, spawnPositions[2], CreatePlayerStats(CharacterType.Player3), rangeSprite);
        SpawnEnemy(spawnPositions[3], CreateEnemyStats(1), GetEnemySprite(0));
        SpawnEnemy(spawnPositions[4], CreateEnemyStats(2), GetEnemySprite(1));
    }

    private List<GridCell> GetRandomPositions(List<GridCell> availablePositions, int count) {
        List<GridCell> shuffled = availablePositions.ToList();

        for (int i = 0; i < shuffled.Count; i++) {
            GridCell temp = shuffled[i];
            int randomIndex = UnityEngine.Random.Range(i, shuffled.Count);
            shuffled[i] = shuffled[randomIndex];
            shuffled[randomIndex] = temp;
        }

        return shuffled.Take(count).ToList();
    }

    private Sprite GetEnemySprite(int enemyIndex) {
        if (enemySprites == null || enemySprites.Length == 0) return null;
        return enemySprites[enemyIndex % enemySprites.Length];
    }

    private BaseCharacterStats CreateEnemyStats(int enemyIndex) {
        BaseCharacterStats stats = CharacterStatsConfigurator.CreateEnemyStats();

        if (stats != null) {
            stats.characterName = $"Enemy {enemyIndex}";
            stats.characterSprite = GetEnemySprite(enemyIndex - 1);
        }

        return stats;
    }

    private void SpawnPlayer(CharacterType type, GridCell position, BaseCharacterStats stats, Sprite sprite) {
        Vector2 characterWorldPosition = gridSystem.GetCharacterWorldPosition(position);
        GameObject playerObject = Instantiate(characterPrefab, characterWorldPosition, Quaternion.identity, transform);
        Player player = playerObject.GetComponent<Player>();
        if (!player) player = playerObject.AddComponent<Player>();

        SetupCharacter(player, type, position, stats, sprite);
        players.Add(player);

        if (actionController != null)
            player.SetActionController(actionController);

        player.OnCharacterDeath += OnPlayerDeath;
    }

    private void SpawnEnemy(GridCell position, BaseCharacterStats stats, Sprite sprite) {
        Vector2 characterWorldPosition = gridSystem.GetCharacterWorldPosition(position);
        GameObject enemyObject = Instantiate(characterPrefab, characterWorldPosition, Quaternion.identity, transform);
        Enemy enemy = enemyObject.GetComponent<Enemy>();
        if (!enemy) enemy = enemyObject.AddComponent<Enemy>();

        SetupCharacter(enemy, CharacterType.Enemy, position, stats, sprite);
        enemies.Add(enemy);

        enemy.OnCharacterDeath += OnEnemyDeath;
    }

    private void SetupCharacter(Character character, CharacterType type, GridCell position, BaseCharacterStats stats, Sprite sprite) {
        character.SetStats(stats);
        character.SetCharacterType(type);
        character.SetPosition(position);
        gridSystem.SetCharacterPosition(character, position);

        SpriteRenderer spriteRenderer = character.GetComponent<SpriteRenderer>();
        if (spriteRenderer && sprite) spriteRenderer.sprite = sprite;

        allCharacters.Add(character);
    }

    private void SetupTurnManager() {
        turnManager.Initialize(allCharacters);

        if (actionController != null) {
            actionController.Initialize(allCharacters);
            turnManager.SetActionController(actionController);
        }
    }

    private void StartGame() {
        currentGameState = GameState.Playing;
        turnManager.StartGame();
    }

    private void OnPlayerDeath(Character player) {
        if (currentGameState != GameState.Playing) return;

        gridSystem.RemoveCharacterFromGrid(player);

        bool anyEnemiesAlive = enemies.Any(e => !e.IsDead);
        if (anyEnemiesAlive) {
            GameOver(false);
            return;
        }

        CheckGameOverConditions();
    }

    private void OnEnemyDeath(Character enemy) {
        if (currentGameState != GameState.Playing) return;

        gridSystem.RemoveCharacterFromGrid(enemy);
        CheckGameOverConditions();
    }

    private void CheckGameOverConditions() {
        if (currentGameState != GameState.Playing) return;

        bool anyEnemiesAlive = enemies.Any(e => !e.IsDead);
        int playersAlive = players.Count(p => !p.IsDead);

        if (!anyEnemiesAlive && playersAlive == 1) {
            Character winner = players.First(p => !p.IsDead);
            GameOver(true, winner);
        }
    }

    private void GameOver(bool victory, Character winner = null) {
        if (currentGameState != GameState.Playing) return;

        currentGameState = victory ? GameState.Won : GameState.Lost;

        StopAllSystems();

        if (victory && winner != null)
            OnGameWon?.Invoke(winner);
        else
            OnGameLost?.Invoke();
    }

    private void StopAllSystems() {
        if (turnManager != null)
            turnManager.StopGame();

        if (playerActionUI != null)
            playerActionUI.gameObject.SetActive(false);

        if (characterStatsUI != null) {
            characterStatsUI.ClearDisplays();
            characterStatsUI.gameObject.SetActive(false);
        }

        foreach (var character in allCharacters)
            if (character != null)
                character.gameObject.SetActive(false);

        if (gridSystem != null)
            gridSystem.gameObject.SetActive(false);
    }

    public List<Character> GetAllPlayers() {
        return players.ToList();
    }

    public List<Character> GetAllEnemies() {
        return enemies.ToList();
    }

    public void RestartGame() {
        currentGameState = GameState.NotStarted;

        foreach (var character in allCharacters)
            if (character != null)
                Destroy(character.gameObject);

        allCharacters.Clear();
        players.Clear();
        enemies.Clear();

        ReactivateAllSystems();
        InitializeGame();
    }

    private void ReactivateAllSystems() {
        if (turnManager != null) {
            turnManager.gameObject.SetActive(true);
            turnManager.ResetGame();
        }

        if (playerActionUI != null)
            playerActionUI.gameObject.SetActive(true);

        if (characterStatsUI != null)
            characterStatsUI.gameObject.SetActive(true);

        if (gridSystem != null)
            gridSystem.gameObject.SetActive(true);
    }
}