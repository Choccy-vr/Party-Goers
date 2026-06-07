using Unity.VisualScripting;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Gravity;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Turning;
public class PlayerConfig : MonoBehaviour
{
    //Locomotion
    [Header("Locomotion")]
    public bool movement = true;
    public bool turning = true;
    public bool jumping = true;
    public bool gravity = true;
    public bool climbing = true;

    [Space(15)]

    // movement
    [Header("Movement")]
    public bool teleportation = true;
    public bool continuousMove = false;

    [Space(15)]

    // turn
    [Header("Turn")]
    public bool snapTurn = true;
    public bool continuousTurn = false;

    [Space(15)]

    // gravity
    [Header("Gravity")]
    public float gravityMultiplier = 1f;

    [Space(15)]

    //Interaction 
    [Header("Interaction")]
    public bool pokeInteraction = true;
    public bool directInteraction = true;


    void Awake()
    {
        //locomotion
        if (movement)
        {
            GameObject moveObj = GameObject.Find("Move");
            GameObject teleportationObj = GameObject.Find("Teleportation");
            moveObj.SetActive(true);
            teleportationObj.SetActive(true);
            if (!continuousMove)
            {
                moveObj.SetActive(false);
            }
            else if (!teleportation)
            {
                teleportationObj.SetActive(false);
                GameObject RightHand = GameObject.Find("Right Hand");
                RightHand.GetComponentInChildren<XRRayInteractor>().gameObject.SetActive(false);

            }
        }
        else
        {
            GameObject moveObj = GameObject.Find("Move");
            GameObject teleportationObj = GameObject.Find("Teleportation");
            moveObj.SetActive(false);
            teleportationObj.SetActive(false);
        }

        if (turning)
        {
            GameObject turnObj = GameObject.Find("Turn");
            turnObj.SetActive(true);
            if (snapTurn)
            {
                turnObj.GetComponent<SnapTurnProvider>().enabled = true;
                turnObj.GetComponent<ContinuousTurnProvider>().enabled = false;
            }
            else if (continuousTurn)
            {
                turnObj.GetComponent<SnapTurnProvider>().enabled = false;
                turnObj.GetComponent<ContinuousTurnProvider>().enabled = true;
            }
        }
        else
        {
            GameObject turnObj = GameObject.Find("Turn");
            turnObj.SetActive(false);
        }

        if (jumping)
        {
            GameObject jumpObj = GameObject.Find("Jump");
            jumpObj.SetActive(true);
        }
        else
        {
            GameObject jumpObj = GameObject.Find("Jump");
            jumpObj.SetActive(false);
        }

        if (gravity)
        {
            GameObject gravityObj = GameObject.Find("Gravity");
            gravityObj.SetActive(true);
            gravityObj.GetComponent<GravityProvider>().gravityAccelerationModifier = gravityMultiplier;
        }
        else
        {
            GameObject gravityObj = GameObject.Find("Gravity");
            gravityObj.SetActive(false);
        }

        if (climbing)
        {
            GameObject climbObj = GameObject.Find("Climb");
            climbObj.SetActive(true);
        }
        else
        {
            GameObject climbObj = GameObject.Find("Climb");
            climbObj.SetActive(false);
        }

        //Interaction
        if(!pokeInteraction || !directInteraction)
        {
            GameObject LeftHand = GameObject.Find("Left Hand");
            GameObject RightHand = GameObject.Find("Right Hand");

            if (!pokeInteraction)
            {
                LeftHand.GetComponentInChildren<XRPokeInteractor>().gameObject.SetActive(false);
                RightHand.GetComponentInChildren<XRPokeInteractor>().gameObject.SetActive(false);
            }
            if (!directInteraction)
            {
                LeftHand.GetComponentInChildren<XRDirectInteractor>().gameObject.SetActive(false);
                RightHand.GetComponentInChildren<XRDirectInteractor>().gameObject.SetActive(false);
            }
        }

    }
}
