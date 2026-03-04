using UnityEngine;

public class LoopEnvironment : MonoBehaviour
{
    [Tooltip("Reset height")]
    public float despawnHeight = 50f;
    
    [Tooltip("Reset depth")]
    public float respawnHeight = -100f;

    [Tooltip("Spawning area in x,z")]
    public Vector2 spawnArea = new Vector2(100f, 100f);

    void Update()
    {
        if (transform.position.y >= despawnHeight)
        {
            ResetPosition();
        }
    }

    void ResetPosition()
    {
        float randomX = Random.Range(-spawnArea.x / 2f, spawnArea.x / 2f);
        float randomZ = Random.Range(-spawnArea.y / 2f, spawnArea.y / 2f);

        transform.position = new Vector3(randomX, respawnHeight, randomZ);
    }
}