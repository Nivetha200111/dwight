using UnityEngine;
using UnityEditor;
using UnityEngine.Tilemaps;

/// <summary>
/// Unity Editor helper to automatically set up the Dwight V12 scene.
/// Use: Window -> Dwight V12 -> Setup Scene
/// </summary>
public class SceneSetupHelper : EditorWindow
{
    [MenuItem("Window/Dwight V12/Setup Scene")]
    public static void ShowWindow()
    {
        GetWindow<SceneSetupHelper>("Dwight V12 Setup");
    }
    
    private void OnGUI()
    {
        GUILayout.Label("Dwight V12 Scene Setup", EditorStyles.boldLabel);
        GUILayout.Space(10);
        
        if (GUILayout.Button("1. Create Grid and Tilemaps", GUILayout.Height(30)))
        {
            CreateGridAndTilemaps();
        }
        
        if (GUILayout.Button("2. Create GameManager", GUILayout.Height(30)))
        {
            CreateGameManager();
        }
        
        if (GUILayout.Button("3. Create DisasterManager", GUILayout.Height(30)))
        {
            CreateDisasterManager();
        }
        
        if (GUILayout.Button("4. Create PathfindingVisualizer", GUILayout.Height(30)))
        {
            CreatePathfindingVisualizer();
        }
        
        if (GUILayout.Button("5. Create Basic Tiles", GUILayout.Height(30)))
        {
            CreateBasicTiles();
        }
        
        if (GUILayout.Button("6. Setup Complete Scene", GUILayout.Height(40)))
        {
            SetupCompleteScene();
        }
        
        GUILayout.Space(20);
        GUILayout.Label("Note: After setup, assign tile references in GameManager", EditorStyles.helpBox);
    }
    
    private static void CreateGridAndTilemaps()
    {
        // Check if Grid already exists
        Grid existingGrid = FindObjectOfType<Grid>();
        if (existingGrid != null)
        {
            Debug.Log("Grid already exists. Skipping creation.");
            return;
        }
        
        // Create Grid
        GameObject gridObj = new GameObject("Grid");
        Grid grid = gridObj.AddComponent<Grid>();
        grid.cellSize = new Vector3(1, 1, 0);
        
        // Create Tilemaps
        CreateTilemap(gridObj, "FloorTilemap", 0);
        CreateTilemap(gridObj, "WallTilemap", 1);
        CreateTilemap(gridObj, "OverlayTilemap", 2);
        
        Debug.Log("Grid and Tilemaps created successfully!");
    }
    
    private static void CreateTilemap(GameObject parent, string name, int sortingOrder)
    {
        GameObject tilemapObj = new GameObject(name);
        tilemapObj.transform.SetParent(parent.transform);
        
        Tilemap tilemap = tilemapObj.AddComponent<Tilemap>();
        TilemapRenderer renderer = tilemapObj.AddComponent<TilemapRenderer>();
        renderer.sortingOrder = sortingOrder;
        
        // Set different colors for different tilemaps
        if (name.Contains("Wall"))
        {
            renderer.sortingLayerName = "Default";
        }
        else if (name.Contains("Overlay"))
        {
            renderer.sortingLayerName = "Default";
        }
    }
    
    private static void CreateGameManager()
    {
        if (FindObjectOfType<GameManager>() != null)
        {
            Debug.Log("GameManager already exists. Skipping creation.");
            return;
        }
        
        GameObject gmObj = new GameObject("GameManager");
        GameManager gm = gmObj.AddComponent<GameManager>();
        
        // Try to find and assign tilemaps
        Tilemap floorTilemap = GameObject.Find("FloorTilemap")?.GetComponent<Tilemap>();
        Tilemap wallTilemap = GameObject.Find("WallTilemap")?.GetComponent<Tilemap>();
        Tilemap overlayTilemap = GameObject.Find("OverlayTilemap")?.GetComponent<Tilemap>();
        
        // Use reflection to set private fields (or make them public)
        SerializedObject so = new SerializedObject(gm);
        if (floorTilemap != null)
            so.FindProperty("floorTilemap").objectReferenceValue = floorTilemap;
        if (wallTilemap != null)
            so.FindProperty("wallTilemap").objectReferenceValue = wallTilemap;
        if (overlayTilemap != null)
            so.FindProperty("overlayTilemap").objectReferenceValue = overlayTilemap;
        so.ApplyModifiedProperties();
        
        Debug.Log("GameManager created! Don't forget to assign Tile assets.");
    }
    
    private static void CreateDisasterManager()
    {
        if (FindObjectOfType<DisasterManager>() != null)
        {
            Debug.Log("DisasterManager already exists. Skipping creation.");
            return;
        }
        
        GameObject dmObj = new GameObject("DisasterManager");
        dmObj.AddComponent<DisasterManager>();
        
        Debug.Log("DisasterManager created!");
    }
    
    private static void CreatePathfindingVisualizer()
    {
        if (FindObjectOfType<PathfindingVisualizer>() != null)
        {
            Debug.Log("PathfindingVisualizer already exists. Skipping creation.");
            return;
        }
        
        GameObject pvObj = new GameObject("PathfindingVisualizer");
        pvObj.AddComponent<PathfindingVisualizer>();
        
        Debug.Log("PathfindingVisualizer created!");
    }
    
    private static void CreateBasicTiles()
    {
        string tilesPath = "Assets/Tiles";
        
        // Create Tiles folder if it doesn't exist
        if (!AssetDatabase.IsValidFolder(tilesPath))
        {
            AssetDatabase.CreateFolder("Assets", "Tiles");
        }
        
        // Create colored tiles using ScriptableObject
        CreateColoredTile("FloorTile", new Color(0.8f, 0.8f, 0.7f), tilesPath);
        CreateColoredTile("WallTile", new Color(0.4f, 0.3f, 0.2f), tilesPath);
        CreateColoredTile("RubbleTile", new Color(0.5f, 0.3f, 0.2f), tilesPath);
        CreateColoredTile("ExitTile", new Color(0.2f, 0.8f, 0.2f), tilesPath);
        
        AssetDatabase.Refresh();
        Debug.Log("Basic tiles created in Assets/Tiles/ folder!");
        Debug.Log("Note: These are placeholder tiles. Replace with proper sprites for better visuals.");
    }
    
    private static void CreateColoredTile(string tileName, Color color, string folderPath)
    {
        // Check if tile already exists
        string tilePath = $"{folderPath}/{tileName}.asset";
        if (AssetDatabase.LoadAssetAtPath<TileBase>(tilePath) != null)
        {
            Debug.Log($"{tileName} already exists. Skipping.");
            return;
        }
        
        // Create a simple tile (we'll use a basic tile, but ideally you'd use sprites)
        Tile tile = ScriptableObject.CreateInstance<Tile>();
        
        // Create a simple colored texture for the tile
        Texture2D texture = new Texture2D(32, 32);
        Color[] pixels = new Color[32 * 32];
        for (int i = 0; i < pixels.Length; i++)
        {
            pixels[i] = color;
        }
        texture.SetPixels(pixels);
        texture.Apply();
        
        // Create sprite from texture
        Sprite sprite = Sprite.Create(texture, new Rect(0, 0, 32, 32), new Vector2(0.5f, 0.5f), 32);
        tile.sprite = sprite;
        
        AssetDatabase.CreateAsset(tile, tilePath);
        AssetDatabase.SaveAssets();
    }
    
    private static void SetupCompleteScene()
    {
        Debug.Log("Starting complete scene setup...");
        
        CreateGridAndTilemaps();
        CreateBasicTiles();
        CreateGameManager();
        CreateDisasterManager();
        CreatePathfindingVisualizer();
        
        // Setup camera
        Camera mainCam = Camera.main;
        if (mainCam != null)
        {
            mainCam.orthographic = true;
            mainCam.orthographicSize = 20f;
            mainCam.transform.position = new Vector3(25, 20, -10);
        }
        
        Debug.Log("Scene setup complete!");
        Debug.Log("Next steps:");
        Debug.Log("1. Assign Tile assets to GameManager (Assets/Tiles/)");
        Debug.Log("2. Create an Agent prefab with PersonController");
        Debug.Log("3. Press Play and test with [B], [E], [F] keys");
    }
}

