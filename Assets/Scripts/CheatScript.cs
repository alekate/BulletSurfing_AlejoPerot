using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheatScript : MonoBehaviour
{
    public static CheatScript instance;

    public bool cheatModeIsActive = false;
    [SerializeField] GameObject cheatText;

    [SerializeField] NewSkateMovement skateMovement;

    private string inputBuffer = string.Empty;
    private Dictionary<string, System.Action> cheatCodes;


    private void Awake()
    {
        skateMovement = FindObjectOfType<NewSkateMovement>();

        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
            return;
        }

        cheatCodes = new Dictionary<string, System.Action>()
        {
            { "CHEATMODE", ActivateCheatMode },

            //Plantilla para poner mas cheat :o
        };
    }

    // Update is called once per frame
    void Update()
    {
        CheatmodeEnabler();

        if (cheatModeIsActive == true)
        {
            if (skateMovement != null)
            { 
                cheatText.SetActive(true);
            }
            else
            {
            }

            SpeedCheat();
        }
        else
        {
            if (skateMovement != null)
            {
                cheatText.SetActive(false);
            }
            else
            {
            }
        }
    }

    void ActivateCheatMode()
    {
        cheatModeIsActive = true;
    }

    void CheatmodeEnabler()
    {
        foreach (char c in Input.inputString)
        {
            inputBuffer += char.ToUpper(c);

            if (inputBuffer.Length > 30)
            {
                inputBuffer = inputBuffer.Substring(inputBuffer.Length - 30);
            }

            foreach (var code in cheatCodes)
            {
                if (inputBuffer.EndsWith(code.Key))
                {
                    code.Value.Invoke();
                    Debug.Log($"Cheat code {code.Key} activated.");
                    inputBuffer = "";
                    break;
                }
            }
        }

    }
    void SpeedCheat()
    {

        if(skateMovement != null)
        { 
            if (Input.GetKey(KeyCode.W))
            {
                skateMovement.currentSpeed += 10;
                skateMovement.maxSpeed = 100;
            }
        }
        else
        {
            skateMovement = FindObjectOfType<NewSkateMovement>();
        }
    }


}
