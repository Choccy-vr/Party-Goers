using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class ShopItem : MonoBehaviour
{
    public ItemConfig itemOnSale;
    [SerializeField] XRGrabInteractable grabInteractable;

    void OnEnable()
    {
        grabInteractable.selectEntered.AddListener(purchaseItem);
    }
    void OnDisable()
    {
        grabInteractable.selectEntered.RemoveListener(purchaseItem);
    }

    void purchaseItem(SelectEnterEventArgs args)
    {
        Debug.Log("Trying to purchase Item");
        VRPartyPlayer player = NetworkIdenity.Instance.networkPlayerIdenity.GetComponent<VRPartyPlayer>();

        if (player.playerData.coins >= itemOnSale.itemCost)
        {
            Debug.Log("Player has enough coins. Purchasing");
            GameSessionManager.Instance.AddCoinsToPlayer(player.playerData.networkClientId, -itemOnSale.itemCost);
            GameSessionManager.Instance.AddItemToPlayer(player.playerData.networkClientId, itemOnSale.itemID);
            Destroy(this);
        }
    }
}
