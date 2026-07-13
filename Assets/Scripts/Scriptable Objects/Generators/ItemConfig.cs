using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
[CreateAssetMenu(fileName = "Item Config", menuName = "Scriptable Objects/Create New Item")]
public class ItemConfig : ScriptableObject
{
    public string itemName;
    public string itemID;
    public string itemDescription;
    public int itemCost;
    public GameObject itemModel;
    public GameObject itemPrefab;
    public UnityAction onItemUsed;

    public void ActivateOnItemUsedEvent()
    {
        onItemUsed?.Invoke();
    }
}
