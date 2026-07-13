using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class PurchaseStarScreenHelper : MonoBehaviour
{
    [SerializeField] Button purchaseButton;

    void OnEnable()
    {
        if (NetworkIdenity.Instance.networkPlayerIdenity.GetComponent<VRPartyPlayer>().playerData.coins <= 3)
        {
            purchaseButton.interactable = false;
        }
    }
    public void purchaseStar()
    {
        PartyManager.Instance.purchaseStar(NetworkManager.Singleton.LocalClientId);
        Destroy(gameObject.transform.root.gameObject);
    }
    public void leaveStar()
    {
        Destroy(gameObject.transform.root.gameObject);
    }
}
