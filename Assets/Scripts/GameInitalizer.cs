using Unity.Netcode;
using UnityEngine;

public class GameInitializer : NetworkBehaviour
{
    public GameObject[] managerPrefabs;

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            foreach (var prefab in managerPrefabs)
            {
                GameObject managerInstance = Instantiate(prefab);
                managerInstance.GetComponent<NetworkObject>().Spawn();
            }
        }
    }
}
