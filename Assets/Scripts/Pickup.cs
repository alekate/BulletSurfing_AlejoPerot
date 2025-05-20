using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup : MonoBehaviour
{
    private PickupCounter pickupCounter;
    private Transform playerPos;

    private bool followPlayer = false;
    private bool hasBeenCollected = false; 

    private void Start()
    {
        GameObject player = GameObject.FindWithTag("Player");
        pickupCounter = player.GetComponent<PickupCounter>();
        playerPos = player.transform;
    }

    private void Update()
    {
        InstaWin();
    }

    public void InstaWin()
    {
        // Easter Egg
        if (Input.GetKeyDown(KeyCode.W) && !hasBeenCollected)
        {
            hasBeenCollected = true;
            followPlayer = true;
            pickupCounter.currentPickups++;

            StartCoroutine(FollowAndDestroy());
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !hasBeenCollected)
        {
            hasBeenCollected = true; 
            followPlayer = true;
            pickupCounter.currentPickups++;

            StartCoroutine(FollowAndDestroy());
        }
    }

    private IEnumerator FollowAndDestroy()
    {
        float followDuration = 0.5f;
        float elapsedTime = 0f;

        while (elapsedTime < followDuration)
        {
            transform.position = Vector3.Lerp(transform.position, playerPos.position, Time.deltaTime * 10f);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        Destroy(gameObject);
    }
}
