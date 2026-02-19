using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
public class UIController : MonoBehaviour
{
    [Header("Player")]
    [SerializeField] private NewSkateMovement skateMovement;
    [SerializeField] private TextMeshProUGUI velocityText;

    [Header("Pickups")]
    [SerializeField] private PickupCounter pickupCounterScript;
    [SerializeField] private TextMeshProUGUI currentPickupText;
    [SerializeField] private TextMeshProUGUI allPickupsText;
    [SerializeField] private TextMeshProUGUI youWinText;
    [SerializeField] private TextMeshProUGUI youLoseText;

    [Header("Sound Controller")]
    [SerializeField] private SoundController soundController;

    private void Start()
    {
        youLoseText.gameObject.SetActive(false);
        youWinText.gameObject.SetActive(false);
        skateMovement = FindObjectOfType<NewSkateMovement>();
    }
    void Update()
    {
        UpdateVelocityUI();
        UpdatePickupUI();
    }

    public void UpdatePickupUI()
    {
        currentPickupText.text = pickupCounterScript.currentPickups.ToString();
        allPickupsText.text = pickupCounterScript.allPickups.ToString();
    }

    public void UpdateVelocityUI()
    {
        if (skateMovement != null)
        {
            velocityText.text = Mathf.Ceil(skateMovement.currentSpeed).ToString();
        }
    }

    public void FinishGameUI()
    {
        youWinText.gameObject.SetActive(true);
        soundController.WinSFX();
    }

    public void LoseGameUI()
    {
        youLoseText.gameObject.SetActive(true);
        soundController.LoseSFX();
    }
}
