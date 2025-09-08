# Modular Enemy AI System

This is a modular and extensible AI system for Unity that provides three main states: Patrol, Chase, and Attack. The system is designed to be reusable across different enemy types and easily customizable.

## System Overview

The AI system consists of several key components:

### Core Components

1. **AIState** - Abstract base class for all AI states
2. **EnemyAI** - Main manager that handles state transitions and player detection
3. **Patrol** - State for patrolling between waypoints
4. **Chase** - State for following the player when detected
5. **Attack** - State for attacking the player when in range

### Additional Components

- **PlayerHealth** - Simple health system for testing attacks
- **EnemyAIExample** - Example script showing how to use the system

## Setup Instructions

### 1. Basic Setup

1. Add the `EnemyAI` component to your enemy GameObject
2. Ensure your enemy has a `NavMeshAgent` component (will be added automatically if missing)
3. Assign the player Transform to the EnemyAI component
4. The system will automatically add the three state components (Patrol, Chase, Attack)

### 2. Configure Patrol Waypoints

1. Create empty GameObjects to serve as waypoints
2. Assign these waypoints to the Patrol state's waypoints array
3. The enemy will patrol between these waypoints in order

### 3. Configure Detection

- **Detection Range**: How far the enemy can detect the player
- **Field of View**: The angle of the detection cone (in degrees)
- **Player Layer**: The layer the player is on
- **Obstacle Layer**: Layers that block line of sight

### 4. Configure Attack

- **Attack Range**: How close the enemy needs to be to attack
- **Attack Damage**: How much damage each attack deals
- **Attack Cooldown**: Time between attacks
- **Lose Player Distance**: How far the player can get before being lost

## State Transitions

The AI automatically transitions between states based on conditions:

- **Patrol → Chase**: When player is detected within detection range and field of view
- **Chase → Attack**: When player is within attack range
- **Attack → Chase**: When player moves out of attack range but is still detected
- **Chase/Attack → Patrol**: When player is lost (out of detection range)

## Customization

### Creating Custom States

To create a new AI state, inherit from `AIState`:

```csharp
public class CustomState : AIState
{
    protected override void OnStateEnter()
    {
        // State entry logic
    }
    
    protected override void OnStateUpdate()
    {
        // State update logic
    }
    
    protected override void OnStateExit()
    {
        // State exit logic
    }
}
```

### Modifying Existing States

You can extend or modify the existing states by:

1. Creating new classes that inherit from the base states
2. Overriding specific methods to change behavior
3. Adding new configuration parameters

### Using Events

The EnemyAI component provides events for external systems:

```csharp
enemyAI.OnStateChanged += OnAIStateChanged;
enemyAI.OnPlayerDetected += OnPlayerDetected;
enemyAI.OnPlayerLost += OnPlayerLost;
```

## Requirements

- Unity 2020.3 or later
- NavMesh system (for pathfinding)
- Player GameObject with "Player" tag (or manually assign player Transform)

## Troubleshooting

### Common Issues

1. **Enemy not moving**: Check if NavMeshAgent is properly configured and NavMesh is baked
2. **Player not detected**: Ensure player has correct tag and layers are set up properly
3. **States not transitioning**: Check detection range, field of view, and line of sight settings
4. **Attack not working**: Ensure player has a health component with TakeDamage method

### Debug Features

- Enable "Show Debug Gizmos" in EnemyAI to visualize detection range and field of view
- Check the console for debug messages about state transitions
- Use the context menu options in EnemyAIExample to test state transitions

## Performance Considerations

- The system uses NavMeshAgent for efficient pathfinding
- Player detection is optimized with configurable update intervals
- State updates only run when the state is active
- Gizmos are only drawn in the Scene view when enabled

## Future Enhancements

This modular system can be easily extended with:

- Additional states (Investigate, Flee, etc.)
- More sophisticated detection systems (hearing, smell, etc.)
- Animation integration
- Sound system integration
- Different AI personalities or behaviors
- Group AI behaviors
