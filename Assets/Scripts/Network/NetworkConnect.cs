using UnityEngine;
using Unity.Netcode;
public class NetworkConnect : MonoBehaviour
{

    private void Start()
    {
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.OnClientConnectedCallback += OnPlayerJoinedSession;
        }
    }
    public void Create()
    {
        NetworkManager.Singleton.StartHost();
    }
    public void Join()
    {
        NetworkManager.Singleton.StartClient();
    }

    private void OnPlayerJoinedSession(ulong clientId)
    {
        if (!NetworkManager.Singleton.IsServer) return;

        string playerName = $"Player_{clientId}";

        GameSessionManager.Instance.RegisterPlayer(clientId, playerName);

        Debug.Log($"Player registered! ClientID: {clientId}");
    }

    private void OnDestroy()
    {
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.OnClientConnectedCallback -= OnPlayerJoinedSession;
        }
    }
}
