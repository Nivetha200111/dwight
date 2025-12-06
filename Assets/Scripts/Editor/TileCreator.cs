using UnityEngine;
using UnityEditor;
using UnityEngine.Tilemaps;

/// <summary>
/// Creates colored tile assets for the simulation.
/// These are simple placeholder tiles - replace with proper sprites later.
/// </summary>
public class TileCreator
{
    [MenuItem("Assets/Create/Dwight V12/Floor Tile", false, 1)]
    public static void CreateFloorTile()
    {
        CreateTileAsset("FloorTile", new Color(0.8f, 0.8f, 0.7f));
    }
    
    [MenuItem("Assets/Create/Dwight V12/Wall Tile", false, 2)]
    public static void CreateWallTile()
    {
        CreateTileAsset("WallTile", new Color(0.4f, 0.3f, 0.2f));
    }
    
    [MenuItem("Assets/Create/Dwight V12/Rubble Tile", false, 3)]
    public static void CreateRubbleTile()
    {
        CreateTileAsset("RubbleTile", new Color(0.5f, 0.3f, 0.2f));
    }
    
    [MenuItem("Assets/Create/Dwight V12/Exit Tile", false, 4)]
    public static void CreateExitTile()
    {
        CreateTileAsset("ExitTile", new Color(0.2f, 0.8f, 0.2f));
    }
    
    private static void CreateTileAsset(string tileName, Color color)
    {
        // Get the selected folder or use Assets root
        string path = "Assets";
        if (Selection.activeObject != null)
        {
            path = AssetDatabase.GetAssetPath(Selection.activeObject);
            if (!AssetDatabase.IsValidFolder(path))
            {
                path = System.IO.Path.GetDirectoryName(path);
            }
        }
        
        // Create texture
        Texture2D texture = new Texture2D(32, 32, TextureFormat.RGBA32, false);
        Color[] pixels = new Color[32 * 32];
        for (int i = 0; i < pixels.Length; i++)
        {
            pixels[i] = color;
        }
        texture.SetPixels(pixels);
        texture.Apply();
        texture.filterMode = FilterMode.Point;
        
        // Create sprite
        Sprite sprite = Sprite.Create(texture, new Rect(0, 0, 32, 32), new Vector2(0.5f, 0.5f), 32);
        
        // Create tile
        Tile tile = ScriptableObject.CreateInstance<Tile>();
        tile.sprite = sprite;
        
        // Save asset
        string assetPath = $"{path}/{tileName}.asset";
        assetPath = AssetDatabase.GenerateUniqueAssetPath(assetPath);
        AssetDatabase.CreateAsset(tile, assetPath);
        AssetDatabase.SaveAssets();
        
        Selection.activeObject = tile;
        Debug.Log($"Created {tileName} at {assetPath}");
    }
}

