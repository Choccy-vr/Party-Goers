using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
public class VRPartyPlayer : NetworkBehaviour
{
    public int currentSpaceId;
    public int spacesToMove = 0;
    public PlayerSessionData playerData { get; private set; }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (IsOwner)
        {
            NetworkIdenity.Instance.networkPlayerIdenity = gameObject;
        }
        RefreshLocalData();
        if (GameSessionManager.Instance != null)
        {
            GameSessionManager.Instance.activePlayers.OnListChanged += OnNetworkPlayersChanged;
        }
        PlayerManager.Instance.activePlayerObj.Add(this);
        TurnManager.Instance.turnOrderObj.Add(this);

    }
    public override void OnNetworkDespawn()
    {
        if (GameSessionManager.Instance != null)
        {
            GameSessionManager.Instance.activePlayers.OnListChanged -= OnNetworkPlayersChanged;
        }
        base.OnNetworkDespawn();
    }
    void OnNetworkPlayersChanged(NetworkListEvent<PlayerSessionData> changeEvent)
    {
        RefreshLocalData();
    }

    void RefreshLocalData()
    {
        if (GameSessionManager.Instance != null)
        {
            PlayerSessionData? playerDataNull = GameSessionManager.Instance.getPlayerData(OwnerClientId);
            if (playerDataNull is PlayerSessionData data)
            {
                playerData = data;
            }
        }
    }
    public void addCoins(int coins)
    {
        if (IsServer)
        {
            GameSessionManager.Instance.AddCoinsToPlayer(OwnerClientId, coins);
        }
        else
        {
            AddCoinsServerRPC(coins);
        }
    }
    [ServerRpc]
    void AddCoinsServerRPC(int coins)
    {
        GameSessionManager.Instance.AddCoinsToPlayer(OwnerClientId, coins);
    }

    public void addSpacesToMove(int spaces)
    {
        spacesToMove += spaces;
    }
}
