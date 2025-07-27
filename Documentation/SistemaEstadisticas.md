# Sistema de Estadísticas de Personajes

## Resumen

Este sistema implementa la visualización en tiempo real de las estadísticas de personajes, incluyendo salud, estado de muerte, indicadores de turno y sprites de personajes, usando una arquitectura escalable y event-driven que responde automáticamente a cambios en el estado del juego.

## Arquitectura del Sistema

### Componentes Principales

#### 1. CharacterStatsUI - Coordinador Principal

```csharp
public class CharacterStatsUI : MonoBehaviour
{
    [Header("UI Configuration")]
    public Transform charactersContainer;        // Contenedor para displays
    public GameObject characterStatsPrefab;      // Prefab del display individual
    
    private List<CharacterStatsDisplay> characterDisplays = new List<CharacterStatsDisplay>();
}
```

**Principios SOLID**:

- **SRP**: Solo coordina la creación y gestión de displays de estadísticas
- **OCP**: Extensible para diferentes tipos de displays sin modificación
- **DIP**: Depende de eventos abstractos, no implementaciones específicas

#### 2. CharacterStatsDisplay - Display Individual

```csharp
public class CharacterStatsDisplay : MonoBehaviour
{
    [Header("UI Components")]
    public Image characterImage;        // Sprite del personaje
    public TMP_Text characterNameText;  // Nombre del personaje
    public TMP_Text healthText;         // Salud actual
    
    private Character assignedCharacter;
    private Outline outlineComponent;   // Indicador de turno
}
```

**Principios SOLID**:

- **SRP**: Solo maneja la visualización de un personaje específico
- **ISP**: Expone solo métodos necesarios para actualización de stats
- **LSP**: Funciona uniformemente con Player y Enemy

## Implementación de Principios SOLID

### Principio de Responsabilidad Única (SRP)

**CharacterStatsUI**:

- Solo gestiona la creación y coordinación de displays
- No maneja lógica de personajes o cálculos de estadísticas

**CharacterStatsDisplay**:

- Solo actualiza la visualización de un personaje
- No gestiona múltiples personajes o lógica global

### Principio Abierto/Cerrado (OCP)

```csharp
// Fácil agregar nuevos tipos de información sin modificar código existente
public class CharacterStatsDisplay : MonoBehaviour
{
    // Campos existentes
    public TMP_Text healthText;
    public TMP_Text characterNameText;
    
    // Nuevos campos se pueden agregar sin modificar métodos existentes
    // public TMP_Text manaText;     ← Extensible
    // public TMP_Text armorText;    ← Extensible
    // public Slider experienceBar;  ← Extensible
}
```

### Principio de Inversión de Dependencias (DIP)

```csharp
// Sistema depende de eventos abstractos, no implementaciones concretas
private void Start()
{
    // Depende de GameManager events, no métodos específicos
    GameManager.Instance.OnCharactersSpawned += InitializeCharacterDisplays;
    
    // Depende de TurnManager events, no implementación específica
    TurnManager turnManager = FindFirstObjectByType<TurnManager>();
    if (turnManager != null)
    {
        turnManager.OnTurnStart += OnTurnStart;
        turnManager.OnTurnEnd += OnTurnEnd;
    }
}
```

## Sistema de Actualización en Tiempo Real

### Suscripción a Eventos de Personajes

```csharp
public void AssignCharacter(Character character)
{
    if (assignedCharacter != null)
    {
        // Desuscribir eventos previos
        assignedCharacter.OnCharacterDeath -= OnCharacterDeath;
        assignedCharacter.OnHealthChanged -= OnHealthChanged;
    }

    assignedCharacter = character;

    if (assignedCharacter != null)
    {
        // Suscribir a eventos del nuevo personaje
        assignedCharacter.OnCharacterDeath += OnCharacterDeath;
        assignedCharacter.OnHealthChanged += OnHealthChanged;
        
        UpdateDisplay();
    }
}
```

### Actualización de Salud

```csharp
private void OnHealthChanged(Character character, int newHealth)
{
    if (character == assignedCharacter)
    {
        UpdateHealthDisplay();
    }
}

private void UpdateHealthDisplay()
{
    if (assignedCharacter != null && healthText != null)
    {
        healthText.text = $"HP: {assignedCharacter.CurrentHealth}/{assignedCharacter.GetStats().maxHealth}";
    }
}
```

### Manejo de Muerte de Personajes

```csharp
private void OnCharacterDeath(Character character)
{
    if (character == assignedCharacter)
    {
        // Actualizar visualización para mostrar estado de muerte
        UpdateDeathVisuals();
    }
}

private void UpdateDeathVisuals()
{
    if (assignedCharacter != null && assignedCharacter.IsDead)
    {
        // Aplicar transparencia para indicar muerte
        if (characterImage != null)
        {
            Color color = characterImage.color;
            color.a = 0.3f;  // 30% de opacidad
            characterImage.color = color;
        }
        
        // Actualizar texto de salud
        if (healthText != null)
        {
            healthText.text = "DEAD";
            healthText.color = Color.red;
        }
    }
}
```

## Sistema de Indicadores de Turno

### Uso del Componente Outline

```csharp
private void OnTurnStart(Character character)
{
    UpdateTurnIndicator(character);
}

private void OnTurnEnd(Character character)
{
    UpdateTurnIndicator(null);
}

private void UpdateTurnIndicator(Character currentCharacter)
{
    bool isCurrentTurn = (currentCharacter == assignedCharacter);
    
    if (outlineComponent != null)
    {
        outlineComponent.enabled = isCurrentTurn;
    }
}
```

**Beneficios del Outline**:

- **Visual Claro**: Indicador visual distintivo sin elementos adicionales
- **Performance**: Componente Unity optimizado
- **Simplicidad**: Fácil de configurar y usar

## Gestión de Nombres de Personajes

### Formateo Inteligente de Nombres

```csharp
private void UpdateCharacterName()
{
    if (assignedCharacter != null && characterNameText != null)
    {
        string formattedName = GetFormattedCharacterName(assignedCharacter);
        characterNameText.text = formattedName;
    }
}

private string GetFormattedCharacterName(Character character)
{
    string baseName = character.GetStats().characterName;
    
    // Agregar información adicional para jugadores
    if (character.CharacterType.IsPlayer())
    {
        int playerNumber = GetPlayerNumber(character.CharacterType);
        return $"{baseName} (Player {playerNumber})";
    }
    
    return baseName;
}

private int GetPlayerNumber(CharacterType characterType)
{
    return characterType switch
    {
        CharacterType.Player1 => 1,
        CharacterType.Player2 => 2,
        CharacterType.Player3 => 3,
        _ => 0
    };
}
```

## Integración con Sprites de Personajes

### Carga Automática de Sprites

```csharp
private void UpdateCharacterSprite()
{
    if (assignedCharacter != null && characterImage != null)
    {
        var stats = assignedCharacter.GetStats();
        if (stats.characterSprite != null)
        {
            characterImage.sprite = stats.characterSprite;
        }
    }
}
```

### Configuración de Sprites en GameManager

```csharp
// En GameManager.cs
private BaseCharacterStats CreatePlayerStats(CharacterType playerType)
{
    BaseCharacterStats stats = playerType switch
    {
        CharacterType.Player1 => CharacterStatsConfigurator.CreatePlayer1Stats(),
        CharacterType.Player2 => CharacterStatsConfigurator.CreatePlayer2Stats(),
        CharacterType.Player3 => CharacterStatsConfigurator.CreatePlayer3Stats(),
        _ => throw new ArgumentException($"Invalid player type: {playerType}")
    };

    // Asignar sprite apropiado
    stats.characterSprite = playerType switch
    {
        CharacterType.Player1 => fighterSprite,
        CharacterType.Player2 => healerSprite,
        CharacterType.Player3 => rangeSprite,
        _ => null
    };

    return stats;
}
```

## Sistema de Limpieza y Reinicio

### Limpieza Apropiada

```csharp
public void ClearDisplays()
{
    foreach (var display in characterDisplays)
    {
        if (display != null)
        {
            display.Cleanup();
            Destroy(display.gameObject);
        }
    }
    
    characterDisplays.Clear();
}

// En CharacterStatsDisplay.cs
public void Cleanup()
{
    if (assignedCharacter != null)
    {
        assignedCharacter.OnCharacterDeath -= OnCharacterDeath;
        assignedCharacter.OnHealthChanged -= OnHealthChanged;
    }
}
```

### Reinicio para Nuevos Juegos

```csharp
public void ResetUI()
{
    ClearDisplays();
    
    // Reactivar componentes si fueron desactivados
    if (gameObject != null)
    {
        gameObject.SetActive(true);
    }
}
```

## Optimizaciones y Consideraciones

### 1. Event Management Eficiente

```csharp
// Suscripción/desuscripción apropiada previene memory leaks
private void OnDestroy()
{
    if (GameManager.Instance != null)
        GameManager.Instance.OnCharactersSpawned -= InitializeCharacterDisplays;
        
    // Limpiar referencias a personajes
    foreach (var display in characterDisplays)
    {
        if (display != null)
            display.Cleanup();
    }
}
```

### 2. Null Checking Defensivo

```csharp
private void UpdateDisplay()
{
    if (assignedCharacter == null) return;
    
    // Verificar componentes UI antes de usar
    if (characterNameText != null)
        UpdateCharacterName();
    
    if (healthText != null)
        UpdateHealthDisplay();
    
    if (characterImage != null)
        UpdateCharacterSprite();
}
```

### 3. Lazy Loading

```csharp
private void EnsureOutlineComponent()
{
    if (outlineComponent == null)
    {
        outlineComponent = GetComponent<Outline>();
        if (outlineComponent == null)
        {
            outlineComponent = gameObject.AddComponent<Outline>();
        }
    }
}
```

## Configuración de Prefab

### Estructura del Prefab CharacterStatsDisplay

```bash
CharacterStatsDisplay
├── Background (Image)           # Fondo del panel
├── CharacterImage (Image)       # Sprite del personaje
├── CharacterNameText (TMP_Text) # Nombre del personaje
├── HealthText (TMP_Text)        # Salud actual
├── CharacterStatsDisplay (Script) # Lógica de display
└── Outline (Outline)           # Indicador de turno
```

### Configuración Recomendada

- **Background**: Color semi-transparente para contraste
- **CharacterImage**: Preserve Aspect habilitado
- **Outline**: Color distintivo (amarillo/verde), grosor 2-3 pixels
- **ContentSizeFitter**: Ajuste automático de contenido

## Beneficios del Sistema

### 1. Información en Tiempo Real

- **Actualización Inmediata**: Cambios reflejados instantáneamente
- **Sincronización**: Siempre coherente con el estado del juego
- **Responsividad**: UI responde a todos los eventos relevantes

### 2. Escalabilidad

- **Número Variable**: Funciona con cualquier cantidad de personajes
- **Tipos Flexibles**: Maneja Player y Enemy uniformemente
- **Extensibilidad**: Fácil agregar nuevos tipos de información

### 3. Mantenibilidad

- **Separación Clara**: UI separada de lógica de juego
- **Event-Driven**: Bajo acoplamiento entre componentes
- **Limpieza Apropiada**: No memory leaks o referencias colgantes

### 4. Usabilidad

- **Información Clara**: Toda la información relevante visible
- **Indicadores Visuales**: Turnos y muerte claramente señalados
- **Layout Consistente**: Organización predecible y familiar
