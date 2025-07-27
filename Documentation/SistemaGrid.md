# Sistema de Grilla y Posicionamiento

## Resumen

Este sistema implementa un campo de batalla de grilla 4x6 con posicionamiento dinámico de personajes, cálculo automático de tamaños de celda basado en contenedores UI y gestión eficiente de posiciones usando distancia de Chebyshev para movimiento táctico.

## Arquitectura del Sistema

### Componentes Principales

#### 1. GridSystem - Gestor Principal

```csharp
public class GridSystem : MonoBehaviour
{
    [Header("Grid Configuration")]
    public int width = 4;           // Ancho de la grilla
    public int height = 6;          // Alto de la grilla
    public RectTransform container; // Contenedor UI para cálculo de tamaño
    public GameObject cellPrefab;   // Prefab de celda
    
    [Header("Character Positioning")]
    public Vector2 characterSpriteOffset = new Vector2(0, 0.2f); // Offset visual
}
```

**Principios SOLID**:

- **SRP**: Solo gestiona la grilla y posicionamiento de personajes
- **OCP**: Extensible para diferentes tamaños de grilla sin modificación
- **DIP**: No depende de implementaciones específicas de Character

#### 2. GridCell - Celda Individual

```csharp
public class GridCell : MonoBehaviour
{
    public int x, y;                    // Coordenadas en la grilla
    public Vector2 worldPosition;       // Posición en el mundo
    public Character occupyingCharacter; // Personaje que ocupa la celda
    
    public int GetChebyshevDistance(GridCell other)
    {
        return Mathf.Max(Mathf.Abs(x - other.x), Mathf.Abs(y - other.y));
    }
}
```

**Principios SOLID**:

- **SRP**: Solo maneja estado y cálculos de una celda específica
- **ISP**: Expone solo métodos relevantes para cálculos de distancia

## Implementación de Principios SOLID

### Principio de Responsabilidad Única (SRP)

**GridSystem**:

- Solo gestiona la creación y mantenimiento de la grilla
- No maneja lógica de combate o UI

**GridCell**:

- Solo maneja estado de una celda individual
- No gestiona múltiples celdas o lógica global

**MovementSystem**:

- Solo valida y ejecuta movimiento de personajes
- No gestiona la grilla física

### Principio Abierto/Cerrado (OCP)

```csharp
// Fácil cambiar tamaño de grilla sin modificar código
[Header("Grid Configuration")]
public int width = 4;   // Configurable desde Inspector
public int height = 6;  // Configurable desde Inspector

// Sistema soporta diferentes algoritmos de distancia
public int GetChebyshevDistance(GridCell other) { /* ... */ }
// Se puede agregar GetManhattanDistance() sin modificar existente
```

### Principio de Inversión de Dependencias (DIP)

```csharp
// GridSystem depende de abstracción Character, no implementaciones
public void SetCharacterPosition(Character character, GridCell targetCell)
{
    // Funciona con Player, Enemy, o cualquier Character
}

// MovementSystem depende de GridSystem abstraction
public MovementSystem(GridSystem gridSystem)
{
    this.gridSystem = gridSystem; // Inyección de dependencia
}
```

## Sistema de Cálculo de Tamaño Dinámico

### Configuración Basada en Contenedor

```csharp
[Header("Container-Based Sizing")]
public RectTransform container;              // Contenedor UI de referencia
public float containerToWorldScale = 0.01f; // Factor de conversión
public float cellSpacing = 0.1f;            // Espaciado entre celdas
```

### Cálculo Automático de Dimensiones

```csharp
private void CalculateFromContainer()
{
    Canvas canvas = container.GetComponentInParent<Canvas>();
    if (canvas == null)
    {
        calculatedCellWidth = manualCellSize;
        calculatedCellHeight = manualCellSize;
        CalculateOriginFromCamera();
        return;
    }

    // Obtener tamaño del contenedor en unidades UI
    Rect containerRect = container.rect;
    float containerUIWidth = containerRect.width;
    float containerUIHeight = containerRect.height;

    // Convertir tamaño UI a tamaño mundial usando factor de escala
    float containerWorldWidth = containerUIWidth * containerToWorldScale;
    float containerWorldHeight = containerUIHeight * containerToWorldScale;

    // Calcular espacio disponible para celdas (restando espaciado)
    float spacingWidthTotal = (width - 1) * cellSpacing;
    float spacingHeightTotal = (height - 1) * cellSpacing;

    float availableWidth = containerWorldWidth - spacingWidthTotal;
    float availableHeight = containerWorldHeight - spacingHeightTotal;

    // Calcular tamaño de celda
    calculatedCellWidth = availableWidth / width;
    calculatedCellHeight = availableHeight / height;

    // Asegurar tamaño mínimo razonable
    calculatedCellWidth = Mathf.Max(calculatedCellWidth, 0.1f);
    calculatedCellHeight = Mathf.Max(calculatedCellHeight, 0.1f);
}
```

## Sistema de Posicionamiento de Personajes

### Offset Centralizado

```csharp
[Header("Character Positioning")]
public Vector2 characterSpriteOffset = new Vector2(0, 0.2f);

public Vector2 GetCharacterWorldPosition(GridCell cell)
{
    return cell.worldPosition + characterSpriteOffset;
}

public Vector2 GetCharacterWorldPosition(int x, int y)
{
    GridCell cell = GetGridCell(x, y);
    return cell != null ? GetCharacterWorldPosition(cell) : Vector2.zero;
}
```

**Beneficios**:

- **Consistencia**: Todos los personajes usan el mismo offset
- **Configurabilidad**: Ajustable desde el Inspector
- **Mantenibilidad**: Cambio centralizado afecta todos los personajes

### Gestión de Ocupación

```csharp
public void SetCharacterPosition(Character character, GridCell targetCell)
{
    // Limpiar posición anterior
    if (character.CurrentPosition != null)
        character.CurrentPosition.occupyingCharacter = null;

    // Establecer nueva posición
    targetCell.occupyingCharacter = character;
    character.SetPosition(targetCell);
}

public void RemoveCharacterFromGrid(Character character)
{
    if (character.CurrentPosition != null)
    {
        character.CurrentPosition.occupyingCharacter = null;
    }
}
```

## Cálculo de Distancias

### Distancia de Chebyshev

```csharp
public int GetChebyshevDistance(GridCell other)
{
    // Considera movimiento diagonal como distancia 1
    return Mathf.Max(Mathf.Abs(x - other.x), Mathf.Abs(y - other.y));
}
```

**Razón de Uso**:

- **Movimiento Táctico**: Permite movimiento diagonal eficiente
- **Adyacencia**: Considera todas las 8 direcciones como adyacentes
- **Coherencia**: Consistent con expectativas de juegos de tablero

### Aplicación en Combate

```csharp
// En Character.cs
public virtual bool CanAttack(Character target)
{
    if (hasActedThisTurn || isDead || target == null || target.IsDead) 
        return false;

    int distance = currentPosition.GetChebyshevDistance(target.CurrentPosition);
    return stats.CanAttackAtDistance(distance);
}
```

## Validación de Movimiento

### Sistema de Validación

```csharp
public bool CanMoveToPosition(GridCell targetCell)
{
    // Verificar límites de grilla
    if (targetCell.x < 0 || targetCell.x >= width || 
        targetCell.y < 0 || targetCell.y >= height)
        return false;

    // Verificar si la celda está ocupada
    return targetCell.occupyingCharacter == null;
}

public List<GridCell> GetValidMovePositions(GridCell currentPosition, int speed)
{
    var validPositions = new List<GridCell>();
    
    for (int x = 0; x < width; x++)
    {
        for (int y = 0; y < height; y++)
        {
            GridCell cell = grid[x, y];
            if (cell.GetChebyshevDistance(currentPosition) <= speed && 
                CanMoveToPosition(cell))
            {
                validPositions.Add(cell);
            }
        }
    }
    
    return validPositions;
}
```

## Optimizaciones de Rendimiento

### 1. Lookups Eficientes

```csharp
// Array 2D para acceso O(1)
private GridCell[,] grid;

public GridCell GetGridCell(int x, int y)
{
    if (x >= 0 && x < width && y >= 0 && y < height)
        return grid[x, y];
    return null;
}
```

### 2. Cálculos Pre-computados

```csharp
// Posiciones mundiales calculadas una vez en Awake()
private void CreateGridCells()
{
    for (int x = 0; x < width; x++)
    {
        for (int y = 0; y < height; y++)
        {
            Vector2 worldPosition = GetWorldPosition(x, y);
            // ... crear celda con posición pre-calculada
        }
    }
}
```

### 3. Validación Temprana

```csharp
// Early returns para evitar cálculos innecesarios
public bool CanMoveToPosition(GridCell targetCell)
{
    if (targetCell == null) return false;  // Early return
    if (targetCell.occupyingCharacter != null) return false;  // Early return
    
    // Solo hacer cálculos costosos si las verificaciones simples pasan
    return IsWithinBounds(targetCell);
}
```

## Configuración Flexible

### Parámetros Ajustables

```csharp
[Header("Grid Configuration")]
public int width = 4;
public int height = 6;

[Header("Container-Based Sizing")]
public RectTransform container;
public float containerToWorldScale = 0.01f;
public float cellSpacing = 0.1f;

[Header("Manual Sizing (Fallback)")]
public float manualCellSize = 1f;

[Header("Character Positioning")]
public Vector2 characterSpriteOffset = new Vector2(0, 0.2f);
```

### Métodos de Configuración

```csharp
// Para configuración runtime
public void SetGridDimensions(int newWidth, int newHeight)
{
    width = newWidth;
    height = newHeight;
    RegenerateGrid();
}

public void SetCellSpacing(float newSpacing)
{
    cellSpacing = newSpacing;
    RecalculateCellSizes();
}
```

## Beneficios del Sistema

### 1. Flexibilidad

- **Tamaño Configurable**: Fácil cambiar dimensiones de grilla
- **Escalado Automático**: Se adapta a diferentes tamaños de contenedor
- **Offset Adjustable**: Posicionamiento visual personalizable

### 2. Performance

- **Acceso O(1)**: Lookup de celdas optimizado
- **Cálculos Eficientes**: Distancias pre-calculadas cuando es posible
- **Validación Temprana**: Evita cálculos innecesarios

### 3. Mantenibilidad

- **Separación Clara**: Responsabilidades bien definidas
- **Extensibilidad**: Fácil agregar nuevas funcionalidades
- **Configurabilidad**: Parámetros ajustables desde Inspector

### 4. Robustez

- **Validación Completa**: Verificaciones de límites y ocupación
- **Fallbacks**: Sistema manual si el contenedor no está disponible
- **Error Handling**: Gestión apropiada de casos extremos
