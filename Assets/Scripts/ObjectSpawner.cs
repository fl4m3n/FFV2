using UnityEngine;

public class ObjectSpawner : MonoBehaviour
{
    public GameObject objectPrefab;
    public int amountToSpawn = 30;
    
    public float minHeight = -150f;
    public float maxHeight = 40f;
    public Vector2 spawnArea = new Vector2(100f, 100f);

    void Start()
    {
        for (int i = 0; i < amountToSpawn; i++)
        {
            // Generate random starting pos
            float randomX = Random.Range(-spawnArea.x / 2f, spawnArea.x / 2f);
            float randomY = Random.Range(minHeight, maxHeight);
            float randomZ = Random.Range(-spawnArea.y / 2f, spawnArea.y / 2f);

            Vector3 startPos = new Vector3(randomX, randomY, randomZ);
            
            // Spawn object
            Instantiate(objectPrefab, startPos, Quaternion.identity, transform);
        }
    }
}