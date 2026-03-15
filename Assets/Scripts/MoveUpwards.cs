using UnityEngine;

public class MoveUpwards : MonoBehaviour
{
    [Tooltip("Speed in m/s")]
    public float speed = 20f;
    public float startHeight = 12000f;
    public bool isMoving = false;

    void Update()
    {
        if (isMoving)
        {
            transform.Translate(Vector3.up * speed * Time.deltaTime, Space.World);
        }
    }
    
    public void SetMoving(bool status)
    {
        isMoving = status;
        if (!isMoving)
        {
            ResetToStart();
        }
    }
    private void ResetToStart()
    {
        Vector3 currentPos = transform.position;
        currentPos.y = startHeight;
        transform.position = currentPos;
    }
}