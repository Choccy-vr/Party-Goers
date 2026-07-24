using UnityEngine;
using Unity.Netcode;
public class NetworkPlayer : NetworkBehaviour
{



    public Transform root;
    public Transform head;
    public Transform leftHand;
    public Transform rightHand;

    public Renderer[] meshToDisable;

    [SerializeField] private CapsuleCollider targetCollider;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (IsOwner)
        {
            foreach (var item in meshToDisable)
            {
                item.enabled = false;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (IsOwner)
        {
            root.position = VRRigReferences.Singleton.root.position;
            root.rotation = VRRigReferences.Singleton.root.rotation;

            head.position = VRRigReferences.Singleton.head.position;
            head.rotation = VRRigReferences.Singleton.head.rotation;

            leftHand.position = VRRigReferences.Singleton.leftHand.position;
            leftHand.rotation = VRRigReferences.Singleton.leftHand.rotation;

            rightHand.position = VRRigReferences.Singleton.rightHand.position;
            rightHand.rotation = VRRigReferences.Singleton.rightHand.rotation;
        }
    }
    void LateUpdate()
    {
        if (VRRigReferences.Singleton.characterController == null || targetCollider == null) return;

        targetCollider.height = VRRigReferences.Singleton.characterController.height;
        targetCollider.radius = VRRigReferences.Singleton.characterController.radius;
        targetCollider.center = VRRigReferences.Singleton.characterController.center;
    }
}
