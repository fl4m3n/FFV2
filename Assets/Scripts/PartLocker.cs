using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class PartLocker : MonoBehaviour
{
    // When an object is tried to 'lock in' the following runs:
    public void LockPart(SelectEnterEventArgs args)
    {
        // 1. Which object has been clicked in:
        GameObject attachedPart = args.interactableObject.transform.gameObject;

        // 2. Remove the possibility to grab it.
        UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable grabComp = attachedPart.GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
        if (grabComp != null)
        {
            Destroy(grabComp);
        }

        // 3. Remove grafiti 
        Rigidbody rb = attachedPart.GetComponent<Rigidbody>();
        if (rb != null)
        {
            Destroy(rb);
        }

        // 4. Visually attach it to the body
        attachedPart.transform.SetParent(this.transform.parent);

        // Optional: Sound of snapping in right here
        Debug.Log(attachedPart.name + " is permanent vastgezet! Vast is vast.");
    }
}