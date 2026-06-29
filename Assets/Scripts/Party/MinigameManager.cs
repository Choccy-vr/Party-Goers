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
    public List<MinigameSpawnPoint> minigameSpawnPoints;

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
        DontDestroyOnLoad(gameObject);
    }
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    static void ResetStatics()
    {
        Instance = null;
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
    /*public void endMinigame()
    {
        isMinigame = false;
        teleportToMap()
    }*/

    public void teleportToMinigame(string targetMinigame)
    {
        MinigameConfig minigameConfig = FindMinigameFromID(targetMinigame);
        if (!IsSpawned)
        {
            Debug.LogWarning("MinigameManager is not fully spawned on the network yet! Ignoring teleport request.");
            return;
        }
        Debug.Log("Teleporting to map");
        if (!IsServer)
        {
            Debug.Log("Not server. starting rpc");
            teleportToMinigameServerRpc(targetMinigame);
            return;
        }
        currentTransition = TransitionType.minigame;
        NetworkSceneManager.Instance.LoadSceneNetwork(minigameConfig.sceneName);
        //Apply player configs

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
        if (!IsSpawned)
        {
            Debug.LogWarning("MinigameManager is not fully spawned on the network yet! Ignoring teleport request.");
            return;
        }
        Debug.Log("Teleporting to map");
        if (!IsServer)
        {
            Debug.Log("Not server. starting rpc");
            teleportToMapServerRpc(targetMap);
            return;
        }

        NetworkSceneManager.Instance.LoadSceneNetwork(MapManager.Instance.findPartyMapWithID(targetMap).sceneName);
        //Apply player config
    }
    [ClientRpc]
    void teleportPlayerToMinigameSpawnClientRpc()
    {
        GameObject playerXROrigin = FindAnyObjectByType<XROrigin>().gameObject;
        if (playerXROrigin != null)
        {
            VRPartyPlayer player = playerXROrigin.GetComponent<NetworkIdenity>().networkPlayerIdenity.GetComponent<VRPartyPlayer>();
            if (player != null)
            {
                playerXROrigin.transform.position = minigameSpawnPoints[(int)player.playerData.networkClientId].spawnTransform.position;
            }

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
