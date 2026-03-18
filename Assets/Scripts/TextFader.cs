using UnityEngine;
using TMPro; 
using System.Collections;

public class TextFader : MonoBehaviour
{
    [Header("Settings")]
    public TextMeshProUGUI textComponent;
    public float fadeDuration = 2.0f;
    public float startDelay = 3.0f;

    [Header("Fade In & Out Settings")]
    [Tooltip("Sec on screen before vanishing")]
    public float waitTimeBeforeFadeOut = 5.0f; 

    private Coroutine fadeCoroutine;

    // Start with delay 
    public void StartDelayedFadeIn()
    {
        // Stop current fades to avoid conflicts
        if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
        
        fadeCoroutine = StartCoroutine(FadeRoutine(0, 1));
    }

    // Fade
    private IEnumerator FadeRoutine(float startAlpha, float endAlpha)
    {
        //short delay here
        yield return new WaitForSeconds(startDelay);

        float timer = 0;
        Color textColor = textComponent.color;

        while (timer < fadeDuration)
        {
            float alpha = Mathf.Lerp(startAlpha, endAlpha, timer / fadeDuration);
            textComponent.color = new Color(textColor.r, textColor.g, textColor.b, alpha);
            
            timer += Time.deltaTime;
            yield return null;
        }

        textComponent.color = new Color(textColor.r, textColor.g, textColor.b, endAlpha);
    }

    // snap to clear again! 
    public void SnapToClear()
    {
        StopAllCoroutines();
        fadeCoroutine = null;
        
        Color textColor = textComponent.color;
        textComponent.color = new Color(textColor.r, textColor.g, textColor.b, 0);
    }


    // Phase Event
    public void StartFadeInOut()
    {
        if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
        
        fadeCoroutine = StartCoroutine(FadeInOutRoutine());
    }

    private IEnumerator FadeInOutRoutine()
    {
        yield return new WaitForSeconds(startDelay);

        Color textColor = textComponent.color;
        float timer = 0;

        // 1. FADE IN ( 0 - 1)
        while (timer < fadeDuration)
        {
            float alpha = Mathf.Lerp(0, 1, timer / fadeDuration);
            textComponent.color = new Color(textColor.r, textColor.g, textColor.b, alpha);
            timer += Time.deltaTime;
            yield return null;
        }
        textComponent.color = new Color(textColor.r, textColor.g, textColor.b, 1);

        // 2. WAIT
        yield return new WaitForSeconds(waitTimeBeforeFadeOut);

        // 3. FADE OUT (1 - 0)
        timer = 0;
        while (timer < fadeDuration)
        {
            float alpha = Mathf.Lerp(1, 0, timer / fadeDuration);
            textComponent.color = new Color(textColor.r, textColor.g, textColor.b, alpha);
            timer += Time.deltaTime;
            yield return null;
        }
        textComponent.color = new Color(textColor.r, textColor.g, textColor.b, 0);
    }
}