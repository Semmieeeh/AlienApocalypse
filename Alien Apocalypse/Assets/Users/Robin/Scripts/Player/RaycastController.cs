using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class RaycastController : MonoBehaviourPunCallbacks
{
    [Header("Player")]
    public WeaponInputHandler handler;

    [Space]
    [Header("Raycast")]
    public float raycastLength;

    void Update()
    {
        if(Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, raycastLength))
        {
            if(hit.transform.TryGetComponent<IInteractable>(out IInteractable interactable))
            {
                if(Input.GetKeyDown(KeyCode.E))
                {
                    interactable.Interact(handler);
                }
            }
        }
    }
}
