using System.Collections.Generic;
using Unity.Netcode;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Teleportation;

public class MinigameManager : NetworkBehaviour
{
    public static MinigameManager Instance;

    public List<MinigameConfig> minigames;

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

    public void startRandom4PlayerMinigame()
    {
        MinigameConfig targetMinigame = selectRandomMinigame(MinigameType.fourPlayer);
        currentMinigame = targetMinigame;
        isMinigame = true;
        teleportToMinigame(targetMinigame);
        minigameStart?.Invoke(targetMinigame);
    }
    /*public void endMinigame()
    {
        isMinigame = false;
        teleportToMap()
    }*/

    public void teleportToMinigame(MinigameConfig targetMinigame)
    {
        if (!IsServer) return;
        NetworkSceneManager.Instance.LoadSceneNetwork(targetMinigame.sceneName);
        //Apply player configs
        //teleportPlayerToMinigameSpawnClientRpc(targetMinigame.minigameID);
    }
    public void teleportToMap(MapConfig targetMap)
    {
        if (!IsServer) return;
        NetworkSceneManager.Instance.LoadSceneNetwork(targetMap.sceneName);
        //Apply player config
        teleportPlayerToPartySpaceClientRpc();
    }
    /*[ClientRpc]
    void teleportPlayerToMinigameSpawnClientRpc(string targetMinigameID)
    {
        GameObject playerXROrigin = FindAnyObjectByType<XROrigin>().gameObject;
        if (playerXROrigin != null)
        {
            VRPartyPlayer player = playerXROrigin.GetComponent<NetworkIdenity>().networkPlayerIdenity.GetComponent<VRPartyPlayer>();
            if (player != null)
            {
                playerXROrigin.transform.position = FindMinigameFromID(targetMinigameID).playerSpawnPoints[(int)player.playerData.networkClientId].position;
            }

        }
    }*/
    [ClientRpc]
    void teleportPlayerToPartySpaceClientRpc()
    {
        GameObject playerXROrigin = FindAnyObjectByType<XROrigin>().gameObject;
        if (playerXROrigin != null)
        {
            VRPartyPlayer player = playerXROrigin.GetComponent<NetworkIdenity>().networkPlayerIdenity.GetComponent<VRPartyPlayer>();
            if (player != null)
            {
                playerXROrigin.transform.position = MapManager.Instance.findPartySpaceWithID(player.currentSpaceId).GetComponent<TeleportationAnchor>().teleportAnchorTransform.position;
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
