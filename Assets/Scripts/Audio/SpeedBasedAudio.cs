using UnityEngine;

[System.Serializable]
public class SpeedBasedSound
{
    public string name;
    public AudioSource audioSource;

    public bool onlyPlayWhenGrinding = false;

    public float minVolume = 0.1f;
    public float maxVolume = 1f;
    public float minPitch = 0.8f;
    public float maxPitch = 1.2f;
    public float minSpeedToPlaySound = 0.5f;
    public float maxSpeed = 10f;
}
