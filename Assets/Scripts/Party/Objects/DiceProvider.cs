using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class DiceProvider : MonoBehaviour
{
    [SerializeField] float speedThreshold = 0.05f;
    [SerializeField] float stableDuration = 0.2f;
    [SerializeField] TextMeshProUGUI text;
    public int upNumber, downNumber, leftNumber, rightNumber, forwardNumber, backNumber;

    XRGrabInteractable grabInteractable;
    Rigidbody rb;
    bool isCheckingDiceRoll = false;

    void Awake()
    {
        grabInteractable = GetComponent<XRGrabInteractable>();
        rb = GetComponent<Rigidbody>();
    }

    void Start()
    {
        grabInteractable.selectExited.AddListener(StartDiceRoll);
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
        text.text = diceResult.ToString();
    }

    void StartDiceRoll(SelectExitEventArgs args)
    {
        //TODO: Add a force to throw
        if (!isCheckingDiceRoll)
        {
            StartCoroutine(CheckDiceRoll());
        }
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


