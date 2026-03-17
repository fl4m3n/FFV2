using UnityEngine;
using TMPro; 
using System.Collections;

public class TextFader : MonoBehaviour
{
    [Header("Settings")]
    public TextMeshProUGUI textComponent;
    public float fadeDuration = 2.0f;
    public float startDelay = 3.0f;

    private Coroutine fadeCoroutine;

    // Start with delay
    public void StartDelayedFadeIn()
    {
        // Stop current fades to avoid conflicts
        if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
        
        fadeCoroutine = StartCoroutine(FadeRoutine(0, 1));
    }

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
}