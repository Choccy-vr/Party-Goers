using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class DodgeballManager : NetworkBehaviour
{
    [Header("Dodgeball Settings")]
    [SerializeField] float matchDuration = 60f;
    [SerializeField] Transform[] playerSpawnPoints;

    // Track active players remaining on the court
    private NetworkList<ulong> activePlayerIds;
    // Track order of elimination to determine 1st, 2nd, 3rd, 4th place
    private List<ulong> eliminationOrder = new List<ulong>();

    private NetworkVariable<float> timeRemaining = new NetworkVariable<float>();
    private bool isGameActive = false;

    private void Awake()
    {
        activePlayerIds = new NetworkList<ulong>();
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
        TeleportPlayersToSpawns();

        timeRemaining.Value = matchDuration;
        isGameActive = true;

        while (timeRemaining.Value > 0f && activePlayerIds.Count > 1 && isGameActive)
        {
            timeRemaining.Value -= Time.deltaTime;
            yield return null;
        }

        isGameActive = false;
        EndDodgeballMatch();
    }

    [ServerRpc(RequireOwnership = false)]
    public void EliminatePlayerServerRpc(ulong hitClientId)
    {
        if (!isGameActive || !activePlayerIds.Contains(hitClientId)) return;

        activePlayerIds.Remove(hitClientId);
        eliminationOrder.Add(hitClientId);

        NotifyPlayerEliminatedClientRpc(hitClientId);

        // Instant end if only 1 player remains
        if (activePlayerIds.Count <= 1)
        {
            isGameActive = false;
        }
    }

    private void EndDodgeballMatch()
    {
        // Any remaining active players get top placement
        foreach (ulong remainingId in activePlayerIds)
        {
            eliminationOrder.Add(remainingId);
        }

        // Convert the elimination list into final placings / scores
        // (1st place gets highest points/coins)
        Dictionary<ulong, int> finalPlacings = CalculatePlacings(eliminationOrder);

        // HAND-OFF: Pass results back to the central Orchestrator!
        MinigameManager.Instance.endMinigame(finalPlacings);
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

    // Helper Spawners
    private void TeleportPlayersToSpawns() { /* Position VR rigs at spawnPoints */ }

    [ClientRpc]
    private void NotifyPlayerEliminatedClientRpc(ulong targetClientId) { /* Play haptics / disable ball grabs for eliminated player */ }
}

