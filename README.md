# Sistema de Combate RPG

## Descripción del Juego

Sistema de combate RPG por turnos desarrollado en Unity, que presenta un campo de batalla de grilla 4x6 con 3 jugadores únicos y 2 enemigos controlados por IA. El juego implementa combate táctico estratégico donde los jugadores deben posicionarse y usar sus habilidades únicas para derrotar a todos los enemigos mientras al menos un jugador sobrevive.

El sistema cuenta con tres clases de personajes distintas (Luchador, Sanador, Arquero), cada una con estadísticas, habilidades y rangos de ataque únicos. Los jugadores se turnan con los enemigos en un sistema de combate estratégico basado en turnos.

## Información del Proyecto

**Autor**: Daniel Fimiani  
**Proyecto**: Final
**Materia**: Portabilidad y Optimización  
**Institución**: Image Campus  

## Características Principales

- **Combate por Turnos**: Mecánicas estratégicas de movimiento y acción
- **Clases de Personajes**: Tres tipos únicos de jugadores con habilidades distintivas
- **Movimiento Basado en Grilla**: Campo de batalla táctico 4x6
- **Sistema de Combate**: Ataques cuerpo a cuerpo, a distancia y habilidades de sanación
- **Arquitectura ScriptableObject**: Estadísticas de personajes configurables
- **Arquitectura Limpia**: Implementa principios SOLID y patrones de diseño

## Controles

### Movimiento

#### Teclado

- **WASD** o **Flechas**: Mover una celda en la dirección correspondiente
- **Click**: Ejecutar acción(Atacar, sanar, terminar turno)

#### Móviles

- **Touch** o **Flechas**: Mover una celda en la dirección correspondiente y ejecutar acción(Atacar, sanar, terminar turno)

### Acciones

- **UI de Botones**: Interfaz visual con botones dinámicos para acciones disponibles
- **Botón "Heal Self"**: Sanación propia (si es posible)
- **Botón "End Turn"**: Terminar turno
- **Enter**: Forzar fin de turno

## Condiciones de Victoria/Derrota

### Victoria

- Todos los enemigos son derrotados
- Exactamente 1 jugador sobrevive

### Derrota

- Cualquier jugador muere mientras haya enemigos vivos

## Tipos de Personajes

### Jugador 1 - Luchador

- **Salud**: 20 HP
- **Velocidad**: 3 celdas por turno
- **Ataque Cuerpo a Cuerpo**: 5 de daño
- **Ataque a Distancia**: Ninguno
- **Sanación**: 2 HP (solo a sí mismo)

### Jugador 2 - Sanador

- **Salud**: 15 HP
- **Velocidad**: 2 celdas por turno
- **Ataque Cuerpo a Cuerpo**: 2 de daño
- **Ataque a Distancia**: 2 de daño (rango 3)
- **Sanación**: 5 HP (a sí mismo y otros, rango 2)

### Jugador 3 - Arquero

- **Salud**: 15 HP
- **Velocidad**: 4 celdas por turno
- **Ataque Cuerpo a Cuerpo**: 1 de daño
- **Ataque a Distancia**: 3 de daño (rango ilimitado)
- **Sanación**: 2 HP (solo a sí mismo)

### Enemigo

- **Salud**: 10 HP
- **Velocidad**: 1 celda por turno
- **Ataque Cuerpo a Cuerpo**: 3 de daño
- **Ataque a Distancia**: 1 de daño (rango 3)
- **Sanación**: Ninguna

## Documentación de Sistemas

El proyecto incluye documentación detallada sobre la implementación de sistemas y principios SOLID:

### Documentación Principal

- **[Arquitectura General](Documentation/ArquitecturaGeneral.md)**: Visión general del sistema y implementación de principios SOLID
- **[Adaptación a móviles](Documentation/AdaptacionMoviles.md)**: Cambios realizados para adaptar el juego a dispositivos móviles.

## Instalación y Configuración

### Requisitos

- Unity 2021.3 o posterior
- Universal Render Pipeline (URP)

### Instrucciones de Instalación

1. Clonar este repositorio
2. Abrir el proyecto en Unity
3. Abrir la escena principal (`Assets/Scenes/SampleScene.unity`)
4. Presionar Play para iniciar el juego

## Arquitectura del Proyecto

### Sistemas Principales

- **GridSystem**: Gestiona el campo de batalla 4x6 y seguimiento de posiciones
- **Character System**: Maneja comportamiento, estadísticas y acciones de personajes
- **TurnManager**: Controla el flujo de juego por turnos
- **GameManager**: Orquesta el estado general del juego
- **UI System**: Muestra información del juego y maneja retroalimentación del usuario
- **Ads Manager**: Controla la publicidad
- **Notifications Manager**: Gestiona las notificaciones push

### Estructura del Proyecto

```bash
Assets/
├── Scripts/
│   ├── Characters/          # Clases de personajes
│   ├── Configuration/       # Configuradores de estadísticas
│   ├── Controllers/         # Controladores de acciones
│   ├── Data/               # Scriptable Objects
│   ├── Enums/              # Enumeraciones del sistema
│   ├── Grid/               # Sistema de grilla
│   ├── Input/              # Gestor de inputs
│   ├── Interfaces/         # Interfaces del sistema
│   ├── Managers/           # Gestores principales
│   ├── Models/             # Modelos de datos
│   ├── Systems/            # Sistemas de lógica
│   ├── UI/                 # Interfaz de usuario
│   └── Documentation/      # Documentación técnica
├── Prefabs/                # Prefabs del juego
├── Art/Sprites/           # Sprites de personajes y UI
└── Scenes/                # Escenas del juego
```
