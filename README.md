# RPG Combat System

A turn-based RPG combat system built in Unity, featuring a 4x6 grid battlefield with 3 players and 2 enemies.

## Features

- **Turn-based Combat**: Strategic turn-based gameplay with movement and action phases
- **Character Classes**: Three unique player types and enemy AI
- **Grid-based Movement**: 4x6 battlefield with tactical positioning
- **Combat System**: Melee and ranged attacks with healing abilities
- **ScriptableObject Architecture**: Configurable character stats
- **Clean Architecture**: Implements SOLID principles and design patterns

## Character Types

### Player 1 - Fighter

- **Health**: 20 HP
- **Speed**: 3 cells per turn
- **Melee Attack**: 5 damage
- **Ranged Attack**: None
- **Healing**: 2 HP (self only)

### Player 2 - Healer

- **Health**: 15 HP
- **Speed**: 2 cells per turn
- **Melee Attack**: 2 damage
- **Ranged Attack**: 2 damage (range 3)
- **Healing**: 5 HP (self and others, range 2)

### Player 3 - Ranger

- **Health**: 15 HP
- **Speed**: 4 cells per turn
- **Melee Attack**: 1 damage
- **Ranged Attack**: 3 damage (unlimited range)
- **Healing**: 2 HP (self only)

### Enemy

- **Health**: 10 HP
- **Speed**: 1 cell per turn
- **Melee Attack**: 3 damage
- **Ranged Attack**: 1 damage (range 3)
- **Healing**: None

## Controls

### Movement

- **WASD** or **Arrow Keys**: Move one cell in direction
- **Space**: Skip movement phase

### Actions

- **1**: Attack nearest enemy
- **2**: Heal self (if possible)
- **3**: Heal nearest player (if possible)
- **Space**: Skip action phase
- **Enter**: Force end turn

## Setup Instructions

### Requirements

- Unity 2021.3 or later
- Universal Render Pipeline (URP)

### Installation

1. Clone this repository
2. Open the project in Unity
3. Open the main scene (`Assets/Scenes/SampleScene.unity`)
4. Press Play to start the game

### Scene Setup

1. Create an empty GameObject and add the `GameManager` script
2. Create another empty GameObject and add the `GridSystem` script
3. Create another empty GameObject and add the `TurnManager` script
4. Create prefabs for Player and Enemy characters
5. Assign the character sprites to the GameManager
6. Set up the UI Canvas with the `GameUI` script

## Architecture

### Core Systems

- **GridSystem**: Manages the 4x6 battlefield and position tracking
- **Character System**: Handles character behavior, stats, and actions
- **TurnManager**: Controls the turn-based gameplay flow
- **GameManager**: Orchestrates the overall game state
- **UI System**: Displays game information and handles user feedback

### SOLID Principles

- **Single Responsibility**: Each class has one specific purpose
- **Open/Closed**: Easy to extend with new character types
- **Liskov Substitution**: Player and Enemy are interchangeable as Characters
- **Interface Segregation**: Classes only expose necessary methods
- **Dependency Inversion**: High-level modules depend on abstractions

### Design Patterns

- **Observer Pattern**: Event-driven architecture for loose coupling
- **Strategy Pattern**: Different character behavior implementations
- **Factory Pattern**: Character stats creation and configuration
- **Command Pattern**: Encapsulated character actions

## Project Structure

```bash
Assets/
├── Scripts/
│   ├── Characters/
│   │   ├── Character.cs          # Abstract base class
│   │   ├── Player.cs             # Player character implementation
│   │   └── Enemy.cs              # Enemy AI implementation
│   ├── Data/
│   │   ├── BaseCharacterStats.cs        # ScriptableObject for stats
│   │   ├── CharacterType.cs             # Character type enum
│   │   └── CharacterStatsConfigurator.cs # Stats factory
│   ├── Grid/
│   │   ├── GridPosition.cs       # Grid position struct
│   │   ├── GridSystem.cs         # Grid management system
│   │   └── GridCell.cs           # Individual grid cell
│   ├── Managers/
│   │   ├── GameManager.cs        # Main game controller
│   │   └── TurnManager.cs        # Turn-based system
│   ├── UI/
│   │   └── GameUI.cs             # User interface controller
│   └── Documentation/
│       └── DesignDocument.md     # Detailed design documentation
├── Art/
│   └── Sprites/                  # Character and UI sprites
└── Scenes/
    └── SampleScene.unity         # Main game scene
```

## Game Rules

### Victory Conditions

- All enemies are defeated
- At least one player survives

### Defeat Conditions

- All players are defeated
- Any enemies remain alive

### Combat Rules

- **Melee Attack**: Must be adjacent (including diagonally)
- **Ranged Attack**: Must be more than 1 cell away, within character's range
- **Healing**: Can heal self or others within healing range
- **Movement**: Can move up to character's speed in cells per turn
- **Turn Order**: Players and enemies alternate turns

## Development Notes

### Best Practices Applied

- **Clean Code**: Descriptive naming, small methods, clear structure
- **SOLID Principles**: Followed throughout the codebase
- **Design Patterns**: Used appropriately for maintainability
- **Error Handling**: Defensive programming with null checks
- **Performance**: Efficient algorithms and data structures

### Optimization Considerations

- Dictionary-based grid lookups for O(1) performance
- Event-driven UI updates to minimize processing
- Struct usage for value types (GridPosition)
- ScriptableObject sharing for memory efficiency

### Testing Strategy

- Unit tests for character stats and grid validation
- Integration tests for turn management and game state
- Performance testing for large grids and long sessions

## Future Enhancements

### Planned Features

- Character animations and visual effects
- Sound system with audio feedback
- Save/Load game functionality
- Multiple difficulty levels
- Level editor for custom battlefields

### Technical Improvements

- Asynchronous enemy AI processing
- A* pathfinding for complex movement
- More sophisticated ability system
- Network multiplayer support

## Contributing

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add some amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## Acknowledgments

- Built using Unity Engine
- Follows Unity best practices and conventions
- Implements clean architecture principles
- Designed for educational and portfolio purposes
