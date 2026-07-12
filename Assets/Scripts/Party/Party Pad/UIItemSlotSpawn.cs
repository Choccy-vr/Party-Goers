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

        Vector3 startWorldScale = Vector3.one;
        if (displayItemObject != null)
        {
            startWorldScale = displayItemObject.transform.lossyScale;
            displayItemObject.SetActive(false);
        }

        GameObject realItem = Instantiate(realItemPrefab, hand.transform.position, hand.transform.rotation);

        Vector3 targetLocalScale = realItemPrefab.transform.localScale;

        IXRSelectInteractable grabInteractable = realItem.GetComponent<IXRSelectInteractable>();
        if (grabInteractable != null)
        {
            hand.interactionManager.SelectEnter(hand, grabInteractable);
        }

        StartCoroutine(ScaleOverTime(realItem.transform, startWorldScale, targetLocalScale, scaleUpDuration));
    }

    IEnumerator ScaleOverTime(Transform itemTransform, Vector3 startWorldScale, Vector3 targetLocalScale, float duration)
    {
        yield return new WaitForFixedUpdate();

        if (itemTransform == null) yield break;

        Vector3 startLocalScale = itemTransform.parent != null
            ? itemTransform.parent.InverseTransformVector(startWorldScale)
            : startWorldScale;

        if (duration <= 0f)
        {
            itemTransform.localScale = targetLocalScale;
            yield break;
        }

        float elapsed = 0f;
        while (elapsed < duration)
        {
            if (itemTransform == null) yield break;

            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);

            itemTransform.localScale = Vector3.Lerp(startLocalScale, targetLocalScale, t);
            yield return null;
        }

        if (itemTransform != null)
        {
            itemTransform.localScale = targetLocalScale;
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