using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class DodgeballProvider : NetworkBehaviour
{
    [SerializeField] Transform dodgeballSpawnPoint;
    [SerializeField] List<Collider> courtColiders;
    [SerializeField] Collider ballCollider;
    [SerializeField] XRGrabInteractable grabInteractable;
    [SerializeField] float respawnDuration = 15f;
    float respawnTimer;
    bool hasBeenGrabed = false;
    bool isGrabbed = false;
    private NetworkVariable<bool> isActive = new NetworkVariable<bool>(false);
    private ulong lastThrowerClientId;

    public void OnThrown(ulong throwerId)
    {
        if (IsServer)
        {
            lastThrowerClientId = throwerId;
            isActive.Value = true;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (!IsServer || !isActive.Value) return;

        // Check if collision hit a VR Player Body/Head collider
        VRPartyPlayer player = collision.collider.GetComponentInParent<VRPartyPlayer>();

        if (player != null && player.OwnerClientId != lastThrowerClientId)
        {
            // Ball hit a valid opponent!
            isActive.Value = false; // Neutralize ball on impact

            // Notify the DodgeballManager to eliminate them
            DodgeballManager.Instance.EliminatePlayerServerRpc(player.OwnerClientId);
        }
        else if (collision.collider.CompareTag("Ground"))
        {
            // Hit floor/wall -> ball is no longer active
            isActive.Value = false;
        }
    }

    void Start()
    {
        if (grabInteractable == null)
        {
            grabInteractable = GetComponent<XRGrabInteractable>();
        }
        if (ballCollider == null)
        {
            ballCollider = GetComponent<SphereCollider>();
        }
        foreach (Collider collider in courtColiders)
        {
            Physics.IgnoreCollision(collider, ballCollider);
        }
    }
    void Update()
    {
        if (hasBeenGrabed)
        {
            respawnTimer -= Time.deltaTime;
            if (respawnTimer <= 0)
            {
                transform.position = dodgeballSpawnPoint.position;
            }
        }
    }
    void OnEnable()
    {
        grabInteractable.selectEntered.AddListener(OnSelectEntered);
        grabInteractable.selectExited.AddListener(OnSelectRelease);
    }
    void OnDisable()
    {
        grabInteractable.selectEntered.RemoveListener(OnSelectEntered);
        grabInteractable.selectExited.RemoveListener(OnSelectRelease);
    }

    void OnSelectEntered(SelectEnterEventArgs args)
    {
        isGrabbed = true;
        if (!hasBeenGrabed)
        {
            hasBeenGrabed = true;
        }
        respawnTimer = respawnDuration;
    }
    void OnSelectRelease(SelectExitEventArgs args)
    {
        isGrabbed = false;
        OnThrown(NetworkManager.Singleton.LocalClientId);
    }

}
