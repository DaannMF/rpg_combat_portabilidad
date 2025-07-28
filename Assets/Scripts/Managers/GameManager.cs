using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

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
    [SerializeField] private MenuCanvasUI menuCanvas;

    [Header("Input System")]
    [SerializeField] private InputManager inputManager;

    public event Action OnCharactersSpawned;

    private GameState currentGameState = GameState.NotStarted;
    private List<BaseCharacter> allCharacters = new List<BaseCharacter>();
    private List<BaseCharacter> players = new List<BaseCharacter>();
    private List<BaseCharacter> enemies = new List<BaseCharacter>();

    private PlayerActionController actionController;
    private MovementSystem movementSystem;
    private PlayerAbilitySystem abilitySystem;

    public GameState CurrentGameState => currentGameState;

    void Awake() {
        UIEvents.OnGamePaused += OnGamePaused;
        UIEvents.OnGameResumed += OnGameResumed;
    }

    private void Start() {
        InitializeGame();
    }

    private void OnDestroy() {
        UIEvents.OnGamePaused -= OnGamePaused;
        UIEvents.OnGameResumed -= OnGameResumed;

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

        if (inputManager == null)
            inputManager = FindFirstObjectByType<InputManager>();

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

        if (inputManager != null) {
            GridVisualFeedbackSystem visualFeedbackSystem = new GridVisualFeedbackSystem(gridSystem, movementSystem);
            inputManager.Initialize(actionController, visualFeedbackSystem);
        }
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

    private void SetupCharacter(BaseCharacter character, CharacterType type, GridCell position, BaseCharacterStats stats, Sprite sprite) {
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

        if (inputManager != null) {
            turnManager.SetInputManager(inputManager);
        }
    }

    private void StartGame() {
        currentGameState = GameState.Playing;
        turnManager.StartGame();
    }

    private void OnPlayerDeath(BaseCharacter player) {
        if (currentGameState != GameState.Playing) return;

        gridSystem.RemoveCharacterFromGrid(player);

        bool anyEnemiesAlive = enemies.Any(e => !e.IsDead);
        if (anyEnemiesAlive) {
            GameOver(false);
            return;
        }

        CheckGameOverConditions();
    }

    private void OnEnemyDeath(BaseCharacter enemy) {
        if (currentGameState != GameState.Playing) return;

        gridSystem.RemoveCharacterFromGrid(enemy);
        CheckGameOverConditions();
    }

    private void CheckGameOverConditions() {
        if (currentGameState != GameState.Playing) return;

        bool anyEnemiesAlive = enemies.Any(e => !e.IsDead);
        int playersAlive = players.Count(p => !p.IsDead);

        if (!anyEnemiesAlive && playersAlive == 1) {
            BaseCharacter winner = players.First(p => !p.IsDead);
            GameOver(true, winner);
        }
    }

    public void OnGamePaused() {
        if (currentGameState != GameState.Playing) return;

        if (menuCanvas != null) {
            menuCanvas.gameObject.SetActive(true);
            menuCanvas.ShowMainMenu();
        }
    }

    private void OnGameResumed() {
        if (currentGameState != GameState.Playing) return;

        if (menuCanvas != null)
            menuCanvas.Hide();
    }

    private void GameOver(bool victory, BaseCharacter winner = null) {
        if (currentGameState != GameState.Playing) return;

        currentGameState = victory ? GameState.Won : GameState.Lost;

        StopAllSystems();

        InterstitialManager.Instance?.ShowInterstitialAd();

        bool isVictory = victory && winner != null;
        if (menuCanvas != null) {
            menuCanvas.gameObject.SetActive(true);
            menuCanvas.ShowGameOverPanel(isVictory, winner);
        }
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

    public List<BaseCharacter> GetAllPlayers() {
        return players.ToList();
    }

    public List<BaseCharacter> GetAllEnemies() {
        return enemies.ToList();
    }

    public void RestartGame() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void QuitAndroid() {
        AndroidJavaObject activity = new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity");
        activity.Call<bool>("moveTaskToBack", true);
    }

    public void QuitGame() {
#if UNITY_ANDROID
        QuitAndroid();
#elif UNITY_STANDALONE
        Application.Quit();
#elif UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}