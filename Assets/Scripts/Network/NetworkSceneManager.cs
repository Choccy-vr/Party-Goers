using System;
using Unity.Netcode;
using UnityEngine;

public class NetworkSceneManager : NetworkBehaviour
{

    public static NetworkSceneManager Instance;

    [SerializeField] PersistantNetworkObject[] objectsToSave;

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

        if (Instance == this)
        {
            Instance = null;
        }
    }
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    static void ResetStatics()
    {
        Instance = null;
    }

    public void LoadSceneNetwork(string sceneName, Action onLoaded = null)
    {
        if (!IsServer) { Debug.LogWarning("Can't load scene because not server!"); return; }

        NetworkManager.Singleton.SceneManager.OnSceneEvent += HandleSceneEvent;

        foreach (var manager in objectsToSave)
        {
            if (manager != null)
            {
                manager.PrepareForSceneChange();
            }
        }
        SceneEventProgressStatus status = NetworkManager.Singleton.SceneManager.LoadScene(sceneName, UnityEngine.SceneManagement.LoadSceneMode.Single);
        if (status != SceneEventProgressStatus.Started)
        {
            NetworkManager.Singleton.SceneManager.OnSceneEvent -= HandleSceneEvent;
            Debug.LogError("Failed to init load scene: " + status);
        }
    }

    void HandleSceneEvent(SceneEvent sceneEvent)
    {
        if (sceneEvent.SceneEventType != SceneEventType.LoadEventCompleted)
        {
            return;
        }

        NetworkManager.Singleton.SceneManager.OnSceneEvent -= HandleSceneEvent;

        foreach (PersistantNetworkObject persistantNetworkObject in objectsToSave)
        {
            persistantNetworkObject.GetComponent<NetworkObject>().Spawn();
        }
    }
}
