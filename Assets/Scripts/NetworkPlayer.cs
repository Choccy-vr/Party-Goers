using UnityEngine;
using Unity.Netcode;
public class NetworkPlayer : NetworkBehaviour
{

    public Transform root;
    public Transform head;
    public Transform leftHand;
    public Transform rightHand;

    public Renderer[] meshToDisable;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (IsOwner)
        {
            foreach(var item in meshToDisable)
            {
                item.enabled = false;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(IsOwner){
        root.position = VRRigReferences.Singleton.root.position;
        root.rotation = VRRigReferences.Singleton.root.rotation;

        head.position = VRRigReferences.Singleton.root.position;
        head.rotation = VRRigReferences.Singleton.root.rotation;

        leftHand.position = VRRigReferences.Singleton.root.position;
        leftHand.rotation = VRRigReferences.Singleton.root.rotation;

        rightHand.position = VRRigReferences.Singleton.root.position;
        rightHand.rotation = VRRigReferences.Singleton.root.rotation;
        }
    }
}
