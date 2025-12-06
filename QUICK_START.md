# Quick Start Guide - Dwight V12

## 5-Minute Setup

### 1. Import Scripts
- Copy all `.cs` files to `Assets/Scripts/` folder in your Unity project

### 2. Create Scene Structure
```
Hierarchy:
├── Grid
│   ├── FloorTilemap
│   ├── WallTilemap
│   └── OverlayTilemap (optional)
├── GameManager (Empty GameObject)
├── DisasterManager (Empty GameObject)
├── PathfindingVisualizer (Empty GameObject)
└── Main Camera
```

### 3. Assign Components
- **GameManager**: Add `GameManager` script, assign tilemaps and tile assets
- **DisasterManager**: Add `DisasterManager` script
- **PathfindingVisualizer**: Add `PathfindingVisualizer` script

### 4. Create Basic Tiles
- Create 4 Tile assets: FloorTile, WallTile, RubbleTile, ExitTile
- Assign simple colored sprites (or use Unity's default sprites)

### 5. Create Agent Prefab
- Create a Sprite GameObject
- Add `PersonController` script
- Configure stats (defaults work fine)
- Drag to Project to create prefab

### 6. Spawn Agents (Optional)
- Create empty GameObject
- Add `AgentSpawner` script
- Assign agent prefab
- Set number of agents

### 7. Test
- Press Play
- Press **[B]** to trigger bomb at mouse position
- Press **[E]** for earthquake
- Press **[F]** to start fire at mouse position

## Viewing Algorithm Visualization

1. Open **Scene View** (not Game View)
2. Enable **Gizmos** button (top toolbar)
3. You'll see:
   - Green cubes = Open Set
   - Red cubes = Closed Set  
   - Blue lines = Final Path
   - Red/Green tints = Pheromones

## Common Issues

**"NullReferenceException: GameManager.Instance"**
→ Ensure GameManager script is attached and enabled

**"No path found"**
→ Check that exits exist (GameManager creates 4 by default)

**"Agents not moving"**
→ Verify PersonController is enabled and has a valid path

**"Can't see gizmos"**
→ Make sure you're in Scene View with Gizmos enabled

## Next Steps

- Read `README.md` for detailed setup instructions
- Customize grid size in GameManager
- Adjust disaster parameters in DisasterManager
- Modify agent stats in PersonController
- Set up URP lighting for Fog of War effect (see README)

