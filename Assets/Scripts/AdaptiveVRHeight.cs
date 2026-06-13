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
    }
}
