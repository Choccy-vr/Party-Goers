using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MapManager : MonoBehaviour
{

    public static MapManager Instance { get; private set; }

    public List<MapConfig> maps = new List<MapConfig>();
    [HideInInspector] public List<PartySpace> partySpaces;

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
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    public PartySpace findPartySpaceWithID(int ID)
    {
        return partySpaces.Find(s => s.spaceID == ID);
    }
    public MapConfig findPartyMapWithID(string ID)
    {
        return maps.Find(m => m.mapID == ID);
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
    {
        partySpaces.Clear();

        PartySpace[] foundSpaces = FindObjectsByType<PartySpace>();

        partySpaces.AddRange(foundSpaces);
        Debug.Log($"Restored {foundSpaces.Length} PartySpaces");
    }
}
