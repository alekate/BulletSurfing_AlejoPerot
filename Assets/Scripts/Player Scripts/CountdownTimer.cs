using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class CountdownTimer : MonoBehaviour
{
    [SerializeField] private UIController UIController;
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private float startTime = 10f;

    private float currentTime;
    private bool timerActive = true;
    private bool gameOver = false;

    void Start()
    {
        currentTime = startTime;
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
                gameOver = true;
                OnTimerEnd();
            }

            UpdateTimerUI();
        }

        if (gameOver && Input.GetKeyDown(KeyCode.Space))
        {
            RestartGame();
        }
    }

    private void UpdateTimerUI()
    {
        timerText.text = Mathf.Ceil(currentTime).ToString("0");
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void OnTimerEnd()
    {
        Time.timeScale = 0f;
        UIController.LoseGameUI();
    }
}
