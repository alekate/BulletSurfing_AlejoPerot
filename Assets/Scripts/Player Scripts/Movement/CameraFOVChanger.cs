using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFOVChanger : MonoBehaviour
{
    [SerializeField] private float FOVChangerGrade = 5f; // Qué tan rápido interpola
    [SerializeField] private float speedMultiplier = 1f; // Cuánto influye la velocidad
    [SerializeField] private SkateMovement skateMovement;

    [SerializeField] private float playerCameraFOV = 60f;
    [SerializeField] private float maxFOV;


    void FixedUpdate()
    {
        SpeedFOVChanger();
    }

    void SpeedFOVChanger()
    {
        float targetFOV = Mathf.Min(60 + (skateMovement.currentSpeed * speedMultiplier), maxFOV);

        playerCameraFOV = Mathf.Lerp(playerCameraFOV, targetFOV, FOVChangerGrade * Time.fixedDeltaTime);
        Camera.main.fieldOfView = playerCameraFOV;
    }
}
