using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Climbing;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Gravity;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Teleportation;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Turning;
public class PlayerConfigManager : MonoBehaviour
{
    public static PlayerConfigManager Instance;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    static void ResetStatics()
    {
        Instance = null;
    }

    public PlayerConfig playerConfig;


    LocomotionContinuousMove continuousMoveProvider;
    TeleportationProvider teleportationProvider;
    ContinuousTurnProvider continuousTurnProvider;
    SnapTurnProvider snapTurnProvider;
    LocomotionJumpProvider jumpProvider;
    GravityProvider gravityProvider;
    ClimbProvider climbProvider;
    LocomotionDashProvider dashProvider;

    void GetObjects()
    {
        continuousMoveProvider = GetComponentInChildren<LocomotionContinuousMove>();
        teleportationProvider = GetComponentInChildren<TeleportationProvider>();
        continuousTurnProvider = GetComponentInChildren<ContinuousTurnProvider>();
        snapTurnProvider = GetComponentInChildren<SnapTurnProvider>();
        jumpProvider = GetComponentInChildren<LocomotionJumpProvider>();
        gravityProvider = GetComponentInChildren<GravityProvider>();
        climbProvider = GetComponentInChildren<ClimbProvider>();
        dashProvider = GetComponentInChildren<LocomotionDashProvider>();
    }

    public void applyPlayerConfig()
    {
        //Locomotion
        if (playerConfig.movementEnabled)
        {
            continuousMoveProvider.gameObject.SetActive(playerConfig.continuousMoveEnabled);
            teleportationProvider.gameObject.SetActive(playerConfig.teleportationEnabled);
        }
        else
        {
            continuousMoveProvider.gameObject.SetActive(false);
            teleportationProvider.gameObject.SetActive(false);
        }
        if (playerConfig.turnEnabled)
        {
            continuousTurnProvider.gameObject.SetActive(playerConfig.continuousTurnEnabled);
            snapTurnProvider.gameObject.SetActive(playerConfig.snapTurnEnabled);
        }
        else
        {
            continuousTurnProvider.gameObject.SetActive(false);
            snapTurnProvider.gameObject.SetActive(false);
        }
        jumpProvider.gameObject.SetActive(playerConfig.jumpEnabled);
        gravityProvider.gameObject.SetActive(playerConfig.gravityEnabled);
        climbProvider.gameObject.SetActive(playerConfig.climbingEnabled);
        dashProvider.gameObject.SetActive(playerConfig.dashingEnabled);
        //Movement
        if (playerConfig.movementEnabled)
        {
            //Continuous Move
            if (playerConfig.continuousMoveEnabled)
            {
                continuousMoveProvider.moveSpeed = playerConfig.moveSpeed;
                continuousMoveProvider.inAirControlModifier = playerConfig.inAirControlModifier;
                continuousMoveProvider.useGravity = playerConfig.useGravity;
                continuousMoveProvider.haptics = playerConfig.useHapticsContinuousMovement;
                continuousMoveProvider.sprintMoveSpeed = playerConfig.sprintMoveSpeed;
                continuousMoveProvider.sprintEnabled = playerConfig.sprintEnabled;
                continuousMoveProvider.physicalSprint = playerConfig.physicalSprint;
                continuousMoveProvider.sprintAcceleration = playerConfig.sprintAccelerationEnabled;
                continuousMoveProvider.sprintAccelLength = playerConfig.sprintAcceleration;
                continuousMoveProvider.sprintDecelLength = playerConfig.sprintDeceleration;
            }
            //Teleportation
            if (playerConfig.teleportationEnabled)
            {
                teleportationProvider.delayTime = playerConfig.delayTimeTeleportation;
            }
        }
        //Turning
        if (playerConfig.turnEnabled)
        {
            //Continuous Turn
            if (playerConfig.continuousTurnEnabled)
            {
                continuousTurnProvider.turnSpeed = playerConfig.turnSpeed;
            }
            //Snap Turn
            if (playerConfig.snapTurnEnabled)
            {
                snapTurnProvider.turnAmount = playerConfig.turnAmount;
                snapTurnProvider.delayTime = playerConfig.delayTimeSnapTurn;
            }
        }
        //Jump
        if (playerConfig.jumpEnabled)
        {
            jumpProvider.jumpHeight = playerConfig.jumpHeight;
            jumpProvider.upwardGravity = playerConfig.upwardGravity;
            jumpProvider.downwardGravity = playerConfig.downwardGravity;
            jumpProvider.coyoteTimeDuration = playerConfig.coyoteTimeDuration;
        }
        //Gravity
        if (playerConfig.gravityEnabled)
        {
            gravityProvider.terminalVelocity = playerConfig.terminalVelocity;
            gravityProvider.gravityAccelerationModifier = playerConfig.gravityMultiplier;
        }
        // Dash
        if (playerConfig.dashingEnabled)
        {
            dashProvider.dashForce = playerConfig.dashForce;
            dashProvider.dashCooldown = playerConfig.dashCooldown;
            dashProvider.airFriction = playerConfig.airFriction;
            dashProvider.groundFriction = playerConfig.groundFriction;
            dashProvider.baseFrictionMultiplier = playerConfig.baseFrictionMultiplier;
            dashProvider.manualFriction = playerConfig.manualFriction;
            dashProvider.fricitonValue = playerConfig.fricitonValue;
            dashProvider.haptics = playerConfig.useHapticsDash;
        }
        //misc
        GetComponent<AdaptiveVRHeight>().enabled = playerConfig.adaptiveHeight;

    }

    void Start()
    {
        setPlayerConfig();
    }
    public void setNewPlayerConfig(PlayerConfig newConfig)
    {
        playerConfig = newConfig;
        setPlayerConfig();
    }
    void setPlayerConfig()
    {
        GetObjects();
        applyPlayerConfig();
    }

}
