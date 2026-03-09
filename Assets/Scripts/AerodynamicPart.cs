using UnityEngine;

public class AerodynamicPart : MonoBehaviour
{
    [Header("State")]
    public bool isFreefalling = false;
    public bool hasBeenGrabbed = false; 

    [Header("Targets")]
    public Transform playerTarget; // Dit is je XR Origin / Camera
    public Transform anchorTarget; 

    [Header("Catch-Up Logic (Y-Axis)")]
    public AnimationCurve heightBasedOnDistance; 
    [Tooltip("Hoe snel het object naar zijn doel-hoogte gaat")]
    public float verticalSpeed = 2f;
    [Tooltip("Hoe snel het object horizontaal naar zijn anker gaat")]
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
        if (!isFreefalling || playerTarget == null || anchorTarget == null || hasBeenGrabbed) return;

        if (grabInteractable != null && grabInteractable.isSelected)
        {
            hasBeenGrabbed = true; 
            return;
        }

        // --- 1. HORIZONTAL MOVEMENT (X, Z) ---
        // Going to the X,Z of the anchor pointn
        float newX = Mathf.Lerp(transform.position.x, anchorTarget.position.x, Time.deltaTime * horizontalSpeed);
        float newZ = Mathf.Lerp(transform.position.z, anchorTarget.position.z, Time.deltaTime * horizontalSpeed);

        // --- 2. Distance calc (XZ-vlak) ---
        Vector2 playerXZ = new Vector2(playerTarget.position.x, playerTarget.position.z);
        Vector2 partXZ = new Vector2(transform.position.x, transform.position.z);
        float distance = Vector2.Distance(playerXZ, partXZ);

        // --- 3. relative height logics (Y) ---
        float depthOffset = heightBasedOnDistance.Evaluate(distance);
        float targetY = playerTarget.position.y - depthOffset; 
        
        float newY = Mathf.Lerp(transform.position.y, targetY, Time.deltaTime * verticalSpeed);

        // --- 4. applied ---
        transform.position = new Vector3(newX, newY, newZ);
        transform.Rotate(tumbleSpeed * Time.deltaTime, Space.Self);
    }

    public void SetFreefallState(bool state)
    {
        isFreefalling = state;
        if (state == true) hasBeenGrabbed = false; 
    }
}