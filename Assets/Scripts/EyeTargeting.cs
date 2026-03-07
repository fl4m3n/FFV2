using UnityEngine;

public class EyeTargetingTranslation : MonoBehaviour
{
    [Header("Targeting Settings")]
    public bool isTracking = true;
    public Transform currentTarget; // Sleep hier je VR Camera (speler) in
    
    [Header("Movement Limits")]
    public float maxOffsetX = 0.05f; // Horizontal movement
    public float maxOffsetY = 0.03f; // Vertical
    public float moveSpeed = 5f;     // Smooth

    private Vector3 startLocalPosition;

    void Start()
    {
        // start pos! (maybe be careful; because it needs to be calibrated to the head pos; not whole world pos)
        startLocalPosition = transform.localPosition;
    }

    void Update()
    {
        if (!isTracking || currentTarget == null) return;

        // 1. Direction of player
        Vector3 directionToTarget = (currentTarget.position - transform.position).normalized;

        // 2.Local space around head conversion 
        // Perspective robot where left, up is etc.
        Vector3 localDirection = transform.parent != null 
            ? transform.parent.InverseTransformDirection(directionToTarget) 
            : transform.InverseTransformDirection(directionToTarget);

        // 3. Calc new pos 
        Vector3 targetLocalPos = startLocalPosition + new Vector3(
            localDirection.x * maxOffsetX,
            localDirection.y * maxOffsetY,
            0f 
        );

        // 4. Eye change pos subtle
        transform.localPosition = Vector3.Lerp(transform.localPosition, targetLocalPos, Time.deltaTime * moveSpeed);
    }

    // Modulair
    public void ChangeTarget(Transform newTarget)
    {
        currentTarget = newTarget;
    }
}