using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

[RequireComponent(typeof(Rigidbody))]
public class RobotFollow : MonoBehaviour
{
    public Transform targetCamera;

    [Header("Follow Settings")]
    public Vector3 followOffset = new Vector3(0f, -0.2f, 1.6f);
    public float smoothTime = 0.8f;
    public float maxSpeed = 1.2f;

    [Header("Space Wobble")]
    public float wobbleAmount = 0.06f;   // up/down distance
    public float wobbleSpeed = 1.2f;     // wobble speed

    [Header("Look At Camera")]
    public float rotationSpeedDegrees = 120f;

    private XRGrabInteractable grab;
    private Rigidbody rb;
    private Vector3 velocity;
    private bool isActive = false;

    void Awake()
    {
        grab = GetComponent<XRGrabInteractable>();
        rb = GetComponent<Rigidbody>();
    }

    public void SetActive(bool state)
    {
        isActive = state;

        rb.isKinematic = state;
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }

    void FixedUpdate()
    {
        if (!isActive) return;
        if (targetCamera == null) return;

        // Don't follow while being held
        if (grab != null && grab.isSelected) return;

        Vector3 targetPos =
            targetCamera.position +
            targetCamera.forward * followOffset.z +
            targetCamera.right * followOffset.x +
            targetCamera.up * followOffset.y;

        // Add gentle up/down floating motion
        targetPos += targetCamera.up * (Mathf.Sin(Time.time * wobbleSpeed) * wobbleAmount);

        Vector3 newPos = Vector3.SmoothDamp(
            rb.position,
            targetPos,
            ref velocity,
            smoothTime,
            maxSpeed
        );

        rb.MovePosition(newPos);

        // Make robot face the camera with delay
        Vector3 direction = targetCamera.position - rb.position;
        direction.y = 0f;

        if (direction.sqrMagnitude > 0.001f)
        {
            Quaternion targetRot = Quaternion.LookRotation(direction.normalized);

            // Keep this if your model faces backwards
            targetRot *= Quaternion.Euler(0f, 180f, 0f);

            Quaternion newRot = Quaternion.RotateTowards(
                rb.rotation,
                targetRot,
                rotationSpeedDegrees * Time.fixedDeltaTime
            );

            rb.MoveRotation(newRot);
        }
    }
}