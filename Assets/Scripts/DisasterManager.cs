using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Manages disaster events: Bomb, Earthquake, and Fire.
/// Handles dynamic environment changes that affect pathfinding.
/// </summary>
public class DisasterManager : MonoBehaviour
{
    [Header("Bomb Settings")]
    [SerializeField] private float bombRadius = 5f;
    [SerializeField] private float bombFireIntensity = 0.8f;
    
    [Header("Earthquake Settings")]
    [SerializeField] private float earthquakeRubblePercentage = 0.2f; // 20% of floor tiles
    
    [Header("Fire Settings")]
    [SerializeField] private float fireSpreadRate = 0.1f; // Intensity increase per second
    [SerializeField] private float fireSpreadChance = 0.3f; // Chance to spread to neighbor
    [SerializeField] private float fireDecayRate = 0.05f; // Natural decay rate
    [SerializeField] private float minFireIntensity = 0.1f; // Minimum to spread
    [SerializeField] private float maxFireIntensity = 1f;
    
    [Header("Visualization")]
    [SerializeField] private bool showDisasterGizmos = true;
    [SerializeField] private Color fireColor = Color.red;
    
    // Fire management
    private List<Vector2Int> activeFireNodes = new List<Vector2Int>();
    private float fireUpdateTimer = 0f;
    private float fireUpdateInterval = 0.5f; // Update fire every 0.5 seconds
    
    // Singleton
    public static DisasterManager Instance { get; private set; }
    
    private GameManager gameManager;
    
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
    }
    
    private void Start()
    {
        gameManager = GameManager.Instance;
    }
    
    private void Update()
    {
        // Handle keyboard input
        if (Input.GetKeyDown(KeyCode.B))
        {
            TriggerBomb(GetMouseWorldPosition());
        }
        
        if (Input.GetKeyDown(KeyCode.E))
        {
            TriggerEarthquake();
        }
        
        if (Input.GetKeyDown(KeyCode.F))
        {
            TriggerFire(GetMouseWorldPosition());
        }
        
        // Update fire spread
        UpdateFireSpread();
    }
    
    /// <summary>
    /// Trigger a bomb at the specified world position
    /// Instantly turns walls/floor within radius into Rubble and applies high FireIntensity
    /// </summary>
    public void TriggerBomb(Vector3 worldPosition)
    {
        Vector2Int centerGridPos = gameManager.GetGridPosition(worldPosition);
        int radiusInCells = Mathf.CeilToInt(bombRadius / gameManager.CellSize);
        
        Debug.Log($"Bomb triggered at {worldPosition}!");
        
        for (int x = -radiusInCells; x <= radiusInCells; x++)
        {
            for (int y = -radiusInCells; y <= radiusInCells; y++)
            {
                Vector2Int checkPos = new Vector2Int(
                    centerGridPos.x + x,
                    centerGridPos.y + y
                );
                
                float distance = Vector2Int.Distance(centerGridPos, checkPos) * gameManager.CellSize;
                if (distance <= bombRadius)
                {
                    Node node = gameManager.GetNode(checkPos);
                    if (node != null)
                    {
                        // Convert to rubble (unless it's an exit)
                        if (!node.isExit)
                        {
                            if (node.isWall)
                            {
                                node.isWall = false;
                            }
                            node.isRubble = true;
                        }
                        
                        // Apply fire intensity (stronger at center)
                        float intensity = bombFireIntensity * (1f - distance / bombRadius);
                        node.fireIntensity = Mathf.Max(node.fireIntensity, intensity);
                        
                        // Add to active fire nodes
                        if (node.fireIntensity >= minFireIntensity && !activeFireNodes.Contains(checkPos))
                        {
                            activeFireNodes.Add(checkPos);
                        }
                        
                        // Update tilemap
                        gameManager.UpdateTilemapAt(checkPos);
                    }
                }
            }
        }
    }
    
    /// <summary>
    /// Trigger an earthquake
    /// Randomly turns 20% of floor tiles into Rubble across the map
    /// </summary>
    public void TriggerEarthquake()
    {
        Debug.Log("Earthquake triggered!");
        
        List<Node> floorNodes = new List<Node>();
        Node[,] grid = gameManager.Grid;
        
        // Collect all walkable floor nodes
        for (int x = 0; x < gameManager.GridWidth; x++)
        {
            for (int y = 0; y < gameManager.GridHeight; y++)
            {
                Node node = grid[x, y];
                if (node.IsWalkable && !node.isExit && !node.isRubble)
                {
                    floorNodes.Add(node);
                }
            }
        }
        
        // Calculate number of nodes to turn into rubble
        int nodesToRubble = Mathf.RoundToInt(floorNodes.Count * earthquakeRubblePercentage);
        
        // Randomly select nodes
        for (int i = 0; i < nodesToRubble; i++)
        {
            if (floorNodes.Count == 0)
                break;
            
            int randomIndex = Random.Range(0, floorNodes.Count);
            Node node = floorNodes[randomIndex];
            floorNodes.RemoveAt(randomIndex);
            
            node.isRubble = true;
            gameManager.UpdateTilemapAt(node.gridPosition);
        }
    }
    
    /// <summary>
    /// Trigger fire at the specified world position
    /// Starts a fire that will spread using Cellular Automata
    /// </summary>
    public void TriggerFire(Vector3 worldPosition)
    {
        Vector2Int gridPos = gameManager.GetGridPosition(worldPosition);
        Node node = gameManager.GetNode(gridPos);
        
        if (node != null && node.IsWalkable)
        {
            node.fireIntensity = 0.5f; // Start with medium intensity
            
            if (!activeFireNodes.Contains(gridPos))
            {
                activeFireNodes.Add(gridPos);
            }
            
            Debug.Log($"Fire started at {worldPosition}!");
        }
    }
    
    /// <summary>
    /// Update fire spread using Cellular Automata
    /// Fire spreads to neighbors over time
    /// </summary>
    private void UpdateFireSpread()
    {
        fireUpdateTimer += Time.deltaTime;
        
        if (fireUpdateTimer < fireUpdateInterval)
            return;
        
        fireUpdateTimer = 0f;
        
        // Create a copy of active fire nodes to iterate over
        List<Vector2Int> nodesToProcess = new List<Vector2Int>(activeFireNodes);
        List<Vector2Int> newFireNodes = new List<Vector2Int>();
        
        foreach (Vector2Int firePos in nodesToProcess)
        {
            Node node = gameManager.GetNode(firePos);
            if (node == null)
                continue;
            
            // Increase fire intensity
            if (node.fireIntensity < maxFireIntensity)
            {
                node.fireIntensity = Mathf.Min(maxFireIntensity, node.fireIntensity + fireSpreadRate * fireUpdateInterval);
            }
            
            // Try to spread to neighbors
            List<Node> neighbors = gameManager.GetNeighbors(node);
            foreach (Node neighbor in neighbors)
            {
                if (neighbor.IsWalkable && neighbor.fireIntensity < maxFireIntensity)
                {
                    // Chance to spread based on current fire intensity
                    float spreadChance = fireSpreadChance * node.fireIntensity;
                    
                    if (Random.value < spreadChance)
                    {
                        // Spread fire
                        neighbor.fireIntensity = Mathf.Min(
                            maxFireIntensity,
                            neighbor.fireIntensity + fireSpreadRate * fireUpdateInterval
                        );
                        
                        if (neighbor.fireIntensity >= minFireIntensity && 
                            !activeFireNodes.Contains(neighbor.gridPosition) &&
                            !newFireNodes.Contains(neighbor.gridPosition))
                        {
                            newFireNodes.Add(neighbor.gridPosition);
                        }
                    }
                }
            }
            
            // Natural decay (fire can die out)
            if (node.fireIntensity > 0)
            {
                node.fireIntensity = Mathf.Max(0f, node.fireIntensity - fireDecayRate * fireUpdateInterval);
                
                // Remove from active fires if intensity too low
                if (node.fireIntensity < minFireIntensity)
                {
                    activeFireNodes.Remove(firePos);
                }
            }
        }
        
        // Add new fire nodes
        foreach (Vector2Int newFirePos in newFireNodes)
        {
            if (!activeFireNodes.Contains(newFirePos))
            {
                activeFireNodes.Add(newFirePos);
            }
        }
    }
    
    /// <summary>
    /// Get mouse position in world space
    /// </summary>
    private Vector3 GetMouseWorldPosition()
    {
        Camera cam = Camera.main;
        if (cam == null)
            return Vector3.zero;
        
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = 10f; // Distance from camera
        return cam.ScreenToWorldPoint(mousePos);
    }
    
    private void OnDrawGizmos()
    {
        if (!showDisasterGizmos || gameManager == null)
            return;
        
        // Draw fire nodes
        Gizmos.color = fireColor;
        foreach (Vector2Int firePos in activeFireNodes)
        {
            Node node = gameManager.GetNode(firePos);
            if (node != null && node.fireIntensity > 0)
            {
                float alpha = node.fireIntensity;
                Gizmos.color = new Color(fireColor.r, fireColor.g, fireColor.b, alpha);
                Gizmos.DrawCube(node.WorldPosition, Vector3.one * gameManager.CellSize * 0.8f);
            }
        }
    }
}

