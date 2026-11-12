using UnityEngine;
using TMPro;

public class CountdownTimer : MonoBehaviour
{
    [SerializeField] private UIController UIController;
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private float startTime = 60f;

    [Header("Efectos Visuales")]
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color warningColor = Color.red;
    [SerializeField] private float blinkSpeed = 4f;

    private float currentTime;
    private bool timerActive = true;

    // Variables públicas para que el menú las lea
    public bool isGameOver { get; private set; } = false;
    public bool hasWon { get; private set; } = false;

    void Start()
    {
        currentTime = startTime;
        timerText.color = normalColor;
        UpdateTimerUI();
    }

    void Update()
    {
        if (timerActive)
        {
            currentTime -= Time.deltaTime;

            if (currentTime <= 0f)
            {
                currentTime = 0f;
                timerActive = false;
                isGameOver = true;
                hasWon = false; // Derrota
                timerText.enabled = true;
                timerText.color = warningColor;
                OnTimerEnd();
            }

            UpdateTimerUI();
            UpdateTimerEffects();
        }
    }

    // LLAMA A ESTA FUNCIÓN CUANDO EL JUGADOR RECOJA EL ÚLTIMO PICKUP
    public void StopTimerForWin()
    {
        timerActive = false;
        isGameOver = true;
        hasWon = true; // Victoria
    }

    private void UpdateTimerUI()
    {
        int minutes = Mathf.FloorToInt(currentTime / 60F);
        int seconds = Mathf.FloorToInt(currentTime % 60F);
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    private void UpdateTimerEffects()
    {
        if (currentTime <= 10f && currentTime > 5f)
        {
            float lerp = Mathf.InverseLerp(10f, 5f, currentTime);
            timerText.color = Color.Lerp(normalColor, warningColor, lerp);
        }
        else if (currentTime <= 5f)
        {
            timerText.color = warningColor;
            timerText.enabled = Mathf.PingPong(Time.time * blinkSpeed, 1f) > 0.5f;
        }
    }

    private void OnTimerEnd()
    {
        // ¡ELIMINAMOS Time.timeScale = 0f; DE AQUÍ!
        // Ahora el jugador puede seguir moviéndose tranquilamente.
        if (UIController != null) UIController.LoseGameUI();
    }
}