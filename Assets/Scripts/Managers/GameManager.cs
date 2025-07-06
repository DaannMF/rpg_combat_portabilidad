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

    public event Action OnGameWon;
    public event Action OnGameLost;
    public event Action<GameState> OnGameStateChanged;

    private List<Character> allCharacters = new List<Character>();
    private List<Character> players = new List<Character>();
    private List<Character> enemies = new List<Character>();

    private BaseCharacterStats player1Stats;
    private BaseCharacterStats player2Stats;
    private BaseCharacterStats player3Stats;
    private BaseCharacterStats enemyStats;


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

        CreateCharacterStats();
        SpawnCharacters();
        SetupTurnManager();
        StartGame();
    }

    private void CreateCharacterStats() {
        player1Stats = CharacterStatsConfigurator.CreatePlayer1Stats();
        player2Stats = CharacterStatsConfigurator.CreatePlayer2Stats();
        player3Stats = CharacterStatsConfigurator.CreatePlayer3Stats();
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
        SpawnEnemy(spawnPositions[3], enemyStats, GetEnemySprite(0));
        SpawnEnemy(spawnPositions[4], enemyStats, GetEnemySprite(1));
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

    private void SpawnPlayer(CharacterType type, GridCell position, BaseCharacterStats stats, Sprite sprite) {
        Vector2 spritePositionWithOffset = gridSystem.GetWorldPosition(position) + new Vector2(0, 0.2f);
        GameObject playerObject = Instantiate(characterPrefab, spritePositionWithOffset, Quaternion.identity, transform);
        Player player = playerObject.GetComponent<Player>();
        if (!player) player = playerObject.AddComponent<Player>();

        SetupCharacter(player, type, position, stats, sprite);
        players.Add(player);

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
    }

    private void StartGame() {
        turnManager.StartGame();
    }

    private void OnPlayerDeath(Character player) {
        bool anyPlayersAlive = players.Any(p => !p.IsDead);
        bool anyEnemiesAlive = enemies.Any(e => !e.IsDead);

        if (!anyPlayersAlive && anyEnemiesAlive)
            GameOver(false);
    }

    private void OnEnemyDeath(Character enemy) {
        bool anyEnemiesAlive = enemies.Any(e => !e.IsDead);

        if (!anyEnemiesAlive) {
            IEnumerable<Character> survivingPlayers = players.Where(p => !p.IsDead);
            if (survivingPlayers.Count() == 1)
                GameOver(true);
        }
    }

    private void GameOver(bool victory) {
        if (victory)
            OnGameWon?.Invoke();
        else
            OnGameLost?.Invoke();

        OnGameStateChanged?.Invoke(victory ? GameState.Won : GameState.Lost);
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