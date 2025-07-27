# Arquitectura General del Sistema RPG

## Resumen

Este documento describe la arquitectura del Sistema de Combate RPG, enfocándose en la implementación de los principios SOLID y los patrones de diseño utilizados para crear un sistema mantenible, extensible y robusto.

## Sistemas Principales

### 1. Sistema de Gestión del Juego (GameManager)

**Responsabilidad**: Coordinación central del estado del juego y gestión de sistemas

**Características SOLID**:

- **SRP**: Solo gestiona el estado general del juego y la coordinación de sistemas
- **OCP**: Extensible para nuevos tipos de juego sin modificar código existente
- **DIP**: Depende de abstracciones (Character) en lugar de implementaciones concretas

### 2. Sistema de Turnos (TurnManager)

**Responsabilidad**: Control del flujo de turnos y transiciones

**Características SOLID**:

- **SRP**: Solo maneja la lógica de turnos
- **LSP**: Trata a Player y Enemy de manera uniforme como Character
- **ISP**: Expone solo los eventos necesarios para cada sistema

### 3. Sistema de Personajes (Character, Player, Enemy)

**Responsabilidad**: Comportamiento y estado de los personajes

**Características SOLID**:

- **SRP**: Character maneja solo estado y comportamiento base
- **OCP**: Fácil agregar nuevos tipos heredando de Character
- **LSP**: Player y Enemy son intercambiables donde se espera Character
- **DIP**: Usa BaseCharacterStats como abstracción de datos

### 4. Sistema de Grilla (GridSystem)

**Responsabilidad**: Gestión del campo de batalla y posicionamiento

**Características SOLID**:

- **SRP**: Solo maneja lógica de grilla y posicionamiento
- **OCP**: Extensible para diferentes tamaños de grilla
- **DIP**: No depende de implementaciones específicas de Character

### 5. Sistema de Interfaz de Usuario

**Responsabilidad**: Presentación visual y interacción del usuario

**Características SOLID**:

- **SRP**: Cada componente UI tiene una responsabilidad específica
- **OCP**: Fácil agregar nuevos tipos de UI sin modificar existentes
- **ISP**: Interfaces especializadas para diferentes tipos de interacción

## Implementación de Principios SOLID

### Principio de Responsabilidad Única (SRP)

**Implementación**:

- `GameManager`: Solo gestiona estado del juego y coordinación
- `TurnManager`: Solo controla flujo de turnos
- `GridSystem`: Solo maneja lógica de grilla
- `PlayerActionController`: Solo coordina acciones de jugadores
- `MovementSystem`: Solo gestiona movimiento de personajes

**Beneficio**: Cada clase es fácil de entender, probar y mantener.

### Principio Abierto/Cerrado (OCP)

**Implementación**:

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

**Beneficio**: Sistema extensible para nuevos tipos de personajes.

### Principio de Sustitución de Liskov (LSP)

**Implementación**:

```csharp
// TurnManager trata a todos los Character de manera uniforme
public void Initialize(List<Character> characters)
{
    // Player y Enemy son intercambiables aquí
    allCharacters = characters.ToList();
}
```

**Beneficio**: Código genérico que funciona con cualquier tipo de Character.

### Principio de Segregación de Interfaces (ISP)

**Implementación**:

```csharp
// Interfaces específicas en lugar de una interfaz monolítica
public interface IPlayerAbilitySystem
{
    List<PlayerAction> GetAvailableActions(Character character);
    bool CanPerformAction(Character character, PlayerAction action);
    bool ExecuteAction(Character character, PlayerAction action);
}
```

**Beneficio**: Clases solo dependen de métodos que realmente usan.

### Principio de Inversión de Dependencias (DIP)

**Implementación**:

```csharp
// Depende de abstracción (BaseCharacterStats) no implementación
public class Character : MonoBehaviour
{
    protected BaseCharacterStats stats; // Abstracción
}

// Sistema de alto nivel depende de interfaz
public class PlayerActionController
{
    private readonly IPlayerAbilitySystem abilitySystem; // Abstracción
}
```

**Beneficio**: Bajo acoplamiento y alta flexibilidad.

## Patrones de Diseño Implementados

### 1. Patrón Observer (Event-Driven)

**Uso**: Comunicación entre sistemas

```csharp
public event Action<Character> OnCharacterDeath;
public event Action<Character> OnTurnStart;
public event Action<Character> OnGameWon;
```

**Beneficio**: Bajo acoplamiento entre sistemas.

### 2. Patrón Strategy

**Uso**: Diferentes comportamientos de ExecuteTurn()

```csharp
// Player usa estrategia de entrada de usuario
public override void ExecuteTurn() { /* input handling */ }

// Enemy usa estrategia de IA
public override void ExecuteTurn() { /* AI logic */ }
```

**Beneficio**: Algoritmos intercambiables sin modificar cliente.

### 3. Patrón Factory

**Uso**: Creación de estadísticas de personajes

```csharp
public static class CharacterStatsConfigurator
{
    public static BaseCharacterStats CreatePlayer1Stats() { /* ... */ }
    public static BaseCharacterStats CreateEnemyStats() { /* ... */ }
}
```

**Beneficio**: Creación centralizada y configurable de objetos.

### 4. Patrón Command (Implícito)

**Uso**: Acciones de personajes encapsuladas

```csharp
public class PlayerAction
{
    public ActionType actionType;
    public Character target;
    public bool isAvailable;
}
```

**Beneficio**: Acciones como objetos de primera clase.

## Arquitectura de Eventos

### Flujo de Eventos Principal

1. **Inicio de Turno**: `TurnManager.OnTurnStart` → UI actualiza jugador actual
2. **Cambio de Salud**: `Character.OnHealthChanged` → UI actualiza estadísticas
3. **Muerte de Personaje**: `Character.OnCharacterDeath` → GameManager verifica condiciones
4. **Fin de Juego**: `GameManager.OnGameWon/OnGameLost` → UI muestra resultado

### Beneficios de la Arquitectura de Eventos

- **Desacoplamiento**: Sistemas no necesitan referencias directas
- **Extensibilidad**: Fácil agregar nuevos observadores
- **Mantenibilidad**: Cambios localizados sin afectar otros sistemas

## Consideraciones de Rendimiento

### Optimizaciones Implementadas

1. **Lookups O(1)**: Dictionary-based para posiciones de grilla
2. **Eventos Eficientes**: Solo se actualiza UI cuando es necesario
3. **Structs para Valores**: GridCell usa struct para mejor localidad de memoria
4. **ScriptableObjects Compartidos**: Stats no se duplican en memoria

### Escalabilidad

- Sistema preparado para grillas más grandes
- Arquitectura soporta más tipos de personajes
- UI dinámica escala con número de acciones

## Beneficios de la Arquitectura

1. **Mantenibilidad**: Código limpio y bien organizado
2. **Extensibilidad**: Fácil agregar nuevas características
3. **Testabilidad**: Componentes aislados y mockeable
4. **Reutilización**: Sistemas genéricos reutilizables
5. **Rendimiento**: Optimizaciones sin comprometer claridad
