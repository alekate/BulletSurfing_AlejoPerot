using UnityEngine;

public class SkateAudioController : MonoBehaviour
{
    [SerializeField] private AudioSource skateAudioSource;
    [SerializeField] private float minVolume = 0.1f;
    [SerializeField] private float maxVolume = 1f;
    [SerializeField] private float minPitch = 0.8f;
    [SerializeField] private float maxPitch = 1.2f;
    [SerializeField] private float minSpeedToPlaySound = 0.5f;
    [SerializeField] private NewSkateMovement skateMovement;

    private void Start()
    {
        skateMovement = FindObjectOfType<NewSkateMovement>();
    }

    private void FixedUpdate()
    {
        float currentSpeed = skateMovement.currentSpeed;

        if (currentSpeed > minSpeedToPlaySound)
        {
            if (!skateAudioSource.isPlaying)
            {
                skateAudioSource.Play();
            }

            float t = Mathf.InverseLerp(minSpeedToPlaySound, skateMovement.maxSpeed, currentSpeed);
            skateAudioSource.volume = Mathf.Lerp(minVolume, maxVolume, t);
            skateAudioSource.pitch = Mathf.Lerp(minPitch, maxPitch, t);
        }
        else
        {
            if (skateAudioSource.isPlaying)
            {
                skateAudioSource.Stop();
            }
        }
    }
}
