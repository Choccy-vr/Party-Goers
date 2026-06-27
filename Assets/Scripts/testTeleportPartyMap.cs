using UnityEngine;

public class testTeleportPartyMap : MonoBehaviour
{
    public void teleportPartyMap()
    {
        MinigameManager.Instance.teleportToMap("demo");
    }
}
