using UnityEngine;
using UnityEngine.SceneManagement; // NUEVO: Para guardar un récord distinto por nivel
using TMPro;

public class CountdownTimer : MonoBehaviour
{
    [SerializeField] private UIController UIController;
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private TextMeshProUGUI bestTimeText; // NUEVO: Texto en el HUD para mostrar el récord
    [SerializeField] private float startTime = 60f;
    public float CurrentTime => currentTime;

    [Header("Efectos Visuales")]
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color warningColor = Color.red;
    [SerializeField] private float blinkSpeed = 4f;

    private float currentTime;
    private bool timerActive = true;
    private string bestTimeKey;

    public bool isGameOver { get; private set; } = false;
    public bool hasWon { get; private set; } = false;

    void Start()
    {
        currentTime = startTime;
        timerText.color = normalColor;

        // Crea una clave única para este nivel (ej: "BestTime_Level1")
        bestTimeKey = "BestTime_" + SceneManager.GetActiveScene().name;

        UpdateTimerUI();
        UpdateBestTimeUI();
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
                hasWon = false;
                timerText.enabled = true;
                timerText.color = warningColor;
                OnTimerEnd();
            }

            UpdateTimerUI();
            UpdateTimerEffects();
        }
    }

    public void StopTimerForWin()
    {
        timerActive = false;
        isGameOver = true;
        hasWon = true;

        CheckAndSaveBestTime();
    }

    private void CheckAndSaveBestTime()
    {
        // Si el modo truco está activo, no guardamos el récord
        if (CheatScript.instance != null && CheatScript.instance.cheatModeIsActive) return;

        float savedBest = PlayerPrefs.GetFloat(bestTimeKey, -1f);

        if (savedBest < 0f || currentTime > savedBest)
        {
            PlayerPrefs.SetFloat(bestTimeKey, currentTime);
            PlayerPrefs.Save();
            UpdateBestTimeUI();
        }
    }

    private void UpdateTimerUI()
    {
        timerText.text = FormatTime(currentTime);
    }

    private void UpdateBestTimeUI()
    {
        if (bestTimeText == null) return;

        float savedBest = PlayerPrefs.GetFloat(bestTimeKey, -1f);

        if (savedBest < 0f)
        {
            bestTimeText.text = "Best: --:--";
        }
        else
        {
            bestTimeText.text = "Best: " + FormatTime(savedBest);
        }
    }

    private string FormatTime(float timeToFormat)
    {
        int minutes = Mathf.FloorToInt(timeToFormat / 60F);
        int seconds = Mathf.FloorToInt(timeToFormat % 60F);
        return string.Format("{0:00}:{1:00}", minutes, seconds);
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
        if (UIController != null) UIController.LoseGameUI();
    }
}