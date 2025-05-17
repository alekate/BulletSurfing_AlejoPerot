using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SkateMovement : MonoBehaviour
{
    [SerializeField] private float acceleration = 2.0f; // Velocidad de aceleración
    [SerializeField] private float maxSpeed = 24.0f; // Velocidad máxima (Maximo hasta que Unity no se la banca)
    [SerializeField] private float friction = 0.02f; // Fricción para reducir la velocidad
    [SerializeField] private Camera mainCamera; // Referencia a la cámara principal

    [SerializeField] private TextMeshProUGUI velocityText; // Componente TextMeshProUGUI para mostrar la velocidad

    [SerializeField] private Transform cameraHolder; // Referencia al objeto "camera holder" que sostiene la cámara

    private Vector3 moveDirection; // Dirección de movimiento
    private float currentSpeed; // Velocidad actual del jugador
    private bool isSkating; // Indica si el jugador está patinando

    float horizontalMouseMovementValue;

    void FixedUpdate()
    {
        PlayerInput(); // Gestiona la entrada del usuario
        Move(); // Controla el movimiento del jugador
        UpdateCameraTilt(); // Actualiza el tilt de la cámara
        velocityText.text = Mathf.Ceil(currentSpeed).ToString(); // Muestra la velocidad redondeada en el texto

        horizontalMouseMovementValue = Input.GetAxis("Mouse X"); //Toma los valores del Mouse y los almacena

    }

    private void PlayerInput()
    {
        if (Input.GetMouseButton(1)) // Verifica si se mantiene presionado el botón derecho del mouse
        {
            isSkating = true; // El jugador está patinando


            if (horizontalMouseMovementValue != 0)
            {
                moveDirection = GetCameraForwardDirection(); // Obtiene la dirección hacia adelante de la cámara
                currentSpeed += acceleration * Time.deltaTime;
                currentSpeed = Mathf.Min(currentSpeed, maxSpeed); // Limita la velocidad a la máxima permitida
            }
        }
        else if (Input.GetMouseButton(0)) // Solo cuando se suelta el botón derecho
        {
            isSkating = false;
        }

        Debug.Log(isSkating);

    }

    private void Move()
    {
        if (currentSpeed > 0 && isSkating)
        {
            // Mueve al jugador en la dirección determinada por la cámara
            transform.Translate(moveDirection * currentSpeed * Time.deltaTime, Space.World);
        }

        if (!isSkating && currentSpeed > 0)
        {

            // Aplica fricción solo cuando no se está patinando
            currentSpeed *= 1 - (friction * Time.deltaTime);

            // Reduce suavemente la velocidad a 0
            if (currentSpeed < 0.01f)
            {
                currentSpeed = 0f; // Detiene el movimiento si la velocidad es muy baja
            }
        }

        Debug.Log($"Dirección: {moveDirection}, Velocidad: {currentSpeed}");

    }

    private void UpdateCameraTilt()
    {
        if (cameraHolder != null)
        {
            float tiltAngle = horizontalMouseMovementValue * 10f; // Ajusta este multiplicador para controlar la velocidad del tilt

            // Rota el objeto "camera holder" alrededor de su eje hacia arriba basado en el movimiento horizontal del mouse
            cameraHolder.Rotate(Vector3.up, tiltAngle, Space.Self);
        }
    }

    private Vector3 GetCameraForwardDirection()
    {
        Vector3 cameraForward = mainCamera.transform.forward;
        cameraForward.y = 0; // Asegura que el movimiento sea solo en el plano horizontal
        cameraForward.Normalize();
        return cameraForward;
    }
}
