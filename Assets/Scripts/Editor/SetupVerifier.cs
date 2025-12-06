using UnityEngine;
using UnityEditor;

/// <summary>
/// Verifies that the Dwight V12 scene is set up correctly.
/// Use: Window -> Dwight V12 -> Verify Setup
/// </summary>
public class SetupVerifier : EditorWindow
{
    [MenuItem("Window/Dwight V12/Verify Setup")]
    public static void ShowWindow()
    {
        GetWindow<SetupVerifier>("Setup Verifier");
    }
    
    private void OnGUI()
    {
        GUILayout.Label("Dwight V12 Setup Verification", EditorStyles.boldLabel);
        GUILayout.Space(10);
        
        bool allGood = true;
        
        // Check GameManager
        GUILayout.Label("Core Components:", EditorStyles.boldLabel);
        if (FindObjectOfType<GameManager>() == null)
        {
            EditorGUILayout.HelpBox("❌ GameManager not found in scene!", MessageType.Error);
            allGood = false;
        }
        else
        {
            EditorGUILayout.HelpBox("✓ GameManager found", MessageType.Info);
        }
        
        if (FindObjectOfType<DisasterManager>() == null)
        {
            EditorGUILayout.HelpBox("❌ DisasterManager not found in scene!", MessageType.Error);
            allGood = false;
        }
        else
        {
            EditorGUILayout.HelpBox("✓ DisasterManager found", MessageType.Info);
        }
        
        if (FindObjectOfType<PathfindingVisualizer>() == null)
        {
            EditorGUILayout.HelpBox("❌ PathfindingVisualizer not found in scene!", MessageType.Error);
            allGood = false;
        }
        else
        {
            EditorGUILayout.HelpBox("✓ PathfindingVisualizer found", MessageType.Info);
        }
        
        // Check Grid
        GUILayout.Space(10);
        GUILayout.Label("Scene Objects:", EditorStyles.boldLabel);
        if (FindObjectOfType<Grid>() == null)
        {
            EditorGUILayout.HelpBox("❌ Grid not found in scene!", MessageType.Error);
            allGood = false;
        }
        else
        {
            EditorGUILayout.HelpBox("✓ Grid found", MessageType.Info);
            
            // Check tilemaps
            Tilemap floorTilemap = GameObject.Find("FloorTilemap")?.GetComponent<Tilemap>();
            Tilemap wallTilemap = GameObject.Find("WallTilemap")?.GetComponent<Tilemap>();
            
            if (floorTilemap == null)
            {
                EditorGUILayout.HelpBox("⚠ FloorTilemap not found", MessageType.Warning);
            }
            else
            {
                EditorGUILayout.HelpBox("✓ FloorTilemap found", MessageType.Info);
            }
            
            if (wallTilemap == null)
            {
                EditorGUILayout.HelpBox("⚠ WallTilemap not found", MessageType.Warning);
            }
            else
            {
                EditorGUILayout.HelpBox("✓ WallTilemap found", MessageType.Info);
            }
        }
        
        // Check Camera
        Camera mainCam = Camera.main;
        if (mainCam == null)
        {
            EditorGUILayout.HelpBox("⚠ Main Camera not found", MessageType.Warning);
        }
        else
        {
            if (mainCam.orthographic)
            {
                EditorGUILayout.HelpBox("✓ Camera is Orthographic", MessageType.Info);
            }
            else
            {
                EditorGUILayout.HelpBox("⚠ Camera should be Orthographic for 2D", MessageType.Warning);
            }
        }
        
        // Check for agents
        GUILayout.Space(10);
        GUILayout.Label("Agents:", EditorStyles.boldLabel);
        PersonController[] agents = FindObjectsOfType<PersonController>();
        if (agents.Length == 0)
        {
            EditorGUILayout.HelpBox("⚠ No agents found in scene. Create a Person prefab with PersonController.", MessageType.Warning);
        }
        else
        {
            EditorGUILayout.HelpBox($"✓ {agents.Length} agent(s) found", MessageType.Info);
        }
        
        // Summary
        GUILayout.Space(20);
        if (allGood && agents.Length > 0)
        {
            EditorGUILayout.HelpBox("✅ Setup looks good! You can press Play to test.", MessageType.Info);
        }
        else
        {
            EditorGUILayout.HelpBox("⚠ Some components are missing. Use 'Window → Dwight V12 → Setup Scene' to fix.", MessageType.Warning);
        }
        
        if (GUILayout.Button("Run Setup Helper", GUILayout.Height(30)))
        {
            SceneSetupHelper.ShowWindow();
        }
    }
}

