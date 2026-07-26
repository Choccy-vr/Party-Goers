using UnityEngine;

public class DodgeballRefs : MonoBehaviour
{
    public static DodgeballRefs Instance;

    public Transform[] playerSpawnPoints;
    public Transform[] spectatorSpawnPoints;
    public Transform[] dodgeballSpawnPoints;
    public Collider[] courtColliders;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
}
