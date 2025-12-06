using UnityEngine;

/// <summary>
/// Spawns agents at random positions in the grid.
/// Useful for testing with multiple agents.
/// </summary>
public class AgentSpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    [SerializeField] private GameObject agentPrefab;
    [SerializeField] private int numberOfAgents = 10;
    [SerializeField] private Vector2 spawnArea = new Vector2(10, 10);
    [SerializeField] private bool spawnOnlyOnWalkable = true;
    
    [Header("Spectator Settings")]
    [SerializeField] private bool setFirstAsSpectator = true;
    
    private GameManager gameManager;
    
    private void Start()
    {
        gameManager = GameManager.Instance;
        
        if (gameManager == null)
        {
            Debug.LogError("AgentSpawner: GameManager not found!");
            return;
        }
        
        if (agentPrefab == null)
        {
            Debug.LogError("AgentSpawner: Agent Prefab not assigned!");
            return;
        }
        
        SpawnAgents();
    }
    
    /// <summary>
    /// Spawn all agents
    /// </summary>
    private void SpawnAgents()
    {
        int spawnedCount = 0;
        int attempts = 0;
        int maxAttempts = numberOfAgents * 10; // Prevent infinite loop
        
        while (spawnedCount < numberOfAgents && attempts < maxAttempts)
        {
            attempts++;
            
            Vector3 spawnPos = new Vector3(
                Random.Range(-spawnArea.x, spawnArea.x),
                Random.Range(-spawnArea.y, spawnArea.y),
                0
            );
            
            // Check if position is walkable
            if (spawnOnlyOnWalkable)
            {
                Node node = gameManager.GetNode(spawnPos);
                if (node == null || !node.IsWalkable)
                {
                    continue;
                }
                spawnPos = node.WorldPosition; // Snap to grid
            }
            
            GameObject agent = Instantiate(agentPrefab, spawnPos, Quaternion.identity);
            agent.name = $"Person_{spawnedCount + 1}";
            
            // Set first agent as spectator if enabled
            if (spawnedCount == 0 && setFirstAsSpectator)
            {
                PersonController pc = agent.GetComponent<PersonController>();
                if (pc != null)
                {
                    pc.IsSpectated = true;
                }
            }
            
            spawnedCount++;
        }
        
        if (spawnedCount < numberOfAgents)
        {
            Debug.LogWarning($"AgentSpawner: Only spawned {spawnedCount} out of {numberOfAgents} agents. " +
                           $"Check spawn area and walkable nodes.");
        }
        else
        {
            Debug.Log($"AgentSpawner: Successfully spawned {spawnedCount} agents.");
        }
    }
    
    /// <summary>
    /// Spawn a single agent at a specific position
    /// </summary>
    public GameObject SpawnAgentAt(Vector3 position)
    {
        if (agentPrefab == null)
        {
            Debug.LogError("AgentSpawner: Agent Prefab not assigned!");
            return null;
        }
        
        Node node = gameManager.GetNode(position);
        if (node != null && node.IsWalkable)
        {
            position = node.WorldPosition;
        }
        
        GameObject agent = Instantiate(agentPrefab, position, Quaternion.identity);
        return agent;
    }
}

