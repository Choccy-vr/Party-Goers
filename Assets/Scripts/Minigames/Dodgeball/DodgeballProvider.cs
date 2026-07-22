using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class DodgeballProvider : MonoBehaviour
{
    [SerializeField] Transform dodgeballSpawnPoint;
    [SerializeField] List<Collider> courtColiders;
    [SerializeField] Collider ballCollider;
    [SerializeField] XRGrabInteractable grabInteractable;
    [SerializeField] float respawnDuration = 15f;
    float respawnTimer;
    bool hasBeenGrabed = false;
    bool isGrabbed = false;

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
                Instantiate(gameObject, dodgeballSpawnPoint);
                Destroy(gameObject);
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

    }

}
