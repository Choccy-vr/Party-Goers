using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Teleportation;

public class PartySpace : MonoBehaviour
{
    //Variables
    public int spaceID;

    public PartySpaceType type;
    PartySpaceType lastType;

    [SerializeField] Renderer innerSpace;

    [SerializeField] Material starMaterial;
    Material lastMaterial;

    [SerializeField] int coinAmount = 3;

    [Tooltip("If it is more than one Party Space it indicates a fork in which case all Party Spaces in the array will be unlocked")]
    public List<PartySpace> nextSpace;

    [SerializeField] InteractionLayerMask unlockInteractionLayer;
    [SerializeField] InteractionLayerMask disabledInteractionLayer;

    [SerializeField] bool isFirstSpace;

    public UnityEvent onSpaceArrive;
    public UnityEvent onSpaceLand;
    public UnityEvent onSpaceLeave;



    [HideInInspector] public bool isOccupied;
    [HideInInspector] public VRPartyPlayer playerOnSpace;


    [HideInInspector] public TeleportationAnchor teleportationAnchor;


    void Awake()
    {
        //Get required components
        teleportationAnchor = GetComponent<TeleportationAnchor>();
    }

    void Start()
    {
        teleportationAnchor.teleporting.AddListener(OnTeleportLanded);

        if (isFirstSpace)
        {
            teleportationAnchor.interactionLayers = unlockInteractionLayer;
        }
        else
        {
            teleportationAnchor.interactionLayers = disabledInteractionLayer;
        }
    }

    void OnTeleportLanded(TeleportingEventArgs args)
    {
        VRPartyPlayer arrivingPlayer = getLocalNetworkPlayer();
        if (arrivingPlayer == null) return;

        OnTeleportLeave(arrivingPlayer, arrivingPlayer.currentSpaceId);

        isOccupied = true;
        playerOnSpace = arrivingPlayer;
        arrivingPlayer.currentSpaceId = spaceID;
        teleportationAnchor.interactionLayers = disabledInteractionLayer;

        arrivingPlayer.spacesToMove--;
        if (arrivingPlayer.spacesToMove > 0)
        {
            unlockNextSpaces();
        }
        onSpaceArrive?.Invoke();
        if (arrivingPlayer.spacesToMove <= 0)
        {
            spaceLandedEvent();
        }


    }

    public void setSpaceToStar()
    {
        lastType = type;
        type = PartySpaceType.star;
        lastMaterial = innerSpace.material;
        innerSpace.material = starMaterial;
    }
    public void revertStarSpace()
    {
        type = lastType;
        innerSpace.material = lastMaterial;
    }

    public void unlockNextSpaces()
    {
        foreach (PartySpace space in nextSpace)
        {
            //unlock possible spaces
            space.teleportationAnchor.interactionLayers = unlockInteractionLayer;
        }
    }

    void spaceLandedEvent()
    {
        onSpaceLand?.Invoke();
        switch (type)
        {
            case PartySpaceType.normal:
                playerOnSpace.addCoins(coinAmount);
                endTurnAndGoToNext();
                break;
            case PartySpaceType.bad:
                if (playerOnSpace.playerData.coins >= 3)
                {
                    playerOnSpace.addCoins(-coinAmount);
                }
                else
                {
                    playerOnSpace.addCoins(-playerOnSpace.playerData.coins);
                }
                endTurnAndGoToNext();
                break;
            case PartySpaceType.star:
                landedOnStarExchange();
                endTurnAndGoToNext();
                break;
            case PartySpaceType.duel:
                landedOnDuelSpace();
                break;
            case PartySpaceType.lucky:
                landedOnLuckySpace();
                break;
            case PartySpaceType.item:
                landedOnItemSpace();
                break;
            case PartySpaceType.shop:
                landedOnShopSpace();
                break;


        }




    }
    void endTurnAndGoToNext()
    {
        TurnManager.Instance.endPlayerTurn();
        TurnManager.Instance.nextPlayerTurn();
    }
    void landedOnStarExchange()
    {
        Debug.Log("Landed on Star");
        PartyManager.Instance.LandOnStarSpace();
    }
    void landedOnDuelSpace()
    {
        Debug.Log("Landed on Duel");
        PartyManager.Instance.landedOnDuelSpace();
        endTurnAndGoToNext();
    }
    void landedOnLuckySpace()
    {
        Debug.Log("Landed on Lucky");
    }
    void landedOnItemSpace()
    {
        Debug.Log("Landed on Item");
        PartyManager.Instance.landedOnItemSpace(transform);
    }
    void landedOnShopSpace()
    {
        Debug.Log("Landed on Shop");
        PartyManager.Instance.landedOnShopSpace();
        endTurnAndGoToNext();
    }

    void OnTeleportLeave(VRPartyPlayer player, int spaceBeingLeftID)
    {
        PartySpace spaceBeingLeft = GetPartySpace(spaceBeingLeftID);
        spaceBeingLeft.teleportationAnchor.interactionLayers = disabledInteractionLayer;
        if (spaceBeingLeft.type == PartySpaceType.shop)
        {
            leavingShop(spaceBeingLeft.GetComponent<ShopProvider>());
        }
        if (spaceBeingLeft.nextSpace.Count > 1)
        {
            foreach (PartySpace partySpace in spaceBeingLeft.nextSpace)
            {
                partySpace.teleportationAnchor.interactionLayers = disabledInteractionLayer;
            }
        }
        onSpaceLeave?.Invoke();
    }

    void leavingShop(ShopProvider shop)
    {
        shop.despawnItems();
    }



    VRPartyPlayer getLocalNetworkPlayer()
    {
        foreach (var p in FindObjectsByType<VRPartyPlayer>())
        {
            if (p.IsOwner) return p;
        }
        return null;
    }

    PartySpace GetPartySpace(int spaceID)
    {
        return MapManager.Instance.partySpaces.Find(s => s.spaceID == spaceID);
    }

    private void OnDrawGizmos()
    {
        if (nextSpace != null)
        {
            foreach (PartySpace space in nextSpace)
            {
                Gizmos.color = Color.green;
                // Draws a line from this space to the next one
                Gizmos.DrawLine(transform.position, space.transform.position);

                // Draw a small directional arrow/sphere pointing to the next space
                Vector3 direction = (space.transform.position - transform.position).normalized;
                Gizmos.DrawSphere(transform.position + direction * 1.0f, 0.15f);
            }

        }
    }

}

public enum PartySpaceType
{
    normal,
    bad,
    star,
    duel,
    lucky,
    item,
    shop
}
