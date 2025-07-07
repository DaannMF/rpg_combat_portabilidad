# Simple Grid Container System

## Overview

The GridSystem now supports automatic cell sizing based on a container while maintaining manual row/column configuration. **Features a simple UI-to-World scale conversion system and cell spacing control for easy customization.**

## How It Works

### Manual Configuration (Default)

- Set `width` and `height` for grid dimensions (e.g., 4x6)
- Use `manualCellSize` for fixed cell dimensions
- Grid centers automatically based on camera

### Container-Based Sizing

- Assign a `RectTransform` to the `container` field
- Set `containerToWorldScale` to control the conversion from UI units to world units
- Set `cellSpacing` to add space between grid cells
- Grid automatically calculates cell size: `(containerSize * containerToWorldScale - totalSpacing) / gridDimensions`
- Origin is placed at the **bottom-left corner** of the container

## New Scale Control

### **containerToWorldScale** Parameter

- **Default**: `0.01` (100 UI units = 1 world unit)
- **Purpose**: Controls how UI container size converts to world space
- **Examples**:
  - `0.01`: Container 500x300 UI → 5x3 world units
  - `0.02`: Container 500x300 UI → 10x6 world units
  - `0.005`: Container 500x300 UI → 2.5x1.5 world units

### **cellSpacing** Parameter

- **Default**: `0` (no spacing between cells)
- **Purpose**: Adds space between individual grid cells in world units
- **Examples**:
  - `0`: No spacing (cells touch each other)
  - `0.1`: Small gaps between cells
  - `0.5`: Larger gaps for clear separation

### Scale Calculation with Spacing

```
Total Spacing = (gridSize - 1) × cellSpacing
Available Space = Container World Size - Total Spacing
Cell Size = Available Space ÷ Grid Dimensions
```

## Setup Instructions

### For Container-Based Sizing

1. Create a UI Panel or assign any RectTransform **within a Canvas**
2. Drag it to the `Container` field in GridSystem
3. Set your desired `width` and `height` (rows/columns)
4. **Adjust `containerToWorldScale`** to get the desired cell size:
   - **Too small?** Increase the value (e.g., 0.02)
   - **Too large?** Decrease the value (e.g., 0.005)
5. **Adjust `cellSpacing`** to control gaps between cells:
   - **No gaps?** Keep at 0
   - **Small gaps?** Try 0.1-0.2
   - **Large gaps?** Try 0.5+
6. The system will automatically:
   - Calculate world size from UI size and scale factor
   - Subtract total spacing from available area
   - Position the grid at the container's bottom-left corner
   - Scale grid cells to fit perfectly with spacing

### For Manual Sizing

1. Leave `Container` field empty
2. Set `Manual Cell Size` to desired value
3. Set `cellSpacing` for gaps between cells
4. Grid will center automatically based on camera viewport

## Key Features

- **🎯 Simple Setup**: Just assign a container and set rows/columns
- **🔧 Scale Control**: Easily adjust `containerToWorldScale` for perfect sizing
- **📏 Spacing Control**: Add gaps between cells with `cellSpacing`
- **📍 Bottom-Left Origin**: Grid starts at container's bottom-left corner
- **🔄 Runtime Recalculation**: Use "Recalculate Grid" context menu to update
- **🎮 Backward Compatible**: Works without container (manual mode)
- **🖥️ Canvas Compatible**: Works with all Canvas modes
- **⚙️ No Complex Math**: Simple linear scale conversion

## Example Usage

```csharp
// In Inspector:
// - Width: 4 (columns)
// - Height: 6 (rows)  
// - Container: MyUIPanel (500x300 UI units)
// - containerToWorldScale: 0.01
// - cellSpacing: 0.1

// Result:
// - Container World Size: 5x3 world units
// - Total Spacing: 0.3x0.5 world units (spacing between cells)
// - Available Space: 4.7x2.5 world units
// - Cell Size: 1.17x0.42 world units
// - Grid fits perfectly with gaps between cells
```

## Quick Size Adjustment

### If cells are too small

```
containerToWorldScale = 0.02  // Double the size
containerToWorldScale = 0.03  // Triple the size
```

### If cells are too large

```
containerToWorldScale = 0.005  // Half the size  
containerToWorldScale = 0.001  // Very small
```

### For spacing adjustment

```
cellSpacing = 0     // No gaps (cells touch)
cellSpacing = 0.1   // Small gaps
cellSpacing = 0.2   // Medium gaps
cellSpacing = 0.5   // Large gaps
```

## Debug Information

The system now logs detailed spacing information:

```
Canvas Mode: ScreenSpaceOverlay
Container UI Size: 500 x 300
Container World Size: 5.00 x 3.00
Cell Spacing: 0.10, Total Spacing: 0.30 x 0.50
Available Space: 4.70 x 2.50
Scale Factor: 0.010
Grid: 4x6, Cell Size: 1.17, Origin: (-2.50, -1.50)
```

This shows:

- **Container UI Size**: Size in UI units (pixels)
- **Container World Size**: Size after scale conversion
- **Cell Spacing**: Space between individual cells
- **Total Spacing**: Total space used by all gaps
- **Available Space**: Remaining space for actual cells
- **Scale Factor**: The conversion multiplier
- **Cell Size**: Final calculated cell size
- **Origin**: Bottom-left world position of the grid

## Utility Methods

```csharp
GetCellSize()        // Returns individual cell size
GetCellSpacing()     // Returns spacing between cells
GetTotalGridSize()   // Returns total grid size including spacing
GetGridDimensions()  // Returns width x height
```

## Troubleshooting

1. **Cells too small**: Increase `containerToWorldScale` (e.g., 0.01 → 0.02)
2. **Cells too large**: Decrease `containerToWorldScale` (e.g., 0.01 → 0.005)
3. **Need gaps between cells**: Increase `cellSpacing` (e.g., 0 → 0.1)
4. **Gaps too large**: Decrease `cellSpacing` (e.g., 0.5 → 0.1)
5. **Grid not positioning correctly**: Ensure container is within a Canvas
6. **No size change**: Check that container is assigned and has non-zero size
