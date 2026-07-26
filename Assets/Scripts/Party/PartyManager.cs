using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class PartyManager : NetworkBehaviour
{
    public static PartyManager Instance;

    [SerializeField] GameObject partyPadPrefab;
    [SerializeField] GameObject itemBoxPrefab;

    public NetworkVariable<int> currentStarSpaceID = new NetworkVariable<int>();

    [Header("Player Start Turn Haptics")]

    [Range(0f, 1f)]
    [SerializeField] float amplitude = 0.5f;
    [SerializeField] float frequency = 140;
    [SerializeField] float duration = 0.25f;

    [Header("PartyPad Spawn")]
    [SerializeField] InputActionProperty partyPadSpawnAction;

    [Header("Player Spawn")]
    public Transform[] playerSpawnPoints;

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
        DontDestroyOnLoad(gameObject);
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
            DestoryPartyPad();
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
    public void landedOnItemSpace(Transform spaceTransform)
    {
        if (itemBoxPrefab == null) return;

        // Grab camera height (fallback to space height if camera is somehow missing)
        float targetHeight = (Camera.main != null) ? Camera.main.transform.position.y - 0.25f : spaceTransform.position.y + 1.2f;

        // 1. Get flat forward from the space
        Vector3 spaceForward = Vector3.ProjectOnPlane(spaceTransform.forward, Vector3.up).normalized;
        Vector3 spaceRight = Vector3.ProjectOnPlane(spaceTransform.right, Vector3.up).normalized;

        // 2. Center position (Space's X/Z + Camera's Y)
        Vector3 centerSpawnPosition = spaceTransform.position + (spaceForward * 0.75f);
        centerSpawnPosition.y = targetHeight; // Apply the chest-level height

        // 3. Flip 180 degrees so the front faces the player looking at it
        Quaternion spawnRotation = Quaternion.LookRotation(-spaceForward);

        float spacing = 0.35f;

        for (int i = -1; i <= 1; i++)
        {
            Vector3 spawnPosition = centerSpawnPosition + (spaceRight * (i * spacing));
            Instantiate(itemBoxPrefab, spawnPosition, spawnRotation);
        }
    }

    public void landedOnShopSpace()
    {
        foreach (ShopProvider shop in MapManager.Instance.shops)
        {
            shop.spawnItems();
        }
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

    public void DestoryPartyPad()
    {
        Debug.Log($"Is Scene Object: {partyPadObject.scene.IsValid()} | Name: {partyPadObject.name}");
        Debug.Log("Destroying Party Pad");
        Destroy(partyPadObject);
        partyPadObject = null;
    }

    public void diceRolled(VRPartyPlayer player)
    {
        getSpaceObj(player.currentSpaceId).unlockNextSpaces();
    }
    public void endPlayerTurn(VRPartyPlayer player)
    {
        Destroy(currentDiceObject);
    }

    public void startGame()
    {
        if (!IsServer)
        {
            StartGameServerRpc();
            return;
        }

        StartGameInternal();
    }

    [ServerRpc(RequireOwnership = false)]
    void StartGameServerRpc()
    {
        StartGameInternal();
    }

    void StartGameInternal()
    {
        Debug.Log("STARTING NEW GAME");
        TurnManager.Instance.nextPlayerTurn();
        ChangeStarSpace();
        TeleportPlayersToSpawnPoints();
        PlayerConfigManager.Instance.setNewPlayerConfig(MapManager.Instance.currentMap.playerConfig);
    }

    void TeleportPlayersToSpawnPoints()
    {
        if (!IsServer) return;

        var connectedClients = NetworkManager.Singleton.ConnectedClientsList;

        for (int i = 0; i < connectedClients.Count; i++)
        {
            ulong clientId = connectedClients[i].ClientId;

            if (i >= playerSpawnPoints.Length)
            {
                Debug.LogWarning($"Not enough spawn points for client {clientId}.");
                continue;
            }

            VRPartyPlayer player = connectedClients[i].PlayerObject != null
                ? connectedClients[i].PlayerObject.GetComponent<VRPartyPlayer>()
                : null;

            if (player == null)
            {
                Debug.LogWarning($"No VRPartyPlayer found for client {clientId}.");
                continue;
            }

            int spawnIndex = (int)(clientId % (ulong)playerSpawnPoints.Length);
            Transform spawn = playerSpawnPoints[spawnIndex];

            var rpcParams = new ClientRpcParams
            {
                Send = new ClientRpcSendParams
                {
                    TargetClientIds = new ulong[] { clientId }
                }
            };

            player.TeleportPlayerClientRpc(spawn.position, spawn.rotation, rpcParams);
        }
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
