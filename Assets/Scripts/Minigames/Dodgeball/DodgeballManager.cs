using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class DodgeballManager : NetworkBehaviour
{

    public static DodgeballManager Instance;

    [Header("Dodgeball Settings")]
    [SerializeField] float matchDuration = 60f;
    [SerializeField] Transform[] playerSpawnPoints;
    [SerializeField] Transform[] spectatorSpawnPoints;
    [SerializeField] PlayerConfig playerConfig;
    [SerializeField] GameObject dodgeballPrefab;
    [SerializeField] Transform[] dodgeballSpawnPoints;
    // Track active players remaining on the court
    private NetworkList<ulong> activePlayerIds;
    // Track order of elimination to determine 1st, 2nd, 3rd, 4th place
    private List<ulong> eliminationOrder = new List<ulong>();

    private NetworkVariable<float> timeRemaining = new NetworkVariable<float>();
    private bool isGameActive = false;

    private void Awake()
    {
        activePlayerIds = new NetworkList<ulong>();
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

    }

    void Start()
    {
        playerSpawnPoints = DodgeballRefs.Instance.playerSpawnPoints;
        spectatorSpawnPoints = DodgeballRefs.Instance.spectatorSpawnPoints;
        dodgeballSpawnPoints = DodgeballRefs.Instance.dodgeballSpawnPoints;
    }

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            foreach (var client in NetworkManager.Singleton.ConnectedClientsList)
            {
                activePlayerIds.Add(client.ClientId);
            }

            StartCoroutine(DodgeballLifecycleRoutine());
        }
    }

    private IEnumerator DodgeballLifecycleRoutine()
    {
        Debug.Log("Waiting for players to teleport...");

        // Give the clients 1.5 seconds to finish their teleport RPCs and initialize
        yield return new WaitForSeconds(1.5f);

        Debug.Log("Starting Dodgeball game");

        // NOW it is safe to apply configs and spawn balls!
        SpawnDodgeballs();
        SetPlayerConfig();


        timeRemaining.Value = matchDuration;
        isGameActive = true;

        while (timeRemaining.Value > 0f && activePlayerIds.Count > 1 && isGameActive)
        {
            Debug.Log("Game still running!");
            timeRemaining.Value -= Time.deltaTime;
            yield return null;
        }

        isGameActive = false;
        EndDodgeballMatch();
    }

    [ServerRpc(RequireOwnership = false)]
    public void EliminatePlayerServerRpc(ulong hitClientId)
    {
        if (!activePlayerIds.Contains(hitClientId)) return;

        activePlayerIds.Remove(hitClientId);
        eliminationOrder.Add(hitClientId);

        // Get the hit player's NetworkObject
        if (NetworkManager.Singleton.ConnectedClients.TryGetValue(hitClientId, out var client))
        {
            VRPartyPlayer player = client.PlayerObject.GetComponent<VRPartyPlayer>();

            // Pick an spectator spawn point off-court
            Vector3 spectatorPos = spectatorSpawnPoints[eliminationOrder.Count - 1].position;
            Quaternion spectatorRot = spectatorSpawnPoints[eliminationOrder.Count - 1].rotation;

            var rpcParams = new ClientRpcParams
            {
                Send = new ClientRpcSendParams
                {
                    TargetClientIds = new ulong[] { player.OwnerClientId }
                }
            };

            player.TeleportPlayerClientRpc(spectatorPos, spectatorRot, rpcParams);
        }
    }

    void SetPlayerConfig()
    {
        PlayerConfigManager.Instance.playerConfig = playerConfig;
        PlayerConfigManager.Instance.applyPlayerConfig();
    }

    private void EndDodgeballMatch()
    {
        // Any remaining active players get top placement
        foreach (ulong remainingId in activePlayerIds)
        {
            eliminationOrder.Add(remainingId);
        }

        Dictionary<ulong, int> finalPlacings = CalculatePlacings(eliminationOrder);

        MinigameManager.Instance.endMinigame(finalPlacings);
    }

    void SpawnDodgeballs()
    {
        Debug.Log("Spawning Dodgeballs");
        foreach (Transform spawnPoint in dodgeballSpawnPoints)
        {
            GameObject dodgeball = Instantiate(dodgeballPrefab, spawnPoint.position, spawnPoint.rotation);
            dodgeball.GetComponent<NetworkObject>().Spawn();

        }
    }

    private Dictionary<ulong, int> CalculatePlacings(List<ulong> order)
    {
        Dictionary<ulong, int> results = new Dictionary<ulong, int>();

        // Zero-based placings: 0 = 1st place, 3 = 4th place
        for (int i = 0; i < order.Count; i++)
        {
            ulong id = order[i];
            results[id] = i;
        }

        return results;
    }
}

