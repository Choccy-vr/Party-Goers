using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class ItemBoxProvider : MonoBehaviour
{
    [SerializeField] XRGrabInteractable grabInteractable;

    [SerializeField] GameObject boxVisuals;

    public UnityEvent<ItemBoxProvider> onItemBoxGrab;

    void Start()
    {
        if (grabInteractable == null)
        {
            grabInteractable = GetComponent<XRGrabInteractable>();
        }
    }
    void OnEnable()
    {
        if (grabInteractable != null)
        {
            grabInteractable.selectEntered.AddListener(OnGrabItemBox);
            grabInteractable.selectExited.AddListener(OnGrabLeaveItemBox);
        }
    }
    void OnDisable()
    {
        if (grabInteractable != null)
        {
            grabInteractable.selectEntered.RemoveListener(OnGrabItemBox);
            grabInteractable.selectExited.RemoveListener(OnGrabLeaveItemBox);
        }
    }
    void OnGrabItemBox(SelectEnterEventArgs args)
    {
        ItemConfig item = ItemManager.Instance.getRandomItem();
        Destroy(boxVisuals);
        Instantiate(item.itemModel, gameObject.transform);
        ItemManager.Instance.AddItemToInventory(item);
        Debug.Log("Item Box revealed: " + item.itemName);
        onItemBoxGrab?.Invoke(this);
    }
    void OnGrabLeaveItemBox(SelectExitEventArgs args)
    {
        Destroy(gameObject);
    }


}
