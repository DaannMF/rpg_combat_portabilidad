# Grid-Based Action UI System

## Overview

Este nuevo sistema de UI reemplaza los botones individuales por un diseño más eficiente que agrupa las acciones por tipo y muestra grillas de characters objetivo como sprites clickeables.

## Características Principales

### 1. **Agrupación por Tipo de Acción**

- Las acciones se agrupan por tipo (Heal Self, Heal Range, Attack Melee, Attack Range)
- Cada tipo de acción tiene su propia grilla de targets
- Reduce la cantidad de botones y mejora la organización visual

### 2. **Sprites como Botones**

- Los characters objetivo se muestran como sprites clickeables
- Cada sprite muestra el character sprite del objetivo
- Outline amarillo para efectos visuales (hover/selection)

### 3. **Diseño Escalable**

- GridLayoutGroup se adapta automáticamente al número de targets
- Máximo 6 targets por fila con espaciado automático
- Textos claros para cada tipo de acción

## Componentes del Sistema

### **ActionGroup** (`Assets/Scripts/Models/ActionGroup.cs`)

- Agrupa acciones por tipo
- Mantiene lista de characters objetivo disponibles
- Determina disponibilidad del grupo

### **CharacterTargetButton** (`Assets/Scripts/UI/CharacterTargetButton.cs`)

- Botón individual para cada character objetivo
- Muestra sprite del character
- Maneja clicks y callbacks

### **ActionTargetGrid** (`Assets/Scripts/UI/ActionTargetGrid.cs`)

- Contenedor para un tipo de acción
- Muestra título de la acción
- Crea y maneja grilla de botones objetivo

### **PlayerActionUI** (Modificado)

- Cambiado para usar el nuevo sistema de grillas
- Mantiene compatibilidad con Heal Self como botón individual
- Maneja eventos de selección de targets

## Configuración en Unity

### 1. **Actualizar PlayerActionUI**

```csharp
// Campos requeridos en PlayerActionUI:
[SerializeField] private GameObject actionGridPrefab;      // Asignar action_target_grid prefab
[SerializeField] private Transform actionGridContainer;   // Contenedor para las grillas
[SerializeField] private Button healSelfButton;          // Botón independiente para Heal Self
```

### 2. **Configurar Prefabs**

- **character_target_button.prefab**: Botón de 50x50 pixels con Image, Button, CharacterTargetButton y Outline
- **action_target_grid.prefab**: Contenedor con título y GridLayoutGroup para botones de targets

### 3. **Referencias de Escena**

```
PlayerActionUI
├── actionGridPrefab → action_target_grid.prefab
├── actionGridContainer → Panel que contendrá las grillas
├── healSelfButton → Botón independiente para Heal Self
├── endTurnButton → Botón de terminar turno
├── movementText → Texto de movimiento restante
└── currentPlayerText → Texto del jugador actual
```

## Flujo de Funcionamiento

### 1. **Inicio del Turno**

```csharp
PlayerActionController.StartPlayerTurn()
↓
PlayerAbilitySystem.GetAvailableActions()
↓
PlayerActionController.GroupActionsByType()
↓
PlayerActionUI.UpdateActionGrids()
```

### 2. **Creación de Grillas**

```csharp
Por cada ActionGroup:
- Crear ActionTargetGrid desde prefab
- Configurar título de la acción
- Crear CharacterTargetButton para cada target
- Configurar callbacks de selección
```

### 3. **Selección de Target**

```csharp
CharacterTargetButton.OnButtonClicked()
↓
ActionTargetGrid.OnTargetSelected()
↓
PlayerActionUI.OnTargetSelected()
↓
PlayerActionController.TryExecuteActionOnTarget()
```

## Ventajas del Nuevo Sistema

### **Mejor Organización Visual**

- Acciones agrupadas por tipo
- Menos botones repetidos
- Interfaz más limpia

### **Escalabilidad**

- Se adapta automáticamente a cualquier número de targets
- Fácil de expandir con nuevos tipos de acciones
- GridLayoutGroup maneja el layout automáticamente

### **Mejor UX**

- Sprites visuales en lugar de texto largo
- Identificación rápida de targets
- Menos scroll vertical necesario

## Tipos de Acciones Soportadas

### **Heal Self**

- Botón individual (reutiliza el sistema anterior)
- No necesita grilla de targets

### **Heal Ally (HealOther)**

- Grilla de players aliados en rango
- Sprites de players disponibles

### **Melee Attack**

- Grilla de characters adyacentes
- Sprites de enemies y players en rango 1

### **Ranged Attack**

- Grilla de characters en rango
- Sprites de todos los targets válidos

## Próximos Pasos

1. **Configurar Referencias**: Asignar prefabs y contenedores en la escena
2. **Probar Funcionalidad**: Verificar que las grillas se crean correctamente
3. **Ajustar Layout**: Configurar tamaños y espaciado según necesidades
4. **Personalizar Sprites**: Agregar efectos visuales adicionales si es necesario

## Troubleshooting

### **Problema**: Grillas no se muestran

**Solución**: Verificar que `actionGridPrefab` esté asignado y que el prefab tenga el componente `ActionTargetGrid`

### **Problema**: Sprites no se muestran

**Solución**: Verificar que los characters tengan `characterSprite` asignado en sus `BaseCharacterStats`

### **Problema**: Botones no funcionan

**Solución**: Verificar que los callbacks estén configurados correctamente y que `CharacterTargetButton` tenga referencias válidas
