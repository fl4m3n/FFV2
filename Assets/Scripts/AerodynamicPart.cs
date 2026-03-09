using UnityEngine;


public class AerodynamicPart : MonoBehaviour
{
    [Header("State")]
    public bool isFreefalling = false;
    public bool hasBeenGrabbed = false; // KILL-SWITCH 

    [Header("Targets")]
    public Transform playerTarget; 
    public Transform anchorTarget; 

    [Header("Catch-Up Logic (Y-Axis)")]
    public AnimationCurve heightBasedOnDistance; 
    public float verticalSpeed = 2f;
    public float horizontalSpeed = 1f;

    [Header("Tumbling (Rotation)")]
    public Vector3 tumbleSpeed = new Vector3(10f, 15f, 5f);

    private UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable grabInteractable;

    void Awake()
    {
        grabInteractable = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
        if (grabInteractable == null) grabInteractable = GetComponentInParent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
        if (grabInteractable == null) grabInteractable = GetComponentInChildren<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
    }

    void Update()
    {
        // 1. Safetychecks
        if (!isFreefalling || playerTarget == null || anchorTarget == null) return;

        // 2. definitive stop: Make sure it does not work once caught
        if (hasBeenGrabbed) return;

        // 3. CHECK moment of grabbing
        if (grabInteractable != null && grabInteractable.isSelected)
        {
            // BOOM! Kill-switch activated
            hasBeenGrabbed = true; 
            return;
        }

        // --- Tumbling logic ---

        float newX = Mathf.Lerp(transform.position.x, anchorTarget.position.x, Time.deltaTime * horizontalSpeed);
        float newZ = Mathf.Lerp(transform.position.z, anchorTarget.position.z, Time.deltaTime * horizontalSpeed);

        Vector2 playerXZ = new Vector2(playerTarget.position.x, playerTarget.position.z);
        Vector2 partXZ = new Vector2(transform.position.x, transform.position.z);
        float distance = Vector2.Distance(playerXZ, partXZ);

        float targetY = heightBasedOnDistance.Evaluate(distance);
        float newY = Mathf.Lerp(transform.position.y, targetY, Time.deltaTime * verticalSpeed);

        transform.position = new Vector3(newX, newY, newZ);
        transform.Rotate(tumbleSpeed * Time.deltaTime, Space.Self);
    }

    // This function can be called at the start of the round
    public void SetFreefallState(bool state)
    {
        isFreefalling = state;
        
        if (state == true)
        {
            // Reset kill-switch
            hasBeenGrabbed = false; 
        }
    }
}