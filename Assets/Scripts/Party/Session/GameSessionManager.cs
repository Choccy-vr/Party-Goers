using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class GameSessionManager : NetworkBehaviour
{

    public static GameSessionManager Instance { get; private set; }

    public MapConfig activeMap;

    public NetworkList<PlayerSessionData> activePlayers = new NetworkList<PlayerSessionData>();

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

    public void RegisterPlayer(ulong clientId, string username)
    {
        if (!IsServer) return;
        activePlayers.Add(new PlayerSessionData(clientId, username));

        Debug.Log("New Player Registered! " + username);
    }

    public PlayerSessionData? getCurrentPlayerData()
    {
        return getPlayerData(NetworkManager.Singleton.LocalClientId);
    }

    public PlayerSessionData? getPlayerData(ulong clientId)
    {
        foreach (var player in activePlayers)
        {
            if (player.networkClientId == clientId)
            {
                return player;
            }
        }
        return null;
    }

    public void AddCoinsToPlayer(ulong clientId, int amount)
    {

        if (!IsServer) return;
        if (amount == 0) return;

        for (int i = 0; i < activePlayers.Count; i++)
        {
            if (activePlayers[i].networkClientId == clientId)
            {
                Debug.Log("Found player to add coins");
                PlayerSessionData data = activePlayers[i];

                data.coins += amount;
                Debug.Log("Estimated coins amount: " + data.coins);

                activePlayers.RemoveAt(i);
                activePlayers.Insert(i, data);
                Debug.Log("Final coin count = " + activePlayers[i].coins);
                break;
            }
        }
    }
    public void AddItemToPlayer(ulong clientId, string ItemID)
    {
        if (!IsServer) return;

        for (int i = 0; i < activePlayers.Count; i++)
        {
            if (activePlayers[i].networkClientId == clientId)
            {
                Debug.Log("Found player to prize coins");
                PlayerSessionData data = activePlayers[i];

                if (data.TryAddItem(ItemID))
                {
                    activePlayers.RemoveAt(i);
                    activePlayers.Insert(i, data);
                }
                else
                {
                    Debug.LogError("Inventory full!");
                }
                break;
            }
        }
    }
}
