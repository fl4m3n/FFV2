using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public AudioSource myAudioSource;
    // public AudioClip startSound;
    public bool canIPlay = false;

    // Method 1: Simply play
    public void PlayStartAudio()
    {
        if (myAudioSource != null)
        {
            myAudioSource.Play();
        }
        
    }

    // Methode 2: OneShot; multiple sounds at the same time
    // public void PlayStartEffect()
    // {
    //     if (myAudioSource != null && startSound != null)
    //     {
    //         myAudioSource.PlayOneShot(startSound);
    //     }
    // }
    public void PlaySound(bool sound)
    {
        canIPlay = sound;
        if (canIPlay)
        {
            PlayStartAudio();
        }
    }
}