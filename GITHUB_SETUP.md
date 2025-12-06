# GitHub Repository Setup - Dwight V12

## Quick Setup Guide

### Step 1: Initialize Git Repository

Open PowerShell in your project directory and run:

```powershell
# Initialize git repository
git init

# Add all files
git add .

# Create initial commit
git commit -m "Initial commit: Dwight V12 Unity project"
```

### Step 2: Connect to GitHub

```powershell
# Add remote repository
git remote add origin https://github.com/Nivetha200111/dwight.git

# Verify remote
git remote -v
```

### Step 3: Push to GitHub

```powershell
# Push to main branch
git branch -M main
git push -u origin main
```

---

## Complete Setup Script

Run this PowerShell script to set up everything:

```powershell
# Initialize repository
git init

# Add all files
git add .

# Create initial commit
git commit -m "Initial commit: Dwight V12 - Disaster Evacuation Simulator

- Unity 2022+ project with custom grid system
- A* pathfinding with visualization
- Agent state machine (IDLE/MOVING/PANIC/DEAD)
- Disaster system (Bomb, Earthquake, Fire)
- Pheromone-based navigation (ACO)
- Complete Unity Editor setup helpers
- Vercel deployment configuration"

# Add remote
git remote add origin https://github.com/Nivetha200111/dwight.git

# Set main branch
git branch -M main

# Push to GitHub
git push -u origin main
```

---

## What Gets Committed

✅ **Included**:
- All C# scripts (`Assets/Scripts/`)
- Editor helper scripts
- Documentation (README.md, etc.)
- Configuration files (vercel.json, etc.)
- .gitignore file

❌ **Excluded** (via .gitignore):
- Unity generated files (Library/, Temp/, etc.)
- Build folders (except WebGL if you want)
- Python files (old versions)
- IDE files (.vscode/, .idea/)
- OS files (.DS_Store, Thumbs.db)

---

## After Pushing to GitHub

### Option 1: Connect to Vercel via GitHub

1. Go to [vercel.com](https://vercel.com)
2. **Add New Project**
3. **Import Git Repository**
4. Select `Nivetha200111/dwight`
5. Configure:
   - **Root Directory**: `.`
   - **Output Directory**: `Build/WebGL`
   - **Build Command**: (leave empty)
6. Click **Deploy**

### Option 2: Manual Vercel Deployment

After building Unity project:
```powershell
vercel --prod
```

---

## Updating the Repository

When you make changes:

```powershell
# Add changes
git add .

# Commit
git commit -m "Description of changes"

# Push
git push
```

---

## Repository Structure

```
dwight/
├── Assets/
│   └── Scripts/
│       ├── Node.cs
│       ├── GameManager.cs
│       ├── Pathfinding.cs
│       ├── PersonController.cs
│       ├── DisasterManager.cs
│       ├── PathfindingVisualizer.cs
│       ├── AgentSpawner.cs
│       ├── LightingSetupHelper.cs
│       └── Editor/
│           ├── SceneSetupHelper.cs
│           ├── TileCreator.cs
│           └── SetupVerifier.cs
├── Build/
│   └── WebGL/          # (Add this after building)
├── .gitignore
├── vercel.json
├── netlify.toml
├── README.md
├── DEPLOYMENT_GUIDE.md
├── VERCEL_DEPLOYMENT.md
└── (other documentation)
```

---

## Troubleshooting

**"Repository not found"**
→ Check that the GitHub repository exists and you have access

**"Authentication failed"**
→ Use GitHub Personal Access Token or SSH keys

**"Large file rejected"**
→ Check .gitignore excludes large build files
→ Use Git LFS for large files if needed

---

## Next Steps

1. ✅ Initialize git repository
2. ✅ Push to GitHub
3. ✅ Build Unity project for WebGL
4. ✅ Deploy to Vercel (via GitHub or CLI)

