using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class UIItemSlotSpawn : MonoBehaviour
{
    [SerializeField] GameObject realItemPrefab;
    [SerializeField] GameObject displayItemObject;
    [SerializeField] float scaleUpDuration = 0.2f;

    bool isSlotEmpty = false;

    void OnTriggerStay(Collider other)
    {
        Debug.Log("TRIGGERED");
        if (isSlotEmpty)
        {
            Debug.Log("Slot is already empty");
            return;
        }

        XRDirectInteractor handInteractor = other.GetComponentInChildren<XRDirectInteractor>();

        if (handInteractor != null)
        {
            Debug.Log("Checking if wanting to grab");
            if (handInteractor.isSelectActive)
            {
                Debug.Log("Grabbing fake item");
                spawnAndGrabItem(handInteractor);
            }
        }
        else
        {
            Debug.Log("Hand interactor is null");
        }
    }

    void spawnAndGrabItem(XRDirectInteractor hand)
    {
        isSlotEmpty = true;

        if (displayItemObject != null)
        {
            displayItemObject.SetActive(false);
        }

        GameObject realItem = Instantiate(realItemPrefab, hand.transform.position, hand.transform.rotation);
        IXRSelectInteractable grabInteractable = realItem.GetComponent<IXRSelectInteractable>();

        if (grabInteractable != null)
        {
            hand.interactionManager.SelectEnter(hand, grabInteractable);
        }
    }



    public void resetItemSlot()
    {
        isSlotEmpty = false;
        if (displayItemObject != null)
        {
            displayItemObject.SetActive(true);
        }
    }



}
