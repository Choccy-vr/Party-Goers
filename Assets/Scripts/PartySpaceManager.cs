using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Teleportation;

public class PartySpaceManager : MonoBehaviour
{

    public PartySpaceTypes partySpaceTypes;

    TeleportationAnchor tileAnchor;

    void Awake()
    {
        tileAnchor = GetComponentInChildren<TeleportationAnchor>();
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

}
public enum PartySpaceTypes
{
    normal, negative, duel
}
