using UnityEngine;

public class DynamicWindSystem : MonoBehaviour
{
    [Header("Setup")]
    [SerializeField] private Transform headsetTransform;
    [SerializeField] private bool isActive = false;
    [SerializeField] private float fadeSpeed = 2f;

    [Header("Audio Sources")]
    [SerializeField] private AudioSource roarSource; // perpendicular
    [SerializeField] private AudioSource whistleSource;    // parallel

    private float currentSystemVolume = 0f; // crossfade between phases

    void Update()
    {
        // 1. The global volume (fase)
        float targetVolume = isActive ? 1f : 0f;
        currentSystemVolume = Mathf.MoveTowards(currentSystemVolume, targetVolume, Time.deltaTime * fadeSpeed);

        if (currentSystemVolume <= 0 && !isActive) return;

        // 2. Calc wind-impact with dot product
        // headsetTransform.right compared to Vector3.down
        float dot = Vector3.Dot(headsetTransform.right, Vector3.down);
        
        // abs value because ear in focus does not matter
        // 0 = horizontal (Roar), 1 = vertical (More still)
        float influence = Mathf.Abs(dot);

        // 3. volumes
        // Whistle is loudest when influence is 0 (1 - influence)
        whistleSource.volume = (1f - influence) * currentSystemVolume;
        
        // Roar is loudest if influence is 1
        roarSource.volume = influence * currentSystemVolume;
    }

    // public function to use this in the manager
    public void SetWindActive(bool active) => isActive = active;
}