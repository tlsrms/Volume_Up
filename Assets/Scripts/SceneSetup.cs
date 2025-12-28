using UnityEngine;
using UnityEngine.InputSystem;

public class SceneSetup : MonoBehaviour
{
    [ContextMenu("Setup Game Scene")]
    public void SetupScene()
    {
        // 1. Create Game Manager
        if (FindAnyObjectByType<GameManager>() == null)
        {
            GameObject gm = new GameObject("GameManager");
            gm.AddComponent<GameManager>();
            Debug.Log("Created GameManager");
        }

        // 2. Create Core (Center)
        Transform coreTransform = null;
        GameObject core = GameObject.Find("Core");
        if (core == null)
        {
            core = new GameObject("Core");
            core.transform.position = Vector3.zero;
            core.transform.localScale = Vector3.one * 2f; // Make it big
            
            // Add Visuals
            var sr = core.AddComponent<SpriteRenderer>();
            var neon = core.AddComponent<NeonVisuals>();
            // Use reflection or serialized property normally, but here simple Setup is fine
            // We can't easily set the 'type' field via code without making it public or using SerializedObject.
            // Assuming NeonVisuals defaults to Guide or we leave it white.
            
            Debug.Log("Created Core");
        }
        coreTransform = core.transform;

        // 3. Create Player
        GameObject player = GameObject.Find("Player");
        if (player == null)
        {
            player = new GameObject("Player");
            player.transform.localScale = Vector3.one * 0.5f;

            // Add Components
            player.AddComponent<SpriteRenderer>();
            var visuals = player.AddComponent<NeonVisuals>();
            // visuals.type = NeonVisuals.NeonType.Player; // Need to expose or set this manually in Inspector for now

            var movement = player.AddComponent<Movement>();
            
            // Try to assign Core to Movement via SerializedObject or naming convention logic inside Movement (which we added: if null, creates new. But better to assign here if possible, but fields are private serialized).
            // Actually, Movement.cs creates its own center if null. But we want it to orbit "Core".
            // Since fields are private [SerializeField], we can't set them directly from another script without reflection or making them public.
            // For this helper, we'll rely on the user dragging it OR Movement.cs finding "Core".
            
            Debug.Log("Created Player");
        }

        // 4. Setup Camera
        Camera cam = Camera.main;
        if (cam != null)
        {
            if (cam.GetComponent<CameraController>() == null)
            {
                cam.gameObject.AddComponent<CameraController>();
            }
        }

        Debug.Log("Scene Setup Complete! Important: Assign 'Core' to Movement script, and 'Input Actions' to PlayerInput manually if missing.");
    }
}
