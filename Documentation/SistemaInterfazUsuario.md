# Sistema de Interfaz de Usuario

## Resumen

Este sistema implementa una interfaz de usuario dinámica basada en grillas que organiza las acciones por tipo y presenta objetivos como sprites de personajes clickeables, resolviendo problemas de escalabilidad y usabilidad de la UI tradicional basada en botones de texto.

## Arquitectura del Sistema

### Componentes Principales

#### 1. ActionGroup - Agrupador de Acciones

```csharp
public class ActionGroup
{
    public ActionType actionType;          // Tipo de acción (MeleeAttack, RangedAttack, etc.)
    public List<Character> availableTargets;  // Objetivos válidos para esta acción
    public bool isAvailable;               // Si la acción está disponible
}
```

**Principios SOLID**:

- **SRP**: Solo agrupa acciones relacionadas por tipo
- **OCP**: Extensible para nuevos tipos de acción sin modificación

#### 2. CharacterTargetButton - Botón de Objetivo Individual

```csharp
public class CharacterTargetButton : MonoBehaviour
{
    private Character targetCharacter;
    private ActionType actionType;
    private System.Action<ActionType, Character> onTargetSelected;
}
```

**Principios SOLID**:

- **SRP**: Solo maneja la interacción con un objetivo específico
- **ISP**: Interfaz simple enfocada en selección de objetivo

#### 3. ActionTargetGrid - Contenedor de Grilla

```csharp
public class ActionTargetGrid : MonoBehaviour
{
    public void Setup(ActionType actionType, System.Action<ActionType, Character> callback)
    public void UpdateTargets(List<Character> targets)
}
```

**Principios SOLID**:

- **SRP**: Solo gestiona la grilla de objetivos para un tipo de acción
- **DIP**: Depende de abstracción (callback) no implementación concreta

## Implementación de Principios SOLID

### Principio de Responsabilidad Única (SRP)

**PlayerActionUI**:

- Solo maneja la presentación visual de acciones disponibles
- No contiene lógica de juego o validación de acciones

**PlayerActionController**:

- Solo coordina acciones y movimiento
- No maneja presentación visual

**ActionTargetGrid**:

- Solo gestiona la disposición visual de objetivos
- No valida acciones o maneja estado del juego

### Principio Abierto/Cerrado (OCP)

```csharp
// Fácil agregar nuevos tipos de acción sin modificar UI existente
public enum ActionType
{
    MeleeAttack,
    RangedAttack,
    HealSelf,
    HealOther,
    EndTurn,
    // NewActionType  ← Se puede agregar sin modificar código UI
}
```

### Principio de Inversión de Dependencias (DIP)

```csharp
// PlayerActionUI depende de abstracción (events) no implementación
public class PlayerActionUI : MonoBehaviour
{
    private void Initialize(PlayerActionController controller)
    {
        // Depende de eventos abstractos, no métodos específicos
        controller.OnActionGroupsUpdated += UpdateActionGrids;
        controller.OnMovementUpdated += UpdateMovementDisplay;
    }
}
```

## Flujo de Funcionamiento

### 1. Generación de Grupos de Acciones

```csharp
// PlayerActionController agrupa acciones por tipo
private List<ActionGroup> GroupActionsByType(List<PlayerAction> actions)
{
    var groups = new Dictionary<ActionType, ActionGroup>();

    foreach (var action in actions)
    {
        if (action.actionType == ActionType.EndTurn) continue;

        if (!groups.ContainsKey(action.actionType))
            groups[action.actionType] = new ActionGroup(action.actionType);

        if (action.isAvailable && action.target != null)
            groups[action.actionType].AddTarget(action.target);
    }

    return groups.Values.ToList();
}
```

### 2. Actualización de UI

```csharp
// PlayerActionUI responde a cambios en acciones disponibles
private void UpdateActionGrids(List<ActionGroup> actionGroups)
{
    ClearActionGrids();

    foreach (var group in actionGroups)
    {
        if (group.actionType == ActionType.HealSelf)
            continue; // HealSelf usa botón individual

        CreateActionGrid(group);
    }
}
```

### 3. Creación Dinámica de Grillas

```csharp
private void CreateActionGrid(ActionGroup actionGroup)
{
    GameObject gridObj = Instantiate(actionGridPrefab, actionGridContainer);
    var actionGrid = gridObj.GetComponent<ActionTargetGrid>();

    if (actionGrid != null)
    {
        actionGrid.Setup(actionGroup.actionType, OnTargetSelected);
        actionGrid.UpdateTargets(actionGroup.availableTargets);
        actionGrids.Add(actionGrid);
    }
}
```

## Beneficios del Sistema

### 1. Organización Visual Mejorada

- **Agrupación por Tipo**: Acciones similares se muestran juntas
- **Sprites en lugar de Texto**: Representación visual más clara
- **Escalabilidad**: Se adapta automáticamente al número de objetivos

### 2. Usabilidad Optimizada

- **Menos Botones**: Reduce saturación visual
- **Interacción Intuitiva**: Sprites de personajes son self-explanatory
- **Layout Responsive**: Se ajusta al espacio disponible

### 3. Mantenibilidad del Código

- **Separación de Responsabilidades**: UI, lógica y datos separados
- **Extensibilidad**: Fácil agregar nuevos tipos de acciones
- **Reutilización**: Componentes reutilizables para diferentes contextos

## Manejo de Casos Especiales

### 1. Acción HealSelf

```csharp
// HealSelf mantiene botón individual por simplicidad
private void UpdateActionButtons(List<PlayerAction> availableActions)
{
    if (healSelfButton != null)
    {
        var healSelfAction = availableActions.Find(a => a.actionType == ActionType.HealSelf);
        healSelfButton.interactable = healSelfAction != null && healSelfAction.isAvailable;
    }
}
```

**Razón**: HealSelf no requiere selección de objetivo, por lo que una grilla sería innecesaria.

### 2. Botón EndTurn

```csharp
// EndTurn siempre disponible como botón individual
private void EndTurn()
{
    if (currentPlayer != null && actionController != null)
    {
        var endTurnAction = new PlayerAction(ActionType.EndTurn);
        actionController.TryExecuteAction(currentPlayer, endTurnAction);
    }
}
```

**Razón**: EndTurn es una acción siempre disponible que no requiere objetivos.

## Configuración de Prefabs

### Prefab ActionTargetGrid

**Estructura**:

```bash
ActionTargetGrid
├── TitleText (TMP_Text)           # Título del tipo de acción
└── TargetsContainer              # Contenedor con GridLayoutGroup
    ├── GridLayoutGroup           # Layout automático
    └── ContentSizeFitter         # Ajuste automático de tamaño
```

### Prefab CharacterTargetButton

**Estructura**:

```bash
CharacterTargetButton
├── Button (Button)               # Componente de interacción
├── Image (Image)                # Sprite del personaje
├── CharacterTargetButton (Script) # Lógica de interacción
└── Outline (Outline)            # Indicador visual opcional
```

## Eventos y Comunicación

### Patrón Observer Implementado

```csharp
// PlayerActionController notifica cambios
public event Action<List<ActionGroup>> OnActionGroupsUpdated;

// PlayerActionUI escucha y responde
controller.OnActionGroupsUpdated += UpdateActionGrids;

// Callback para selección de objetivos
private void OnTargetSelected(ActionType actionType, Character target)
{
    actionController.TryExecuteActionOnTarget(currentPlayer, actionType, target);
}
```

**Beneficios**:

- **Desacoplamiento**: UI no conoce lógica interna del controlador
- **Responsividad**: UI se actualiza automáticamente
- **Flexibilidad**: Múltiples observadores pueden reaccionar a cambios

## Consideraciones de Rendimiento

### 1. Instanciación Eficiente

- Botones se crean solo cuando son necesarios
- Destrucción inmediata de botones obsoletos
- Pooling de objetos no implementado pero preparado para futuras optimizaciones

### 2. Updates Optimizados

- UI se actualiza solo cuando cambian las acciones disponibles
- No hay polling continuo de estado
- Eventos discretos minimizan llamadas de actualización

### 3. Memoria

- Referencias se limpian apropiadamente en OnDestroy()
- No hay memory leaks por event listeners
- Prefabs compartidos reducen uso de memoria

## Extensibilidad

### Agregar Nuevo Tipo de Acción

1. **Actualizar Enum**:

    ```csharp
    public enum ActionType
    {
        // ... existing types
        NewSpecialAttack
    }
    ```

2. **Implementar en PlayerAbilitySystem**:

    ```csharp
    case ActionType.NewSpecialAttack:
        // Lógica de validación y ejecución
        break;
    ```

3. **UI Automática**: El sistema UI maneja automáticamente el nuevo tipo sin modificaciones adicionales.

### Personalizar Apariencia

- **Sprites**: Cambiar sprites en CharacterTargetButton prefab
- **Layout**: Modificar GridLayoutGroup para diferentes disposiciones
- **Colores**: Usar Outline component para indicadores visuales
- **Animaciones**: Agregar Animator para transiciones suaves
