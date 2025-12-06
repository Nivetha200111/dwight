# Dwight V12 - Disaster Evacuation Simulator

An isometric 2D grid-based simulation where agents (Humans) try to escape a building while avoiding dynamic disasters (Fire, Rubble). The system visualizes underlying algorithms (A*, Pheromones) for debugging purposes.

## Tech Stack
- Unity 2022+ (tested on Unity 2022.3 LTS)
- C#
- 2D URP (Universal Render Pipeline)
- Custom Grid System (No NavMesh)

---

## Project Structure

```
Assets/
├── Scripts/
│   ├── Node.cs                    # Grid node data structure
│   ├── GameManager.cs             # Grid generation & tilemap management
│   ├── Pathfinding.cs             # A* algorithm implementation
│   ├── PersonController.cs        # Agent state machine & movement
│   ├── DisasterManager.cs         # Disaster event system
│   └── PathfindingVisualizer.cs   # Gizmo visualization
├── Scenes/
│   └── MainScene.unity
└── Tilemaps/
    └── (Tile assets)
```

---

## Unity Editor Setup

### Step 1: Create the Scene

1. **Create a new 2D Scene**:
   - File → New Scene → 2D (Core)

2. **Set up the Grid**:
   - Right-click in Hierarchy → 2D Object → Tilemap → Rectangular
   - This creates a `Grid` GameObject with a `Tilemap` child
   - Rename the Grid to "Grid" (if not already)

3. **Create Additional Tilemaps**:
   - Right-click on "Grid" → 2D Object → Tilemap
   - Create three tilemaps:
     - `FloorTilemap` (for floor tiles)
     - `WallTilemap` (for walls)
     - `OverlayTilemap` (for fire/pheromone visualization)

### Step 2: Configure the Grid Component

1. Select the **Grid** GameObject
2. In the Inspector, set:
   - **Cell Size**: X=1, Y=1, Z=0
   - **Cell Layout**: Rectangle
   - **Cell Gap**: X=0, Y=0

### Step 3: Create Tile Assets

1. **Create a Tile Palette**:
   - Window → 2D → Tile Palette
   - Create New Palette
   - Name it "BuildingTiles"

2. **Create Basic Tiles**:
   - Right-click in Project → Create → 2D → Tiles → Tile
   - Create four tiles:
     - `FloorTile` (light gray/beige)
     - `WallTile` (dark gray/brown)
     - `RubbleTile` (brown/red)
     - `ExitTile` (green/yellow)

3. **Assign Sprites**:
   - For each tile, assign a simple colored sprite or import sprites
   - You can use Unity's built-in shapes or import custom sprites

### Step 4: Set Up GameManager

1. **Create GameManager GameObject**:
   - Right-click in Hierarchy → Create Empty
   - Name it "GameManager"
   - Add Component → `GameManager` script

2. **Configure GameManager**:
   - **Grid Width**: 50
   - **Grid Height**: 40
   - **Cell Size**: 1
   - **Floor Tilemap**: Drag `FloorTilemap` from Hierarchy
   - **Wall Tilemap**: Drag `WallTilemap` from Hierarchy
   - **Overlay Tilemap**: Drag `OverlayTilemap` from Hierarchy (optional)
   - **Floor Tile**: Drag `FloorTile` asset
   - **Wall Tile**: Drag `WallTile` asset
   - **Rubble Tile**: Drag `RubbleTile` asset
   - **Exit Tile**: Drag `ExitTile` asset

### Step 5: Set Up DisasterManager

1. **Create DisasterManager GameObject**:
   - Right-click in Hierarchy → Create Empty
   - Name it "DisasterManager"
   - Add Component → `DisasterManager` script

2. **Configure DisasterManager** (defaults are fine, but you can adjust):
   - **Bomb Radius**: 5
   - **Bomb Fire Intensity**: 0.8
   - **Earthquake Rubble Percentage**: 0.2
   - **Fire Spread Rate**: 0.1
   - **Fire Spread Chance**: 0.3

### Step 6: Set Up PathfindingVisualizer

1. **Create PathfindingVisualizer GameObject**:
   - Right-click in Hierarchy → Create Empty
   - Name it "PathfindingVisualizer"
   - Add Component → `PathfindingVisualizer` script

2. **Configure Visualization** (all enabled by default):
   - **Show Open Set**: ✓
   - **Show Closed Set**: ✓
   - **Show Path**: ✓
   - **Show Pheromones**: ✓

### Step 7: Create Agent Prefab

1. **Create Agent GameObject**:
   - Right-click in Hierarchy → 2D Object → Sprite → Circle (or Square)
   - Name it "Person"
   - Add Component → `PersonController` script

2. **Configure PersonController**:
   - **Max Health**: 100
   - **Max Stamina**: 100
   - **Walk Speed**: 2
   - **Sprint Speed**: 4
   - **Panic Threshold**: 50
   - **Is Spectated**: false (set to true for one agent to follow with camera)

3. **Set Up POV Camera** (optional):
   - The script will auto-create a camera child
   - Or manually create: Right-click "Person" → Camera
   - Name it "POV Camera"
   - Set it as Orthographic
   - Assign to PersonController's **POV Camera** field

4. **Create Prefab**:
   - Drag "Person" from Hierarchy to Project window
   - Delete the instance from Hierarchy (we'll spawn them via code)

### Step 8: Set Up Camera

1. **Main Camera Setup**:
   - Select Main Camera
   - Set **Projection**: Orthographic
   - Set **Size**: 15-20 (adjust to see the whole grid)
   - Position at (25, 20, -10) to center on a 50x40 grid

2. **Camera for POV Mode** (if using):
   - The PersonController will handle this automatically
   - Ensure the Main Camera can be replaced when spectating

### Step 9: Configure URP (Universal Render Pipeline)

1. **Create URP Asset** (if not already):
   - Right-click in Project → Create → Rendering → URP Asset (with 2D Renderer)
   - Name it "URP2D"

2. **Assign to Graphics Settings**:
   - Edit → Project Settings → Graphics
   - Set **Scriptable Render Pipeline Settings** to "URP2D"

3. **Set Up 2D Lights** (for Fog of War effect):
   - Right-click in Hierarchy → Light → 2D → Global Light
   - Name it "GlobalLight"
   - Set **Intensity**: 0.3 (dim global light)
   - Set **Color**: Dark gray/blue

4. **Add Spot Light to Agents**:
   - In PersonController, we'll add a 2D Spot Light component
   - Or manually: Select Person → Add Component → Light 2D → Spot
   - Set **Intensity**: 1.5
   - Set **Inner/Outer Angle**: 30/60 degrees
   - Set **Range**: 5-10 units

### Step 10: Create Spawner Script (Optional)

Create a simple script to spawn agents:

```csharp
using UnityEngine;

public class AgentSpawner : MonoBehaviour
{
    [SerializeField] private GameObject agentPrefab;
    [SerializeField] private int numberOfAgents = 10;
    [SerializeField] private Vector2 spawnArea = new Vector2(10, 10);
    
    private void Start()
    {
        for (int i = 0; i < numberOfAgents; i++)
        {
            Vector3 spawnPos = new Vector3(
                Random.Range(-spawnArea.x, spawnArea.x),
                Random.Range(-spawnArea.y, spawnArea.y),
                0
            );
            
            GameObject agent = Instantiate(agentPrefab, spawnPos, Quaternion.identity);
            
            // Set one agent to be spectated
            if (i == 0)
            {
                PersonController pc = agent.GetComponent<PersonController>();
                if (pc != null)
                {
                    pc.IsSpectated = true;
                }
            }
        }
    }
}
```

---

## Gizmo Visualization Guide

### Viewing Pathfinding in Scene View

1. **Open Scene View**:
   - Window → General → Scene (or press the Scene tab)

2. **Enable Gizmos**:
   - In Scene View, ensure **Gizmos** button is enabled (top toolbar)

3. **What You'll See**:
   - **Green Cubes**: Open Set (nodes currently being considered by A*)
   - **Red Cubes**: Closed Set (nodes already checked)
   - **Blue Lines**: Final Path (the computed path)
   - **Red/Green Tinted Floors**: Pheromone values (red = danger, green = success)

4. **To Visualize a Specific Agent's Pathfinding**:
   - Select an agent in Hierarchy
   - The PathfindingVisualizer will show that agent's pathfinding data
   - Or call `PathfindingVisualizer.VisualizeAgentPathfinding(agent)` from code

### Debugging Tips

- **No Path Found**: Check if exits are set correctly and walls aren't blocking all paths
- **Agents Stuck**: Verify nodes are walkable (not walls/rubble)
- **Fire Not Spreading**: Check Fire Spread Rate and Chance values
- **Pheromones Not Visible**: Adjust `minPheromoneThreshold` in PathfindingVisualizer

---

## Controls

### Keyboard Input

- **[B]**: Trigger Bomb at mouse position
  - Converts walls/floors to rubble
  - Applies high fire intensity in radius
  
- **[E]**: Trigger Earthquake
  - Randomly turns 20% of floor tiles into rubble
  
- **[F]**: Trigger Fire at mouse position
  - Starts a fire that spreads using Cellular Automata

### Agent Controls (Future Enhancement)

- Currently agents automatically pathfind to exits
- Sprint can be toggled via `PersonController.SetSprinting(bool)`

---

## Algorithm Details

### A* Pathfinding Cost Function

```
Movement Cost = Base Cost (1) + Rubble Penalty (10) + Fire Penalty (50 * intensity)
```

- **Base Cost**: 1 (normal movement)
- **Rubble Penalty**: +10 (difficult terrain)
- **Fire Penalty**: +50 × fireIntensity (0-50, very dangerous)

### Pheromone System (ACO Memory)

- **Positive Pheromones**: Successful path segments (green)
- **Negative Pheromones**: Dangerous areas (red)
- **Decay**: Pheromones decay over time (configurable rate)

### Fire Spread (Cellular Automata)

- Fire spreads to neighboring walkable nodes
- Spread chance based on current fire intensity
- Natural decay over time
- Minimum intensity threshold to spread

---

## URP 2D Lighting Setup (Fog of War)

### Method 1: Light Layers (Recommended)

1. **Create Light Layers**:
   - Edit → Project Settings → Tags and Layers
   - Add custom layers: "AgentLight", "GlobalLight"

2. **Assign Layers**:
   - Global Light: Set **Light Layer** to "GlobalLight"
   - Agent Spot Lights: Set **Light Layer** to "AgentLight"

3. **Camera Culling**:
   - When spectating an agent, set camera's **Culling Mask** to only show "AgentLight" and "GlobalLight"
   - This ensures you only see what the agent's light illuminates

### Method 2: Light Blend Styles

1. **Configure Blend Styles**:
   - In URP Asset, set **Light Blend Styles**
   - Use "Multiply" for global light (darkens)
   - Use "Additive" for agent lights (brightens)

2. **Camera Setup**:
   - Main Camera: Shows all lights
   - POV Camera: Only shows agent's light + global light

### Implementation in PersonController

The `UpdateCamera()` method handles camera following. To implement proper light culling:

```csharp
// In PersonController.UpdateCamera()
if (isSpectated && povCamera != null)
{
    // Set camera to only see agent's light layer
    povCamera.cullingMask = (1 << LayerMask.NameToLayer("AgentLight")) | 
                            (1 << LayerMask.NameToLayer("Default"));
}
```

---

## Troubleshooting

### Common Issues

1. **"GameManager.Instance is null"**
   - Ensure GameManager script is attached and enabled
   - Check that Awake() is being called

2. **"Tilemap references not set"**
   - Drag tilemaps from Hierarchy to GameManager fields
   - Ensure tilemaps are children of the Grid GameObject

3. **"No path found"**
   - Check that exits exist and are walkable
   - Verify walls aren't completely blocking paths
   - Ensure grid is properly initialized

4. **"Agents not moving"**
   - Check that PersonController is enabled
   - Verify currentPath is not null
   - Check that nodes are walkable

5. **"Fire not spreading"**
   - Increase Fire Spread Rate
   - Increase Fire Spread Chance
   - Check that nodes are walkable

---

## Performance Optimization

- **Grid Size**: 50x40 is manageable, but larger grids may need optimization
- **Pathfinding**: Only calculate paths when needed (not every frame)
- **Fire Updates**: Fire spread updates every 0.5 seconds (configurable)
- **Pheromone Decay**: Batch updates to avoid per-frame calculations

---

## Future Enhancements

- Multiple agent types (firefighters, medics)
- More disaster types (flood, gas leak)
- Building floor system (multi-level)
- Save/Load simulation states
- Statistics and analytics dashboard
- Agent communication/grouping behavior

---

## License

This project is provided as-is for educational and development purposes.

---

## Credits

**Dwight V12** - Disaster Evacuation Simulator
- Custom A* Pathfinding
- Pheromone-based Navigation (ACO)
- Dynamic Disaster System
- Real-time Algorithm Visualization

