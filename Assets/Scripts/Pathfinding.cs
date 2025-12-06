using UnityEngine;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Static class implementing A* pathfinding algorithm with visualization support.
/// Provides methods to find paths while avoiding dynamic obstacles (fire, rubble).
/// </summary>
public static class Pathfinding
{
    // Visualization data (for gizmos)
    private static HashSet<Node> openSet = new HashSet<Node>();
    private static HashSet<Node> closedSet = new HashSet<Node>();
    private static List<Node> currentPath = new List<Node>();
    
    // Properties for visualization
    public static HashSet<Node> OpenSet => openSet;
    public static HashSet<Node> ClosedSet => closedSet;
    public static List<Node> CurrentPath => currentPath;
    
    /// <summary>
    /// Find a path from start to target using A* algorithm
    /// </summary>
    /// <param name="startNode">Starting node</param>
    /// <param name="targetNode">Target node</param>
    /// <param name="visualize">Whether to store visualization data</param>
    /// <returns>List of nodes representing the path, or null if no path found</returns>
    public static List<Node> FindPath(Node startNode, Node targetNode, bool visualize = false)
    {
        if (startNode == null || targetNode == null)
        {
            Debug.LogWarning("Pathfinding: Start or target node is null!");
            return null;
        }
        
        if (startNode == targetNode)
        {
            return new List<Node> { startNode };
        }
        
        // Clear visualization data
        if (visualize)
        {
            openSet.Clear();
            closedSet.Clear();
            currentPath.Clear();
        }
        
        // Initialize
        List<Node> openList = new List<Node>();
        HashSet<Node> closedList = new HashSet<Node>();
        
        // Reset all nodes
        ResetGridPathfinding();
        
        startNode.gCost = 0;
        startNode.hCost = GetDistance(startNode, targetNode);
        startNode.parent = null;
        
        openList.Add(startNode);
        
        while (openList.Count > 0)
        {
            // Get node with lowest fCost
            Node currentNode = openList.OrderBy(n => n.fCost).ThenBy(n => n.hCost).First();
            openList.Remove(currentNode);
            closedList.Add(currentNode);
            
            if (visualize)
            {
                closedSet.Add(currentNode);
            }
            
            // Check if we reached the target
            if (currentNode == targetNode)
            {
                // Reconstruct path
                List<Node> path = RetracePath(startNode, targetNode);
                
                if (visualize)
                {
                    currentPath = new List<Node>(path);
                    openSet = new HashSet<Node>(openList);
                }
                
                return path;
            }
            
            // Check neighbors
            List<Node> neighbors = GameManager.Instance.GetNeighbors(currentNode);
            
            foreach (Node neighbor in neighbors)
            {
                // Skip if not walkable or already checked
                if (!neighbor.IsWalkable || closedList.Contains(neighbor))
                    continue;
                
                // Calculate new gCost
                float newGCost = currentNode.gCost + GetDistance(currentNode, neighbor) + neighbor.GetMovementCost();
                
                // Check if this is a better path
                if (newGCost < neighbor.gCost || !openList.Contains(neighbor))
                {
                    neighbor.gCost = newGCost;
                    neighbor.hCost = GetDistance(neighbor, targetNode);
                    neighbor.parent = currentNode;
                    
                    if (!openList.Contains(neighbor))
                    {
                        openList.Add(neighbor);
                        
                        if (visualize)
                        {
                            openSet.Add(neighbor);
                        }
                    }
                }
            }
        }
        
        // No path found
        if (visualize)
        {
            openSet = new HashSet<Node>(openList);
        }
        
        return null;
    }
    
    /// <summary>
    /// Find path to nearest exit
    /// </summary>
    public static List<Node> FindPathToNearestExit(Node startNode, bool visualize = false)
    {
        if (startNode == null)
            return null;
        
        // Find all exit nodes
        List<Node> exits = FindAllExits();
        
        if (exits.Count == 0)
        {
            Debug.LogWarning("Pathfinding: No exits found!");
            return null;
        }
        
        // Find path to closest exit
        List<Node> bestPath = null;
        float bestDistance = float.MaxValue;
        
        foreach (Node exit in exits)
        {
            List<Node> path = FindPath(startNode, exit, false);
            if (path != null && path.Count > 0)
            {
                float distance = path.Count;
                if (distance < bestDistance)
                {
                    bestDistance = distance;
                    bestPath = path;
                }
            }
        }
        
        // If we found a path and want to visualize, run it again with visualization
        if (bestPath != null && visualize)
        {
            Node targetExit = bestPath[bestPath.Count - 1];
            return FindPath(startNode, targetExit, true);
        }
        
        return bestPath;
    }
    
    /// <summary>
    /// Find all exit nodes in the grid
    /// </summary>
    private static List<Node> FindAllExits()
    {
        List<Node> exits = new List<Node>();
        Node[,] grid = GameManager.Instance.Grid;
        
        for (int x = 0; x < GameManager.Instance.GridWidth; x++)
        {
            for (int y = 0; y < GameManager.Instance.GridHeight; y++)
            {
                if (grid[x, y].isExit && grid[x, y].IsWalkable)
                {
                    exits.Add(grid[x, y]);
                }
            }
        }
        
        return exits;
    }
    
    /// <summary>
    /// Retrace path from end to start using parent nodes
    /// </summary>
    private static List<Node> RetracePath(Node startNode, Node endNode)
    {
        List<Node> path = new List<Node>();
        Node currentNode = endNode;
        
        while (currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }
        
        path.Add(startNode);
        path.Reverse();
        
        return path;
    }
    
    /// <summary>
    /// Calculate distance between two nodes (using diagonal distance)
    /// </summary>
    private static float GetDistance(Node nodeA, Node nodeB)
    {
        int dstX = Mathf.Abs(nodeA.gridPosition.x - nodeB.gridPosition.x);
        int dstY = Mathf.Abs(nodeA.gridPosition.y - nodeB.gridPosition.y);
        
        if (dstX > dstY)
            return 14f * dstY + 10f * (dstX - dstY);
        return 14f * dstX + 10f * (dstY - dstX);
    }
    
    /// <summary>
    /// Reset pathfinding data for all nodes in the grid
    /// </summary>
    private static void ResetGridPathfinding()
    {
        Node[,] grid = GameManager.Instance.Grid;
        
        for (int x = 0; x < GameManager.Instance.GridWidth; x++)
        {
            for (int y = 0; y < GameManager.Instance.GridHeight; y++)
            {
                grid[x, y].ResetPathfinding();
            }
        }
    }
    
    /// <summary>
    /// Update pheromone value at a node (ACO memory)
    /// Positive values = good path, negative values = danger
    /// </summary>
    public static void UpdatePheromone(Node node, float value)
    {
        if (node != null)
        {
            node.pheromoneValue = Mathf.Clamp(node.pheromoneValue + value, -1f, 1f);
        }
    }
    
    /// <summary>
    /// Decay all pheromones in the grid
    /// </summary>
    public static void DecayAllPheromones(float decayRate)
    {
        Node[,] grid = GameManager.Instance.Grid;
        
        for (int x = 0; x < GameManager.Instance.GridWidth; x++)
        {
            for (int y = 0; y < GameManager.Instance.GridHeight; y++)
            {
                grid[x, y].DecayPheromone(decayRate);
            }
        }
    }
}

