using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

/// <summary>
/// Manages the grid system, tilemap visualization, and overall game state.
/// Generates a 50x40 grid of Nodes and maps them to Unity's Tilemap system.
/// </summary>
public class GameManager : MonoBehaviour
{
    [Header("Grid Settings")]
    [SerializeField] private int gridWidth = 50;
    [SerializeField] private int gridHeight = 40;
    [SerializeField] private float cellSize = 1f;
    
    [Header("Tilemap References")]
    [SerializeField] private Tilemap floorTilemap;
    [SerializeField] private Tilemap wallTilemap;
    [SerializeField] private Tilemap overlayTilemap; // For fire, pheromones, etc.
    
    [Header("Tile Assets")]
    [SerializeField] private TileBase floorTile;
    [SerializeField] private TileBase wallTile;
    [SerializeField] private TileBase rubbleTile;
    [SerializeField] private TileBase exitTile;
    
    [Header("Visualization")]
    [SerializeField] private bool showGridGizmos = true;
    [SerializeField] private Color gridColor = Color.white;
    
    // Grid data
    private Node[,] grid;
    private Grid gridComponent;
    
    // Singleton instance
    public static GameManager Instance { get; private set; }
    
    // Properties
    public int GridWidth => gridWidth;
    public int GridHeight => gridHeight;
    public float CellSize => cellSize;
    public Node[,] Grid => grid;
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        
        InitializeGrid();
    }
    
    private void Start()
    {
        GenerateTilemap();
    }
    
    /// <summary>
    /// Initialize the grid array and create all nodes
    /// </summary>
    private void InitializeGrid()
    {
        grid = new Node[gridWidth, gridHeight];
        
        // Get or create Grid component
        gridComponent = GetComponent<Grid>();
        if (gridComponent == null)
        {
            gridComponent = gameObject.AddComponent<Grid>();
        }
        
        // Create nodes
        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                Vector2Int gridPos = new Vector2Int(x, y);
                Vector3 worldPos = GetWorldPosition(gridPos);
                grid[x, y] = new Node(gridPos, worldPos);
            }
        }
        
        // Set up some default walls and exits (you can customize this)
        SetupDefaultLayout();
    }
    
    /// <summary>
    /// Set up default building layout with walls and exits
    /// </summary>
    private void SetupDefaultLayout()
    {
        // Create outer walls
        for (int x = 0; x < gridWidth; x++)
        {
            SetNodeWall(new Vector2Int(x, 0), true);
            SetNodeWall(new Vector2Int(x, gridHeight - 1), true);
        }
        for (int y = 0; y < gridHeight; y++)
        {
            SetNodeWall(new Vector2Int(0, y), true);
            SetNodeWall(new Vector2Int(gridWidth - 1, y), true);
        }
        
        // Create some interior walls (example: create a few rooms)
        CreateInteriorWalls();
        
        // Set exits (on the outer walls)
        SetNodeExit(new Vector2Int(gridWidth / 2, 0), true);
        SetNodeExit(new Vector2Int(gridWidth / 2, gridHeight - 1), true);
        SetNodeExit(new Vector2Int(0, gridHeight / 2), true);
        SetNodeExit(new Vector2Int(gridWidth - 1, gridHeight / 2), true);
    }
    
    /// <summary>
    /// Create some interior walls to make the building more interesting
    /// </summary>
    private void CreateInteriorWalls()
    {
        // Example: Create a few room dividers
        int roomWidth = gridWidth / 3;
        int roomHeight = gridHeight / 3;
        
        // Vertical dividers
        for (int y = 5; y < gridHeight - 5; y++)
        {
            SetNodeWall(new Vector2Int(roomWidth, y), true);
            SetNodeWall(new Vector2Int(roomWidth * 2, y), true);
        }
        
        // Horizontal dividers
        for (int x = 5; x < gridWidth - 5; x++)
        {
            SetNodeWall(new Vector2Int(x, roomHeight), true);
            SetNodeWall(new Vector2Int(x, roomHeight * 2), true);
        }
        
        // Create doorways (remove some walls)
        SetNodeWall(new Vector2Int(roomWidth, roomHeight), false);
        SetNodeWall(new Vector2Int(roomWidth, roomHeight * 2), false);
        SetNodeWall(new Vector2Int(roomWidth * 2, roomHeight), false);
        SetNodeWall(new Vector2Int(roomWidth * 2, roomHeight * 2), false);
    }
    
    /// <summary>
    /// Generate the tilemap from the grid data
    /// </summary>
    private void GenerateTilemap()
    {
        if (floorTilemap == null || wallTilemap == null)
        {
            Debug.LogWarning("Tilemap references not set in GameManager!");
            return;
        }
        
        // Clear existing tiles
        floorTilemap.ClearAllTiles();
        wallTilemap.ClearAllTiles();
        if (overlayTilemap != null)
            overlayTilemap.ClearAllTiles();
        
        // Place tiles based on node data
        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                Node node = grid[x, y];
                Vector3Int tilePos = new Vector3Int(x, y, 0);
                
                if (node.isExit)
                {
                    floorTilemap.SetTile(tilePos, exitTile != null ? exitTile : floorTile);
                }
                else if (node.isWall)
                {
                    wallTilemap.SetTile(tilePos, wallTile);
                }
                else if (node.isRubble)
                {
                    floorTilemap.SetTile(tilePos, rubbleTile != null ? rubbleTile : floorTile);
                }
                else
                {
                    floorTilemap.SetTile(tilePos, floorTile);
                }
            }
        }
    }
    
    /// <summary>
    /// Convert grid position to world position
    /// </summary>
    public Vector3 GetWorldPosition(Vector2Int gridPos)
    {
        if (gridComponent != null)
        {
            return gridComponent.CellToWorld(new Vector3Int(gridPos.x, gridPos.y, 0));
        }
        return new Vector3(gridPos.x * cellSize, gridPos.y * cellSize, 0);
    }
    
    /// <summary>
    /// Convert world position to grid position
    /// </summary>
    public Vector2Int GetGridPosition(Vector3 worldPos)
    {
        if (gridComponent != null)
        {
            Vector3Int cellPos = gridComponent.WorldToCell(worldPos);
            return new Vector2Int(cellPos.x, cellPos.y);
        }
        return new Vector2Int(
            Mathf.FloorToInt(worldPos.x / cellSize),
            Mathf.FloorToInt(worldPos.y / cellSize)
        );
    }
    
    /// <summary>
    /// Get node at grid position
    /// </summary>
    public Node GetNode(Vector2Int gridPos)
    {
        if (IsValidGridPosition(gridPos))
        {
            return grid[gridPos.x, gridPos.y];
        }
        return null;
    }
    
    /// <summary>
    /// Get node at world position
    /// </summary>
    public Node GetNode(Vector3 worldPos)
    {
        return GetNode(GetGridPosition(worldPos));
    }
    
    /// <summary>
    /// Check if grid position is valid
    /// </summary>
    public bool IsValidGridPosition(Vector2Int gridPos)
    {
        return gridPos.x >= 0 && gridPos.x < gridWidth &&
               gridPos.y >= 0 && gridPos.y < gridHeight;
    }
    
    /// <summary>
    /// Get neighboring nodes (8-directional)
    /// </summary>
    public List<Node> GetNeighbors(Node node)
    {
        List<Node> neighbors = new List<Node>();
        
        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0)
                    continue;
                
                Vector2Int checkPos = new Vector2Int(
                    node.gridPosition.x + x,
                    node.gridPosition.y + y
                );
                
                if (IsValidGridPosition(checkPos))
                {
                    neighbors.Add(grid[checkPos.x, checkPos.y]);
                }
            }
        }
        
        return neighbors;
    }
    
    /// <summary>
    /// Set node as wall
    /// </summary>
    public void SetNodeWall(Vector2Int gridPos, bool isWall)
    {
        Node node = GetNode(gridPos);
        if (node != null)
        {
            node.isWall = isWall;
            UpdateTilemapAt(gridPos);
        }
    }
    
    /// <summary>
    /// Set node as rubble
    /// </summary>
    public void SetNodeRubble(Vector2Int gridPos, bool isRubble)
    {
        Node node = GetNode(gridPos);
        if (node != null)
        {
            node.isRubble = isRubble;
            UpdateTilemapAt(gridPos);
        }
    }
    
    /// <summary>
    /// Set node as exit
    /// </summary>
    public void SetNodeExit(Vector2Int gridPos, bool isExit)
    {
        Node node = GetNode(gridPos);
        if (node != null)
        {
            node.isExit = isExit;
            UpdateTilemapAt(gridPos);
        }
    }
    
    /// <summary>
    /// Update tilemap at specific grid position
    /// </summary>
    public void UpdateTilemapAt(Vector2Int gridPos)
    {
        Node node = GetNode(gridPos);
        if (node == null) return;
        
        Vector3Int tilePos = new Vector3Int(gridPos.x, gridPos.y, 0);
        
        if (node.isWall)
        {
            wallTilemap.SetTile(tilePos, wallTile);
            floorTilemap.SetTile(tilePos, null);
        }
        else
        {
            wallTilemap.SetTile(tilePos, null);
            if (node.isExit)
            {
                floorTilemap.SetTile(tilePos, exitTile != null ? exitTile : floorTile);
            }
            else if (node.isRubble)
            {
                floorTilemap.SetTile(tilePos, rubbleTile != null ? rubbleTile : floorTile);
            }
            else
            {
                floorTilemap.SetTile(tilePos, floorTile);
            }
        }
    }
    
    /// <summary>
    /// Update overlay tilemap for fire/pheromone visualization
    /// </summary>
    public void UpdateOverlayTilemap()
    {
        if (overlayTilemap == null) return;
        
        overlayTilemap.ClearAllTiles();
        
        // This would be called from a separate visualization system
        // For now, we'll handle this in the PathfindingVisualizer
    }
    
    private void OnDrawGizmos()
    {
        if (!showGridGizmos || grid == null) return;
        
        Gizmos.color = gridColor;
        
        // Draw grid lines
        for (int x = 0; x <= gridWidth; x++)
        {
            Vector3 start = GetWorldPosition(new Vector2Int(x, 0));
            Vector3 end = GetWorldPosition(new Vector2Int(x, gridHeight));
            Gizmos.DrawLine(start, end);
        }
        
        for (int y = 0; y <= gridHeight; y++)
        {
            Vector3 start = GetWorldPosition(new Vector2Int(0, y));
            Vector3 end = GetWorldPosition(new Vector2Int(gridWidth, y));
            Gizmos.DrawLine(start, end);
        }
    }
}

