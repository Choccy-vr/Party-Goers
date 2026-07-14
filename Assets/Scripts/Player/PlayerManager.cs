using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance;
    public List<VRPartyPlayer> activePlayerObj;

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
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    static void ResetStatics()
    {
        Instance = null;
    }
    public VRPartyPlayer FindPlayerFromID(ulong networkClientID)
    {
        return activePlayerObj.Find(p => p.playerData.networkClientId == networkClientID);
    }
    public List<VRPartyPlayer> getAllOtherPlayers()
    {
        var otherPlayers = new List<VRPartyPlayer>(activePlayerObj);
        otherPlayers.Remove(NetworkIdenity.Instance.networkPlayerIdenity.GetComponent<VRPartyPlayer>());
        return otherPlayers;
    }
}
