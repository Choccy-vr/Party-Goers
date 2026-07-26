using System;
using Unity.Netcode;
using UnityEngine;

public class NetworkSceneManager : NetworkBehaviour
{
    public static NetworkSceneManager Instance;

    // Store whatever action we want to run AFTER the scene loads
    private Action pendingLoadCallback;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
        if (Instance == this) Instance = null;
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    static void ResetStatics()
    {
        Instance = null;
    }

    // Takes ANY scene name, and an optional action to run when finished
    public void LoadSceneNetwork(string sceneName, Action onLoaded = null)
    {
        if (!IsServer) return;

        // Store the callback for later
        pendingLoadCallback = onLoaded;

        // Listen for when everyone finishes loading
        NetworkManager.Singleton.SceneManager.OnSceneEvent += HandleSceneEvent;

        // Start the load
        NetworkManager.Singleton.SceneManager.LoadScene(sceneName, UnityEngine.SceneManagement.LoadSceneMode.Single);
    }

    void HandleSceneEvent(SceneEvent sceneEvent)
    {
        // Fires only when Host and ALL Clients are fully loaded
        if (sceneEvent.SceneEventType == SceneEventType.LoadComplete)
        {
            NetworkManager.Singleton.SceneManager.OnSceneEvent -= HandleSceneEvent;

            Debug.Log($"All clients loaded the scene. Running callback.");

            // Run the stored action (if one was provided)
            pendingLoadCallback?.Invoke();
            pendingLoadCallback = null; // Clear it so it doesn't run twice
        }
    }
}