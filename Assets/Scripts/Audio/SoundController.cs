using System.Collections;
using UnityEngine;

public class SoundController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private NewSkateMovement skateMovement;

    [Header("Audio Clips")]
    [SerializeField] private AudioClip pickupSound;
    [SerializeField] private AudioClip menuSound;
    [SerializeField] private AudioClip menuMusic;
    [SerializeField] private AudioClip gameMusic;

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

    public void MenuSFX()
    {
        audioSource.PlayOneShot(menuSound);
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
