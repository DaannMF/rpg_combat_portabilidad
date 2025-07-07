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

    public event Action OnGameWon;
    public event Action OnGameLost;
    public event Action OnCharactersSpawned;

    private List<Character> allCharacters = new List<Character>();
    private List<Character> players = new List<Character>();
    private List<Character> enemies = new List<Character>();

    private BaseCharacterStats player1Stats;
    private BaseCharacterStats player2Stats;
    private BaseCharacterStats player3Stats;
    private BaseCharacterStats enemyStats;

    private PlayerActionController actionController;
    private MovementSystem movementSystem;
    private PlayerAbilitySystem abilitySystem;


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

        InitializeSystems();
        CreateCharacterStats();
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

    private void CreateCharacterStats() {
        player1Stats = CharacterStatsConfigurator.CreatePlayer1Stats();
        player1Stats.characterSprite = fighterSprite;

        player2Stats = CharacterStatsConfigurator.CreatePlayer2Stats();
        player2Stats.characterSprite = healerSprite;

        player3Stats = CharacterStatsConfigurator.CreatePlayer3Stats();
        player3Stats.characterSprite = rangeSprite;

        enemyStats = CharacterStatsConfigurator.CreateEnemyStats();
    }

    private void SpawnCharacters() {
        List<GridCell> availablePositions = gridSystem.GetAvailablePositions();

        if (availablePositions.Count < 5) {
            Debug.LogError("Not enough available positions to spawn all characters!");
            return;
        }

        List<GridCell> spawnPositions = GetRandomPositions(availablePositions, 5);

        SpawnPlayer(CharacterType.Player1, spawnPositions[0], player1Stats, fighterSprite);
        SpawnPlayer(CharacterType.Player2, spawnPositions[1], player2Stats, healerSprite);
        SpawnPlayer(CharacterType.Player3, spawnPositions[2], player3Stats, rangeSprite);
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
        if (enemySprites == null || enemySprites.Length == 0) {
            Debug.LogWarning("No enemy sprites configured!");
            return null;
        }

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
        Vector2 spritePositionWithOffset = gridSystem.GetWorldPosition(position) + new Vector2(0, 0.2f);
        GameObject playerObject = Instantiate(characterPrefab, spritePositionWithOffset, Quaternion.identity, transform);
        Player player = playerObject.GetComponent<Player>();
        if (!player) player = playerObject.AddComponent<Player>();

        SetupCharacter(player, type, position, stats, sprite);
        players.Add(player);

        if (actionController != null)
            player.SetActionController(actionController);

        player.OnCharacterDeath += OnPlayerDeath;
    }

    private void SpawnEnemy(GridCell position, BaseCharacterStats stats, Sprite sprite) {
        Vector2 spritePositionWithOffset = gridSystem.GetWorldPosition(position) + new Vector2(0, 0.2f);
        GameObject enemyObject = Instantiate(characterPrefab, spritePositionWithOffset, Quaternion.identity, transform);
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

        // Set action controller in turn manager
        if (actionController != null) {
            actionController.Initialize(allCharacters);
            turnManager.SetActionController(actionController);
        }
    }

    private void StartGame() {
        turnManager.StartGame();
    }

    private void OnPlayerDeath(Character player) {
        gridSystem.RemoveCharacterFromGrid(player);

        bool anyEnemiesAlive = enemies.Any(e => !e.IsDead);
        if (anyEnemiesAlive) {
            GameOver(false);
            return;
        }

        CheckGameOverConditions();
    }

    private void OnEnemyDeath(Character enemy) {
        gridSystem.RemoveCharacterFromGrid(enemy);
        CheckGameOverConditions();
    }

    private void CheckGameOverConditions() {
        bool anyEnemiesAlive = enemies.Any(e => !e.IsDead);
        int playersAlive = players.Count(p => !p.IsDead);

        if (!anyEnemiesAlive && playersAlive == 1)
            GameOver(true);
    }

    private void GameOver(bool victory) {
        if (victory)
            OnGameWon?.Invoke();
        else
            OnGameLost?.Invoke();
    }

    public List<Character> GetAllPlayers() {
        return players.ToList();
    }

    public List<Character> GetAllEnemies() {
        return enemies.ToList();
    }

    public void RestartGame() {
        foreach (var character in allCharacters)
            if (character != null)
                Destroy(character.gameObject);

        allCharacters.Clear();
        players.Clear();
        enemies.Clear();

        InitializeGame();
    }
}