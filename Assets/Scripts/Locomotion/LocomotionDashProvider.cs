using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit.Inputs.Haptics;
using UnityEngine.XR.Interaction.Toolkit.Locomotion;

public class LocomotionDashProvider : LocomotionProvider
{

    [SerializeField] CharacterController characterController;
    [SerializeField] InputActionProperty dashAction;
    [SerializeField] InputActionProperty forwardSource;
    [SerializeField] LayerMask groundLayer;

    [Header("Dash Properties")]
    public float dashForce;
    public float dashCooldown;
    public float airFriction;
    public float groundFriction;
    public float baseFrictionMultiplier;

    [Header("Manual Friction")]
    public bool manualFriction;
    public float fricitonValue;

    [Header("Haptics")]
    public bool haptics = true;
    [SerializeField] HapticImpulsePlayer leftControllerHaptics;
    [SerializeField] HapticImpulsePlayer rightControllerHaptics;
    [Range(0f, 1f)]
    [SerializeField] float amplitude = 0.5f;
    [SerializeField] float frequency = 140;
    [SerializeField] float duration = 0.07f;

    [Header("Events")]
    public UnityEvent onDashStart;
    public UnityEvent onDashEnd;

    Vector3 dashingVelocity;
    bool isDashing = false;
    float cooldownTimer;

    void TriggerHaptics(float amplitude, float frequency, float duration, HapticImpulsePlayer controller)
    {
        if (controller != null)
        {
            controller.SendHapticImpulse(amplitude, duration, frequency);
        }
    }

    new void OnEnable()
    {
        base.OnEnable();
        if (dashAction != null && dashAction.action != null)
        {
            dashAction.action.performed += OnDashTriggered;
        }
    }
    new void OnDisable()
    {
        base.OnDisable();
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

        Vector3 rawDirection = new Vector3(moveDirection.x, 0, moveDirection.y);
        Vector3 dashDirection = transform.TransformDirection(rawDirection);
        dashDirection.y = 0;
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
                if (hit.collider != null && hit.collider.sharedMaterial != null)
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


        TriggerHaptics(amplitude, frequency, duration, leftControllerHaptics);
        TriggerHaptics(amplitude, frequency, duration, rightControllerHaptics);




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
            if (TryPrepareLocomotion())
            {
                ExecuteDash();
                TryEndLocomotion();
            }
        }
    }
}