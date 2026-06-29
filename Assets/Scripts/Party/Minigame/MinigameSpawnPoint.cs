using UnityEngine;

public class MinigameSpawnPoint : MonoBehaviour
{
    [HideInInspector] public Transform spawnTransform;
    [SerializeField] int spawnPointID = 0;

    void Awake()
    {
        if (spawnTransform == null)
        {
            spawnTransform = GetComponent<Transform>();
        }
        MinigameManager.Instance.minigameSpawnPoints.Insert(spawnPointID, this);

    }
}
