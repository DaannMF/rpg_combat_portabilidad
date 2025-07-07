# Game State Management System

## Overview

The game state management system ensures that all game systems are properly stopped and cleaned up when the game ends, preventing continued execution of turn logic, UI updates, and player input processing.

## Game States

### GameState Enum

- **NotStarted**: Initial state before game begins
- **Playing**: Active gameplay state
- **Won**: Player victory state
- **Lost**: Player defeat state

## System Components

### 1. GameManager

**Role**: Central controller for game state and system coordination

**Key Features**:

- Tracks `currentGameState` throughout the game lifecycle
- Prevents game over logic from running multiple times
- Coordinates system shutdown when game ends
- Manages system reactivation during restart

**State Transitions**:

```csharp
NotStarted → Playing (when StartGame() called)
Playing → Won (when victory conditions met)
Playing → Lost (when defeat conditions met)
Won/Lost → NotStarted (when RestartGame() called)
```

### 2. TurnManager

**Role**: Manages turn-based gameplay flow

**Game Over Behavior**:

- Adds `isGameStopped` flag to halt all turn processing
- Cancels pending turn transitions using `CancelInvoke()`
- Resets player states (removes current player indicators)
- Prevents input processing in Update() loop

**Key Methods**:

- `StopGame()`: Immediately halts all turn processing
- `ResetGame()`: Resets state for new game session

### 3. System Cleanup

**Automatic Deactivation**:

- **PlayerActionUI**: Hides action buttons and movement controls
- **CharacterStatsUI**: Hides character health and status displays
- **Character GameObjects**: Disables all character sprites and components
- **GridSystem**: Hides the game grid

## Implementation Details

### Game Over Sequence

1. **Detection**: GameManager detects win/lose conditions
2. **State Change**: Updates `currentGameState` to Won/Lost
3. **System Stop**: Calls `StopAllSystems()` to halt all processes
4. **UI Update**: Shows game over panel with appropriate message
5. **Event Trigger**: Fires `OnGameWon` or `OnGameLost` events

### System Cleanup Process

```csharp
private void StopAllSystems() {
    // Stop turn processing
    turnManager.StopGame();
    
    // Hide all UI elements
    playerActionUI.gameObject.SetActive(false);
    
    // Clean up character stats displays
    characterStatsUI.ClearDisplays();
    characterStatsUI.gameObject.SetActive(false);
    
    // Hide all characters
    foreach (var character in allCharacters) {
        character.gameObject.SetActive(false);
    }
    
    // Hide grid system
    gridSystem.gameObject.SetActive(false);
}
```

### Restart Process

```csharp
public void RestartGame() {
    // Reset game state
    currentGameState = GameState.NotStarted;
    
    // Destroy old characters
    foreach (var character in allCharacters) {
        Destroy(character.gameObject);
    }
    
    // Clear character lists
    allCharacters.Clear();
    players.Clear();
    enemies.Clear();
    
    // Reactivate all systems
    ReactivateAllSystems();
    
    // Initialize new game
    InitializeGame();
}

private void ReactivateAllSystems() {
    // Reactivate turn manager
    turnManager.gameObject.SetActive(true);
    turnManager.ResetGame();
    
    // Reactivate player action UI
    playerActionUI.gameObject.SetActive(true);
    
    // Reactivate and reset character stats UI
    characterStatsUI.gameObject.SetActive(true);
    characterStatsUI.ResetUI();
    
    // Reactivate grid system
    gridSystem.gameObject.SetActive(true);
}
```

## Benefits

### 1. Clean Game Over

- No continued turn processing after game ends
- No player input accepted during game over
- All visual elements properly hidden
- UI remains responsive only for restart

### 2. Proper State Management

- Prevents multiple game over triggers
- Ensures consistent system behavior
- Handles edge cases (character death during game over)

### 3. Reliable Restart

- Complete system reset before new game
- All references properly cleared
- Fresh initialization of all components

### 4. Performance

- Stops unnecessary Update() calls
- Prevents memory leaks from continued processing
- Efficient system deactivation

## Usage Examples

### Checking Game State

```csharp
if (gameManager.CurrentGameState != GameState.Playing) {
    return; // Don't process if game isn't active
}
```

### Handling Game Over

```csharp
private void OnPlayerDeath(Character player) {
    if (currentGameState != GameState.Playing) return;
    
    // Handle death logic...
    
    if (shouldGameEnd) {
        GameOver(false); // This will stop all systems
    }
}
```

### UI State Management

```csharp
private void RestartGame() {
    if (gameManager.CurrentGameState == GameState.Won || 
        gameManager.CurrentGameState == GameState.Lost) {
        gameManager.RestartGame();
        gameOverPanel.SetActive(false);
    }
}
```

## Technical Notes

### CharacterStatsUI Reinitialization

**Problem**: After restart, character stats UI was not displaying properly because old character displays and event subscriptions were not cleaned up.

**Solution**:

- `ClearDisplays()`: Removes old character displays and disconnects event listeners
- `ResetUI()`: Reinitializes event subscriptions for new game session
- Called automatically during game restart process

**Implementation**:

```csharp
public void ClearDisplays() {
    foreach (var kvp in characterDisplays) {
        if (kvp.Key != null) {
            kvp.Key.OnHealthChanged -= OnCharacterHealthChanged;
            kvp.Key.OnCharacterDeath -= OnCharacterDeath;
        }
        
        if (kvp.Value != null) {
            Destroy(kvp.Value.gameObject);
        }
    }
    
    characterDisplays.Clear();
}

public void ResetUI() {
    ClearDisplays();
    
    if (gameManager != null) {
        gameManager.OnCharactersSpawned -= InitializeDisplays;
        gameManager.OnCharactersSpawned += InitializeDisplays;
    }
}
```

### Thread Safety

- All state changes happen on main thread
- No async operations during game over
- Immediate system shutdown prevents race conditions

### Memory Management

- Character GameObjects properly destroyed during restart
- Event subscriptions cleaned up in OnDestroy()
- No lingering references after restart
- Character stats displays properly cleaned up and recreated

### Input Handling

- WASD movement disabled during game over
- Action buttons become non-interactive
- Only restart button remains functional

This system ensures a clean, professional game over experience with proper resource management and system shutdown.
