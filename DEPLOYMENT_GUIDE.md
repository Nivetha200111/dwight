# Deployment Guide - Dwight V12

## Can You Deploy on Vercel?

**Short Answer**: Yes, but it's **not ideal**. Vercel is optimized for serverless functions and static sites, while Unity WebGL builds have specific requirements.

**Better Alternatives**: 
- ✅ **GitHub Pages** (Free, easy)
- ✅ **Netlify** (Free, better for large files)
- ✅ **itch.io** (Free, game-focused)
- ✅ **Unity Cloud Build** (Official)
- ⚠️ **Vercel** (Possible but requires workarounds)

---

## Option 1: Build for WebGL (Required First Step)

### Step 1: Configure Unity for WebGL

1. **Open Unity Editor**
2. **File → Build Settings**
3. **Select Platform**: WebGL
4. **Click "Switch Platform"** (if not already selected)
5. **Player Settings**:
   - **Compression Format**: Gzip (smaller files)
   - **Template**: Minimal (or Default)
   - **Code Optimization**: Size
   - **Exception Support**: None (for smaller build)

### Step 2: Build the Project

1. **File → Build Settings → Build**
2. Choose output folder (e.g., `Build/WebGL`)
3. Wait for build to complete (can take 10-30 minutes)

### Step 3: Test Locally

1. The build creates an `index.html` and data files
2. You need a local server to test (Unity WebGL requires HTTP, not file://)
3. Use Python: `python -m http.server 8000` in the build folder
4. Open `http://localhost:8000` in browser

---

## Option 2: Deploy to Vercel

### ⚠️ Important Considerations

- **File Size Limits**: Vercel has limits on file sizes (100MB per file on free tier)
- **Build Time**: Large Unity builds may timeout
- **MIME Types**: Need proper configuration for Unity files
- **Performance**: Not optimized for large binary files

### Vercel Configuration

Create `vercel.json` in your project root:

```json
{
  "version": 2,
  "builds": [
    {
      "src": "Build/WebGL/**",
      "use": "@vercel/static"
    }
  ],
  "routes": [
    {
      "src": "/(.*)",
      "dest": "/Build/WebGL/$1"
    }
  ],
  "headers": [
    {
      "source": "/(.*\\.(data|mem|wasm|symbols\\.json))",
      "headers": [
        {
          "key": "Content-Type",
          "value": "application/octet-stream"
        },
        {
          "key": "Cross-Origin-Embedder-Policy",
          "value": "require-corp"
        },
        {
          "key": "Cross-Origin-Opener-Policy",
          "value": "same-origin"
        }
      ]
    },
    {
      "source": "/(.*\\.js)",
      "headers": [
        {
          "key": "Content-Type",
          "value": "application/javascript"
        }
      ]
    }
  ]
}
```

### Deployment Steps

1. **Build your Unity project** (see Option 1)
2. **Create `vercel.json`** (see above)
3. **Install Vercel CLI**: `npm i -g vercel`
4. **Deploy**: `vercel`
5. **Or use GitHub integration**: Push to GitHub, connect to Vercel

### Issues You May Encounter

- **Large files**: May need to use Vercel Pro or split files
- **CORS errors**: Configure headers properly
- **Loading issues**: Ensure all paths are relative

---

## Option 3: GitHub Pages (Recommended - Free & Easy)

### Steps

1. **Build for WebGL** (see Option 1)
2. **Create a GitHub repository**
3. **Copy build files** to a `docs` folder or root
4. **Enable GitHub Pages**:
   - Settings → Pages
   - Source: `main` branch, `/docs` folder (or root)
5. **Access at**: `https://yourusername.github.io/repo-name/`

### GitHub Actions (Auto-deploy)

Create `.github/workflows/deploy.yml`:

```yaml
name: Deploy Unity WebGL

on:
  push:
    branches: [ main ]

jobs:
  deploy:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v2
      - name: Deploy to GitHub Pages
        uses: peaceiris/actions-gh-pages@v3
        with:
          github_token: ${{ secrets.GITHUB_TOKEN }}
          publish_dir: ./Build/WebGL
```

---

## Option 4: Netlify (Recommended - Better for Large Files)

### Why Netlify is Better

- ✅ Better handling of large files
- ✅ Automatic deployments from Git
- ✅ Free tier with good limits
- ✅ Better CDN for game assets

### Steps

1. **Build for WebGL**
2. **Create `netlify.toml`**:

```toml
[build]
  publish = "Build/WebGL"

[[headers]]
  for = "/*.data"
  [headers.values]
    Content-Type = "application/octet-stream"

[[headers]]
  for = "/*.wasm"
  [headers.values]
    Content-Type = "application/wasm"

[[headers]]
  for = "/*.mem"
  [headers.values]
    Content-Type = "application/octet-stream"
```

3. **Deploy**:
   - Drag & drop the `Build/WebGL` folder to Netlify
   - Or connect GitHub repo for auto-deploy

---

## Option 5: itch.io (Game-Focused Hosting)

### Steps

1. **Build for WebGL**
2. **Create account** at itch.io
3. **Create new project**
4. **Upload** the `Build/WebGL` folder as a ZIP
5. **Set project type**: HTML
6. **Publish**

**Benefits**: 
- Free hosting
- Game-focused platform
- Easy updates
- Analytics included

---

## Option 6: Unity Cloud Build (Official)

### Steps

1. **Unity Dashboard** → Cloud Build
2. **Connect your repository**
3. **Configure build**:
   - Platform: WebGL
   - Branch: main/master
4. **Set up auto-deploy** to hosting service
5. **Build automatically** on every commit

---

## Performance Optimization for WebGL

### Before Building

1. **Reduce Texture Sizes**:
   - Compress textures
   - Use lower resolution sprites

2. **Optimize Code**:
   - Remove unused scripts
   - Enable Code Stripping
   - Set Exception Support to None

3. **Reduce Build Size**:
   - Use Gzip compression
   - Enable Code Optimization: Size
   - Remove development-only code

4. **Test Performance**:
   - Test in browser before deploying
   - Check frame rate
   - Monitor memory usage

### Build Settings Checklist

- [ ] Platform: WebGL
- [ ] Compression: Gzip
- [ ] Code Optimization: Size
- [ ] Exception Support: None
- [ ] Strip Engine Code: Enabled
- [ ] Managed Stripping Level: High

---

## Troubleshooting WebGL Builds

### Common Issues

**"Build fails"**
- Check for WebGL-incompatible code
- Remove Editor-only scripts
- Check console for errors

**"Game doesn't load"**
- Ensure proper MIME types
- Check CORS headers
- Verify all files uploaded

**"Performance is poor"**
- Reduce grid size
- Optimize pathfinding frequency
- Reduce number of agents

**"Large file size"**
- Enable compression
- Reduce texture sizes
- Strip unused code

---

## Recommended Deployment Strategy

### For Development/Demo:
1. **GitHub Pages** - Free, easy, fast setup

### For Production:
1. **Netlify** - Better CDN, larger file support
2. **itch.io** - If targeting gamers

### For Enterprise:
1. **Unity Cloud Build** + **AWS S3/CloudFront**
2. **Custom hosting** with CDN

---

## Quick Comparison

| Platform | Free? | Easy? | Large Files? | Best For |
|----------|-------|-------|--------------|----------|
| **GitHub Pages** | ✅ | ✅✅✅ | ⚠️ | Demos, portfolios |
| **Netlify** | ✅ | ✅✅ | ✅ | Production web games |
| **Vercel** | ✅ | ✅ | ⚠️ | Small projects |
| **itch.io** | ✅ | ✅✅✅ | ✅ | Game distribution |
| **Unity Cloud** | ⚠️ | ✅✅ | ✅ | Enterprise |

---

## Next Steps

1. **Build for WebGL** in Unity
2. **Test locally** with a web server
3. **Choose hosting** based on your needs
4. **Deploy** and test in browser
5. **Optimize** based on performance

---

## Additional Resources

- [Unity WebGL Documentation](https://docs.unity3d.com/Manual/webgl.html)
- [WebGL Build Optimization](https://docs.unity3d.com/Manual/webgl-optimization.html)
- [GitHub Pages Guide](https://pages.github.com/)
- [Netlify Documentation](https://docs.netlify.com/)

