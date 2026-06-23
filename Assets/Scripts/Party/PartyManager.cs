using Unity.Netcode;
using UnityEngine;

public class PartyManager : MonoBehaviour
{
    public static PartyManager Instance;

    [SerializeField] GameObject dicePrefab;

    GameObject currentDiceObject;

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

    public void startPlayerTurn(VRPartyPlayer player)
    {

        if (player == null)
        {
            Debug.LogError("PartyManager: Cannot start turn because the 'player' passed in is NULL!");
            return;
        }

        if (dicePrefab == null)
        {
            Debug.LogError("PartyManager: 'dicePrefab' is not assigned in the Inspector!");
            return;
        }
        Vector3 spawnPosition = player.transform.position + (player.transform.forward * 2) + Vector3.up;

        currentDiceObject = Instantiate(dicePrefab, spawnPosition, dicePrefab.transform.rotation);

        Debug.Log("Starting " + player.playerData.username + "'s turn");

    }
    public void endPlayerTurn(VRPartyPlayer player)
    {
        Destroy(currentDiceObject);
    }


}
