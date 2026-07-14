using UnityEngine;

public class DuelButtonHelper : MonoBehaviour
{
    public VRPartyPlayer player;

    public void challengePlayerToDuel()
    {
        if (player == null) { return; }
        PartyManager.Instance.StartDuel(NetworkIdenity.Instance.networkPlayerIdenity.GetComponent<VRPartyPlayer>(), player);
    }
}
