using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour
{
    public Vector2 turn;
    public float sensX = 5f;
    public float sensY = 5f;

    [SerializeField] private Transform cameraHolder;
    [SerializeField] private SkateMovement skateMovement;


    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }
    void Update()
    {
        UpdateCameraTilt();
        CameraMovement();
    }

    void CameraMovement()
    {
        turn.x += Input.GetAxis("Mouse X") * sensX;
        turn.y += Input.GetAxis("Mouse Y") * sensY;

        turn.y = Mathf.Clamp(turn.y, -30f, 30f);
        transform.localRotation = Quaternion.Euler(-turn.y, turn.x, 0);
    }

    private void UpdateCameraTilt()
    {
        float tiltAngle = skateMovement.horizontalMouseMovementValue * 10f;

        // Obtené la rotación actual y agregá inclinación en Z
        Quaternion targetRotation = Quaternion.Euler(
            cameraHolder.localEulerAngles.x,
            cameraHolder.localEulerAngles.y,
            -tiltAngle // El signo puede depender del efecto visual deseado
        );

        cameraHolder.localRotation = Quaternion.Lerp(
            cameraHolder.localRotation,
            targetRotation,
            Time.deltaTime * 5f // velocidad del tilt
        );
    }

}

