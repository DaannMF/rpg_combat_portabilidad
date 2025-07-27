# Sistema de Movimiento para Móviles

## Resumen

Sistema de movimiento por clics implementado para dispositivos móviles que permite a los jugadores mover sus personajes clickeando en las celdas de la grilla.

## Características Implementadas

### ✅ Funcionalidades Principales

1. **Detección de clics en celdas**: Los jugadores pueden clickear en cualquier celda de la grilla
2. **Validación de movimientos**: El sistema valida que hay suficientes puntos de movimiento
3. **Pathfinding inteligente**: Encuentra automáticamente el camino más corto hacia la celda objetivo
4. **Validación de camino libre**: Verifica que no hay obstáculos en el camino
5. **Validación de celdas ocupadas**: No permite mover a celdas ocupadas por otros personajes
6. **Movimientos ortogonales únicamente**: Solo permite movimientos arriba, abajo, izquierda y derecha (NO diagonales)
7. **Feedback visual**: Muestra celdas válidas (verde), inválidas (rojo) y posición actual (azul)

### 🎮 Sistemas Creados

#### 1. PathfindingSystem

- **Archivo**: `Assets/Scripts/Systems/PathfindingSystem.cs`
- **Función**: Calcula caminos válidos usando algoritmo BFS (Breadth-First Search)
- **Métodos principales**:
  - `FindPath()`: Encuentra el camino más corto entre dos celdas
  - `GetValidMovePositions()`: Obtiene todas las celdas válidas para movimiento
  - `HasValidPath()`: Verifica si existe un camino válido

#### 2. GridVisualFeedbackSystem

- **Archivo**: `Assets/Scripts/Systems/GridVisualFeedbackSystem.cs`
- **Función**: Maneja el feedback visual de las celdas
- **Características**:
  - Resalta celdas válidas para movimiento
  - Muestra celdas inválidas/ocupadas
  - Indica la posición actual del personaje
  - Limpia resaltados automáticamente

#### 3. MobileInputManager

- **Archivo**: `Assets/Scripts/Systems/MobileInputManager.cs`
- **Función**: Coordinador principal del sistema de input móvil
- **Características**:
  - Maneja eventos de clics en celdas
  - Coordina entre sistemas de pathfinding, movimiento y visual
  - Modo debug para desarrollo
  - Eventos para integración con otros sistemas

### 🔧 Sistemas Actualizados

#### GridCell

- **Archivo**: `Assets/Scripts/Grid/GridCell.cs`
- **Nuevas características**:
  - Detección de clics con BoxCollider2D
  - Estados visuales (válido, inválido, actual)
  - Eventos estáticos para comunicación

#### MovementSystem

- **Archivo**: `Assets/Scripts/Systems/MovementSystem.cs`
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

### 🎯 Integración con Managers

#### GameManager

- Inicializa el MobileInputManager
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

## 🚀 Cómo Usar

### Para Desarrolladores

1. Añadir un `MobileInputManager` a la escena
2. Asignar la referencia en el `GameManager`
3. Los sistemas se inicializan automáticamente

### Para Jugadores

1. Durante tu turno, las celdas válidas se resaltan en verde
2. Las celdas inválidas aparecen en rojo
3. Tu posición actual aparece en azul
4. Clickea en cualquier celda verde para moverte
5. El personaje se moverá automáticamente por el camino más corto

## ⚙️ Configuración

### MobileInputManager Settings

- `Enable Mobile Input`: Activa/desactiva el sistema móvil
- `Debug Mode`: Muestra información de debug en consola

### GridCell Colors

- `Valid Move Color`: Color para celdas válidas (verde)
- `Invalid Move Color`: Color para celdas inválidas (rojo)
- `Current Player Color`: Color para posición actual (azul)

### Player Settings

- `Use Keyboard Input`: Mantiene los controles WASD/flechas

## 🔍 Algoritmo de Pathfinding

El sistema usa **BFS (Breadth-First Search)** con costos diferenciados para encontrar el camino más eficiente:

1. **Validación inicial**: Verifica distancia máxima y celda destino libre
2. **Exploración**: Explora celdas adyacentes en 8 direcciones
3. **Costos diferenciados**:
   - Movimientos ortogonales (↑↓←→): Costo 1
   - Movimientos diagonales (↖↗↙↘): Costo 2
4. **Lógica**: Las diagonales cuestan 2 porque requieren 2 movimientos ortogonales reales
5. **Obstáculos**: No puede pasar por celdas ocupadas
6. **Resultado**: Devuelve el camino con menor costo o null si no es posible

## 🔄 Compatibilidad

- ✅ **Mantiene compatibilidad** con controles WASD/flechas
- ✅ **Funciona en paralelo** con el sistema existente
- ✅ **No modifica** la lógica de juego existente
- ✅ **Opcional**: Se puede activar/desactivar fácilmente

## 📱 Optimización para Móviles

- **Colliders optimizados**: BoxCollider2D en cada celda
- **Eventos eficientes**: Sistema de eventos estático
- **Feedback visual claro**: Colores distintivos para diferentes estados
- **Touch-friendly**: Áreas de click optimizadas para dedos

## 🐛 Debug y Troubleshooting

### Activar Debug Mode

```csharp
mobileInputManager.debugMode = true;
```

### Mensajes de Debug Comunes

- `"Cell clicked: (x, y)"`: Confirma detección de clics
- `"Valid move detected. Cost: X"`: Movimiento válido encontrado
- `"Invalid move: Cell is occupied"`: Celda destino ocupada
- `"No movement points remaining"`: Sin puntos de movimiento

### Cambios Importantes

**🔧 Corrección de Costo de Movimientos**

El sistema fue corregido para calcular correctamente el costo de movimientos considerando que solo se permiten movimientos ortogonales:

- **Problema**: Las celdas diagonales tenían costo 1 cuando deberían tener costo 2 ❌
- **Solución**: Sistema de costos diferenciados según tipo de movimiento ✅

**Costos de Movimiento**:

- **Movimientos ortogonales** (↑↓←→): Costo 1
- **Movimientos diagonales** (↖↗↙↘): Costo 2 (porque requieren 2 pasos ortogonales)

**Archivos Modificados**:

- `PathfindingSystem.cs`: Sistema de costos diferenciados (1 para ortogonal, 2 para diagonal)
- `MovementSystem.cs`: Cambio de Chebyshev a Manhattan distance
- `GridSystem.cs`: Cálculo de costo real de movimiento

### Problemas Comunes

1. **Clics no detectados**: Verificar que las celdas tengan BoxCollider2D
2. **Sin feedback visual**: Verificar que MobileInputManager está asignado
3. **Movimientos inválidos**: Verificar pathfinding y puntos de movimiento
4. **Costo incorrecto**: Verificar que diagonales cuesten 2 movimientos, no 1

### Ejemplo Práctico

**Personaje con 3 puntos de movimiento en posición (2,2)**:

✅ **Celdas alcanzables**:

- Ortogonales directas: (1,2), (3,2), (2,1), (2,3) - Costo 1 cada una
- Dos pasos ortogonales: (0,2), (4,2), (2,0), (2,4) - Costo 2 cada una  
- Tres pasos ortogonales: (2,5), (5,2), etc. - Costo 3

❌ **Celdas NO alcanzables**:

- Diagonales: (3,3), (1,1), etc. - Costo 2, pero desde ahí necesitas más movimientos
- Lejanas: (5,5) - Costo demasiado alto

**Antes vs Ahora**:

- **Antes**: (3,3) aparecía como alcanzable ❌
- **Ahora**: (3,3) NO aparece como alcanzable ✅
