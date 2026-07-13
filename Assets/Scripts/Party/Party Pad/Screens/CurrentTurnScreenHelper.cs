using TMPro;
using UnityEngine;

public class CurrentTurnScreenHelper : MonoBehaviour
{
    [SerializeField] UIItemSlotSpawn ItemSlot1;
    [SerializeField] UIItemSlotSpawn ItemSlot2;
    [SerializeField] UIItemSlotSpawn ItemSlot3;

    //TODO: add Items in inventory automatically

    public void testUpdateItem1(ItemConfig item)
    {
        updateItem(ItemSlot1, item);
    }

    void updateItem(UIItemSlotSpawn itemSlot, ItemConfig item)
    {
        if (itemSlot != null)
        {
            GameObject fakeItemModel = Instantiate(item.itemModel, itemSlot.modelParent);
            itemSlot.realItemPrefab = item.itemPrefab;
            itemSlot.displayItemObject = fakeItemModel;
            itemSlot.itemText.text = item.itemName;
        }
    }
}
