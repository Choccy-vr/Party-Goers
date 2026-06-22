using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

public class TurnManager : NetworkBehaviour
{
    public static TurnManager Instance;
    private VRPartyPlayer _currentTurnPlayerObj;
    private List<VRPartyPlayer> _turnOrderObj = new List<VRPartyPlayer>();

    public VRPartyPlayer currentTurnPlayerObj
    {
        get => _currentTurnPlayerObj;
        set
        {
            if (value != _currentTurnPlayerObj)
            {
                _currentTurnPlayerObj = value;
                onLocalPlayerObjChanged(value);
            }
        }
    }

    public List<VRPartyPlayer> turnOrderObj
    {
        get => _turnOrderObj;
        set
        {
            if (value != _turnOrderObj)
            {
                _turnOrderObj = value;
                onLocalOrderObjChanged(value);
            }
        }
    }

    private NetworkVariable<ulong> currentTurnPlayerID = new NetworkVariable<ulong>();
    private NetworkList<ulong> turnOrderID = new NetworkList<ulong>();

    public int amountRounds = 10;

    public UnityEvent<VRPartyPlayer> onTurnStart;
    public UnityEvent<VRPartyPlayer> onTurnEnd;
    public UnityEvent onRoundEnd;

    int currentPlayerIndex = 0;

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
    }
    private void OnTurnOrderIDChanged(NetworkListEvent<ulong> changeEvent)
    {
        updateLocalOrderObj(turnOrderID);
    }

    VRPartyPlayer GetVRPlayerFromID(ulong id)
    {
        return PlayerManager.Instance.activePlayerObj.Find(p => p.playerData.networkClientId == id);
    }

    void updateLocalCurrentObj(ulong newID)
    {
        currentTurnPlayerObj = GetVRPlayerFromID(newID);
    }
    void updateLocalOrderObj(NetworkList<ulong> newID)
    {
        for (int i = 0; i < newID.Count; i++)
        {
            turnOrderObj.Insert(i, GetVRPlayerFromID(newID[i]));
        }
    }

    void onLocalPlayerObjChanged(VRPartyPlayer player)
    {
        if (!IsServer) return;
        currentTurnPlayerID.Value = player.playerData.networkClientId;
    }
    void onLocalOrderObjChanged(List<VRPartyPlayer> order)
    {
        if (!IsServer) return;
        for (int i = 0; i < order.Count; i++)
        {
            turnOrderID.Insert(i, order[i].playerData.networkClientId);
        }
    }
    public void addPlayerToTurnOrder(VRPartyPlayer player, int order)
    {
        turnOrderObj.Insert(order, player);
    }


    public void nextPlayerTurn()
    {
        currentTurnPlayerObj = PlayerManager.Instance.activePlayerObj[currentPlayerIndex];
        onTurnStart?.Invoke(currentTurnPlayerObj);
    }
    public void endPlayerTurn()
    {
        currentPlayerIndex++;
        if (currentPlayerIndex >= 4)
        {
            currentPlayerIndex = 0;
            amountRounds--;
            if (amountRounds <= 0)
            {
                //END GAME
            }
            onRoundEnd?.Invoke();
        }
        onTurnEnd?.Invoke(currentTurnPlayerObj);
    }
}
