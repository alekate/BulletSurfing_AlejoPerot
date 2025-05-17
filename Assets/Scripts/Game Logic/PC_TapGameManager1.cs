using TMPro;
using UnityEngine;


public class PC_TapGameManager : MonoBehaviour
{
    private float tapCounter = 0;
    private float timer = 10f;
    private float highScore = 0;
    private bool isTimerRunning = false;


    [SerializeField] private TMP_Text timerText;
    [SerializeField] private TMP_Text tapText;
    [SerializeField] private TMP_Text highScoreText;
    private float maxTime = 10f;

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

                if (tapCounter > highScore)
                {
                    highScore = tapCounter;
                    highScoreText.text = highScore.ToString();

                    PlayerPrefs.SetFloat("HighScore", highScore); 
                    PlayerPrefs.Save();
                }

                tapCounter = 0;
                tapText.text = "0";
                timer = maxTime;
                timerText.text = maxTime.ToString("F2");
                timerText.color = Color.black;
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
