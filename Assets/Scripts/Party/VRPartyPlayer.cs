using Unity.Netcode;
using Unity.VisualScripting;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.SceneManagement;
public class VRPartyPlayer : NetworkBehaviour
{
    public int currentSpaceId;
    public int spacesToMove = 0;
    public PlayerSessionData playerData { get; private set; }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
    {
        LinkNetworkPlayer();
    }

    void LinkNetworkPlayer()
    {
        if (IsOwner)
        {
            NetworkIdenity.Instance.networkPlayerIdenity = gameObject;
        }
    }

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
            AddCoinsServerRpc(coins);
        }
    }
    public void addItem(ItemConfig item)
    {
        if (IsServer)
        {
            GameSessionManager.Instance.AddItemToPlayer(OwnerClientId, item.itemID);
        }
        else
        {
            AddItemServerRpc(item.itemID);
        }
    }

    [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
    void AddCoinsServerRpc(int coins, RpcParams rpcParams = default)
    {
        ulong actualSenderId = rpcParams.Receive.SenderClientId;

        GameSessionManager.Instance.AddCoinsToPlayer(actualSenderId, coins);
    }

    [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
    void AddItemServerRpc(string itemID, RpcParams rpcParams = default)
    {
        ulong actualSenderId = rpcParams.Receive.SenderClientId;

        GameSessionManager.Instance.AddItemToPlayer(actualSenderId, itemID);
    }


    public void addSpacesToMove(int spaces)
    {
        spacesToMove += spaces;
    }

    // Call this ClientRpc from the Server when someone is eliminated or spawned
    [ClientRpc]
    public void TeleportPlayerClientRpc(Vector3 newPosition, Quaternion newRotation, ClientRpcParams clientRpcParams = default)
    {
        if (!IsOwner)
        {
            return;
        }

        Debug.Log("Teleporting Player");

        XROrigin origin = FindAnyObjectByType<XROrigin>();
        if (origin != null)
        {
            origin.MoveCameraToWorldLocation(newPosition);
            origin.transform.rotation = newRotation;
        }
        else
        {
            Debug.LogError("XR Origin is null");
        }
    }
}
