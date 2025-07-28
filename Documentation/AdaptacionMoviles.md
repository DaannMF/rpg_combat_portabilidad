# Adaptación para móviles

## Resumen

Creé un sistema de movimiento por clics para dispositivos móviles que permite a los jugadores mover sus personajes tocando las celdas de la grilla.

## Lo Que Implementé

### Funcionalidades Principales

1. **Detección de toques en celdas**: Los jugadores pueden tocar cualquier celda de la grilla con el táctil o con un click del mouse.
2. **Validación de movimientos**: El sistema valida que hay suficientes puntos de movimiento
3. **Pathfinding**: Encuentra automáticamente el camino más corto hacia la celda objetivo
4. **Validación de camino libre**: Verifica que no hay obstáculos en el camino
5. **Validación de celdas ocupadas**: No permite mover a celdas ocupadas por otros personajes
6. **Movimientos ortogonales únicamente**: Solo permite movimientos arriba, abajo, izquierda y derecha (NO diagonales)
7. **Feedback visual**: Muestra celdas válidas (verde), inválidas (rojo) y posición actual (azul)

### Sistemas que Creé

#### 1. PathfindingSystem

- **Archivo**: `Assets/Scripts/Grid/Pathfinding.cs`
- **Qué hace**: Calcula caminos válidos usando algoritmo BFS (Breadth-First Search)
  - Inspiración tomada del blog (<https://medium.com/@hanxuyang0826/mastering-dfs-and-bfs-in-c-techniques-implementations-and-leetcode-examples-57dbe66a140c>)
- **Métodos principales**:
  - `FindPath()`: Encuentra el camino más corto entre dos celdas
  - `GetValidMovePositions()`: Obtiene todas las celdas válidas para movimiento
  - `HasValidPath()`: Verifica si existe un camino válido

#### 2. GridVisualFeedbackSystem

- **Archivo**: `Assets/Scripts/Grid/GridVisualFeedbackSystem.cs`
- **Qué hace**: Maneja el feedback visual de las celdas
- **Características**:
  - Resalta celdas válidas para movimiento
  - Muestra celdas inválidas/ocupadas
  - Indica la posición actual del personaje
  - Limpia resaltados automáticamente

##### GridCell Colors

- **Valid Move Color**: Color para celdas válidas (verde)
- **Invalid Move Color**: Color para celdas inválidas (rojo)
- **Current Player Color**: Color para posición actual (azul)

#### 3. MobileInputSystem

- **Archivo**: `Assets/Scripts/Input/MobileInputSystem.cs`
- **Qué hace**: Sistema de input específico para móviles
- **Características**:
  - Maneja eventos de toques en celdas
  - Coordina entre sistemas de pathfinding, movimiento y visual
  - Eventos para integración con otros sistemas

### Sistemas que Actualicé

#### GridCell

- **Archivo**: `Assets/Scripts/Grid/GridCell.cs`
- **Nuevas características**:
  - Detección de toques con BoxCollider2D
  - Estados visuales (válido, inválido, actual)
  - Eventos estáticos para comunicación

#### MovementSystem

- **Archivo**: `Assets/Scripts/Grid/MovementSystem.cs`
- **Nuevos métodos**:
  - `MoveCharacterToPosition()`: Movimiento por pathfinding
  - `CanMoveToPosition()`: Validación de movimientos múltiples
  - `GetValidMovePositions()`: Obtiene posiciones válidas
  - `GetMovementCost()`: Calcula costo de movimiento

#### PlayerActionController

- **Archivo**: `Assets/Scripts/Controllers/PlayerActionController.cs`
- **Nuevos métodos**:
  - `TryMovePlayerToPosition()`: Movimiento por pathfinding
  - `GetValidMovePositions()`: Interfaz para posiciones válidas

### Integración con Managers

#### GameManager

- Inicializa el InputManager apropiado (KeyboardInputSystem o MobileInputSystem)
- Conecta todos los sistemas
- Mantiene compatibilidad con el sistema existente

#### TurnManager

- Activa/desactiva el input móvil según el turno
- Coordina entre jugadores y enemigos
- Limpia feedback visual al cambiar turnos

#### Player

- Mantiene compatibilidad con controles WASD/flechas
- Opción para activar/desactivar controles de teclado
- Integración transparente con el sistema móvil

## Cómo Usar

### Para Desarrolladores

1. El InputManager apropiado se selecciona automáticamente en el GameManager
2. Los sistemas se inicializan automáticamente
3. Se puede cambiar entre input systems fácilmente

### Para Jugadores

1. Durante tu turno, las celdas válidas se resaltan en verde
2. Las celdas inválidas aparecen en rojo
3. Tu posición actual aparece en azul
4. Toca cualquier celda verde para moverte
5. El personaje se moverá automáticamente por el camino más corto

## Configuración

### InputManager Settings

Para este manager implemente un sistema de herencia de una clase base InputSystem, la idea principalmente es que sea escalable, si quiero agregar por ejemplo un input system para Joystick heredo de la clase base y sobre escribo el método HandleInput, la única contra es que el para móviles no se implementa y se está llamando constantemente en el Update sin código lo que pueda degradar la performance.

- **Input System Type**: Selección automática entre teclado y móvil

### Player Settings

- **Use Keyboard Input**: Mantiene los controles WASD/flechas para desarrollo

## Algoritmo de Pathfinding

El sistema usa **BFS (Breadth-First Search)** para encontrar el camino más eficiente:

1. **Validación inicial**: Verifica distancia máxima y celda destino libre
2. **Exploración**: Explora celdas adyacentes en 4 direcciones ortogonales
3. **Costos uniformes**: Cada movimiento ortogonal cuesta 1 punto
4. **Obstáculos**: No puede pasar por celdas ocupadas
5. **Resultado**: Devuelve el camino más corto o null si no es posible
