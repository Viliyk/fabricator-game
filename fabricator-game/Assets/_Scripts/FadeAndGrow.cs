using System.Collections;
using UnityEngine;

public class FadeAndGrow : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;

    // Set the starting and ending alpha values for the sprite
    public float startAlpha = 1.0f;
    public float endAlpha = 0.0f;

    // Set the starting and ending scale values for the sprite
    public Vector3 startScale = Vector3.one;
    public Vector3 endScale = Vector3.one * 2.0f;

    // Set the duration of the fade and grow animation
    public float duration = 1.0f;

    void Start()
    {
        // Start the fade and grow coroutine
        StartCoroutine(FadeAndGrowCoroutine());
    }

    IEnumerator FadeAndGrowCoroutine()
    {
        // Set the starting alpha and scale values
        spriteRenderer.color = new Color(1, 1, 1, startAlpha);
        spriteRenderer.transform.localScale = startScale;

        // Calculate the lerp value increment for each frame
        float lerpValueIncrement = 1.0f / duration;
        float currentLerpValue = 0.0f;

        // Lerp the alpha and scale values over the duration
        while (currentLerpValue < 1.0f)
        {
            currentLerpValue += lerpValueIncrement * Time.deltaTime;
            spriteRenderer.color = new Color(1, 1, 1, Mathf.Lerp(startAlpha, endAlpha, currentLerpValue));
            spriteRenderer.transform.localScale = Vector3.Lerp(startScale, endScale, currentLerpValue);
            yield return null;
        }

        // Destroy the game object after the fade and grow animation is complete
        Destroy(gameObject);
    }
}
