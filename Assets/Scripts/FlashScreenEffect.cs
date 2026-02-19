using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class FlashScreenEffect : MonoBehaviour
{
    [Header("Screen Effects")]
    [SerializeField] private Image whiteEffect;
    [SerializeField] private float desiredAlpha = 0.7f; 
    [SerializeField] private float effectDuration;

    public void WhiteScreenEffect()
    {
        StartCoroutine(FlashRoutine());
    }

    private IEnumerator FlashRoutine()
    {
        float timer = 0f;

        // Aparece instantáneamente
        Color color = whiteEffect.color;
        color.a = desiredAlpha;
        whiteEffect.color = color;

        // Espera mientras baja el alpha
        while (timer < effectDuration)
        {
            timer += Time.deltaTime;
            float alpha = Mathf.Lerp(desiredAlpha, 0f, timer / effectDuration);

            color.a = alpha;
            whiteEffect.color = color;

            yield return null;
        }

        // Asegura que quede en 0
        color.a = 0f;
        whiteEffect.color = color;
    }
}