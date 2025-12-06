using UnityEngine;

/// <summary>
/// Represents a single node in the grid-based pathfinding system.
/// Stores all necessary data for A* pathfinding and pheromone-based navigation.
/// </summary>
[System.Serializable]
public class Node
{
    public Vector2Int gridPosition;
    
    // Terrain properties
    public bool isWall;
    public bool isRubble;
    public bool isExit;
    
    // Dynamic properties
    [Range(0f, 1f)]
    public float fireIntensity; // 0 = no fire, 1 = maximum fire
    
    [Range(0f, 1f)]
    public float pheromoneValue; // ACO memory: positive = good path, negative = danger
    
    // A* pathfinding variables
    public float gCost; // Cost from start node
    public float hCost; // Heuristic cost to target
    public Node parent; // For path reconstruction
    
    // Cached world position (calculated from grid position)
    private Vector3 _worldPosition;
    
    public Node(Vector2Int gridPos, Vector3 worldPos)
    {
        gridPosition = gridPos;
        _worldPosition = worldPos;
        isWall = false;
        isRubble = false;
        isExit = false;
        fireIntensity = 0f;
        pheromoneValue = 0f;
        gCost = 0f;
        hCost = 0f;
        parent = null;
    }
    
    /// <summary>
    /// Total cost for A* (f = g + h)
    /// </summary>
    public float fCost
    {
        get { return gCost + hCost; }
    }
    
    /// <summary>
    /// Get the world position of this node
    /// </summary>
    public Vector3 WorldPosition
    {
        get { return _worldPosition; }
        set { _worldPosition = value; }
    }
    
    /// <summary>
    /// Check if this node is walkable
    /// </summary>
    public bool IsWalkable
    {
        get { return !isWall && !isRubble; }
    }
    
    /// <summary>
    /// Calculate movement cost to enter this node
    /// Base Cost (1) + Rubble Penalty (10) + Fire Penalty (50 * intensity)
    /// </summary>
    public float GetMovementCost()
    {
        if (!IsWalkable)
            return float.MaxValue;
        
        float cost = 1f; // Base cost
        if (isRubble)
            cost += 10f; // Rubble penalty
        cost += 50f * fireIntensity; // Fire penalty (0-50)
        
        return cost;
    }
    
    /// <summary>
    /// Reset A* pathfinding values
    /// </summary>
    public void ResetPathfinding()
    {
        gCost = 0f;
        hCost = 0f;
        parent = null;
    }
    
    /// <summary>
    /// Decay pheromone over time
    /// </summary>
    public void DecayPheromone(float decayRate)
    {
        if (pheromoneValue > 0)
        {
            pheromoneValue = Mathf.Max(0f, pheromoneValue - decayRate * Time.deltaTime);
        }
        else if (pheromoneValue < 0)
        {
            pheromoneValue = Mathf.Min(0f, pheromoneValue + decayRate * Time.deltaTime);
        }
    }
}

