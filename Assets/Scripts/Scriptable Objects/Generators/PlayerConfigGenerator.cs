using UnityEngine;

[CreateAssetMenu(fileName = "Player Config", menuName = "Scriptable Objects/Create New Player Config")]
public class PlayerConfig : ScriptableObject
{
    [Header("Locomotion")]
    public bool movementEnabled;
    public bool turnEnabled;
    public bool jumpEnabled;
    public bool gravityEnabled;
    public bool climbingEnabled;
    public bool dashingEnabled;

    [Space(15)]

    [Header("Movement")]
    public bool continuousMoveEnabled;
    public bool teleportationEnabled;

    [Space(15)]

    [Header("Turning")]
    public bool continuousTurnEnabled;
    public bool snapTurnEnabled;

    [Space(15)]

    [Header("Continuous Movement")]
    public float moveSpeed = 4;
    public float inAirControlModifier = 0.5f;
    public bool useGravity = true;
    public bool useHapticsContinuousMovement = true;
    public bool sprintEnabled = true;
    public float sprintMoveSpeed = 8;
    public bool physicalSprint = true;
    public bool sprintAccelerationEnabled = false;
    public float sprintAcceleration = 0.1f;
    public float sprintDeceleration = 0.2f;

    [Space(15)]

    [Header("Teleportation")]
    public float delayTimeTeleportation = 0.5f;

    [Space(15)]

    [Header("Continuous Turn")]
    public float turnSpeed = 80;

    [Space(15)]

    [Header("Snap Turn")]
    public float turnAmount = 45;
    public float delayTimeSnapTurn = 0;

    [Space(15)]

    [Header("Jump")]
    public float jumpHeight = 2;
    public int upwardGravity = -19;
    public int downwardGravity = -27;
    public float coyoteTimeDuration = 0.15f;

    [Space(15)]

    [Header("Gravity")]
    public float terminalVelocity = 90;
    public float gravityMultiplier = 1;

    [Space(15)]

    [Header("Dash")]
    public float dashForce = 15;
    public float dashCooldown = 2;
    public float airFriction = 3;
    public float groundFriction = 5;
    public float baseFrictionMultiplier = 5;
    public bool manualFriction = false;
    public float fricitonValue = 0;
    public bool useHapticsDash = true;

    [Space(15)]

    [Header("Misc")]
    public bool adaptiveHeight = true;

}
