using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Movement;

public class LocomotionSprint : MonoBehaviour
{
    public float sprintSpeed = 2f;
    [SerializeField] private InputActionProperty sprintAction;

    ContinuousMoveProvider moveProvider;

    bool isSprinting = false;

    float normalSpeed = 0;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        moveProvider = GetComponent<ContinuousMoveProvider>();
    }

    // Update is called once per frame
    void Update()
    {
       bool sprintButton = sprintAction.action.IsPressed();
       if(sprintButton){
        if (!isSprinting)
        {
            normalSpeed = moveProvider.moveSpeed;
            moveProvider.moveSpeed = sprintSpeed;
            isSprinting = true;
        }
        else if (isSprinting)
        {
            moveProvider.moveSpeed = normalSpeed;
            isSprinting = false;
        }
       }
    }
}
