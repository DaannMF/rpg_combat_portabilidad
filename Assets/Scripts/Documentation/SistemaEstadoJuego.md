# Sistema de Gestión de Estado del Juego

## Resumen

Este sistema asegura que todos los componentes del juego se detengan y limpien correctamente cuando el juego termina, evitando la ejecución continúa de lógica de turnos, actualizaciones de UI y procesamiento de entrada del jugador.

## Estados del Juego

### Enumeración GameState

- **NotStarted**: Estado inicial antes de que comience el juego
- **Playing**: Estado de juego activo
- **Won**: Estado de victoria del jugador
- **Lost**: Estado de derrota del jugador

## Componentes del Sistema

### 1. GameManager - Coordinador Central

**Rol**: Controlador central para el estado del juego y coordinación de sistemas

**Características Clave**:

- Rastrea `currentGameState` durante todo el ciclo de vida del juego
- Previene que la lógica de fin de juego se ejecute múltiples veces
- Coordina el cierre de sistemas cuando el juego termina
- Gestiona la reactivación de sistemas durante el reinicio

**Transiciones de Estado**:

```csharp
NotStarted → Playing (cuando se llama StartGame())
Playing → Won (cuando se cumplen condiciones de victoria)
Playing → Lost (cuando se cumplen condiciones de derrota)
Won/Lost → NotStarted (cuando se llama RestartGame())
```

### 2. TurnManager - Control de Flujo

**Rol**: Gestiona el flujo de juego basado en turnos

**Comportamiento al Fin del Juego**:

- Agrega flag `isGameStopped` para detener todo procesamiento de turnos
- Cancela transiciones de turnos pendientes usando `CancelInvoke()`
- Resetea estados de jugadores (remueve indicadores de jugador actual)
- Previene procesamiento de entrada en el bucle Update()

**Métodos Clave**:

```csharp
public void StopGame()
{
    isGameStopped = true;
    isProcessingTurn = false;
    CurrentCharacter = null;
    CancelInvoke(nameof(StartNextTurn));
}

public void ResetGame()
{
    isGameStopped = false;
    isProcessingTurn = false;
    CurrentCharacter = null;
    currentTurnIndex = -1;
}
```

## Implementación de Principios SOLID

### Principio de Responsabilidad Única (SRP)

**GameManager**:

- Solo gestiona estados del juego y coordinación de sistemas
- No maneja lógica específica de UI o turnos

**TurnManager**:

- Solo controla flujo de turnos y transiciones
- No gestiona estados globales del juego

### Principio Abierto/Cerrado (OCP)

```csharp
// Fácil agregar nuevos estados sin modificar lógica existente
public enum GameState
{
    NotStarted,
    Playing,
    Won,
    Lost,
    // Paused,  ← Se puede agregar sin modificar código existente
    // Loading  ← Se puede agregar sin modificar código existente
}
```

### Principio de Inversión de Dependencias (DIP)

```csharp
// GameManager depende de abstracción Character, no implementaciones
private void CheckGameOverConditions()
{
    bool anyEnemiesAlive = enemies.Any(e => !e.IsDead);  // Character.IsDead
    int playersAlive = players.Count(p => !p.IsDead);    // Character.IsDead
}
```

## Secuencia de Fin de Juego

### 1. Detección de Condiciones

```csharp
private void OnPlayerDeath(Character player)
{
    if (currentGameState != GameState.Playing) return;  // Guard clause

    gridSystem.RemoveCharacterFromGrid(player);

    // Verificar derrota inmediata
    bool anyEnemiesAlive = enemies.Any(e => !e.IsDead);
    if (anyEnemiesAlive)
    {
        GameOver(false);  // Derrota inmediata
        return;
    }

    CheckGameOverConditions();  // Verificar otras condiciones
}
```

### 2. Limpieza de Sistemas

```csharp
private void StopAllSystems()
{
    // Detener procesamiento de turnos
    if (turnManager != null)
        turnManager.StopGame();

    // Ocultar elementos de UI
    if (playerActionUI != null)
        playerActionUI.gameObject.SetActive(false);

    // Limpiar displays de estadísticas
    if (characterStatsUI != null)
    {
        characterStatsUI.ClearDisplays();
        characterStatsUI.gameObject.SetActive(false);
    }

    // Ocultar todos los personajes
    foreach (var character in allCharacters)
        if (character != null)
            character.gameObject.SetActive(false);

    // Ocultar sistema de grilla
    if (gridSystem != null)
        gridSystem.gameObject.SetActive(false);
}
```

### 3. Notificación de Eventos

```csharp
private void GameOver(bool victory, Character winner = null)
{
    if (currentGameState != GameState.Playing) return;

    currentGameState = victory ? GameState.Won : GameState.Lost;
    StopAllSystems();

    // Notificar a observers
    if (victory && winner != null)
        OnGameWon?.Invoke(winner);
    else
        OnGameLost?.Invoke();
}
```

## Proceso de Reinicio

### 1. Limpieza Completa

```csharp
public void RestartGame()
{
    currentGameState = GameState.NotStarted;

    // Destruir personajes existentes
    foreach (var character in allCharacters)
        if (character != null)
            Destroy(character.gameObject);

    // Limpiar listas
    allCharacters.Clear();
    players.Clear();
    enemies.Clear();

    ReactivateAllSystems();
    InitializeGame();
}
```

### 2. Reactivación de Sistemas

```csharp
private void ReactivateAllSystems()
{
    // Reactivar turn manager
    if (turnManager != null)
    {
        turnManager.gameObject.SetActive(true);
        turnManager.ResetGame();
    }

    // Reactivar UI de acciones
    if (playerActionUI != null)
        playerActionUI.gameObject.SetActive(true);

    // Reactivar UI de estadísticas
    if (characterStatsUI != null)
        characterStatsUI.gameObject.SetActive(true);

    // Reactivar sistema de grilla
    if (gridSystem != null)
        gridSystem.gameObject.SetActive(true);
}
```

## Beneficios del Sistema

### 1. Fin de Juego Limpio

- No hay procesamiento continuo de turnos después del fin del juego
- No se acepta entrada del jugador durante el fin del juego
- Todos los elementos visuales se ocultan apropiadamente
- UI permanece responsiva solo para reinicio

### 2. Gestión de Estado Apropiada

- Previene múltiples activaciones de fin de juego
- Asegura comportamiento consistente del sistema
- Maneja casos extremos (muerte de personaje durante fin de juego)

### 3. Reinicio Confiable

- Reseteo completo del sistema antes de nuevo juego
- Todas las referencias se limpian apropiadamente
- Inicialización fresca de todos los componentes

### 4. Rendimiento

- Detiene llamadas innecesarias a Update()
- Previene memory leaks por procesamiento continuo
- Desactivación eficiente de sistemas

## Ejemplos de Uso

### Verificar Estado del Juego

```csharp
if (gameManager.CurrentGameState != GameState.Playing)
{
    return; // No procesar si el juego no está activo
}
```

### Manejar Fin de Juego

```csharp
private void OnPlayerDeath(Character player)
{
    if (currentGameState != GameState.Playing) return;
    
    // Manejar lógica de muerte...
    
    if (shouldGameEnd)
    {
        GameOver(false); // Esto detendrá todos los sistemas
    }
}
```

### Gestión de Estado de UI

```csharp
private void RestartGame()
{
    if (gameManager.CurrentGameState == GameState.Won || 
        gameManager.CurrentGameState == GameState.Lost)
    {
        gameManager.RestartGame();
        gameOverPanel.SetActive(false);
    }
}
```

## Consideraciones de Arquitectura

### Event-Driven Design

- Usa eventos para desacoplar sistemas
- `OnGameWon` y `OnGameLost` notifican a múltiples observers
- UI responde a cambios de estado automáticamente

### Defensive Programming

- Guard clauses previenen ejecución en estados incorrectos
- Verificaciones de null para robustez
- Estados bien definidos previenen bugs de timing
