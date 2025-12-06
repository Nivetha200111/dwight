using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Visualizes pathfinding algorithms in the Scene View using Gizmos.
/// Shows Open Set (green), Closed Set (red), Final Path (blue), and Pheromones.
/// </summary>
public class PathfindingVisualizer : MonoBehaviour
{
    [Header("Visualization Settings")]
    [SerializeField] private bool showOpenSet = true;
    [SerializeField] private bool showClosedSet = true;
    [SerializeField] private bool showPath = true;
    [SerializeField] private bool showPheromones = true;
    
    [Header("Colors")]
    [SerializeField] private Color openSetColor = Color.green;
    [SerializeField] private Color closedSetColor = Color.red;
    [SerializeField] private Color pathColor = Color.blue;
    [SerializeField] private Color positivePheromoneColor = Color.green;
    [SerializeField] private Color negativePheromoneColor = Color.red;
    
    [Header("Gizmo Settings")]
    [SerializeField] private float nodeCubeSize = 0.3f;
    [SerializeField] private float pheromoneAlpha = 0.3f;
    [SerializeField] private float lineWidth = 0.1f;
    
    [Header("Pheromone Visualization")]
    [SerializeField] private float minPheromoneThreshold = 0.1f; // Only show pheromones above this value
    
    private GameManager gameManager;
    private List<PersonController> agents;
    
    private void Start()
    {
        gameManager = GameManager.Instance;
        
        // Find all agents
        agents = new List<PersonController>(FindObjectsOfType<PersonController>());
    }
    
    private void OnDrawGizmos()
    {
        if (gameManager == null)
            gameManager = GameManager.Instance;
        
        if (gameManager == null)
            return;
        
        // Visualize pathfinding data
        if (showOpenSet || showClosedSet || showPath)
        {
            VisualizePathfinding();
        }
        
        // Visualize pheromones
        if (showPheromones)
        {
            VisualizePheromones();
        }
    }
    
    /// <summary>
    /// Visualize A* pathfinding: Open Set, Closed Set, and Path
    /// </summary>
    private void VisualizePathfinding()
    {
        // Get pathfinding data from the Pathfinding class
        HashSet<Node> openSet = Pathfinding.OpenSet;
        HashSet<Node> closedSet = Pathfinding.ClosedSet;
        List<Node> currentPath = Pathfinding.CurrentPath;
        
        // Draw Open Set (green cubes)
        if (showOpenSet && openSet != null)
        {
            Gizmos.color = openSetColor;
            foreach (Node node in openSet)
            {
                if (node != null)
                {
                    Vector3 pos = node.WorldPosition;
                    Gizmos.DrawWireCube(pos, Vector3.one * nodeCubeSize);
                }
            }
        }
        
        // Draw Closed Set (red cubes)
        if (showClosedSet && closedSet != null)
        {
            Gizmos.color = closedSetColor;
            foreach (Node node in closedSet)
            {
                if (node != null)
                {
                    Vector3 pos = node.WorldPosition;
                    Gizmos.DrawCube(pos, Vector3.one * nodeCubeSize);
                }
            }
        }
        
        // Draw Final Path (blue line)
        if (showPath && currentPath != null && currentPath.Count > 0)
        {
            Gizmos.color = pathColor;
            for (int i = 0; i < currentPath.Count - 1; i++)
            {
                if (currentPath[i] != null && currentPath[i + 1] != null)
                {
                    Vector3 start = currentPath[i].WorldPosition;
                    Vector3 end = currentPath[i + 1].WorldPosition;
                    
                    // Draw line with thickness
                    DrawThickLine(start, end, lineWidth);
                    
                    // Draw node markers
                    Gizmos.DrawSphere(start, nodeCubeSize * 0.5f);
                }
            }
            
            // Draw final node
            if (currentPath[currentPath.Count - 1] != null)
            {
                Gizmos.DrawSphere(
                    currentPath[currentPath.Count - 1].WorldPosition,
                    nodeCubeSize * 0.5f
                );
            }
        }
    }
    
    /// <summary>
    /// Visualize pheromone values on the grid
    /// Red for danger (negative), Green for success (positive)
    /// </summary>
    private void VisualizePheromones()
    {
        Node[,] grid = gameManager.Grid;
        float cellSize = gameManager.CellSize;
        
        for (int x = 0; x < gameManager.GridWidth; x++)
        {
            for (int y = 0; y < gameManager.GridHeight; y++)
            {
                Node node = grid[x, y];
                
                if (node == null || Mathf.Abs(node.pheromoneValue) < minPheromoneThreshold)
                    continue;
                
                Vector3 pos = node.WorldPosition;
                float pheromoneValue = node.pheromoneValue;
                
                // Determine color based on pheromone value
                Color pheromoneColor;
                if (pheromoneValue > 0)
                {
                    // Positive pheromone (good path) - Green
                    pheromoneColor = positivePheromoneColor;
                }
                else
                {
                    // Negative pheromone (danger) - Red
                    pheromoneColor = negativePheromoneColor;
                }
                
                // Set alpha based on intensity
                pheromoneColor.a = Mathf.Abs(pheromoneValue) * pheromoneAlpha;
                Gizmos.color = pheromoneColor;
                
                // Draw semi-transparent cube
                Gizmos.DrawCube(pos, Vector3.one * cellSize * 0.9f);
            }
        }
    }
    
    /// <summary>
    /// Draw a thick line using multiple cubes
    /// </summary>
    private void DrawThickLine(Vector3 start, Vector3 end, float width)
    {
        Vector3 direction = (end - start).normalized;
        Vector3 perpendicular = Vector3.Cross(direction, Vector3.forward).normalized * width;
        
        // Draw line as a series of cubes
        int segments = Mathf.CeilToInt(Vector3.Distance(start, end) / (width * 2));
        for (int i = 0; i <= segments; i++)
        {
            float t = (float)i / segments;
            Vector3 pos = Vector3.Lerp(start, end, t);
            Gizmos.DrawCube(pos, Vector3.one * width);
        }
    }
    
    /// <summary>
    /// Force a pathfinding visualization update by finding a path
    /// Call this from a selected agent to visualize its pathfinding
    /// </summary>
    public void VisualizeAgentPathfinding(PersonController agent)
    {
        if (agent == null || gameManager == null)
            return;
        
        Node startNode = gameManager.GetNode(agent.transform.position);
        if (startNode == null)
            return;
        
        // Find path to exit with visualization enabled
        List<Node> path = Pathfinding.FindPathToNearestExit(startNode, true);
    }
    
    /// <summary>
    /// Update pheromone visualization in real-time
    /// Call this periodically to see pheromone decay
    /// </summary>
    public void UpdatePheromoneVisualization()
    {
        // Pheromones are already visualized in OnDrawGizmos
        // This method can be used to trigger updates if needed
    }
}

