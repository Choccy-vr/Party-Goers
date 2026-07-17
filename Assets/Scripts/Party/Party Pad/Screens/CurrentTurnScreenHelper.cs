using TMPro;
using UnityEngine;

public class CurrentTurnScreenHelper : MonoBehaviour
{
    public UIItemSlotSpawn diceSlot;
    [SerializeField] UIItemSlotSpawn ItemSlot1;
    [SerializeField] UIItemSlotSpawn ItemSlot2;
    [SerializeField] UIItemSlotSpawn ItemSlot3;

    public void updateItemsForPlayer()
    {
        PlayerSessionData? playerData = GameSessionManager.Instance.getCurrentPlayerData();

        if (playerData != null)
        {
            for (int i = 0; i < playerData.Value.currentInventoryCount; i++)
            {
                if (i == 0)
                {
                    updateItem(ItemSlot1, ItemManager.Instance.FindItemByID(playerData.Value.GetItemAt(i)));
                }
                else if (i == 1)
                {
                    updateItem(ItemSlot2, ItemManager.Instance.FindItemByID(playerData.Value.GetItemAt(i)));
                }
                else
                {
                    updateItem(ItemSlot3, ItemManager.Instance.FindItemByID(playerData.Value.GetItemAt(i)));
                }
            }
        }
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
