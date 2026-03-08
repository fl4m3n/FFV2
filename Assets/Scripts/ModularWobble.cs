using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class ModularWobble : MonoBehaviour
{
    [Header("Wobble Settings")]
    public float amplitude = 0.05f;
    public float speed = 1.5f;
    public float offset = 0f;

    // Interna state tracking
    private bool isWobbling = false;
    private bool hasBeenGrabbedOnce = false;
    private bool isCurrentlyGrabbed = false;

    private Vector3 basePosition;
    private UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable grabInteractable;

    void Start()
    {
        // Search the grabbable, wherever it is
        grabInteractable = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
        if (grabInteractable == null) grabInteractable = GetComponentInParent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
        if (grabInteractable == null) grabInteractable = GetComponentInChildren<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();

        // Listen to the hands
        if (grabInteractable != null)
        {
            grabInteractable.selectEntered.AddListener(OnGrab);
            grabInteractable.selectExited.AddListener(OnRelease);
        }
    }

    private void OnGrab(SelectEnterEventArgs args)
    {
        isCurrentlyGrabbed = true;
        hasBeenGrabbedOnce = true;
        isWobbling = false; // Stop the wobble always when it is in your hand
    }

    private void OnRelease(SelectExitEventArgs args)
    {
        isCurrentlyGrabbed = false;

        // While we release, and the script is still going (so not clicked in yet)
        if (this.enabled)
        {
            isWobbling = true; 
            basePosition = transform.localPosition; // Retrieve the new fall location as anchor point
        }
    }

    void Update()
    {
        // Only wobble if we have released the object, AND its not in our hands
        if (!isWobbling || isCurrentlyGrabbed || !hasBeenGrabbedOnce) return;

        // The movement
        float newY = basePosition.y + Mathf.Sin((Time.time * speed) + offset) * amplitude;
        transform.localPosition = new Vector3(transform.localPosition.x, newY, transform.localPosition.z);
    }

    // Will be called by the manager during next round
    public void ResetForNewRound()
    {
        hasBeenGrabbedOnce = false;
        isCurrentlyGrabbed = false;
        isWobbling = false;
    }

    void OnDestroy()
    {
        if (grabInteractable != null)
        {
            grabInteractable.selectEntered.RemoveListener(OnGrab);
            grabInteractable.selectExited.RemoveListener(OnRelease);
        }
    }
}