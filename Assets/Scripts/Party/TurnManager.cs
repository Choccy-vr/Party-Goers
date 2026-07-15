using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

public class TurnManager : NetworkBehaviour
{
    public static TurnManager Instance;

    public VRPartyPlayer currentTurnPlayerObj;

    public List<VRPartyPlayer> turnOrderObj;

    private NetworkVariable<ulong> currentTurnPlayerID = new NetworkVariable<ulong>();
    private NetworkList<ulong> turnOrderID = new NetworkList<ulong>();

    public NetworkVariable<int> amountRounds = new NetworkVariable<int>(10);

    public UnityEvent<VRPartyPlayer> onTurnStart;
    public UnityEvent<VRPartyPlayer> onTurnEnd;
    public UnityEvent onRoundEnd;

    int currentPlayerIndex = 0;
    int amountRoundsLeft = 10;

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

    public override void OnNetworkSpawn()
    {
        currentTurnPlayerID.OnValueChanged += OnTurnPlayerIDChanged;
        turnOrderID.OnListChanged += OnTurnOrderIDChanged;

    }
    public override void OnNetworkDespawn()
    {
        currentTurnPlayerID.OnValueChanged -= OnTurnPlayerIDChanged;
        turnOrderID.OnListChanged -= OnTurnOrderIDChanged;

    }


    private void OnTurnPlayerIDChanged(ulong previousValue, ulong newValue)
    {
        updateLocalCurrentObj(newValue);
        Debug.Log("Current Turn Changed");
    }
    private void OnTurnOrderIDChanged(NetworkListEvent<ulong> changeEvent)
    {
        updateLocalOrderObj(turnOrderID);
        Debug.Log("Turn Order Changed");
    }

    VRPartyPlayer GetVRPlayerFromID(ulong id)
    {
        return PlayerManager.Instance.activePlayerObj.Find(p => p.playerData.networkClientId == id);
    }

    void updateLocalCurrentObj(ulong newID)
    {
        currentTurnPlayerObj = GetVRPlayerFromID(newID);
        currentPlayerIndex = turnOrderObj.IndexOf(currentTurnPlayerObj);
        if (newID == NetworkManager.Singleton.LocalClientId)
        {
            nextPlayerTurn();
        }
    }
    void updateLocalOrderObj(NetworkList<ulong> newID)
    {
        for (int i = 0; i < newID.Count; i++)
        {
            turnOrderObj.Insert(i, GetVRPlayerFromID(newID[i]));
        }
    }
    void updateNetworkCurrent(VRPartyPlayer player)
    {
        updateNetworkCurrentServerRpc(player.playerData.networkClientId);
    }

    [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
    void updateNetworkCurrentServerRpc(ulong ID)
    {
        currentTurnPlayerID.Value = ID;
    }
    void updateNetworkOrder(List<VRPartyPlayer> playerOrder)
    {
        List<ulong> idList = new List<ulong>();
        foreach (VRPartyPlayer player in playerOrder)
        {
            idList.Add(player.playerData.networkClientId);
        }
        updateNetworkOrderServerRpc(idList.ToArray());
    }

    [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
    void updateNetworkOrderServerRpc(ulong[] ID)
    {
        turnOrderID.Clear();
        foreach (var id in ID)
        {
            turnOrderID.Add(id);
        }
    }
    [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
    void updateRoundAmountServerRpc(int amount)
    {
        amountRounds.Value = amount;
    }
    public void addPlayerToTurnOrder(VRPartyPlayer player, int order)
    {
        turnOrderObj.Insert(order, player);
        updateNetworkOrder(turnOrderObj);
    }

    public void nextPlayerTurn()
    {
        //Debug.Log("currentPlayerIndex = " + currentPlayerIndex);
        //Debug.Log("activePlayerObj = " + PlayerManager.Instance.activePlayerObj.Count);
        currentTurnPlayerObj = PlayerManager.Instance.activePlayerObj[currentPlayerIndex];
        updateNetworkCurrent(currentTurnPlayerObj);
        onTurnStart?.Invoke(currentTurnPlayerObj);
    }
    public void endPlayerTurn()
    {
        if (!IsSpawned)
        {
            Debug.LogWarning("TurnManager: Cannot end turn because this object is not spawned on the network yet!");
            return;
        }
        Debug.Log("Ending Turn");
        currentPlayerIndex++;
        if (currentPlayerIndex >= PlayerManager.Instance.activePlayerObj.Count)
        {
            currentPlayerIndex = 0;
            updateRoundAmountServerRpc(amountRounds.Value - 1);
            if (amountRounds.Value <= 0)
            {
                //END GAME
            }
            onRoundEnd?.Invoke();
        }

        onTurnEnd?.Invoke(currentTurnPlayerObj);
    }
}
