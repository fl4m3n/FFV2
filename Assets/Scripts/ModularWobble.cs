using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit; // Belangrijk voor VR

[RequireComponent(typeof(UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable))]
public class ModularWobble : MonoBehaviour
{
    [Header("Wobble Settings")]
    public bool isWobbling = true;
    public float amplitude = 0.05f;
    public float speed = 1.5f;
    public float offset = 0f;

    private Vector3 basePosition;
    private UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable grabInteractable;

    void Start()
    {
        basePosition = transform.localPosition;
        
        // Retroeve the XR Grab Interactable component
        grabInteractable = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
        
        // Listen to Grab and release
        grabInteractable.selectEntered.AddListener(OnGrab);
        grabInteractable.selectExited.AddListener(OnRelease);
    }

    void Update()
    {
        if (!isWobbling) return;

        float newY = basePosition.y + Mathf.Sin((Time.time * speed) + offset) * amplitude;
        transform.localPosition = new Vector3(transform.localPosition.x, newY, transform.localPosition.z);
    }

    private void OnGrab(SelectEnterEventArgs args)
    {
        // Stop wobble onGrab
        isWobbling = false;
    }

    private void OnRelease(SelectExitEventArgs args)
    {
        // Set new basePos after release
        basePosition = transform.localPosition;
        
        // start Wobbling again after.
        isWobbling = true; 
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