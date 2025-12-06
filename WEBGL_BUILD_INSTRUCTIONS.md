# Unity WebGL Build Instructions - Dwight V12

## Prerequisites

- Unity 2022+ installed
- Project configured and tested in Editor
- All scripts working correctly

---

## Step-by-Step Build Process

### 1. Configure Build Settings

1. **Open Unity Editor**
2. **File → Build Settings** (or `Ctrl+Shift+B` / `Cmd+Shift+B`)
3. **Select WebGL** platform
4. If not selected, click **"Switch Platform"** (this may take a few minutes)

### 2. Configure Player Settings

1. Click **"Player Settings"** button
2. In the Inspector, configure:

#### Resolution and Presentation
- **Default Canvas Width**: 1920
- **Default Canvas Height**: 1080
- **Run In Background**: ✓ (optional)

#### Publishing Settings
- **Compression Format**: **Gzip** (recommended for smaller files)
- **Template**: **Minimal** (or Default)
- **Code Optimization**: **Size**
- **Exception Support**: **None** (for smaller build, disable if you need debugging)

#### Other Settings
- **Strip Engine Code**: ✓ Enabled
- **Managed Stripping Level**: **High**

### 3. Optimize for WebGL

#### Before Building:

1. **Remove Editor-Only Scripts**:
   - Editor scripts won't work in WebGL
   - They're automatically excluded, but check for any Editor references

2. **Check for WebGL-Incompatible Code**:
   - No file system access (use WebGL-specific APIs)
   - No threading (Unity WebGL is single-threaded)
   - Input handling may differ

3. **Reduce Build Size**:
   - Compress textures
   - Remove unused assets
   - Enable code stripping

### 4. Build the Project

1. **File → Build Settings**
2. Click **"Build"** (or **"Build and Run"** for local testing)
3. Choose output folder:
   - Create: `Build/WebGL` folder
   - Or use existing folder
4. **Wait for build** (10-30 minutes depending on project size)

### 5. Test Locally

Unity WebGL builds **cannot** run from `file://` protocol. You need a local server:

#### Option A: Python
```bash
cd Build/WebGL
python -m http.server 8000
```
Open: `http://localhost:8000`

#### Option B: Node.js
```bash
cd Build/WebGL
npx http-server -p 8000
```

#### Option C: Unity's Built-in Server
- Use **"Build and Run"** in Unity
- Opens browser automatically

### 6. Verify Build Output

Your `Build/WebGL` folder should contain:
- `index.html` - Main HTML file
- `Build/[ProjectName].loader.js` - Unity loader
- `Build/[ProjectName].framework.js` - Unity framework
- `Build/[ProjectName].data` - Game data (large file)
- `Build/[ProjectName].wasm` - WebAssembly binary
- `TemplateData/` - UI templates and styles

---

## Build Size Optimization

### Target Sizes:
- **Small (< 10MB)**: Excellent, fast loading
- **Medium (10-50MB)**: Good, acceptable loading
- **Large (50-100MB)**: Slow loading, consider optimization
- **Very Large (> 100MB)**: May have deployment issues

### Optimization Tips:

1. **Texture Compression**:
   - Use compressed texture formats
   - Reduce texture sizes
   - Remove unused textures

2. **Audio Compression**:
   - Compress audio files
   - Use OGG Vorbis format
   - Remove unused audio

3. **Code Stripping**:
   - Enable Managed Stripping
   - Remove unused code
   - Set Exception Support to None

4. **Asset Optimization**:
   - Remove unused assets
   - Compress sprites
   - Use sprite atlases

---

## Common Build Issues

### Issue: "Build fails with errors"
**Solution**:
- Check Console for specific errors
- Remove Editor-only code
- Ensure all scripts compile

### Issue: "Build is too large"
**Solution**:
- Enable compression (Gzip)
- Reduce texture sizes
- Remove unused assets
- Enable code stripping

### Issue: "Game doesn't load in browser"
**Solution**:
- Ensure using HTTP server (not file://)
- Check browser console for errors
- Verify all files are present
- Check CORS headers if deploying

### Issue: "Performance is poor"
**Solution**:
- Reduce grid size in GameManager
- Optimize pathfinding frequency
- Reduce number of agents
- Lower quality settings

---

## Pre-Deployment Checklist

- [ ] Build completed successfully
- [ ] Tested locally with HTTP server
- [ ] Game loads and runs correctly
- [ ] All features work (disasters, pathfinding, etc.)
- [ ] Build size is acceptable (< 100MB recommended)
- [ ] No console errors in browser
- [ ] Performance is acceptable (30+ FPS)

---

## Next Steps After Building

1. **Test locally** thoroughly
2. **Choose hosting platform** (see DEPLOYMENT_GUIDE.md)
3. **Deploy** using chosen platform
4. **Test in production** environment
5. **Monitor performance** and optimize

---

## Additional Resources

- [Unity WebGL Documentation](https://docs.unity3d.com/Manual/webgl.html)
- [WebGL Build Optimization](https://docs.unity3d.com/Manual/webgl-optimization.html)
- [WebGL Browser Compatibility](https://docs.unity3d.com/Manual/webgl-browsercompatibility.html)

