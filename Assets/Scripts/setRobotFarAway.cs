using UnityEngine;

public class setRobotFarAway : MonoBehaviour
{
    public GameObject robotEmptyGameObject;

    private void setPosRobot(bool active)
    {
        // Check if the reference is assigned to avoid NullReferenceExceptions
        if (robotEmptyGameObject == null) return;

        if (active)
        {
            // Use .transform.position and assign a Vector3
            robotEmptyGameObject.transform.position = new Vector3(1000, 1000, 1000);
        }
        else
        {
            robotEmptyGameObject.transform.position = Vector3.zero;
        }
    }

    public void setRobotFarBoolean(bool active)
    {
        setPosRobot(active);
    }
}