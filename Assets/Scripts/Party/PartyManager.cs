using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class PartyManager : NetworkBehaviour
{
    public static PartyManager Instance;

    [SerializeField] GameObject partyPadPrefab;

    public NetworkVariable<int> currentStarSpaceID = new NetworkVariable<int>();

    [Header("Player Start Turn Haptics")]

    [Range(0f, 1f)]
    [SerializeField] float amplitude = 0.5f;
    [SerializeField] float frequency = 140;
    [SerializeField] float duration = 0.25f;

    [Header("PartyPad Spawn")]
    [SerializeField] InputActionProperty partyPadSpawnAction;

    GameObject currentDiceObject;
    PartySpace currentStarSpace;
    GameObject partyPadObject;

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
        currentStarSpaceID.OnValueChanged += OnNetworkStarSpaceChange;
    }
    public override void OnNetworkDespawn()
    {
        currentStarSpaceID.OnValueChanged -= OnNetworkStarSpaceChange;
    }

    void OnNetworkStarSpaceChange(int previousValue, int newValue)
    {
        if (currentStarSpace == null || currentStarSpace.spaceID != newValue)
        {
            SetSpaceStar(newValue);
            Debug.Log("Changed Star Space to " + newValue);
        }
    }
    void Start()
    {
        partyPadSpawnAction.action.performed += partyPadSpawnActionPerformed;
    }

    private void partyPadSpawnActionPerformed(InputAction.CallbackContext obj)
    {
        if (partyPadObject == null)
        {
            SpawnPartyPad();
            partyPadObject.GetComponent<PartyPadManager>().setPartyProgressUI();
        }
        else
        {
            Destroy(partyPadObject);
        }

    }

    public void LandOnStarSpace()
    {
        SpawnPartyPad();
        partyPadObject.GetComponent<PartyPadManager>().setStarUI();
    }

    public void purchaseStar(ulong clientId)
    {
        if (!IsServer)
        {
            Debug.Log("Not Server. Calling rpc");
            PurchaseStarServerRpc(clientId);
            return;
        }
        GameSessionManager.Instance.AddCoinsToPlayer(clientId, -3);
        GameSessionManager.Instance.AddStarToPlayer(clientId);
        ChangeStarSpace();
    }

    [ServerRpc]
    void PurchaseStarServerRpc(ulong clientId)
    {
        purchaseStar(clientId);
    }

    void ChangeStarSpace()
    {
        if (!IsServer)
        {
            Debug.Log("Not Server. Transfering to rpc");
            ChangeStarSpaceServerRpc();
            return;
        }

        if (currentStarSpace != null)
        {
            currentStarSpace.revertStarSpace();
        }
        PartySpace newStarSpace = getRandomSpaceObj();
        currentStarSpaceID.Value = newStarSpace.spaceID;
        Debug.Log("Changed Star Space to " + currentStarSpace.spaceID);
    }
    [ServerRpc]
    void ChangeStarSpaceServerRpc()
    {
        ChangeStarSpace();
    }
    public void landedOnDuelSpace()
    {
        SpawnPartyPad();
        partyPadObject.GetComponent<PartyPadManager>().setDuelUI();
    }

    public void StartDuel(VRPartyPlayer host, VRPartyPlayer recipient)
    {
        MinigameManager.Instance.startRandomDuelMinigame();
    }

    void SetSpaceStar(int newID)
    {
        if (currentStarSpace != null)
        {
            currentStarSpace.type = PartySpaceType.normal;
        }
        currentStarSpace = getSpaceObj(newID);
        currentStarSpace.setSpaceToStar();
    }

    public void startPlayerTurn(VRPartyPlayer player)
    {

        if (player == null)
        {
            Debug.LogError("PartyManager: Cannot start turn because the 'player' passed in is NULL!");
            return;
        }
        Debug.Log("Starting " + player.playerData.username + "'s turn");
        if (player == NetworkIdenity.Instance.networkPlayerIdenity.GetComponent<VRPartyPlayer>() && NetworkIdenity.Instance.gameObject.GetComponent<PlayerHapticManager>() != null)
        {
            NetworkIdenity.Instance.gameObject.GetComponent<PlayerHapticManager>().sendHaptic(amplitude, frequency, duration);
            SpawnPartyPad();
            partyPadObject.GetComponent<PartyPadManager>().setCurrentPlayerTurnUI();
        }
    }

    public void SpawnPartyPad()
    {
        if (partyPadObject == null)
        {
            partyPadObject = Instantiate(partyPadPrefab);
        }
        else if (!partyPadObject.activeSelf)
        {
            partyPadObject.SetActive(true);
        }
    }

    public void diceRolled(VRPartyPlayer player)
    {
        getSpaceObj(player.currentSpaceId).unlockNextSpaces();
        Destroy(partyPadObject);
    }
    public void endPlayerTurn(VRPartyPlayer player)
    {
        Destroy(currentDiceObject);
        if (player == NetworkIdenity.Instance.networkPlayerIdenity.GetComponent<VRPartyPlayer>() && partyPadObject != null)
        {
            Destroy(partyPadObject);
        }
    }

    public void startGame()
    {
        Debug.Log("STARTING NEW GAME");
        TurnManager.Instance.nextPlayerTurn();
        ChangeStarSpace();
        PlayerConfigManager.Instance.setNewPlayerConfig(MapManager.Instance.currentMap.playerConfig);
    }

    PartySpace getSpaceObj(int spaceID)
    {
        return MapManager.Instance.partySpaces.Find(s => s.spaceID == spaceID);
    }
    PartySpace getRandomSpaceObj()
    {
        return MapManager.Instance.partySpaces[Random.Range(0, MapManager.Instance.partySpaces.Count)];
    }


}
