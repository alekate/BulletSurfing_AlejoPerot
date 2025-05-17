using TMPro;
using UnityEngine;


public class Mobile_TapGameManager : MonoBehaviour
{
    private float tapCounter = 0;
    public float timer = 10f;
    private float highScore = 0;
    private bool isTimerRunning = false;


    public TMP_Text timerText;
    [SerializeField] private TMP_Text tapText;
    [SerializeField] private TMP_Text highScoreText;
    private float maxTime = 10f;

    [SerializeField] private InterstitialManager interstitialManager;

    void Start()
    {
        highScore = PlayerPrefs.GetFloat("HighScore", 0f);
        highScoreText.text = highScore.ToString();
    }

    void Update()
    {
        if (isTimerRunning)
        {
            timer -= Time.deltaTime;
            timer = Mathf.Max(0, timer);
            timerText.text = timer.ToString("F2");

            if (timer <= 3f)
            {
                float t = timer / 3f;
                timerText.color = Color.Lerp(Color.red, Color.black, t);
            }
            else
            {
                timerText.color = Color.black;
            }

            if (timer <= 0)
            {
                isTimerRunning = false;

                bool beatHighScore = false;

                if (tapCounter > highScore)
                {
                    highScore = tapCounter;
                    highScoreText.text = highScore.ToString();

                    PlayerPrefs.SetFloat("HighScore", highScore);
                    PlayerPrefs.Save();

                    beatHighScore = true;
                }

                // Reset values
                tapCounter = 0;
                tapText.text = "0";
                timer = maxTime;
                timerText.text = maxTime.ToString("F2");
                timerText.color = Color.black;

                // Solo muestra ad si no superó el highscore
                if (!beatHighScore)
                {
                    interstitialManager.ShowInterstitialAd();
                }
            }
        }
    }


    public void TapCounter()
    {
        if (!isTimerRunning)
        {
            isTimerRunning = true;
        }

        tapCounter++;
        tapText.text = tapCounter.ToString();
    }

}
