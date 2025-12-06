# Quick Vercel Deployment - Dwight V12

## 🚀 Fastest Way to Deploy

### Step 1: Build Unity Project
1. Open Unity Editor
2. **File → Build Settings → WebGL**
3. Click **"Build"**
4. Choose folder: `Build/WebGL`
5. Wait for build (10-30 min)

### Step 2: Deploy to Vercel

#### Option A: Using PowerShell Script (Windows)
```powershell
.\deploy-vercel.ps1 -Prod
```

#### Option B: Using Vercel CLI
```powershell
# Install Vercel CLI (if not installed)
npm i -g vercel

# Login
vercel login

# Deploy
vercel --prod
```

#### Option C: Using GitHub Integration
1. Push project to GitHub
2. Go to [vercel.com](https://vercel.com)
3. **Add New Project** → Import GitHub repo
4. **Settings**:
   - Root Directory: `.`
   - Output Directory: `Build/WebGL`
   - Build Command: (leave empty)
5. Click **Deploy**

---

## ⚙️ Vercel Dashboard Configuration

After first deploy, configure in dashboard:

1. **Project Settings → General**:
   - Framework Preset: **Other**
   - Root Directory: `.`
   - Output Directory: `Build/WebGL`
   - Build Command: (leave empty)
   - Install Command: (leave empty)

2. **Project Settings → Environment Variables**:
   - Usually not needed for Unity WebGL

---

## ✅ What's Already Configured

- ✅ `vercel.json` - Proper headers and routing
- ✅ `.vercelignore` - Excludes unnecessary files
- ✅ Deployment scripts (PowerShell & Bash)

---

## 📝 Important Notes

1. **Build First**: Always build in Unity before deploying
2. **Output Directory**: Must be `Build/WebGL`
3. **File Size**: With Pro subscription, you have 100MB+ limits
4. **Headers**: Already configured for Unity WebGL

---

## 🔧 Troubleshooting

**"Build folder not found"**
→ Build Unity project first: `File → Build Settings → WebGL → Build`

**"Deployment failed"**
→ Check that `Build/WebGL` folder exists with `index.html`

**"Game doesn't load"**
→ Check browser console, verify all files uploaded

**"File too large"**
→ Optimize Unity build (compress textures, enable code stripping)

---

## 📚 Full Guide

See **VERCEL_DEPLOYMENT.md** for detailed instructions.

---

## 🎯 Quick Checklist

- [ ] Unity project built for WebGL
- [ ] `Build/WebGL` folder exists
- [ ] Vercel CLI installed (or GitHub connected)
- [ ] Deployed to Vercel
- [ ] Tested in browser
- [ ] All features working

---

## 🚀 Deploy Now!

```powershell
# Windows PowerShell
.\deploy-vercel.ps1 -Prod
```

Or:

```bash
# Bash/Linux/Mac
vercel --prod
```

