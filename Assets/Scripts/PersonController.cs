using UnityEngine;
using UnityEngine.Rendering.Universal;
using System.Collections.Generic;

/// <summary>
/// Agent controller with state machine, health, stamina, and pathfinding.
/// Handles movement, panic states, and POV camera following.
/// </summary>
public class PersonController : MonoBehaviour
{
    public enum PersonState
    {
        IDLE,      // Wandering/standing still
        MOVING,    // Escaping (following path)
        PANIC,     // Trapped/Burning (high panic, low health)
        DEAD       // Agent has died
    }
    
    [Header("Stats")]
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float maxStamina = 100f;
    [SerializeField] private float healthRegenRate = 0f; // Health doesn't regenerate
    [SerializeField] private float staminaRegenRate = 20f; // Stamina per second
    [SerializeField] private float staminaDrainRate = 30f; // Stamina per second when sprinting
    
    [Header("Movement")]
    [SerializeField] private float walkSpeed = 2f;
    [SerializeField] private float sprintSpeed = 4f;
    [SerializeField] private float rotationSpeed = 10f;
    [SerializeField] private float panicSpeedMultiplier = 1.5f;
    
    [Header("Panic System")]
    [SerializeField] private float maxPanicLevel = 100f;
    [SerializeField] private float panicIncreaseRate = 5f; // Per second when in danger
    [SerializeField] private float panicDecreaseRate = 2f; // Per second when safe
    [SerializeField] private float panicThreshold = 50f; // When to enter PANIC state
    
    [Header("Damage")]
    [SerializeField] private float fireDamagePerSecond = 10f;
    [SerializeField] private float rubbleDamageOnStep = 5f;
    
    [Header("POV Camera")]
    [SerializeField] private bool isSpectated = false;
    [SerializeField] private Camera povCamera;
    [SerializeField] private float cameraFollowSpeed = 5f;
    [SerializeField] private Vector3 cameraOffset = new Vector3(0, 5, -5);
    
    [Header("2D Lighting (Fog of War)")]
    [SerializeField] private bool useAgentLight = true;
    [SerializeField] private Light2D agentSpotLight;
    [SerializeField] private float agentLightIntensity = 1.5f;
    [SerializeField] private float agentLightRange = 8f;
    
    [Header("Visualization")]
    [SerializeField] private bool showPathGizmo = true;
    [SerializeField] private Color pathColor = Color.blue;
    
    // Current stats
    private float currentHealth;
    private float currentStamina;
    private float currentPanicLevel;
    
    // State
    private PersonState currentState = PersonState.IDLE;
    
    // Pathfinding
    private List<Node> currentPath;
    private int currentPathIndex = 0;
    private Node currentNode;
    private Node targetNode;
    
    // Movement
    private Vector3 targetWorldPosition;
    private bool isSprinting = false;
    private float currentSpeed;
    
    // References
    private GameManager gameManager;
    private DisasterManager disasterManager;
    
    // Events
    public System.Action<PersonController> OnDeath;
    public System.Action<PersonController> OnReachedExit;
    
    // Properties
    public PersonState State => currentState;
    public float Health => currentHealth;
    public float Stamina => currentStamina;
    public float PanicLevel => currentPanicLevel;
    public bool IsSpectated
    {
        get => isSpectated;
        set
        {
            isSpectated = value;
            if (isSpectated && povCamera != null)
            {
                povCamera.gameObject.SetActive(true);
            }
        }
    }
    
    private void Awake()
    {
        currentHealth = maxHealth;
        currentStamina = maxStamina;
        currentPanicLevel = 0f;
        
        gameManager = GameManager.Instance;
        disasterManager = DisasterManager.Instance;
        
        // Get or create POV camera
        if (povCamera == null)
        {
            GameObject cameraObj = new GameObject("POV Camera");
            cameraObj.transform.SetParent(transform);
            povCamera = cameraObj.AddComponent<Camera>();
            povCamera.orthographic = true;
            povCamera.orthographicSize = 5f;
        }
        
        povCamera.gameObject.SetActive(isSpectated);
        
        // Set up agent light for Fog of War effect
        if (useAgentLight)
        {
            SetupAgentLight();
        }
    }
    
    /// <summary>
    /// Set up 2D spot light for agent (Fog of War effect)
    /// </summary>
    private void SetupAgentLight()
    {
        // Check if light already exists
        agentSpotLight = GetComponentInChildren<Light2D>();
        
        if (agentSpotLight == null || agentSpotLight.lightType != Light2D.LightType.Spot)
        {
            // Create light as child
            GameObject lightObj = new GameObject("AgentSpotLight");
            lightObj.transform.SetParent(transform);
            lightObj.transform.localPosition = Vector3.zero;
            
            agentSpotLight = lightObj.AddComponent<Light2D>();
            agentSpotLight.lightType = Light2D.LightType.Spot;
        }
        
        // Configure light
        agentSpotLight.intensity = agentLightIntensity;
        agentSpotLight.pointLightInnerAngle = 30f;
        agentSpotLight.pointLightOuterAngle = 60f;
        agentSpotLight.pointLightInnerRadius = 0.1f;
        agentSpotLight.pointLightOuterRadius = agentLightRange;
        agentSpotLight.color = Color.white;
    }
    
    private void Start()
    {
        // Initialize at current position
        currentNode = gameManager.GetNode(transform.position);
        if (currentNode == null)
        {
            // Find nearest walkable node
            currentNode = FindNearestWalkableNode();
        }
        
        // Start by finding path to exit
        FindPathToExit();
    }
    
    private void Update()
    {
        if (currentState == PersonState.DEAD)
            return;
        
        UpdateStats();
        UpdateState();
        UpdateMovement();
        UpdateCamera();
        CheckForDamage();
    }
    
    /// <summary>
    /// Update health, stamina, and panic levels
    /// </summary>
    private void UpdateStats()
    {
        // Health regeneration (usually none, but configurable)
        if (currentHealth < maxHealth && healthRegenRate > 0)
        {
            currentHealth = Mathf.Min(maxHealth, currentHealth + healthRegenRate * Time.deltaTime);
        }
        
        // Stamina regeneration/drain
        if (isSprinting && currentStamina > 0)
        {
            currentStamina = Mathf.Max(0f, currentStamina - staminaDrainRate * Time.deltaTime);
            if (currentStamina <= 0)
            {
                isSprinting = false;
            }
        }
        else if (!isSprinting && currentStamina < maxStamina)
        {
            currentStamina = Mathf.Min(maxStamina, currentStamina + staminaRegenRate * Time.deltaTime);
        }
        
        // Panic level update
        Node node = gameManager.GetNode(transform.position);
        if (node != null)
        {
            if (node.fireIntensity > 0.1f || node.isRubble)
            {
                // In danger - increase panic
                currentPanicLevel = Mathf.Min(maxPanicLevel, currentPanicLevel + panicIncreaseRate * Time.deltaTime);
            }
            else
            {
                // Safe - decrease panic
                currentPanicLevel = Mathf.Max(0f, currentPanicLevel - panicDecreaseRate * Time.deltaTime);
            }
        }
    }
    
    /// <summary>
    /// Update state machine
    /// </summary>
    private void UpdateState()
    {
        // Check for death
        if (currentHealth <= 0 && currentState != PersonState.DEAD)
        {
            SetState(PersonState.DEAD);
            return;
        }
        
        // State transitions
        switch (currentState)
        {
            case PersonState.IDLE:
                if (currentPath != null && currentPath.Count > 0)
                {
                    SetState(PersonState.MOVING);
                }
                else if (currentPanicLevel >= panicThreshold)
                {
                    SetState(PersonState.PANIC);
                }
                break;
                
            case PersonState.MOVING:
                if (currentPanicLevel >= panicThreshold)
                {
                    SetState(PersonState.PANIC);
                }
                else if (currentPath == null || currentPath.Count == 0)
                {
                    SetState(PersonState.IDLE);
                }
                break;
                
            case PersonState.PANIC:
                if (currentPanicLevel < panicThreshold && currentHealth > 0)
                {
                    if (currentPath != null && currentPath.Count > 0)
                    {
                        SetState(PersonState.MOVING);
                    }
                    else
                    {
                        SetState(PersonState.IDLE);
                    }
                }
                break;
        }
    }
    
    /// <summary>
    /// Set new state
    /// </summary>
    private void SetState(PersonState newState)
    {
        if (currentState == newState)
            return;
        
        currentState = newState;
        
        // State-specific behavior
        switch (newState)
        {
            case PersonState.PANIC:
                // In panic, try to find a new path
                FindPathToExit();
                break;
                
            case PersonState.DEAD:
                // Stop movement
                currentPath = null;
                OnDeath?.Invoke(this);
                break;
        }
    }
    
    /// <summary>
    /// Update movement along path
    /// </summary>
    private void UpdateMovement()
    {
        if (currentState == PersonState.DEAD || currentState == PersonState.IDLE)
            return;
        
        // Check if we need a new path
        if (currentPath == null || currentPath.Count == 0)
        {
            FindPathToExit();
            return;
        }
        
        // Check if current target node is still valid
        if (currentPathIndex >= currentPath.Count)
        {
            // Reached end of path - check if we're at exit
            Node node = gameManager.GetNode(transform.position);
            if (node != null && node.isExit)
            {
                OnReachedExit?.Invoke(this);
                currentPath = null;
                return;
            }
            
            // Find new path
            FindPathToExit();
            return;
        }
        
        // Get current target node
        targetNode = currentPath[currentPathIndex];
        targetWorldPosition = targetNode.WorldPosition;
        
        // Check if we've reached the current node
        float distanceToTarget = Vector3.Distance(transform.position, targetWorldPosition);
        if (distanceToTarget < 0.1f)
        {
            currentNode = targetNode;
            currentPathIndex++;
            
            // Update pheromone (positive for successful path segment)
            Pathfinding.UpdatePheromone(currentNode, 0.1f);
            
            // Check if we stepped on rubble
            if (currentNode.isRubble)
            {
                TakeDamage(rubbleDamageOnStep);
            }
        }
        
        // Calculate movement speed
        float baseSpeed = isSprinting && currentStamina > 0 ? sprintSpeed : walkSpeed;
        if (currentState == PersonState.PANIC)
        {
            baseSpeed *= panicSpeedMultiplier;
        }
        currentSpeed = baseSpeed;
        
        // Move towards target
        Vector3 direction = (targetWorldPosition - transform.position).normalized;
        transform.position += direction * currentSpeed * Time.deltaTime;
        
        // Rotate towards movement direction
        if (direction.magnitude > 0.1f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(Vector3.forward, direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }
    
    /// <summary>
    /// Find path to nearest exit
    /// </summary>
    public void FindPathToExit()
    {
        if (currentNode == null)
        {
            currentNode = gameManager.GetNode(transform.position);
            if (currentNode == null)
            {
                currentNode = FindNearestWalkableNode();
            }
        }
        
        if (currentNode == null)
        {
            Debug.LogWarning("PersonController: Cannot find starting node!");
            return;
        }
        
        currentPath = Pathfinding.FindPathToNearestExit(currentNode, false);
        currentPathIndex = 0;
        
        // If no path found, try to find any walkable path
        if (currentPath == null || currentPath.Count == 0)
        {
            Debug.LogWarning("PersonController: No path to exit found!");
            currentPath = null;
        }
    }
    
    /// <summary>
    /// Find nearest walkable node
    /// </summary>
    private Node FindNearestWalkableNode()
    {
        Node[,] grid = gameManager.Grid;
        Node nearest = null;
        float nearestDistance = float.MaxValue;
        Vector2Int currentGridPos = gameManager.GetGridPosition(transform.position);
        
        for (int x = 0; x < gameManager.GridWidth; x++)
        {
            for (int y = 0; y < gameManager.GridHeight; y++)
            {
                Node node = grid[x, y];
                if (node.IsWalkable)
                {
                    float distance = Vector2Int.Distance(currentGridPos, node.gridPosition);
                    if (distance < nearestDistance)
                    {
                        nearestDistance = distance;
                        nearest = node;
                    }
                }
            }
        }
        
        return nearest;
    }
    
    /// <summary>
    /// Check for damage from fire
    /// </summary>
    private void CheckForDamage()
    {
        Node node = gameManager.GetNode(transform.position);
        if (node != null && node.fireIntensity > 0.1f)
        {
            float damage = fireDamagePerSecond * node.fireIntensity * Time.deltaTime;
            TakeDamage(damage);
        }
    }
    
    /// <summary>
    /// Apply damage to the agent
    /// </summary>
    public void TakeDamage(float damage)
    {
        currentHealth = Mathf.Max(0f, currentHealth - damage);
        
        // Increase panic when taking damage
        currentPanicLevel = Mathf.Min(maxPanicLevel, currentPanicLevel + damage * 0.5f);
    }
    
    /// <summary>
    /// Update POV camera to follow agent
    /// </summary>
    private void UpdateCamera()
    {
        if (!isSpectated || povCamera == null)
            return;
        
        // Update camera position
        Vector3 targetCameraPos = transform.position + cameraOffset;
        povCamera.transform.position = Vector3.Lerp(
            povCamera.transform.position,
            targetCameraPos,
            cameraFollowSpeed * Time.deltaTime
        );
        
        // Make this the main camera if spectating
        if (Camera.main != povCamera)
        {
            povCamera.tag = "MainCamera";
        }
        
        // Update light visibility based on spectating
        if (agentSpotLight != null)
        {
            agentSpotLight.enabled = isSpectated || useAgentLight;
        }
    }
    
    /// <summary>
    /// Set sprinting state
    /// </summary>
    public void SetSprinting(bool sprint)
    {
        if (currentStamina > 0 || !sprint)
        {
            isSprinting = sprint;
        }
    }
    
    private void OnDrawGizmos()
    {
        if (!showPathGizmo || currentPath == null || currentPath.Count == 0)
            return;
        
        // Draw path
        Gizmos.color = pathColor;
        for (int i = 0; i < currentPath.Count - 1; i++)
        {
            Gizmos.DrawLine(
                currentPath[i].WorldPosition,
                currentPath[i + 1].WorldPosition
            );
        }
        
        // Draw current target
        if (currentPathIndex < currentPath.Count)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(currentPath[currentPathIndex].WorldPosition, 0.3f);
        }
    }
}

