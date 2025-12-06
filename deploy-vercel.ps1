# PowerShell script for Vercel deployment
# Usage: .\deploy-vercel.ps1 [-Prod]

param(
    [switch]$Prod
)

Write-Host "🚀 Deploying Dwight V12 to Vercel..." -ForegroundColor Cyan

# Check if Build/WebGL exists
if (-not (Test-Path "Build\WebGL")) {
    Write-Host "❌ Error: Build\WebGL folder not found!" -ForegroundColor Red
    Write-Host "Please build your Unity project for WebGL first."
    Write-Host "In Unity: File → Build Settings → WebGL → Build"
    exit 1
}

# Check if vercel.json exists
if (-not (Test-Path "vercel.json")) {
    Write-Host "❌ Error: vercel.json not found!" -ForegroundColor Red
    Write-Host "Please ensure vercel.json is in the project root."
    exit 1
}

# Check if Vercel CLI is installed
try {
    $null = Get-Command vercel -ErrorAction Stop
} catch {
    Write-Host "❌ Vercel CLI not found!" -ForegroundColor Red
    Write-Host "Install with: npm i -g vercel"
    exit 1
}

Write-Host "✅ Build folder found" -ForegroundColor Green
Write-Host "✅ Configuration found" -ForegroundColor Green
Write-Host ""

# Deploy
if ($Prod) {
    Write-Host "📦 Deploying to production..." -ForegroundColor Yellow
    vercel --prod
} else {
    Write-Host "📦 Deploying to preview..." -ForegroundColor Yellow
    vercel
}

Write-Host ""
Write-Host "✅ Deployment complete!" -ForegroundColor Green
Write-Host "Check your Vercel dashboard for the URL."

