using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIController : MonoBehaviour
{
    [Header("Player")]
    [SerializeField] private SkateMovement skateMovementScripts;
    [SerializeField] private TextMeshProUGUI velocityText;

    [SerializeField] private PickupCounter pickupCounterScript;
    [SerializeField] private TextMeshProUGUI totalPickupsText;

    // Update is called once per frame
    void Update()
    {
        UpdateVelocityUI();
        UpdatePickupUI();
    }

    public void UpdatePickupUI()
    {
        totalPickupsText.text = pickupCounterScript.pickupTotal.ToString();
    }

    public void UpdateVelocityUI()
    {
        velocityText.text = Mathf.Ceil(skateMovementScripts.currentSpeed).ToString();
    }
}
