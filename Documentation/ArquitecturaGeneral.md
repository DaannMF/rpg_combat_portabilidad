# Arquitectura General del Sistema RPG

## Resumen

Este documento describe cómo está armado el Sistema de Combate RPG. Me enfoqué en crear un código fácil de mantener y que se pueda expandir sin problemas.

## Sistemas Principales

### 1. Sistema de Gestión del Juego (GameManager)

**Qué hace**: Se encarga de coordinar todo el estado del juego y manejar los sistemas

- **Responsabilidad única**: Solo se encarga del estado general del juego
- **Se puede extender**: Puedo agregar nuevos tipos de juego sin tocar el código existente

### 2. Sistema de Turnos (TurnManager)

**Qué hace**: Controla el flujo de turnos y las transiciones

- **Responsabilidad única**: Solo maneja la lógica de turnos
- Trata a Player y Enemy de la misma manera como Character

### 3. Sistema de Personajes (Character, Player, Enemy)

**Qué hace**: Maneja el comportamiento y estado de los personajes

- **Responsabilidad única**: Character maneja solo estado y comportamiento base
- **Se puede extender**: Es fácil agregar nuevos tipos heredando de Character
- Player y Enemy son intercambiables donde se necesita Character

### 4. Sistema de Grilla (GridSystem)

**Qué hace**: Gestiona el campo de batalla y posicionamiento

- **Responsabilidad única**: Solo maneja lógica de grilla y posicionamiento
- **Se puede extender**: Puedo cambiar el tamaño de grilla fácilmente

### 5. Sistema de Interfaz de Usuario

**Qué hace**: Presentación visual e interacción del usuario

- **Responsabilidad única**: Cada componente UI tiene una función específica
- **Se puede extender**: Fácil agregar nuevos tipos de UI sin modificar existentes

### 6. Sistema de Gestión de Ads (AdsManager)

**Qué hace**: Se encarga de gestionar creación de la publicidad, en este caso dos tipos implementados Banners y Interstitial

**Por qué está bien hecho**:

- **Responsabilidad única**: Sólo maneja la lógica de creación de propagandas.
- **Se puede extender**: Fácil agregar nuevos tipos de ads sin modificar existentes

### 7. Sistema de notificaciones (NotificationsManager)

**Qué hace**: Se encarga de realizar las configuraciones necesarias para que el juego envíe una notificación push al cabo de 10 minutos.

- **Responsabilidad única**: Sólo maneja la lógica de implementación de notificaciones.

### 7. Sistema de inputs (InputManager)

**Qué hace**: Se encarga de detectar que tipo de input se está utilizando y ejecutar su lógica correspondiente.

- **Responsabilidad única**: Sólo maneja la lógica de detección y ejecución del tipo de input.
- **Se puede extender**: Fácil agregar nuevos tipos de inputs sin modificar existentes.

## Principios que Seguí

### Responsabilidad Única

**Cómo lo apliqué**:

- `GameManager`: Solo gestiona estado del juego y coordinación
- `TurnManager`: Solo controla flujo de turnos
- `GridSystem`: Solo maneja lógica de grilla
- `PlayerActionController`: Solo coordina acciones de jugadores
- `MovementSystem`: Solo gestiona movimiento de personajes
- `AdsManager`: Solo gestiona la lógica de los ADS
- `NotificationManager`: Solo gestiona la lógica de las notificaciones
- `InputManager`: Solo gestiona la lógica de detección y ejecución del input.

### Abierto/Cerrado

**Cómo lo apliqué**:

```csharp
// Clase base abstracta permite extensión sin modificación
public abstract class Character : MonoBehaviour
{
    public abstract void ExecuteTurn();
}

// Nuevos tipos se pueden agregar sin modificar Character
public class Player : Character { /* implementación */ }
public class Enemy : Character { /* implementación */ }
```

## Patrones que Usé

### 1. Patrón Observer (Basado en Eventos)

**Para qué lo uso**: Comunicación entre sistemas

```csharp
public event Action<Character> OnCharacterDeath;
public event Action<Character> OnTurnStart;
public event Action<Character> OnGameWon;
```

### 2. Patrón Command (Implícito)

**Para qué lo uso**: Acciones de personajes encapsuladas

```csharp
public class PlayerAction
{
    public ActionType actionType;
    public Character target;
    public bool isAvailable;
}
```

## Cómo Funcionan los Eventos

### Flujo Principal

1. **Inicio de Turno**: `TurnManager.OnTurnStart` → UI actualiza jugador actual
2. **Cambio de Salud**: `Character.OnHealthChanged` → UI actualiza estadísticas
3. **Muerte de Personaje**: `Character.OnCharacterDeath` → GameManager verifica condiciones
4. **Fin de Juego**: `GameManager.OnGameWon/OnGameLost` → UI muestra resultado

### Por Qué Uso Eventos

- **Desacoplamiento**: Los sistemas no necesitan referencias directas
- **Extensibilidad**: Fácil agregar nuevos observadores
- **Mantenibilidad**: Cambios localizados sin afectar otros sistemas

### Para Escalabilidad

- Sistema preparado para grillas más grandes
- Arquitectura soporta más tipos de personajes
- UI dinámica escala con número de acciones

## Beneficios del Sistema

1. **Mantenibilidad**: Código limpio y bien organizado
2. **Extensibilidad**: Fácil agregar nuevas características
3. **Testabilidad**: Componentes aislados y fáciles de probar
4. **Reutilización**: Sistemas genéricos reutilizables
5. **Rendimiento**: Optimizaciones sin comprometer claridad
6. **Multiplataforma**: Funciona tanto en móviles como en escritorio
