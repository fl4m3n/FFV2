using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SimpleAudioFader : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private bool isActive = false; 
    public float fadeSpeed = 1.5f;  
    public float maxVolume = 0.5f;  

    public AudioSource _audioSource;

    void Start()
    {
        _audioSource = GetComponent<AudioSource>();
        
        // Start on no volume
        if (!isActive) _audioSource.volume = 0;
        
        // Always looping
        if (!_audioSource.isPlaying) _audioSource.Play();
    }

    void Update()
    {
        // What is the goal?
        float targetVolume = isActive ? maxVolume : 0f;

        // Fade volume
        _audioSource.volume = Mathf.MoveTowards(_audioSource.volume, targetVolume, fadeSpeed * Time.deltaTime);
    }
    public void SetAudioSourceActive(bool active) => isActive = active;
}