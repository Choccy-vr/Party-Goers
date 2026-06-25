using UnityEngine;
using UnityEngine.SceneManagement;

public class MinigameManager : MonoBehaviour
{
    public static MinigameManager Instance;

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
    public void switchToTestMinigameScene()
    {
        NetworkSceneManager.Instance.LoadSceneNetwork("DemoMinigameScene");
    }
    public void switchToDemoPartyScene()
    {
        NetworkSceneManager.Instance.LoadSceneNetwork("DemoParty");
    }
}
