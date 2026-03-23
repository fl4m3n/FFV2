using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SceneFader : MonoBehaviour
{
    public Image fadeImage;
    public float fadeDuration = 2.0f;

    public void StartFadeToBlack()
    {
        StartCoroutine(Fade(0, 1));
    }

    private IEnumerator Fade(float startAlpha, float endAlpha)
    {
        float timer = 0;
        while (timer < fadeDuration)
        {
            float alpha = Mathf.Lerp(startAlpha, endAlpha, timer / fadeDuration);
            fadeImage.color = new Color(0, 0, 0, alpha);
            timer += Time.deltaTime;
            yield return null;
        }
        fadeImage.color = new Color(0, 0, 0, endAlpha);
    }
    public void SnapToClear()
    {
        StopAllCoroutines();
        fadeImage.color = new Color(0, 0, 0, 0);
    }
}