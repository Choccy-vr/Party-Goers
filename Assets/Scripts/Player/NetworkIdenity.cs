using UnityEngine;

public class NetworkIdenity : MonoBehaviour
{
    public static NetworkIdenity Instance { get; private set; }

    public GameObject networkPlayerIdenity { get; set; }

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }
}
