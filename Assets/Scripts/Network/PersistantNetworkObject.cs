using Unity.Netcode;
using UnityEngine;

public class PersistantNetworkObject : NetworkBehaviour
{

    public override void OnNetworkSpawn()
    {
        DontDestroyOnLoad(gameObject);
    }

    public void PrepareForSceneChange()
    {
        if (IsServer)
        {
            NetworkObject.Despawn(false);
        }
    }
}

