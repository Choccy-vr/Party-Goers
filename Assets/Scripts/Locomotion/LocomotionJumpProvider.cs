using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit.Inputs.Haptics;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Gravity;

public class LocomotionJumpProvider : MonoBehaviour
{
    [SerializeField] CharacterController characterController;
    [SerializeField] GravityProvider gravityProvider;
    [SerializeField] InputActionProperty jumpAction;

    [Header("Jump Settings")]
    public float jumpHeight = 2.5f;
    public float upwardGravity = -15f;
    public float downwardGravity = -35;
    public float coyoteTimeDuration = 0.15f;
    private float coyoteTimer;

    [Header("Events")]
    public UnityEvent onJumpStart;
    public UnityEvent onJumpEnd;

    Vector3 jumpVelocity;
    bool isJumping = false;

    void Update()
    {
        if (gravityProvider.enabled && gravityProvider.isGrounded)
        {
            coyoteTimer = coyoteTimeDuration;
        }
        else
        {
            coyoteTimer -= Time.deltaTime;
        }

        if (!isJumping && coyoteTimer > 0 && jumpAction.action.WasPerformedThisFrame())
        {
            StartJump();
        }

        if (isJumping)
        {
            ExecuteJump();
        }
    }

    void StartJump()
    {
        isJumping = true;
        coyoteTimer = 0;
        gravityProvider.enabled = false;

        jumpVelocity.y = Mathf.Sqrt(jumpHeight * -2f * upwardGravity);

        onJumpStart?.Invoke();
    }

    void ExecuteJump()
    {
        if (jumpVelocity.y > 0)
        {
            jumpVelocity.y += upwardGravity * Time.deltaTime;
        }
        else
        {
            jumpVelocity.y += downwardGravity * Time.deltaTime;
        }

        characterController.Move(jumpVelocity * Time.deltaTime);

        if (jumpVelocity.y < 0 && characterController.isGrounded)
        {
            StopJump();
        }
    }

    void StopJump()
    {
        isJumping = false;
        jumpVelocity = Vector3.zero;

        gravityProvider.enabled = true;

        onJumpEnd?.Invoke();
    }
}
