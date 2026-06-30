using UnityEngine;

public class MinigameSpawnPoint : MonoBehaviour
{
    [HideInInspector] public Transform spawnTransform;
    public int spawnPointID = 0;

    void Awake()
    {
        if (spawnTransform == null)
        {
            spawnTransform = GetComponent<Transform>();
        }
        if (MinigameManager.Instance != null)
        {
            MinigameManager.Instance.minigameSpawnPoints.Add(this);
        }


    }
}
