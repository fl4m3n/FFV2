using UnityEngine;

public class SetRobotVisibility : MonoBehaviour
{
    [Header("Mesh targets")]
    public GameObject[] robotParts; 

    private void SetMeshVisibility(bool isVisible)
    {
        
        foreach (GameObject part in robotParts)
        {
            if (part != null)
            {
                // Retrieve MeshRenderer
                MeshRenderer renderer = part.GetComponent<MeshRenderer>();
                
                if (renderer != null)
                {
                    // set renderer!
                    renderer.enabled = !isVisible;
                }
            }
        }
    }

    // method
    public void SetRobotFarBoolean(bool active)
    {
        SetMeshVisibility(active);
    }
}