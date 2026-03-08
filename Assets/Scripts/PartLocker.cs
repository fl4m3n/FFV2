using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class PartLocker : MonoBehaviour
{
    public void LockPart(SelectEnterEventArgs args)
    {
        GameObject attachedPart = args.interactableObject.transform.gameObject;

        // 1. Turn off Aerodynamic script 
        AerodynamicPart aeroScript = attachedPart.GetComponent<AerodynamicPart>();
        if (aeroScript == null) aeroScript = attachedPart.GetComponentInChildren<AerodynamicPart>();
        
        if (aeroScript != null)
        {
            aeroScript.enabled = false; 
        }

        // 2. Stop the getting of the component
        UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable grabComp = attachedPart.GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
        if (grabComp != null)
        {
            grabComp.enabled = false;
        }

        // 3.Make it a child of the body
        attachedPart.transform.SetParent(this.transform);
        
        // 4. Reset  local pos so it snaps to the correct place
        attachedPart.transform.localPosition = Vector3.zero;
        attachedPart.transform.localRotation = Quaternion.identity;

        // 5. Physics on kinematic
        Rigidbody rb = attachedPart.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
        }
        // Stop wobble definitive for this round
        ModularWobble wobbleScript = attachedPart.GetComponent<ModularWobble>();
        if (wobbleScript == null) wobbleScript = attachedPart.GetComponentInChildren<ModularWobble>();
        if (wobbleScript != null)
        {
            wobbleScript.enabled = false;
        }

        Debug.Log(attachedPart.name + " is perfect gelockt voor deze ronde!");
    }
}