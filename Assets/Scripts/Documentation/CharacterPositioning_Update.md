# Character Positioning System Update

## Problema Resuelto

**Descripción del problema**: Los characters tenían un offset visual `+ new Vector2(0, 0.2f)` aplicado solo durante el spawn, pero no durante el movimiento con WASD, causando inconsistencia en el posicionamiento visual.

## Solución Implementada

### 1. **Offset Centralizado en GridSystem**

Se agregó un sistema centralizado de posicionamiento en `GridSystem.cs`:

```csharp
[Header("Character Positioning")]
[SerializeField] private Vector2 characterSpriteOffset = new Vector2(0, 0.2f);

public Vector2 GetCharacterWorldPosition(int x, int y) {
    return GetWorldPosition(x, y) + characterSpriteOffset;
}

public Vector2 GetCharacterWorldPosition(GridCell cell) {
    return GetCharacterWorldPosition(cell.x, cell.y);
}
```

### 2. **Beneficios del Nuevo Sistema**

- **Consistencia**: El offset se aplica automáticamente en spawn y movimiento
- **Configurabilidad**: El offset es editable desde el inspector de GridSystem
- **Mantenibilidad**: Un solo lugar para cambiar el offset de todos los characters
- **Escalabilidad**: Fácil de modificar para diferentes tipos de characters si fuera necesario

### 3. **Métodos Actualizados**

#### **MovementSystem.cs**

```csharp
// Antes:
character.transform.position = gridSystem.GetWorldPosition(targetPosition);

// Después:
character.transform.position = gridSystem.GetCharacterWorldPosition(targetPosition);
```

#### **GameManager.cs - SpawnPlayer/SpawnEnemy**

```csharp
// Antes:
Vector2 spritePositionWithOffset = gridSystem.GetWorldPosition(position) + new Vector2(0, 0.2f);

// Después:
Vector2 characterWorldPosition = gridSystem.GetCharacterWorldPosition(position);
```

#### **Enemy.cs - ExecuteRandomMove**

```csharp
// Antes:
transform.position = gridSystem.GetWorldPosition(bestMove);

// Después:
transform.position = gridSystem.GetCharacterWorldPosition(bestMove);
```

### 4. **Configuración en Unity**

1. **Seleccionar** el GameObject `GridSystem` en la escena
2. **En el Inspector**, encontrar la sección "Character Positioning"
3. **Ajustar** `Character Sprite Offset` según necesidades:
   - **X**: Offset horizontal (normalmente 0)
   - **Y**: Offset vertical (por defecto 0.2f para centrar mejor)

### 5. **Diferenciación de Métodos**

#### **Para Celdas de la Grilla:**

```csharp
gridSystem.GetWorldPosition(cell);  // Sin offset, para posicionar celdas
```

#### **Para Characters:**

```csharp
gridSystem.GetCharacterWorldPosition(cell);  // Con offset, para posicionar sprites
```

### 6. **Casos de Uso**

- **Spawn inicial**: Automáticamente centrado
- **Movimiento WASD**: Automáticamente centrado  
- **Movimiento de enemies**: Automáticamente centrado
- **Cualquier reposicionamiento futuro**: Automáticamente centrado

### 7. **Extensibilidad Futura**

El sistema está preparado para futuras mejoras como:

- **Offsets específicos por tipo de character**
- **Animaciones de transición**
- **Offsets basados en tamaño de sprite**
- **Efectos visuales de posicionamiento**

### 8. **Testing**

Para verificar que funciona correctamente:

1. **Ejecutar** el juego
2. **Mover** cualquier player con WASD
3. **Verificar** que el sprite mantiene el centrado visual
4. **Comparar** con el posicionamiento durante el spawn inicial
5. **Observar** que enemies también mantienen consistencia visual

El sistema ahora garantiza posicionamiento visual consistente en todas las situaciones.
