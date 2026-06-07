using UnityEngine;

public class FollowCamera : MonoBehaviour
{

    [SerializeField] Camera headCamera;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        gameObject.transform.position = headCamera.gameObject.transform.position;
        gameObject.transform.rotation = headCamera.gameObject.transform.rotation;
    }
}
