# Files Overview - Dwight V12

## Core Scripts

### Node.cs
**Purpose**: Grid node data structure  
**Key Features**:
- Stores grid position, terrain properties (wall, rubble, exit)
- Dynamic properties (fire intensity, pheromone value)
- A* pathfinding variables (gCost, hCost, parent)
- Movement cost calculation (Base + Rubble + Fire penalties)
- Pheromone decay system

**Usage**: Used by GameManager to create the grid, accessed by Pathfinding for A* algorithm

---

### GameManager.cs
**Purpose**: Grid generation and tilemap management  
**Key Features**:
- Generates 50x40 grid of Nodes
- Maps Node data to Unity Tilemap system
- Provides grid-to-world position conversion
- Manages tilemap updates (walls, rubble, exits)
- Singleton pattern for global access

**Required Setup**:
- Assign FloorTilemap, WallTilemap, OverlayTilemap references
- Assign Tile assets (FloorTile, WallTile, RubbleTile, ExitTile)
- Configure grid dimensions (default: 50x40)

---

### Pathfinding.cs
**Purpose**: A* pathfinding algorithm implementation  
**Key Features**:
- Static class with FindPath() method
- Cost function: Base(1) + Rubble(10) + Fire(50×intensity)
- Finds path to nearest exit
- Stores visualization data (Open Set, Closed Set, Path)
- Pheromone management (ACO memory system)

**Visualization Data**:
- `OpenSet`: Nodes currently being considered
- `ClosedSet`: Nodes already checked
- `CurrentPath`: Final computed path

**Usage**: Called by PersonController to find escape routes

---

### PersonController.cs
**Purpose**: Agent state machine and movement logic  
**Key Features**:
- State machine: IDLE, MOVING, PANIC, DEAD
- Stats: Health, Stamina, Panic Level
- Smooth interpolation between grid nodes
- POV camera following (for spectating)
- 2D Spot Light setup (Fog of War)
- Damage system (fire, rubble)
- Pathfinding integration

**States**:
- **IDLE**: Wandering/standing still
- **MOVING**: Escaping (following path)
- **PANIC**: Trapped/Burning (high panic, low health)
- **DEAD**: Agent has died

**Events**:
- `OnDeath`: Triggered when health reaches 0
- `OnReachedExit`: Triggered when agent reaches exit

---

### DisasterManager.cs
**Purpose**: Dynamic disaster event system  
**Key Features**:
- **Bomb [B]**: Converts walls/floors to rubble, applies fire in radius
- **Earthquake [E]**: Randomly turns 20% of floor tiles into rubble
- **Fire [F]**: Spreads using Cellular Automata
- Fire spread simulation (neighbor-based)
- Fire decay over time

**Keyboard Controls**:
- **[B]**: Trigger bomb at mouse position
- **[E]**: Trigger earthquake
- **[F]**: Trigger fire at mouse position

**Fire Spread**:
- Uses Cellular Automata
- Spreads to neighboring walkable nodes
- Spread chance based on current intensity
- Natural decay over time

---

### PathfindingVisualizer.cs
**Purpose**: Gizmo rendering for algorithm visualization  
**Key Features**:
- Visualizes Open Set (green cubes)
- Visualizes Closed Set (red cubes)
- Visualizes Final Path (blue lines)
- Visualizes Pheromones (red/green tints)
- Configurable colors and sizes

**Viewing**:
- Must be in Scene View (not Game View)
- Enable Gizmos button in Scene View toolbar
- Shows real-time pathfinding data

---

## Helper Scripts

### AgentSpawner.cs
**Purpose**: Spawn multiple agents for testing  
**Key Features**:
- Spawns configurable number of agents
- Spawns only on walkable nodes (optional)
- Can set first agent as spectator
- Spawns in defined area

**Usage**: Attach to empty GameObject, assign agent prefab, set number of agents

---

### LightingSetupHelper.cs
**Purpose**: Helper for 2D lighting setup (Fog of War)  
**Key Features**:
- Sets up global light
- Configures light intensity and color
- Helper method to add agent lights
- Can run automatically on Start

**Usage**: Attach to GameObject, call SetupLighting() or enable setupOnStart

---

## Documentation

### README.md
**Comprehensive setup guide** including:
- Unity Editor setup (step-by-step)
- Tilemap configuration
- URP lighting setup
- Gizmo visualization guide
- Algorithm details
- Troubleshooting
- Performance optimization tips

### QUICK_START.md
**5-minute quick setup** for experienced Unity developers

### FILES_OVERVIEW.md
**This file** - Overview of all scripts and their purposes

---

## File Dependencies

```
Node.cs
  └── Used by: GameManager, Pathfinding, PersonController

GameManager.cs
  ├── Uses: Node
  └── Used by: Pathfinding, PersonController, DisasterManager, PathfindingVisualizer

Pathfinding.cs
  ├── Uses: Node, GameManager
  └── Used by: PersonController, PathfindingVisualizer

PersonController.cs
  ├── Uses: Node, GameManager, Pathfinding
  └── Used by: AgentSpawner

DisasterManager.cs
  ├── Uses: GameManager, Node
  └── Independent (keyboard input)

PathfindingVisualizer.cs
  ├── Uses: GameManager, Pathfinding
  └── Independent (gizmo rendering)

AgentSpawner.cs
  ├── Uses: GameManager, PersonController
  └── Optional helper

LightingSetupHelper.cs
  └── Optional helper (no dependencies)
```

---

## Required Unity Packages

- **2D Tilemap Extras** (for tile assets)
- **Universal RP** (for 2D lighting)
- **2D Renderer** (included with URP)

---

## Script Execution Order

No specific execution order required, but recommended:
1. GameManager (creates grid)
2. DisasterManager (handles disasters)
3. PersonController (agents use grid)
4. PathfindingVisualizer (visualizes algorithms)

All scripts use `Awake()` and `Start()` appropriately to handle initialization.

---

## Customization Points

### Easy to Modify:
- Grid size (GameManager)
- Movement speeds (PersonController)
- Disaster parameters (DisasterManager)
- Visualization colors (PathfindingVisualizer)

### Moderate Complexity:
- Cost function (Pathfinding.cs - GetMovementCost in Node.cs)
- Fire spread algorithm (DisasterManager)
- State machine logic (PersonController)

### Advanced:
- A* heuristic (Pathfinding.cs - GetDistance method)
- Pheromone system (Pathfinding.cs - UpdatePheromone)
- Multi-level pathfinding (requires grid modifications)

