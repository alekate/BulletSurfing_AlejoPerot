using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupCounter : MonoBehaviour
{
    public int currentPickups;               
    public int allPickups;            
    [SerializeField] private UIController UIController;
    [SerializeField] private MenuController menuController;


    void Start()
    {
        GameObject[] pickups = GameObject.FindGameObjectsWithTag("Pickup");
        allPickups = pickups.Length;
        currentPickups = 0;
    }

    private void Update()
    {
        PickedUpAllPickups();
    }

    public void PickedUpAllPickups()
    {        
        if (currentPickups == allPickups)
        {
            UIController.FinnishGameUI();
            if (Input.GetKeyDown(KeyCode.Space))
            {
                menuController.LoadMainMenu();
            }
        }
    }

}
