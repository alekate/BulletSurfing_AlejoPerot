using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRespawn : MonoBehaviour
{
    [SerializeField] private Transform respawnPos;
    [SerializeField] private Transform playerPos;
    [SerializeField] private SkateMovement skateMovement;


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            RespawnPlayer();
        }
    }

    void RespawnPlayer()
    {
        skateMovement.currentSpeed = 0f;
        playerPos.position = respawnPos.position;
    }
}
