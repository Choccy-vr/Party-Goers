using Unity.Netcode;
using UnityEngine;
public class VRPartyPlayer : NetworkBehaviour
{
    public int currentSpaceId;
    public PlayerSessionData playerData { get; private set; }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (GameSessionManager.Instance != null)
        {
            playerData = GameSessionManager.Instance.getPlayerData(OwnerClientId);
        }
        if (playerData != null)
        {
            Debug.Log("State Restored!!!!!!");
        }
    }

    public void addCoins(int coins)
    {
        playerData.coins += coins;
        Debug.Log(playerData.coins);
    }
}
