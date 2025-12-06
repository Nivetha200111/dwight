# Unity Quick Reference - Dwight V12

## 🚀 Fastest Setup (30 seconds)

1. **Open Unity Editor**
2. **Window → Dwight V12 → Setup Scene**
3. **Click "6. Setup Complete Scene"**
4. **Assign tiles to GameManager** (drag from Assets/Tiles/)
5. **Press Play** → Test with **[B]**, **[E]**, **[F]**

---

## 📁 File Structure

```
DWIGHT/
├── Assets/
│   ├── Scripts/
│   │   ├── Node.cs
│   │   ├── GameManager.cs
│   │   ├── Pathfinding.cs
│   │   ├── PersonController.cs
│   │   ├── DisasterManager.cs
│   │   ├── PathfindingVisualizer.cs
│   │   ├── AgentSpawner.cs
│   │   ├── LightingSetupHelper.cs
│   │   └── Editor/
│   │       ├── SceneSetupHelper.cs
│   │       └── TileCreator.cs
│   └── Tiles/
│       ├── FloorTile.asset
│       ├── WallTile.asset
│       ├── RubbleTile.asset
│       └── ExitTile.asset
└── (Documentation files)
```

---

## ⌨️ Controls

| Key | Action |
|-----|--------|
| **[B]** | Bomb at mouse position |
| **[E]** | Earthquake (20% rubble) |
| **[F]** | Fire at mouse position |

---

## 👁️ Visualization

**Scene View** (not Game View):
- **Green cubes** = Open Set
- **Red cubes** = Closed Set
- **Blue lines** = Path
- **Red/Green tints** = Pheromones

**Enable**: Gizmos button in Scene View toolbar

---

## 🔧 Common Fixes

| Problem | Solution |
|---------|----------|
| "GameManager.Instance is null" | Check GameManager is in scene and enabled |
| "No path found" | Verify exits exist (GameManager creates 4) |
| "Can't see gizmos" | Scene View + Gizmos enabled |
| "Tiles not showing" | Assign tiles in GameManager Inspector |

---

## 📝 Quick Checklist

- [ ] All scripts in `Assets/Scripts/`
- [ ] Editor scripts in `Assets/Scripts/Editor/`
- [ ] Grid + 3 Tilemaps created
- [ ] GameManager configured with tilemaps & tiles
- [ ] DisasterManager in scene
- [ ] PathfindingVisualizer in scene
- [ ] Agent prefab created with PersonController
- [ ] Camera set to Orthographic, Size: 20

---

## 🎯 Testing Steps

1. **Press Play**
2. **Click in Scene View** to see mouse position
3. **Press [B]** → Bomb should create rubble + fire
4. **Press [E]** → Earthquake should create random rubble
5. **Press [F]** → Fire should start and spread
6. **Watch agents** pathfind to exits
7. **View gizmos** in Scene View to see algorithm

---

## 📚 Full Documentation

- **README.md** - Complete setup guide
- **SETUP_INSTRUCTIONS.md** - Detailed step-by-step
- **FILES_OVERVIEW.md** - Script documentation
- **QUICK_START.md** - 5-minute setup

---

## 💡 Pro Tips

1. **Use Scene Setup Helper** for fastest setup
2. **Create tiles via menu**: Right-click → Create → Dwight V12 → [Tile Name]
3. **Spectate an agent**: Set `IsSpectated = true` in PersonController
4. **Adjust grid size**: Change in GameManager (default: 50x40)
5. **Customize disasters**: Modify parameters in DisasterManager

