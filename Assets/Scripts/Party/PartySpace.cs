using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Teleportation;

public class PartySpace : MonoBehaviour
{
    //Variables
    public int spaceID;

    [Tooltip("If it is more than one Party Space it indicates a fork in which case all Party Spaces in the array will be unlocked")]
    public PartySpace[] nextSpace;

    [SerializeField] InteractionLayerMask unlockInteractionLayer;
    [SerializeField] InteractionLayerMask disabledInteractionLayer;

    [SerializeField] bool isFirstSpace;

    public UnityEvent onSpaceArrive;
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
            foreach (PartySpace space in nextSpace)
            {
                //unlock possible spaces
                space.teleportationAnchor.interactionLayers = unlockInteractionLayer;
            }
        }
        onSpaceArrive?.Invoke();
        if (arrivingPlayer.spacesToMove <= 0)
        {
            TurnManager.Instance.endPlayerTurn();
            TurnManager.Instance.nextPlayerTurn();
        }


    }

    void OnTeleportLeave(VRPartyPlayer player, int spaceBeingLeftID)
    {
        PartySpace spaceBeingLeft = GetPartySpace(spaceBeingLeftID);
        spaceBeingLeft.teleportationAnchor.interactionLayers = disabledInteractionLayer;
        if (spaceBeingLeft.nextSpace.Length > 1)
        {
            foreach (PartySpace partySpace in spaceBeingLeft.nextSpace)
            {
                partySpace.teleportationAnchor.interactionLayers = disabledInteractionLayer;
            }
        }
        onSpaceLeave?.Invoke();
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

}
