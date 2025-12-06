# PowerShell script to set up GitHub repository for Dwight V12
# Usage: .\setup-github.ps1

Write-Host "Setting up GitHub repository for Dwight V12..." -ForegroundColor Cyan
Write-Host ""

# Check if git is installed
try {
    $null = Get-Command git -ErrorAction Stop
} catch {
    Write-Host "Error: Git is not installed!" -ForegroundColor Red
    Write-Host "Please install Git from https://git-scm.com/download/win"
    exit 1
}

# Check if already a git repository
if (Test-Path ".git") {
    Write-Host "Warning: Git repository already initialized" -ForegroundColor Yellow
    $continue = Read-Host "Continue anyway? (y/n)"
    if ($continue -ne "y") {
        exit 0
    }
} else {
    Write-Host "Initializing git repository..." -ForegroundColor Green
    git init
}

# Check if .gitignore exists
if (-not (Test-Path ".gitignore")) {
    Write-Host "Warning: .gitignore not found" -ForegroundColor Yellow
}

# Add all files
Write-Host ""
Write-Host "Adding files to git..." -ForegroundColor Green
git add .

# Check if there are changes to commit
$status = git status --porcelain
if ([string]::IsNullOrEmpty($status)) {
    Write-Host "No changes to commit" -ForegroundColor Yellow
} else {
    Write-Host ""
    Write-Host "Creating initial commit..." -ForegroundColor Green
    $commitMessage = @"
Initial commit: Dwight V12 - Disaster Evacuation Simulator

- Unity 2022+ project with custom grid system
- A* pathfinding with visualization
- Agent state machine (IDLE/MOVING/PANIC/DEAD)
- Disaster system (Bomb, Earthquake, Fire)
- Pheromone-based navigation (ACO)
- Complete Unity Editor setup helpers
- Vercel deployment configuration
"@
    git commit -m $commitMessage
}

# Check if remote already exists
$remoteExists = $false
try {
    $remote = git remote get-url origin 2>$null
    if ($LASTEXITCODE -eq 0) {
        $remoteExists = $true
    }
} catch {
    $remoteExists = $false
}

if ($remoteExists) {
    Write-Host ""
    Write-Host "Warning: Remote 'origin' already exists" -ForegroundColor Yellow
    $update = Read-Host "Update to https://github.com/Nivetha200111/dwight.git? (y/n)"
    if ($update -eq "y") {
        git remote set-url origin https://github.com/Nivetha200111/dwight.git
        Write-Host "Remote updated" -ForegroundColor Green
    }
} else {
    Write-Host ""
    Write-Host "Adding remote repository..." -ForegroundColor Green
    git remote add origin https://github.com/Nivetha200111/dwight.git
}

# Set main branch
Write-Host ""
Write-Host "Setting main branch..." -ForegroundColor Green
git branch -M main

# Show status
Write-Host ""
Write-Host "Repository status:" -ForegroundColor Cyan
git status

Write-Host ""
Write-Host "Repository setup complete!" -ForegroundColor Green
Write-Host ""
Write-Host "Next steps:" -ForegroundColor Yellow
Write-Host "1. Push to GitHub: git push -u origin main" -ForegroundColor White
Write-Host "2. Build Unity project for WebGL" -ForegroundColor White
Write-Host "3. Deploy to Vercel (see VERCEL_DEPLOYMENT.md)" -ForegroundColor White
Write-Host ""

$push = Read-Host "Push to GitHub now? (y/n)"
if ($push -eq "y") {
    Write-Host ""
    Write-Host "Pushing to GitHub..." -ForegroundColor Green
    git push -u origin main
    
    if ($LASTEXITCODE -eq 0) {
        Write-Host ""
        Write-Host "Successfully pushed to GitHub!" -ForegroundColor Green
        Write-Host "Repository: https://github.com/Nivetha200111/dwight" -ForegroundColor Cyan
    } else {
        Write-Host ""
        Write-Host "Push failed. You may need to:" -ForegroundColor Red
        Write-Host "   - Authenticate with GitHub (use Personal Access Token)" -ForegroundColor Yellow
        Write-Host "   - Ensure the repository exists on GitHub" -ForegroundColor Yellow
        Write-Host "   - Check your internet connection" -ForegroundColor Yellow
    }
}

