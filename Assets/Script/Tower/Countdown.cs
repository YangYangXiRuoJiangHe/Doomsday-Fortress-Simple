using System.Collections;
using UnityEngine;

public class Countdown : MonoBehaviour
{
    private Transform cameraTransform;
    public SpriteRenderer spriteRenderer;
    public SpriteMask spriteMask;
    private void Update()
    {
        cameraTransform = CameraManager.instance.currentCamera?.transform;
        if (cameraTransform != null)
        {
            transform.LookAt(cameraTransform);
        }
    }
    public void CreateOrDismantleVision(Color color,float countdown)
    {
        this.gameObject.SetActive(true);
        StartCoroutine(CreateCountdownVision(color, countdown));
    }
    private IEnumerator CreateCountdownVision(Color color, float countdown)
    {
        float timer = 0f;
        Color originalColor = spriteRenderer.color;
        Color currentColor = color;
        Vector3 originalScale = spriteMask.transform.localScale;
        Vector3 currentScale = originalScale;
        while (timer < countdown)
        {
            timer += Time.deltaTime;
            currentColor = Color.Lerp(originalColor, color, timer / countdown);
            currentScale = Vector3.Lerp(originalScale, Vector3.zero, timer / countdown);
            spriteRenderer.color = currentColor;
            spriteMask.transform.localScale = currentScale;
            yield return null;
        }
        this.gameObject.SetActive(false);
        spriteRenderer.color = originalColor;
        spriteMask.transform.localScale = originalScale;
    }
}
