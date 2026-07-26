using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class LobbyUIHandler : MonoBehaviour
{
    [SerializeField] GameObject createButton;
    [SerializeField] GameObject joinButton;
    [SerializeField] GameObject startButton;
    Button startButtonButton;

    enum LobbyButtonState
    {
        NotConnected,
        Host,
        Client
    }

    LobbyButtonState currentState = LobbyButtonState.NotConnected;

    void Start()
    {
        RefreshButtonVisibility();
    }

    void Update()
    {
        RefreshButtonVisibility();
    }

    private void Awake()
    {
        startButtonButton = startButton.GetComponent<Button>();
        startButtonButton.onClick.AddListener(OnButtonClicked);
    }

    private void OnDestroy()
    {
        if (startButtonButton != null)
        {
            startButtonButton.onClick.RemoveListener(OnButtonClicked);
        }
    }

    private void OnButtonClicked()
    {
        if (PartyManager.Instance != null)
        {
            PartyManager.Instance.startGame();
        }
        else
        {
            Debug.LogError("PartyManager not found in scene!");
        }
    }

    void RefreshButtonVisibility()
    {
        if (createButton == null || joinButton == null || startButton == null)
        {
            return;
        }

        if (NetworkManager.Singleton == null)
        {
            SetState(LobbyButtonState.NotConnected);
            return;
        }

        if (!NetworkManager.Singleton.IsConnectedClient)
        {
            SetState(LobbyButtonState.NotConnected);
            return;
        }

        if (NetworkManager.Singleton.IsHost)
        {
            SetState(LobbyButtonState.Host);
            return;
        }

        SetState(LobbyButtonState.Client);
    }

    void SetState(LobbyButtonState state)
    {
        if (currentState == state)
        {
            return;
        }

        currentState = state;

        switch (state)
        {
            case LobbyButtonState.NotConnected:
                createButton.SetActive(true);
                joinButton.SetActive(true);
                startButton.SetActive(false);
                break;
            case LobbyButtonState.Host:
                createButton.SetActive(false);
                joinButton.SetActive(false);
                startButton.SetActive(true);
                break;
            case LobbyButtonState.Client:
                createButton.SetActive(false);
                joinButton.SetActive(false);
                startButton.SetActive(false);
                break;
        }
    }
}
