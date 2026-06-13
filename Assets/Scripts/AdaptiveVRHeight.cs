using Unity.VisualScripting;
using UnityEngine;

public class AdaptiveVRHeight : MonoBehaviour
{

    [SerializeField] Transform cameraPos;
    CharacterController characterController;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
    }

    void Update()
    {
        characterController.height = cameraPos.localPosition.y;
        characterController.center = new Vector3(characterController.center.x, characterController.height / 2, characterController.center.z);
    }
}
