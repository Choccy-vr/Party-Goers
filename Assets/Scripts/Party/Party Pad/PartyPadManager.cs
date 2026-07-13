using UnityEngine;

public class PartyPadManager : MonoBehaviour
{
    public static PartyPadManager Instance { get; private set; }

    [SerializeField] GameObject currentTurnScreen;
    [SerializeField] GameObject starScreen;
    [SerializeField] GameObject playerStanding;

    GameObject currentActiveScreen;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    static void ResetStatics()
    {
        Instance = null;
    }

    void setActiveScreen(GameObject screen)
    {
        if (currentActiveScreen != null)
        {
            currentActiveScreen.SetActive(false);
        }
        screen.SetActive(true);
        currentActiveScreen = screen;
    }

    public void setCurrentPlayerTurnUI()
    {
        setActiveScreen(currentTurnScreen);
        currentTurnScreen.GetComponent<CurrentTurnScreenHelper>().updateItemsForPlayer();
    }
    public void setStarUI()
    {
        setActiveScreen(starScreen);
    }
    public void setPlayerStandingOverlay(bool isActive)
    {
        playerStanding.SetActive(isActive);
    }
}
