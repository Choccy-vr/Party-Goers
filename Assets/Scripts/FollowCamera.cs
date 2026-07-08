using UnityEngine;

public class FollowCamera : MonoBehaviour
{

    [SerializeField] Camera headCamera;
    [SerializeField] bool constrainObject = false;

    void Update()
    {
        if (!constrainObject)
        {
            gameObject.transform.position = headCamera.gameObject.transform.position;
            gameObject.transform.rotation = headCamera.gameObject.transform.rotation;
        }
        else
        {
            gameObject.transform.rotation = new Quaternion(headCamera.gameObject.transform.rotation.x, 0, headCamera.gameObject.transform.rotation.z, headCamera.gameObject.transform.rotation.w);
        }

    }
}
