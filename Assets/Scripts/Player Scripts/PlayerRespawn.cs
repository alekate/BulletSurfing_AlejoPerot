using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRespawn : MonoBehaviour
{
    [SerializeField] private Transform respawnPos;
    [SerializeField] private Transform playerPos;


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            RespawnPlayer();
        }
    }

    void RespawnPlayer()
    {
        playerPos.position = respawnPos.position;
    }
}
