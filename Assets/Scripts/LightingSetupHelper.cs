using UnityEngine;
using UnityEngine.Rendering.Universal;

/// <summary>
/// Helper script to set up 2D lighting for the Fog of War effect.
/// Attach this to a GameObject and run it once to set up lighting layers.
/// </summary>
public class LightingSetupHelper : MonoBehaviour
{
    [Header("Light Setup")]
    [SerializeField] private bool setupOnStart = false;
    [SerializeField] private float globalLightIntensity = 0.3f;
    [SerializeField] private Color globalLightColor = new Color(0.2f, 0.2f, 0.3f);
    
    [Header("Agent Light Settings")]
    [SerializeField] private float agentLightIntensity = 1.5f;
    [SerializeField] private float agentLightRange = 8f;
    [SerializeField] private float agentLightInnerAngle = 30f;
    [SerializeField] private float agentLightOuterAngle = 60f;
    
    private void Start()
    {
        if (setupOnStart)
        {
            SetupLighting();
        }
    }
    
    /// <summary>
    /// Set up global light and configure lighting system
    /// </summary>
    [ContextMenu("Setup Lighting")]
    public void SetupLighting()
    {
        // Find or create global light
        Light2D globalLight = FindObjectOfType<Light2D>();
        if (globalLight == null || globalLight.lightType != Light2D.LightType.Global)
        {
            GameObject globalLightObj = new GameObject("GlobalLight");
            globalLight = globalLightObj.AddComponent<Light2D>();
            globalLight.lightType = Light2D.LightType.Global;
        }
        
        globalLight.intensity = globalLightIntensity;
        globalLight.color = globalLightColor;
        
        Debug.Log("LightingSetupHelper: Global light configured.");
        
        // Note: Agent lights should be added to PersonController or Agent prefab
        // This script just sets up the global light
    }
    
    /// <summary>
    /// Add a 2D spot light to an agent GameObject
    /// </summary>
    public static Light2D AddAgentLight(GameObject agent)
    {
        // Check if light already exists
        Light2D existingLight = agent.GetComponentInChildren<Light2D>();
        if (existingLight != null && existingLight.lightType == Light2D.LightType.Spot)
        {
            return existingLight;
        }
        
        // Create light as child of agent
        GameObject lightObj = new GameObject("AgentSpotLight");
        lightObj.transform.SetParent(agent.transform);
        lightObj.transform.localPosition = Vector3.zero;
        
        Light2D spotLight = lightObj.AddComponent<Light2D>();
        spotLight.lightType = Light2D.LightType.Spot;
        spotLight.intensity = 1.5f;
        spotLight.pointLightInnerAngle = 30f;
        spotLight.pointLightOuterAngle = 60f;
        spotLight.pointLightInnerRadius = 0.1f;
        spotLight.pointLightOuterRadius = 8f;
        spotLight.color = Color.white;
        
        return spotLight;
    }
}

