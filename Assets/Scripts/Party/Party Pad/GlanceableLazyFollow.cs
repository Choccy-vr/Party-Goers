using Sirenix.OdinInspector;
using UnityEngine;

//AI helped me write this

public class GlanceableTabletFollow : MonoBehaviour
{
    [Header("Camera Reference")]
    [Tooltip("Main Camera / VR Headset transform. Leave empty to auto-detect Camera.main.")]
    [SerializeField] private Transform headCamera;

    [Header("Tablet Distance & Placement")]
    [Tooltip("Distance in front of the player's chest/body.")]
    [SerializeField] private float distance = 0.50f;

    [Tooltip("Base height below headset when fully standing (e.g., -0.45m puts it at upper waist/stomach level).")]
    [SerializeField] private float standingHeightOffset = -0.48f;

    [Tooltip("Extra height adjustment when sitting/lowering (negative value pushes it lower when sitting).")]
    [SerializeField] private float sittingExtraOffset = -0.10f;

    [Tooltip("Headset local height threshold (in meters) below which player is considered 'sitting'. Standard standing eye height is ~1.6m.")]
    [SerializeField] private float sittingEyeHeightThreshold = 1.35f;

    [Tooltip("Tilted angle toward player's eyes for easy glancing.")]
    [Range(0f, 60f)]
    [SerializeField] private float tabletTiltAngle = 30f;

    [Header("Smooth Motion & Deadzone")]
    [Tooltip("How smoothly the tablet moves to the target position.")]
    [SerializeField] private float positionSmoothTime = 0.12f;

    [Tooltip("How smoothly the tablet rotates to face the player.")]
    [SerializeField] private float rotationSmoothTime = 0.12f;

    [Tooltip("Degrees you can turn your head before the tablet starts trailing behind you.")]
    [SerializeField] private float yawDeadzoneDegrees = 18f;

    private Vector3 currentVelocity;
    private float targetYaw;

    private void Start()
    {
        if (headCamera == null && Camera.main != null)
            headCamera = Camera.main.transform;

        if (headCamera != null)
            targetYaw = headCamera.eulerAngles.y;
    }

    private void LateUpdate()
    {
        if (headCamera == null) return;

        // 1. Extract pure HORIZONTAL yaw (completely ignores head pitch/tilt)
        float currentHeadYaw = headCamera.eulerAngles.y;

        // 2. Yaw Deadzone logic (keeps panel centered in front of body)
        float yawDelta = Mathf.DeltaAngle(targetYaw, currentHeadYaw);
        if (Mathf.Abs(yawDelta) > yawDeadzoneDegrees)
        {
            targetYaw = currentHeadYaw - (Mathf.Sign(yawDelta) * yawDeadzoneDegrees);
        }

        // 3. Compute Adaptive Height (Stand vs. Sit)
        // Checks local Y position of camera relative to tracking space origin
        float headLocalY = headCamera.localPosition.y;

        // Blend dynamically between standing offset and sitting offset
        float sittingFactor = Mathf.InverseLerp(1.7f, sittingEyeHeightThreshold, headLocalY);
        float currentHeightOffset = standingHeightOffset + Mathf.Lerp(0f, sittingExtraOffset, sittingFactor);

        // 4. Position Calculation on flat horizontal plane
        Quaternion horizontalRotation = Quaternion.Euler(0f, targetYaw, 0f);
        Vector3 forwardOnHorizon = horizontalRotation * Vector3.forward;

        Vector3 bodyAnchorPoint = headCamera.position + (Vector3.up * currentHeightOffset);
        Vector3 targetPosition = bodyAnchorPoint + (forwardOnHorizon * distance);

        // Smoothly position tablet
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref currentVelocity, positionSmoothTime);

        // 5. Rotation Calculation (Tilted slightly up toward eyes)
        Quaternion targetRotation = Quaternion.Euler(tabletTiltAngle, targetYaw, 0f);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime / rotationSmoothTime);
    }
}