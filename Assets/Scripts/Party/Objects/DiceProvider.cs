using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class DiceProvider : MonoBehaviour
{
    [SerializeField] float speedThreshold = 0.05f;
    [SerializeField] float stableDuration = 0.2f;
    [SerializeField] float minRollVelocity = 1.5f;
    [SerializeField] float minAngularVelocity = 2.0f;
    [SerializeField] TextMeshProUGUI text;
    public int upNumber, downNumber, leftNumber, rightNumber, forwardNumber, backNumber;

    public UnityEvent<VRPartyPlayer> onDiceFinish;

    public bool placementDice;

    IXRSelectInteractable grabInteractable;
    Rigidbody rb;
    bool isCheckingDiceRoll = false;

    void Awake()
    {
        grabInteractable = GetComponent<XRGrabInteractable>();
        rb = GetComponent<Rigidbody>();
    }

    void OnEnable()
    {
        grabInteractable.selectExited.AddListener(StartDiceRoll);
    }
    void OnDisable()
    {
        grabInteractable.selectExited.RemoveListener(StartDiceRoll);
    }

    IEnumerator CheckDiceRoll()
    {
        isCheckingDiceRoll = true;

        yield return new WaitForSeconds(0.2f);

        float stableTimer = 0;

        while (stableTimer < stableDuration)
        {
            if (rb.linearVelocity.sqrMagnitude < speedThreshold && rb.angularVelocity.sqrMagnitude < speedThreshold)
            {
                stableTimer += Time.deltaTime;
            }
            else
            {
                //Dice has started rolling again
                stableTimer = 0;
            }
            yield return null;
        }

        isCheckingDiceRoll = false;
        int diceResult = GetDiceResult();
        Debug.Log("Dice Rolled a " + diceResult);
        if (!placementDice)
        {
            SetPlayerSpaces(diceResult);
        }
        //else{}
        text.text = diceResult.ToString();
        onDiceFinish?.Invoke(NetworkIdenity.Instance.networkPlayerIdenity.GetComponent<VRPartyPlayer>());
    }


    void StartDiceRoll(SelectExitEventArgs args)
    {
        if (!rb.useGravity) { rb.useGravity = true; }

        // Wait so it has time to compute
        StartCoroutine(EvaluateThrowVelocity(args.manager, args.interactorObject));

        if (!isCheckingDiceRoll)
        {
            StartCoroutine(CheckDiceRoll());
        }

        Debug.Log(args.interactorObject.transform.name + " started a Dice Roll");
    }

    IEnumerator EvaluateThrowVelocity(XRInteractionManager manager, IXRSelectInteractor hand)
    {
        yield return new WaitForFixedUpdate();

        if (rb.linearVelocity.magnitude < minRollVelocity || rb.angularVelocity.magnitude < minAngularVelocity)
        {
            Debug.Log($"Linear Velocity: {rb.linearVelocity.magnitude} Angular Velocity: {rb.angularVelocity.magnitude}");
            onFailedDiceRoll(manager, hand);
        }
        else
        {
            Debug.Log($"Successful Throw! Vel: {rb.linearVelocity.magnitude} Spin: {rb.angularVelocity.magnitude}");
        }
    }

    void onFailedDiceRoll(XRInteractionManager manager, IXRSelectInteractor hand)
    {
        Debug.Log("Dice Roll was not sufficent, Roll again but more forceful");
        PartyPadManager.Instance.currentTurnScreen.GetComponent<CurrentTurnScreenHelper>().diceSlot.resetItemSlot();
        Destroy(gameObject);
    }

    void SetPlayerSpaces(int diceResult)
    {
        NetworkIdenity.Instance.networkPlayerIdenity.GetComponent<VRPartyPlayer>().addSpacesToMove(diceResult);
    }

    public int GetDiceResult()
    {
        Vector3[] localPositions = new Vector3[]
        {
            transform.up, //up
            -transform.up, //down
            -transform.right, //left
            transform.right, // right
            transform.forward, //forward
            -transform.forward


        };

        float maxDot = -Mathf.Infinity;
        Vector3 bestDirection = Vector3.zero;

        foreach (Vector3 direction in localPositions)
        {
            float dotProduct = Vector3.Dot(direction, Vector3.up);

            if (dotProduct > maxDot)
            {
                maxDot = dotProduct;
                bestDirection = direction;
            }
        }

        if (bestDirection == transform.up)
        {
            return upNumber;
        }
        else if (bestDirection == -transform.up)
        {
            return downNumber;
        }
        else if (bestDirection == -transform.right)
        {
            return leftNumber;
        }
        else if (bestDirection == transform.right)
        {
            return rightNumber;
        }
        else if (bestDirection == transform.forward)
        {
            return forwardNumber;
        }
        else
        {
            return backNumber;
        }
    }

}


