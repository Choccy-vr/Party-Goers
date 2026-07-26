using UnityEngine;
using System.Collections;
public class PlayerSpawnPointsInit : MonoBehaviour
{
    [SerializeField] Transform[] playerSpawnPoints;

    private IEnumerator Start()
    {
        // Wait until PartyManager.Instance exists and is spawned on the network
        yield return new WaitUntil(() => PartyManager.Instance != null && PartyManager.Instance.IsSpawned);

        // Now it's safe to assign
        PartyManager.Instance.playerSpawnPoints = playerSpawnPoints;
        Debug.Log("Successfully assigned spawn points to PartyManager!");
    }
}
