using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XR;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Inputs.Haptics;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Gravity;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Movement;

public class LocomotionContinuousMove : ContinuousMoveProvider
{

    [Header("Haptics")]
    public bool haptics = true;
    [SerializeField] HapticImpulsePlayer leftControllerHaptics;
    [SerializeField] HapticImpulsePlayer rightControllerHaptics;
    [Range(0f, 1f)]
    [SerializeField] float amplitude = 0.5f;
    [SerializeField] float frequency = 140;
    [SerializeField] float duration = 0.07f;


    [Header("Sprinting")]
    public bool sprintEnabled = true;
    [SerializeField] InputActionProperty sprintAction;
    public float sprintMoveSpeed;
    public bool physicalSprint = true;
    public bool sprintAcceleration = true;
    //both in seconds
    public float sprintAccelLength = 0.3f;
    public float sprintDecelLength = 0.2f;

    [Header("Physical Sprinting")]
    [SerializeField] TrackedPoseDriver leftHand;
    [SerializeField] TrackedPoseDriver rightHand;
    public float swingThreshold = 2f;
    public float swingCommitmentDuration = 0.3f;
    public float swingGraceDuration = 0.4f;






    float normalMoveSpeed;
    bool isSprinting = false;
    float lastLeft, lastRight;
    HapticImpulsePlayer lastHapticPlayer;
    float swingGraceTimer;
    float commitmentTimer;
    float hapticTimer;

    new void Awake()
    {
        base.Awake();
        normalMoveSpeed = moveSpeed;
    }

    new void OnEnable()
    {
        base.OnEnable();
        if (!physicalSprint)
        {
            sprintAction.action.performed += ctx => startSprint();
            sprintAction.action.canceled += ctx => stopSprint();
        }
    }

    void startSprint()
    {
        if (isSprinting) return;

        if (sprintAcceleration)
        {
            StartCoroutine(sprintAccel());
            isSprinting = true;
        }
        else
        {
            moveSpeed = sprintMoveSpeed;
            isSprinting = true;
        }
    }

    void stopSprint()
    {
        if (!isSprinting) { return; }
        if (sprintAcceleration)
        {
            StartCoroutine(sprintDecel());
            isSprinting = false;
        }
        else
        {
            moveSpeed = normalMoveSpeed;
            isSprinting = false;
        }
    }

    IEnumerator sprintAccel()
    {
        float currentTime = 0;
        float startSpeed = moveSpeed;
        while (currentTime < sprintAccelLength)
        {
            moveSpeed = Mathf.Lerp(startSpeed, sprintMoveSpeed, currentTime / sprintAccelLength);
            currentTime += Time.deltaTime;
            yield return null;
        }
        moveSpeed = sprintMoveSpeed;
    }

    IEnumerator sprintDecel()
    {
        float currentTime = 0;
        while (currentTime < sprintDecelLength)
        {
            moveSpeed = Mathf.Lerp(sprintMoveSpeed, normalMoveSpeed, currentTime / sprintDecelLength);
            currentTime += Time.deltaTime;
            yield return null;
        }
        moveSpeed = normalMoveSpeed;
    }

    new void Update()
    {
        base.Update();

        if (physicalSprint)
        {
            float currentLeftY = leftHand.positionInput.action.ReadValue<Vector3>().y;
            float currentRightY = rightHand.positionInput.action.ReadValue<Vector3>().y;

            float velL = Mathf.Abs(currentLeftY - lastLeft) / Time.deltaTime;
            float velR = Mathf.Abs(currentRightY - lastRight) / Time.deltaTime;

            if ((velL + velR) > swingThreshold)
            {
                commitmentTimer += Time.deltaTime;

                if (commitmentTimer >= swingCommitmentDuration)
                {
                    swingGraceTimer = swingGraceDuration;

                    if (!isSprinting)
                    {
                        startSprint();
                    }
                }
            }
            else
            {
                commitmentTimer = Mathf.Max(0f, commitmentTimer - Time.deltaTime);

                if (isSprinting)
                {
                    swingGraceTimer -= Time.deltaTime;

                    if (swingGraceTimer <= 0)
                    {
                        stopSprint();
                    }
                }
            }

            lastLeft = currentLeftY;
            lastRight = currentRightY;
        }
        if (haptics)
        {
            hapticTimer += Time.deltaTime;
        }
    }

    void TriggerHaptics(float amplitude, float frequency, float duration, HapticImpulsePlayer controller)
    {
        if (controller != null)
        {
            // Triggers the controller vibration
            controller.SendHapticImpulse(amplitude, duration, frequency);

            hapticTimer = 0;
        }
    }

    float GetHapticPeriod(float speed)
    {
        if (speed <= 1.0f) return 1.0f;
        if (speed >= 8.0f) return 0.19f;

        return (0.01012f * speed * speed) - (0.20667f * speed) + 1.19655f;
    }

    protected override void MoveRig(Vector3 translationInWorldSpace)
    {
        base.MoveRig(translationInWorldSpace);

        if (haptics && (hapticTimer >= GetHapticPeriod(moveSpeed)))
        {
            if (lastHapticPlayer == leftControllerHaptics)
            {
                TriggerHaptics(amplitude, frequency, duration, rightControllerHaptics);
                lastHapticPlayer = rightControllerHaptics;
            }
            else
            {
                TriggerHaptics(amplitude, frequency, duration, leftControllerHaptics);
                lastHapticPlayer = leftControllerHaptics;
            }
        }

    }
}