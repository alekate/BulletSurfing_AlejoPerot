using UnityEngine;

public class SkateMovement : MonoBehaviour
{
    [Header("Skate Settings")]
    [SerializeField] private float acceleration = 2.0f;
    public float maxSpeed;
    [SerializeField] private float friction;
    [SerializeField] private float brakeForce = 20f;
    public float currentSpeed;

    [Header("Mouse Input")]
    [SerializeField] private float mouseYawTolerance;
    [SerializeField] private float movementThresholdToAccelerate;
    [SerializeField] private float decreasingFactor = 0f;
    public float horizontalMouseMovementValue { get; private set; }
    [SerializeField] private float debugMouseX;

    [Header("Camera")]
    [SerializeField] private Camera mainCamera;

    private Rigidbody rb;
    private Vector3 moveDirection;
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

        currentSpeed = Mathf.Max(currentSpeed, 0f);
    }

    private void PlayerInput()
    {
        if (Input.GetMouseButton(0))
        {
            isSkating = true;
            moveDirection = GetCameraForwardDirection();

            if (Mathf.Abs(horizontalMouseMovementValue) > movementThresholdToAccelerate)
            {
                currentSpeed += acceleration * Time.deltaTime;
                currentSpeed = Mathf.Min(currentSpeed, maxSpeed);
            }

            if (Mathf.Abs(horizontalMouseMovementValue) > mouseYawTolerance)
            {
                DecreaseVelocity();
            }
        }
        else
        {
            isSkating = false;
        }
    }

    private void DecreaseVelocity()
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
            Vector3 frictionForce = -flatVelocity.normalized * friction;
            rb.AddForce(frictionForce, ForceMode.Acceleration);
        }

        if (Input.GetMouseButton(1) && currentVelocityMagnitude > 0.1f)
        {
            Vector3 brakeDirection = -flatVelocity.normalized;
            rb.AddForce(brakeDirection * brakeForce, ForceMode.Acceleration);
        }

        if (currentVelocityMagnitude > maxSpeed)
        {
            Vector3 limitedVelocity = flatVelocity.normalized * maxSpeed;
            rb.velocity = new Vector3(limitedVelocity.x, rb.velocity.y, limitedVelocity.z);
        }

        currentSpeed = rb.velocity.magnitude;
    }

    private Vector3 GetCameraForwardDirection()
    {
        Vector3 cameraForward = mainCamera.transform.forward;
        cameraForward.y = 0;
        return cameraForward.normalized;
    }

    public void SpeedCheat()
    {
        currentSpeed += 10;
    }
}

