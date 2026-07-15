using System.Collections.Generic;
using Unity.Netcode;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Teleportation;

public class MinigameManager : NetworkBehaviour
{
    enum TransitionType { none, partyMap, minigame }
    TransitionType currentTransition = TransitionType.none;
    public static MinigameManager Instance;

    public List<MinigameConfig> minigames;
    public List<MinigameSpawnPoint> minigameSpawnPoints = new List<MinigameSpawnPoint>();

    [SerializeField] int[] minigameRewards = { 10, 5, 3, 0 };

    public UnityEvent<MinigameConfig> minigameStart;
    public UnityEvent<MinigameConfig> minigameEnd;
    MinigameConfig currentMinigame;

    bool isMinigame = false;

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
    void Start()
    {
        minigameSpawnPoints.Sort((a, b) => a.spawnPointID.CompareTo(b.spawnPointID));
    }

    public override void OnNetworkDespawn()
    {
        if (IsServer && NetworkManager.Singleton != null && NetworkManager.Singleton.SceneManager != null)
        {
            NetworkManager.SceneManager.OnSceneEvent -= OnSceneEvent;
        }
    }
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        Debug.Log($"MinigameManager successfully spawned on Network! Am I Server? {IsServer}");

        if (IsServer)
        {
            NetworkManager.SceneManager.OnSceneEvent += OnSceneEvent;
        }
    }

    public void startRandom4PlayerMinigame()
    {
        MinigameConfig targetMinigame = selectRandomMinigame(MinigameType.fourPlayer);
        currentMinigame = targetMinigame;
        isMinigame = true;
        teleportToMinigame(targetMinigame.minigameID);
        minigameStart?.Invoke(targetMinigame);
    }
    public void startRandomDuelMinigame()
    {
        MinigameConfig targetMinigame = selectRandomMinigame(MinigameType.oneVSone);
        currentMinigame = targetMinigame;
        isMinigame = true;
        teleportToMinigame(targetMinigame.minigameID);
        minigameStart?.Invoke(targetMinigame);
    }
    public void endMinigame()
    {
        isMinigame = false;
        teleportToMap(MapManager.Instance.currentMap.mapID);
    }

    public void teleportToMinigame(string targetMinigame)
    {
        if (!IsServer)
        {
            Debug.Log("Not server. starting rpc");
            teleportToMinigameServerRpc(targetMinigame);
            return;
        }

        MinigameConfig minigameConfig = FindMinigameFromID(targetMinigame);
        Debug.Log("Teleporting to map");

        currentTransition = TransitionType.minigame;
        NetworkSceneManager.Instance.LoadSceneNetwork(minigameConfig.sceneName);
        minigameStart?.Invoke(minigameConfig);
    }
    [ServerRpc]
    void teleportToMinigameServerRpc(string targetMinigame)
    {
        teleportToMinigame(targetMinigame);
    }
    [ServerRpc]
    void teleportToMapServerRpc(string targetMap)
    {
        teleportToMap(targetMap);
    }
    public void teleportToMap(string targetMap)
    {
        // If THIS specific instance isn't spawned, look for the one that IS.
        if (!IsSpawned)
        {
            Debug.LogWarning($"This specific instance ({gameObject.name}) is not spawned. Redirecting to the active network instance...");

            // Find the actual network-spawned version in the scene
            MinigameManager[] allManagers = FindObjectsByType<MinigameManager>();
            MinigameManager activeNetworkInstance = null;

            foreach (var mgr in allManagers)
            {
                if (mgr.IsSpawned)
                {
                    activeNetworkInstance = mgr;
                    break;
                }
            }

            if (activeNetworkInstance != null)
            {
                // Route the call through the actual spawned network manager
                Instance = activeNetworkInstance;
                activeNetworkInstance.teleportToMap(targetMap);
                return;
            }

            Debug.LogError("Could not find any network-spawned MinigameManager in the scene!");
            return;
        }
        Debug.Log("Teleporting to map");
        if (!IsServer)
        {
            Debug.Log("Not server. starting rpc");
            teleportToMapServerRpc(targetMap);
            return;
        }
        currentTransition = TransitionType.partyMap;
        NetworkSceneManager.Instance.LoadSceneNetwork(MapManager.Instance.findPartyMapWithID(targetMap).sceneName);
        //Apply player config
    }
    [ClientRpc]
    void teleportPlayerToMinigameSpawnClientRpc()
    {
        StartCoroutine(WaitAndTeleportRoutine());
    }

    System.Collections.IEnumerator WaitAndTeleportRoutine()
    {
        GameObject playerXROrigin = null;
        NetworkIdenity networkIdentity = null;

        float timeout = 1.5f;
        float elapsed = 0f;

        while (elapsed < timeout)
        {
            playerXROrigin = FindAnyObjectByType<XROrigin>()?.gameObject;
            if (playerXROrigin != null)
            {
                networkIdentity = playerXROrigin.GetComponent<NetworkIdenity>();
                if (networkIdentity != null && networkIdentity.networkPlayerIdenity != null)
                {
                    break;
                }
            }
            elapsed += Time.deltaTime;
            yield return null;
        }

        if (playerXROrigin == null)
        {
            Debug.LogError("XR ORIGIN is null");
            yield break;
        }

        if (networkIdentity == null || networkIdentity.networkPlayerIdenity == null)
        {
            Debug.LogError("networkIdentity component or networkPlayerIdenity is null after waiting!");
            yield break;
        }

        VRPartyPlayer player = networkIdentity.networkPlayerIdenity.GetComponent<VRPartyPlayer>();
        if (player == null)
        {
            Debug.LogError("VR PARTY PLAYER COMPONENT IS NULL");
            yield break;
        }

        int spawnIndex = (int)player.playerData.networkClientId;

        if (minigameSpawnPoints == null || minigameSpawnPoints.Count == 0)
        {
            MinigameSpawnPoint[] foundPoints = FindObjectsByType<MinigameSpawnPoint>();
            minigameSpawnPoints = new List<MinigameSpawnPoint>(foundPoints);
            minigameSpawnPoints.Sort((a, b) => a.spawnPointID.CompareTo(b.spawnPointID));
        }

        if (spawnIndex >= 0 && spawnIndex < minigameSpawnPoints.Count)
        {
            MinigameSpawnPoint targetSpawn = minigameSpawnPoints[spawnIndex];
            if (targetSpawn != null && targetSpawn.spawnTransform != null)
            {
                playerXROrigin.transform.position = targetSpawn.spawnTransform.position;
            }
            else
            {
                Debug.LogError($"Spawn point at index {spawnIndex} or its spawnTransform is null!");
            }
        }
        else
        {
            Debug.LogError($"Spawn index {spawnIndex} is out of bounds! Total spawn points found: {minigameSpawnPoints.Count}");
        }
    }
    [ClientRpc]
    void teleportPlayerToPartySpaceClientRpc()
    {
        GameObject playerXROrigin = FindAnyObjectByType<XROrigin>().gameObject;
        if (playerXROrigin != null)
        {
            VRPartyPlayer player = PlayerManager.Instance.FindPlayerFromID(NetworkManager.Singleton.LocalClientId);
            if (player != null)
            {
                playerXROrigin.transform.position = MapManager.Instance.findPartySpaceWithID(player.currentSpaceId).GetComponent<TeleportationAnchor>().teleportAnchorTransform.position;
            }

        }
    }

    void OnSceneEvent(SceneEvent sceneEvent)
    {
        if (sceneEvent.SceneEventType == SceneEventType.LoadEventCompleted)
        {
            if (currentTransition == TransitionType.minigame)
            {
                teleportPlayerToMinigameSpawnClientRpc();
                currentTransition = TransitionType.none;
            }
            else if (currentTransition == TransitionType.partyMap)
            {
                teleportPlayerToPartySpaceClientRpc();
                currentTransition = TransitionType.none;
            }


        }
    }

    public MinigameConfig selectRandomMinigame(MinigameType type)
    {
        List<MinigameConfig> filteredMinigames = minigames.FindAll(m => m.minigameType == type);
        int randomIndex = Random.Range(0, filteredMinigames.Count);
        return filteredMinigames[randomIndex];
    }

    public void awardMinigamePrizes(List<VRPartyPlayer> playersInOrder)
    {
        if (!IsServer) return;
        for (int i = 0; i < playersInOrder.Count; i++)
        {
            GameSessionManager.Instance.AddCoinsToPlayer(playersInOrder[i].playerData.networkClientId, minigameRewards[i]);
        }
    }
    MinigameConfig FindMinigameFromID(string ID)
    {
        return minigames.Find(m => m.minigameID == ID);
    }
}
