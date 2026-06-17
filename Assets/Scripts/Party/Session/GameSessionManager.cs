using System;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSessionManager : NetworkBehaviour
{

    public static GameSessionManager Instance { get; private set; }

    public List<PlayerSessionData> activePlayers = new List<PlayerSessionData>();

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

    public void RegisterPlayer(ulong clientId, string username)
    {
        if (!IsServer) return;
        activePlayers.Add(new PlayerSessionData(clientId, username));

        Debug.Log("New Player Registered! " + username);
    }

    public PlayerSessionData getPlayerData(ulong clientId)
    {
        return activePlayers.Find(p => p.networkClientId == clientId);
    }
}
