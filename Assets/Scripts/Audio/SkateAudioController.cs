using UnityEngine;
using System.Collections.Generic;

public class SkateAudioController : MonoBehaviour
{
    [SerializeField] private NewSkateMovement skateMovement;
    [SerializeField] private Original_PlayerGrind playerGrind;

    [SerializeField] private List<SpeedBasedSound> sounds = new List<SpeedBasedSound>();

    private void Start()
    {
        if (skateMovement == null)
            skateMovement = FindObjectOfType<NewSkateMovement>();
    }

    private void FixedUpdate()
    {
        float currentSpeed = skateMovement.currentSpeed;

        foreach (var sound in sounds)
        {
            if (sound.audioSource == null) continue;

            if (sound.onlyPlayWhenGrinding && !playerGrind.onRail)
            {
                if (sound.audioSource.isPlaying)
                    sound.audioSource.Stop();
                continue;
            }

            if (currentSpeed > sound.minSpeedToPlaySound)
            {
                if (!sound.audioSource.isPlaying)
                    sound.audioSource.Play();

                float t = Mathf.InverseLerp(sound.minSpeedToPlaySound, sound.maxSpeed, currentSpeed);
                sound.audioSource.volume = Mathf.Lerp(sound.minVolume, sound.maxVolume, t);
                sound.audioSource.pitch = Mathf.Lerp(sound.minPitch, sound.maxPitch, t);
            }
            else
            {
                if (sound.audioSource.isPlaying)
                    sound.audioSource.Stop();
            }
        }
    }

}
