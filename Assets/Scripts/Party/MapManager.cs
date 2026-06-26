using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{

    public static MapManager Instance { get; private set; }

    public List<PartySpace> partySpaces;

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

    public PartySpace findPartySpaceWithID(int ID)
    {
        return partySpaces.Find(s => s.spaceID == ID);
    }
}
