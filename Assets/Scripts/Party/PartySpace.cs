using UnityEngine;
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



    [HideInInspector] public bool isOccupied;
    [HideInInspector] public PartyPlayer playerOnSpace;


    TeleportationAnchor teleportationAnchor;


    void Awake()
    {
        //Get required components
        teleportationAnchor = GetComponent<TeleportationAnchor>();
    }


    public void unlockSpace()
    {
        teleportationAnchor.interactionLayers = unlockInteractionLayer;
    }
    public void lockSpace()
    {
        teleportationAnchor.interactionLayers = disabledInteractionLayer;
    }
}
