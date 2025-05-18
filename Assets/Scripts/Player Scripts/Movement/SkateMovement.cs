using UnityEngine;
using TMPro;
using Unity.Collections;

public class SkateMovement : MonoBehaviour
{
    [Header("Skate Settings")]
    [SerializeField] private float acceleration = 2.0f;
    public float maxSpeed;
    [SerializeField] private float friction;

    [Header("Mouse Input")]
    [SerializeField] private float mouseYawTolerance;
    [SerializeField] private float movementThresholdToAccelerate;
    [SerializeField] private float decreasingFactor = 0f;
    public float horizontalMouseMovementValue { get; private set; }
    [SerializeField, ReadOnly] private float debugMouseX;


    [Header("Camera")]
    [SerializeField] private Camera mainCamera;

    [Header("Ground Alignment")]
    [SerializeField] private Transform raycastOrigin; // Desde dónde tirar el raycast
    [SerializeField] private Transform bodyToRotate;  // Qué parte del jugador se inclina
    [SerializeField] private float raycastDistance = 3f;
    [SerializeField] private float rotationSpeed = 5f;

    // --- Runtime ---
    private Rigidbody rb;
    private Quaternion smoothTilt = Quaternion.identity;
    private Vector3 moveDirection;
    public float currentSpeed;
    private bool isSkating;

   private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }


    private void FixedUpdate()
    {
        horizontalMouseMovementValue = Input.GetAxis("Mouse X");
        debugMouseX = horizontalMouseMovementValue;

        PlayerInput();
        Move();
        AlignToGround();

        if(Input.GetKeyDown(KeyCode.Q))
        {
            currentSpeed += 10;
        }

        currentSpeed = Mathf.Max(currentSpeed, 0f);

    }


    private void PlayerInput()
    {
        if (Input.GetMouseButton(0))
        {
            isSkating = true;

            if (Mathf.Abs(horizontalMouseMovementValue) != 0 && Mathf.Abs(horizontalMouseMovementValue) > movementThresholdToAccelerate)
            {
                moveDirection = GetCameraForwardDirection();
                currentSpeed += acceleration * Time.deltaTime;
                currentSpeed = Mathf.Min(currentSpeed, maxSpeed);
            }

            if (Mathf.Abs(horizontalMouseMovementValue) > mouseYawTolerance)
            {
                DecreaseVelocity(); // Penalizás movimientos bruscos
            }
        }
        else
        {
            isSkating = false;
        }
    }


    private void DecreaseVelocity() //Por movimiento bruzco del mouse
    {
        Debug.Log(horizontalMouseMovementValue);
        currentSpeed -= decreasingFactor;
    }

    private void Move()
    {
        Vector3 flatVelocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
        float currentVelocityMagnitude = flatVelocity.magnitude;

        if (isSkating && currentVelocityMagnitude < maxSpeed)
        {
            Vector3 desiredDirection = moveDirection.normalized;
            rb.AddForce(desiredDirection * acceleration, ForceMode.Acceleration);
        }
        else if (!isSkating && currentVelocityMagnitude > 0)
        {
            // Simular fricción cuando no se patina
            Vector3 frictionForce = -flatVelocity.normalized * friction;
            rb.AddForce(frictionForce, ForceMode.Acceleration);
        }

        // Limitar velocidad máxima manualmente
        if (currentVelocityMagnitude > maxSpeed)
        {
            Vector3 limitedVelocity = flatVelocity.normalized * maxSpeed;
            rb.velocity = new Vector3(limitedVelocity.x, rb.velocity.y, limitedVelocity.z);
        }

        currentSpeed = rb.velocity.magnitude;
    }

    private void AlignToGround()
    {
        RaycastHit hit;
        if (Physics.Raycast(raycastOrigin.position, Vector3.down, out hit, raycastDistance))
        {
            Quaternion groundTilt = Quaternion.FromToRotation(Vector3.up, hit.normal);
            smoothTilt = Quaternion.Slerp(smoothTilt, groundTilt, Time.deltaTime * rotationSpeed);

            Quaternion newRotation = Quaternion.Euler(
                smoothTilt.eulerAngles.x,
                bodyToRotate.rotation.eulerAngles.y,
                smoothTilt.eulerAngles.z
            );

            bodyToRotate.rotation = newRotation;
        }
    }

    private Vector3 GetCameraForwardDirection()
    {
        Vector3 cameraForward = mainCamera.transform.forward;
        cameraForward.y = 0;
        return cameraForward.normalized;
    }
}
