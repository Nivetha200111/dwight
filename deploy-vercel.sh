#!/bin/bash
# Vercel deployment script for Unity WebGL build
# Usage: ./deploy-vercel.sh [--prod]

echo "🚀 Deploying Dwight V12 to Vercel..."

# Check if Build/WebGL exists
if [ ! -d "Build/WebGL" ]; then
    echo "❌ Error: Build/WebGL folder not found!"
    echo "Please build your Unity project for WebGL first."
    echo "In Unity: File → Build Settings → WebGL → Build"
    exit 1
fi

# Check if vercel.json exists
if [ ! -f "vercel.json" ]; then
    echo "❌ Error: vercel.json not found!"
    echo "Please ensure vercel.json is in the project root."
    exit 1
fi

# Check if Vercel CLI is installed
if ! command -v vercel &> /dev/null; then
    echo "❌ Vercel CLI not found!"
    echo "Install with: npm i -g vercel"
    exit 1
fi

echo "✅ Build folder found"
echo "✅ Configuration found"
echo ""

# Deploy
if [ "$1" == "--prod" ]; then
    echo "📦 Deploying to production..."
    vercel --prod
else
    echo "📦 Deploying to preview..."
    vercel
fi

echo ""
echo "✅ Deployment complete!"
echo "Check your Vercel dashboard for the URL."

