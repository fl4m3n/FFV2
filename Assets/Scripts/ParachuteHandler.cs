using UnityEngine;

public class ParachuteHandler : MonoBehaviour
{
    public RobotSequenceManager robotSequenceManager;

    public void ReceiveMessage(string messageType, int value)
    {
        if (value == 1){
            if (robotSequenceManager != null){
                robotSequenceManager.deployParachute(delay:0f);
            }
        }
    }
}
