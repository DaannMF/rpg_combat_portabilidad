# Character Stats UI Setup Guide

## Overview

The new Character Stats UI system displays real-time character information including health, status, turn indicators, and character sprites. This replaces the old text-based stats display with a visual, dynamic interface.

## Components Created

### 1. CharacterStatsUI.cs

- **Location**: `Assets/Scripts/UI/CharacterStatsUI.cs`
- **Purpose**: Manager script that handles all character stat displays
- **Features**:
  - Automatically finds all characters in the game
  - Creates individual stat displays for each character
  - Manages turn indicators using the "active" sprite
  - Updates displays in real-time based on character events

### 2. CharacterStatsDisplay.cs

- **Location**: `Assets/Scripts/UI/CharacterStatsDisplay.cs`
- **Purpose**: Individual character stat display component
- **Features**:
  - Shows character sprite, name, health, and status
  - Health displayed as text only (no health bar)
  - Turn indicator using Outline component
  - Death status with visual feedback (transparency)
  - Character sprites loaded from stats configuration

## Required Prefab Structure

Create a prefab called `CharacterStatsPrefab` with the following hierarchy:

```bash
CharacterStatsPrefab (GameObject)
├── CharacterSprite (Image)           // Character's sprite
├── CharacterName (TextMeshPro)       // Character name/type
├── HealthText (TextMeshPro)          // "25/30" format
└── StatusText (TextMeshPro)          // "ALIVE"/"DEAD"

Root GameObject requires:
- CharacterStatsDisplay component
- Outline component (for turn indicator)
```

### Component Configuration

1. **CharacterSprite (Image)**:
   - Set as child of root
   - Will be populated automatically with character sprite

2. **CharacterName (TextMeshPro)**:
   - Set as child of root
   - Will show character name or type

3. **HealthText (TextMeshPro)**:
   - Set as child of root
   - Will show current/max health format

4. **StatusText (TextMeshPro)**:
   - Set as child of root
   - Will show "ALIVE" (green) or "DEAD" (red)

5. **Outline Component (on root GameObject)**:
   - Add Outline component to the root GameObject
   - Configure outline color and thickness as desired
   - Will be enabled/disabled automatically for turn indication

## Setup Instructions

### Step 1: Create the Prefab

1. Create a new GameObject in the scene
2. Add the hierarchy structure above
3. Add a `CharacterStatsDisplay` component to the root GameObject
4. Add an `Outline` component to the root GameObject
5. Configure the UI elements with appropriate sizes and positions
6. Configure the Outline component (color, thickness, etc.)
7. Save as prefab in `Assets/Prefabs/CharacterStatsPrefab.prefab`

### Step 2: Setup Main UI

1. Add a `CharacterStatsUI` component to your main UI GameObject
2. Assign the following references:
   - **Character Stats Container**: Transform where stat displays will be spawned
   - **Character Stats Prefab**: The prefab created in Step 1

### Step 3: Layout Recommendations

- Use a Horizontal or Vertical Layout Group for the container
- Add Content Size Fitter to auto-resize
- Consider using a Scroll Rect if you have many characters
- Position at top or side of screen for easy visibility

## Features

### Real-time Updates

- **Health Changes**: Health text updates immediately when characters take damage or heal
- **Death Status**: Character becomes semi-transparent and shows "DEAD" status
- **Turn Indicators**: Outline component activates only for the current turn character

### Visual Feedback

- **Health Display**: Shows current/max health as text (e.g., "25/30")
- **Character Sprites**: Automatically loaded from character stats configuration
- **Turn Highlighting**: Outline border indicates whose turn it is
- **Death Transparency**: Dead characters appear with 50% opacity

### Character Support

- **Players**: Shows Player1, Player2, Player3 with their respective sprites
- **Enemies**: Shows "Enemy 1", "Enemy 2" with unique names and sprites
- **Dynamic**: Automatically adapts to any number of characters

## Integration

The system automatically integrates with:

- `TurnManager`: For turn start/end events
- `GameManager`: For character lists
- `Character`: For health and death events

No additional setup required beyond prefab creation and component assignment.

## Benefits Over Previous System

1. **Visual**: Shows actual character sprites instead of text
2. **Real-time**: Updates immediately, no polling needed
3. **Intuitive**: Clear turn indicators and health visualization
4. **Scalable**: Works with any number of characters
5. **Informative**: Shows all relevant character information at a glance
