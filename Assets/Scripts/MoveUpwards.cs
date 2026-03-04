using UnityEngine;

public class MoveUpwards : MonoBehaviour
{
    [Tooltip("Speed in m/s")]
    public float speed = 20f;

    void Update()
    {
        transform.Translate(Vector3.up * speed * Time.deltaTime, Space.World);
    }
}