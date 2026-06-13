using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Gravity;

public class LocomotionDashProvider : MonoBehaviour
{

    [SerializeField] CharacterController characterController;
    [SerializeField] InputActionProperty dashAction;
    [SerializeField] InputActionProperty forwardSource;
    [SerializeField] LayerMask groundLayer;

    [Header("Dash Properties")]
    [SerializeField] float dashForce;
    [SerializeField] float dashCooldown;
    [SerializeField] float airFriction;
    [SerializeField] float groundFriction;
    [SerializeField] float baseFrictionMultiplier;

    [Header("Manual Friction")]
    [SerializeField] bool manualFriction;
    [SerializeField] float fricitonValue;

    [Header("Events")]
    public UnityEvent onDashStart;
    public UnityEvent onDashEnd;

    Vector3 dashingVelocity;
    bool isDashing = false;
    float cooldownTimer;

    void OnEnable()
    {
        if (dashAction != null && dashAction.action != null)
        {
            dashAction.action.performed += OnDashTriggered;
        }
    }
    void OnDisable()
    {
        if (dashAction != null && dashAction.action != null)
        {
            dashAction.action.performed -= OnDashTriggered;
        }
    }

    void OnDashTriggered(InputAction.CallbackContext context)
    {
        if (isDashing || cooldownTimer > 0) return;

        StartDash();
    }

    void StartDash()
    {
        isDashing = true;
        cooldownTimer = dashCooldown;

        Vector2 moveDirection = forwardSource.action.ReadValue<Vector2>();

        Vector3 dashDirection = new Vector3(moveDirection.y, 0, -moveDirection.x);
        dashDirection.Normalize();

        if (dashDirection.magnitude == 0)
        {
            dashDirection = transform.forward;
        }
        dashingVelocity = dashDirection * dashForce;

        onDashStart?.Invoke();
    }

    void ExecuteDash()
    {
        float currentFriction = airFriction;
        if (!manualFriction)
        {


            if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, characterController.height + 0.5f, groundLayer))
            {
                if (hit.collider != null || hit.collider.sharedMaterial != null)
                {
                    currentFriction = hit.collider.sharedMaterial.dynamicFriction * baseFrictionMultiplier;
                }
                else
                {
                    currentFriction = groundFriction;
                }
            }
        }
        else
        {
            currentFriction = fricitonValue;
        }

        dashingVelocity = Vector3.MoveTowards(dashingVelocity, Vector3.zero, currentFriction * dashForce * Time.deltaTime);
        characterController.Move(dashingVelocity * Time.deltaTime);



        if (dashingVelocity.magnitude <= 1)
        {
            StopDash();
        }
    }
    void StopDash()
    {
        isDashing = false;
        dashingVelocity = Vector3.zero;

        onDashEnd?.Invoke();
    }

    void Update()
    {
        if (cooldownTimer > 0) cooldownTimer -= Time.deltaTime;

        if (isDashing)
        {
            ExecuteDash();
        }
    }
}
