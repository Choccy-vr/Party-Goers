using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class MinigameManager : NetworkBehaviour
{
    public static MinigameManager Instance;

    public List<MinigameConfig> minigames;

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

    public void teleportToMinigame(MinigameConfig targetMinigame)
    {
        if (!IsServer) return;
        NetworkSceneManager.Instance.LoadSceneNetwork(targetMinigame.sceneName);
        //Apply player configs
    }
    public void teleportToMap(MapConfig targetMap)
    {
        if (!IsServer) return;
        NetworkSceneManager.Instance.LoadSceneNetwork(targetMap.sceneName);
        //Apply player config
    }

    public MinigameConfig selectRandomMinigame(MinigameType type)
    {
        List<MinigameConfig> filteredMinigames = minigames.FindAll(m => m.minigameType == type);
        int randomIndex = Random.Range(0, filteredMinigames.Count);
        return filteredMinigames[randomIndex];
    }
}
