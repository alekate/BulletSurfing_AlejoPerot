using System.Collections;
using UnityEngine;

public class SoundController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private NewSkateMovement skateMovement;

    [Header("Audio Clips")]
    [SerializeField] private AudioClip pickupSound;
    [SerializeField] private AudioClip menuMusic;
    [SerializeField] private AudioClip gameMusic;
    [SerializeField] private AudioClip buttonSound;
    [SerializeField] private AudioClip loseSound;
    [SerializeField] private AudioClip grindSound;

    [Header("Audio Sources")]
    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.ignoreListenerPause = true;
    }

    // --- SFX ---
    public void PickupSFX()
    {
        audioSource.PlayOneShot(pickupSound);
    }

    public void ButtonSFX()
    {
        audioSource.PlayOneShot(buttonSound);
    }

    public void GrindSFX()
    {
        audioSource.PlayOneShot(grindSound);
    }
    public void LoseSFX()
    {
        audioSource.PlayOneShot(loseSound);
    }


    // --- Music ---
    public void MenuMusic()
    {
        audioSource.PlayOneShot(menuMusic);
    }

    public void GameMusic()
    {
        audioSource.PlayOneShot(gameMusic);
    }
}
