# Unity Setup Instructions - Dwight V12

## Quick Setup (Using Editor Helper)

### Option 1: Automated Setup (Recommended)

1. **Open Unity Editor** (Unity 2022+)
2. **Open the Scene Setup Helper**:
   - Go to: `Window → Dwight V12 → Setup Scene`
3. **Click "6. Setup Complete Scene"**
   - This will create everything automatically!

4. **Assign Tiles to GameManager**:
   - Select `GameManager` in Hierarchy
   - In Inspector, drag tiles from `Assets/Tiles/` to:
     - Floor Tile
     - Wall Tile
     - Rubble Tile
     - Exit Tile

5. **Create Agent Prefab**:
   - Right-click Hierarchy → 2D Object → Sprite → Circle
   - Name it "Person"
   - Add Component → `PersonController`
   - Drag to Project to create prefab

6. **Spawn Agents (Optional)**:
   - Create Empty GameObject
   - Add Component → `AgentSpawner`
   - Assign Person prefab
   - Set Number of Agents

7. **Press Play!**
   - Use **[B]**, **[E]**, **[F]** keys to trigger disasters
   - View algorithm visualization in Scene View

---

## Manual Setup (Step by Step)

### Step 1: Create Folder Structure

```
Assets/
├── Scripts/
│   ├── (All .cs files should be here)
│   └── Editor/
│       ├── SceneSetupHelper.cs
│       └── TileCreator.cs
└── Tiles/
    └── (Tile assets will be created here)
```

### Step 2: Create Grid and Tilemaps

1. Right-click Hierarchy → **2D Object → Tilemap → Rectangular**
2. This creates a `Grid` with a `Tilemap` child
3. Create 2 more tilemaps as children of Grid:
   - `FloorTilemap`
   - `WallTilemap`
   - `OverlayTilemap` (optional)

### Step 3: Create GameObjects

Create these Empty GameObjects and add scripts:

1. **GameManager**:
   - Add Component → `GameManager`
   - Assign tilemap references
   - Assign tile assets (create them first - see Step 4)

2. **DisasterManager**:
   - Add Component → `DisasterManager`

3. **PathfindingVisualizer**:
   - Add Component → `PathfindingVisualizer`

### Step 4: Create Tile Assets

**Method A: Using Menu (Easiest)**
- Right-click in Project → **Create → Dwight V12 → Floor Tile**
- Repeat for Wall Tile, Rubble Tile, Exit Tile

**Method B: Using Scene Setup Helper**
- Window → Dwight V12 → Setup Scene
- Click "5. Create Basic Tiles"

**Method C: Manual Creation**
- Right-click Project → Create → 2D → Tiles → Tile
- Assign a sprite (or create a simple colored sprite)
- Name appropriately

### Step 5: Configure GameManager

1. Select `GameManager` in Hierarchy
2. In Inspector:
   - **Grid Width**: 50
   - **Grid Height**: 40
   - **Floor Tilemap**: Drag `FloorTilemap`
   - **Wall Tilemap**: Drag `WallTilemap`
   - **Floor Tile**: Drag `FloorTile` asset
   - **Wall Tile**: Drag `WallTile` asset
   - **Rubble Tile**: Drag `RubbleTile` asset
   - **Exit Tile**: Drag `ExitTile` asset

### Step 6: Setup Camera

1. Select `Main Camera`
2. Set **Projection**: Orthographic
3. Set **Size**: 20
4. Position: (25, 20, -10) for 50x40 grid

### Step 7: Create Agent

1. Right-click Hierarchy → **2D Object → Sprite → Circle**
2. Name it "Person"
3. Add Component → `PersonController`
4. Configure (defaults work fine):
   - Max Health: 100
   - Walk Speed: 2
   - Sprint Speed: 4
5. **Create Prefab**: Drag to Project window

### Step 8: Spawn Agents (Optional)

1. Create Empty GameObject → Name "AgentSpawner"
2. Add Component → `AgentSpawner`
3. Assign Person prefab
4. Set Number of Agents: 10

---

## Testing

### Keyboard Controls

- **[B]**: Trigger Bomb at mouse position
- **[E]**: Trigger Earthquake
- **[F]**: Trigger Fire at mouse position

### Viewing Algorithm Visualization

1. **Open Scene View** (not Game View)
2. **Enable Gizmos** (button in Scene View toolbar)
3. You'll see:
   - **Green cubes**: Open Set (nodes being considered)
   - **Red cubes**: Closed Set (nodes already checked)
   - **Blue lines**: Final Path
   - **Red/Green tints**: Pheromones

### Troubleshooting

**"GameManager.Instance is null"**
- Ensure GameManager script is attached and enabled
- Check that Awake() ran (script execution order)

**"No path found"**
- Check that exits exist (GameManager creates 4 by default)
- Verify walls aren't blocking all paths

**"Can't see gizmos"**
- Make sure you're in Scene View (not Game View)
- Enable Gizmos button in Scene View
- Check PathfindingVisualizer is in scene

**"Tiles not showing"**
- Verify tilemaps are assigned in GameManager
- Check that tiles are assigned
- Ensure tilemaps are children of Grid GameObject

---

## Next Steps

1. **Customize Grid**: Change size in GameManager
2. **Adjust Disasters**: Modify parameters in DisasterManager
3. **Tune Agents**: Change stats in PersonController
4. **Add Sprites**: Replace colored tiles with proper sprites
5. **Setup URP Lighting**: See README.md for Fog of War setup

---

## File Locations

All scripts should be in:
- `Assets/Scripts/` (core scripts)
- `Assets/Scripts/Editor/` (editor helpers)

Tile assets should be in:
- `Assets/Tiles/`

Scene file:
- `Assets/Scenes/MainScene.unity` (or your scene name)

