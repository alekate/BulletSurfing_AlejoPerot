using System.Collections;
using UnityEngine;

public class SkateWindAudioController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private AudioSource windAudioSource;
    [SerializeField] private NewSkateMovement skateMovement;

    [Header("Settings")]
    [SerializeField] private float speedThreshold = 5f;
    [SerializeField] private float fadeDuration = 1.0f;
    [SerializeField] private float minVolume = 0.1f;
    [SerializeField] private float maxVolume = 1.0f;
    [SerializeField] private float minPitch = 0.9f;
    [SerializeField] private float maxPitch = 1.1f;

    private Coroutine fadeCoroutine;
    private bool isPlaying = false;


    private void Update()
    {
        float speed = skateMovement.currentSpeed;

        if (speed >= speedThreshold)
        {
            if (!isPlaying)
                StartFade(true);

            float t = Mathf.InverseLerp(speedThreshold, skateMovement.maxSpeed, speed);
            windAudioSource.volume = Mathf.Lerp(minVolume, maxVolume, t);
            windAudioSource.pitch = Mathf.Lerp(minPitch, maxPitch, t);
        }
        else
        {
            if (isPlaying)
                StartFade(false);
        }
    }

    private void StartFade(bool fadeIn)
    {
        if (fadeCoroutine != null)
            StopCoroutine(fadeCoroutine);

        fadeCoroutine = StartCoroutine(fadeIn ? FadeIn() : FadeOut());
    }

    private IEnumerator FadeIn()
    {
        isPlaying = true;
        if (!windAudioSource.isPlaying)
            windAudioSource.Play();

        float time = 0f;
        float startVolume = windAudioSource.volume;

        while (time < fadeDuration)
        {
            windAudioSource.volume = Mathf.Lerp(startVolume, maxVolume, time / fadeDuration);
            time += Time.deltaTime;
            yield return null;
        }

        windAudioSource.volume = maxVolume;
    }

    private IEnumerator FadeOut()
    {
        float time = 0f;
        float startVolume = windAudioSource.volume;

        while (time < fadeDuration)
        {
            windAudioSource.volume = Mathf.Lerp(startVolume, 0f, time / fadeDuration);
            time += Time.deltaTime;
            yield return null;
        }

        windAudioSource.volume = 0f;
        windAudioSource.Stop();
        isPlaying = false;
    }
}
