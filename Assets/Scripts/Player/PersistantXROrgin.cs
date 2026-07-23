using UnityEngine;

public class PersistantXROrgin : MonoBehaviour
{
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

}
