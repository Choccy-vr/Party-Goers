using UnityEngine;

public class NetworkIdenity : MonoBehaviour
{
    public static NetworkIdenity Instance { get; private set; }

    public GameObject networkPlayerIdenity;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    static void ResetStatics()
    {
        Instance = null;
    }
}
