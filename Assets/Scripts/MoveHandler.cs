using UnityEngine;
using System.Collections.Generic;

public class MoveHandler : MonoBehaviour
{
    public Transform xrOrigin; // XR Origin (whole rig)
    public float speed = 0.2f; // Tweak if you want the player to move on a different speed
    public float smoothing = 5f;
    public bool isActive = false;

    private float currentDirection = 0f;
    private string axisMoving = "ax";


    float targetX;
    float targetZ;

    float currentX;
    float currentZ;
    float ConvertDirection(string axis)
    {
        switch (axis)
        {
            case "fl": return -2f;
            case "sl": return -1f;
            case "ne": return 0f;
            case "sr": return 1f;
            case "fr": return 2f;
            default: return 0f;
        }
    }

    void Update()
    {
        // If this script is not active do not update position!
        if(!isActive){
            return;
        }
        float value = currentDirection * speed;

        // smooth input
        currentX = Mathf.Lerp(currentX, targetX, Time.deltaTime * smoothing);
        // currentZ = Mathf.Lerp(currentZ, targetZ, Time.deltaTime * smoothing);

        Vector3 movement = new Vector3(currentX, 0, 0) * speed * Time.deltaTime;

        xrOrigin.position += movement;
    }
    public void setBoolean(bool active)
    {
        isActive = active;
    }

    public void ReceiveMovementMessage(string axis, string direction)
    {
        // axisMoving = axis;
        if (axis == "ay")
        {
            currentDirection = -ConvertDirection(direction);
            targetX = currentDirection;
        }
    }
}