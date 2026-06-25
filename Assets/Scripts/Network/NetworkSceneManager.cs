using Unity.Netcode;
using UnityEngine;

public class NetworkSceneManager : NetworkBehaviour
{

    public static NetworkSceneManager Instance;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void LoadSceneNetwork(string sceneName)
    {
        if (!IsServer) { Debug.LogWarning("Can't load scene because not server!"); return; }
        SceneEventProgressStatus status = NetworkManager.Singleton.SceneManager.LoadScene(sceneName, UnityEngine.SceneManagement.LoadSceneMode.Single);
        if (status != SceneEventProgressStatus.Started)
        {
            Debug.LogError("Failed to init load scene: " + status);
        }
    }
}
