using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFOVChanger : MonoBehaviour
{
    [SerializeField] private float FOVChangerGrade = 5f; // Qu� tan r�pido interpola
    [SerializeField] private float speedMultiplier = 1f; // Cu�nto influye la velocidad
    [SerializeField] private SkateMovement skateMovement;

    private float playerCameraFOV = 60f;

    void FixedUpdate()
    {
        SpeedFOVChanger();
    }

    void SpeedFOVChanger()
    {
        float targetFOV = 60 + (skateMovement.currentSpeed * speedMultiplier);

        playerCameraFOV = Mathf.Lerp(playerCameraFOV, targetFOV, FOVChangerGrade * Time.fixedDeltaTime);
        Camera.main.fieldOfView = playerCameraFOV;
    }
}
