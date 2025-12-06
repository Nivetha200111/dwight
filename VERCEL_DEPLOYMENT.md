# Vercel Deployment Guide - Dwight V12

## Prerequisites

✅ You have a Vercel subscription (Pro/Enterprise)
✅ Unity project built for WebGL
✅ Vercel CLI installed (or use GitHub integration)

---

## Step 1: Build Unity Project for WebGL

1. **Open Unity Editor**
2. **File → Build Settings**
3. **Select WebGL** platform
4. **Player Settings**:
   - Compression Format: **Gzip**
   - Template: **Minimal** (or Default)
   - Code Optimization: **Size**
   - Exception Support: **None**
5. **Build** → Choose folder: `Build/WebGL`
6. **Wait for build** to complete

---

## Step 2: Prepare Project Structure

Your project should look like this:

```
DWIGHT/
├── Build/
│   └── WebGL/          # Unity build output (after building)
│       ├── index.html
│       ├── Build/
│       └── TemplateData/
├── vercel.json         # Vercel configuration (already created)
├── .vercelignore      # Files to exclude (optional)
└── (other project files)
```

---

## Step 3: Configure Vercel

The `vercel.json` file is already configured with:
- ✅ Proper MIME types for Unity files
- ✅ CORS headers for WebGL
- ✅ Routing configuration
- ✅ Headers for .data, .wasm, .mem files

### Verify vercel.json

The configuration includes:
- Static file serving
- Proper content types
- CORS headers for WebAssembly
- Route rewrites

---

## Step 4: Deploy to Vercel

### Option A: Using Vercel CLI (Recommended)

1. **Install Vercel CLI** (if not already):
   ```bash
   npm i -g vercel
   ```

2. **Login to Vercel**:
   ```bash
   vercel login
   ```

3. **Navigate to project root**:
   ```bash
   cd C:\Users\nnive\Downloads\DWIGHT
   ```

4. **Deploy**:
   ```bash
   vercel
   ```
   
   - Follow prompts:
     - Link to existing project? **No** (first time) or **Yes** (updates)
     - Project name: `dwight-v12` (or your choice)
     - Directory: **./Build/WebGL** (important!)
     - Override settings? **No**

5. **For production deployment**:
   ```bash
   vercel --prod
   ```

### Option B: Using GitHub Integration

1. **Push project to GitHub**:
   ```bash
   git init
   git add .
   git commit -m "Initial commit"
   git remote add origin <your-github-repo-url>
   git push -u origin main
   ```

2. **Connect to Vercel**:
   - Go to [vercel.com](https://vercel.com)
   - Click **"Add New Project"**
   - Import your GitHub repository
   - **Root Directory**: Leave as `.` (root)
   - **Build Command**: Leave empty (Unity build is pre-built)
   - **Output Directory**: `Build/WebGL`
   - Click **"Deploy"**

3. **Configure in Vercel Dashboard**:
   - Go to Project Settings
   - General → Root Directory: `.`
   - Build & Development Settings:
     - Output Directory: `Build/WebGL`
     - Install Command: (leave empty)
     - Build Command: (leave empty)

---

## Step 5: Configure Vercel Settings

### In Vercel Dashboard:

1. **Go to Project Settings**
2. **General**:
   - Framework Preset: **Other**
   - Root Directory: `.` (project root)
   - Output Directory: `Build/WebGL`

3. **Environment Variables** (if needed):
   - Usually not required for Unity WebGL

4. **Headers** (already in vercel.json, but verify):
   - The `vercel.json` handles this automatically

---

## Step 6: Handle Large Files (Pro/Enterprise)

With a Vercel subscription, you have:
- ✅ **100MB file size limit** (Pro) or **250MB** (Enterprise)
- ✅ **Better bandwidth**
- ✅ **No build time limits**

### If files are too large:

1. **Optimize Unity build**:
   - Reduce texture sizes
   - Compress audio
   - Enable code stripping
   - Use Gzip compression

2. **Split large files** (if needed):
   - Unity WebGL can split data files
   - Configure in Player Settings → Publishing Settings

3. **Use Vercel Blob Storage** (Enterprise):
   - Store large assets separately
   - Load via CDN

---

## Step 7: Test Deployment

1. **Visit your Vercel URL**: `https://your-project.vercel.app`
2. **Check browser console** for errors
3. **Test all features**:
   - Grid generation
   - Agent movement
   - Disaster triggers ([B], [E], [F])
   - Pathfinding visualization

### Common Issues:

**"Failed to load"**
- Check file paths are correct
- Verify all files uploaded
- Check browser console

**"CORS errors"**
- Verify `vercel.json` headers are correct
- Check Cross-Origin headers

**"Large file timeout"**
- With Pro subscription, this shouldn't happen
- Check file sizes
- Consider optimization

---

## Step 8: Custom Domain (Optional)

1. **In Vercel Dashboard**:
   - Settings → Domains
   - Add your domain
   - Follow DNS configuration steps

2. **Update vercel.json** (if needed):
   - Add domain to configuration

---

## Continuous Deployment

### Automatic Deploys:

1. **Connect GitHub** (if using Option B)
2. **Every push to main** triggers deploy
3. **Preview deployments** for pull requests

### Manual Deploys:

```bash
vercel --prod
```

---

## Updating the Build

When you update your Unity project:

1. **Rebuild in Unity**: `File → Build Settings → Build`
2. **Deploy to Vercel**:
   ```bash
   vercel --prod
   ```
   Or push to GitHub (if using integration)

---

## Performance Optimization

### For Vercel:

1. **Enable Edge Caching**:
   - Vercel automatically caches static assets
   - Large files benefit from CDN

2. **Optimize Build**:
   - Smaller build = faster loading
   - Target: < 50MB for best experience

3. **Monitor Performance**:
   - Use Vercel Analytics (if available)
   - Check browser performance

---

## Troubleshooting

### Issue: "File too large"
**Solution**: 
- Optimize Unity build
- Check Pro subscription limits
- Consider file splitting

### Issue: "Build fails"
**Solution**:
- Ensure `Build/WebGL` folder exists
- Check `vercel.json` syntax
- Verify output directory setting

### Issue: "Game doesn't load"
**Solution**:
- Check browser console
- Verify MIME types in headers
- Test locally first with HTTP server

### Issue: "CORS errors"
**Solution**:
- Verify `vercel.json` headers
- Check Cross-Origin settings
- Ensure proper content types

---

## Vercel-Specific Configuration

The `vercel.json` includes:

```json
{
  "version": 2,
  "builds": [...],
  "routes": [...],
  "headers": [
    // Unity WebGL file types
    // CORS headers
    // Content-Type headers
  ]
}
```

This handles:
- ✅ Static file serving
- ✅ Proper MIME types
- ✅ CORS for WebAssembly
- ✅ Routing to index.html

---

## Quick Deploy Commands

```bash
# First deployment
vercel

# Production deployment
vercel --prod

# Preview deployment
vercel

# Check deployment status
vercel ls

# View logs
vercel logs
```

---

## Next Steps

1. ✅ Build Unity project for WebGL
2. ✅ Deploy using Vercel CLI or GitHub
3. ✅ Test deployment
4. ✅ Configure custom domain (optional)
5. ✅ Set up continuous deployment

---

## Support

- [Vercel Documentation](https://vercel.com/docs)
- [Vercel CLI Reference](https://vercel.com/docs/cli)
- [Unity WebGL Docs](https://docs.unity3d.com/Manual/webgl.html)

---

## Checklist

- [ ] Unity project built for WebGL
- [ ] `Build/WebGL` folder exists with all files
- [ ] `vercel.json` is in project root
- [ ] Vercel CLI installed (or GitHub connected)
- [ ] Deployed to Vercel
- [ ] Tested in browser
- [ ] All features working
- [ ] Performance acceptable

