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

    // NUEVO: Referencia al temporizador
    private CountdownTimer countdownTimer;

    private void Start()
    {
        youLoseText.gameObject.SetActive(false);
        youWinText.gameObject.SetActive(false);
        skateMovement = FindFirstObjectByType<NewSkateMovement>();

        // Buscamos el timer automáticamente al iniciar
        countdownTimer = FindFirstObjectByType<CountdownTimer>();
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

        // ¡AQUÍ ESTÁ LA MAGIA! Detenemos el timer y marcamos la victoria
        if (countdownTimer != null)
        {
            countdownTimer.StopTimerForWin();
        }
    }

    public void LoseGameUI()
    {
        youLoseText.gameObject.SetActive(true);
        soundController.LoseSFX();
    }
}