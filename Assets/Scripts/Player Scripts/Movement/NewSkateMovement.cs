using UnityEngine;

public class NewSkateMovement : MonoBehaviour
{
    [Header("Skate Settings")]
    [SerializeField] private float acceleration = 2.0f;
    [SerializeField] private float airAcceleration;
    public float maxSpeed;
    [SerializeField] private float friction;
    [SerializeField] private float brakeForce = 20f;
    public float currentSpeed;

    [Header("Jump Settings")]
    [SerializeField] private float jumpForce = 5f;

    [Header("Camera")]
    [SerializeField] private Camera mainCamera;

    private Rigidbody rb;
    private Vector3 moveDirection;
    public bool isSkating;
    public bool hasInput = true;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        PlayerInput();
        Move();

        // Clamp currentSpeed a 0 si es muy baja
        if (currentSpeed < 0.1f)
        {
            currentSpeed = 0f;
        }
    }

    private bool IsGrounded()
    {
        return Physics.Raycast(transform.position, Vector3.down, 1.1f);
    }

    private void PlayerInput()
    {
        if (hasInput == true)
        {
            if (Input.GetMouseButton(0))
            {
                isSkating = true;
                moveDirection = GetCameraForwardDirection();
            }
            else
            {
                isSkating = false;
            }

            if (Input.GetMouseButtonDown(1) && IsGrounded())
            {
                rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            }

            if (Input.GetMouseButton(1) && currentSpeed > 0.1f)
            {
                Vector3 brakeDirection = -rb.velocity.normalized;
                rb.AddForce(brakeDirection * brakeForce, ForceMode.Acceleration);
            }
        }
    }


    private void Move()
    {
        float currentAcceleration = IsGrounded() ? acceleration : airAcceleration;

        if (isSkating && currentSpeed < maxSpeed)
        {
            Vector3 desiredDirection = moveDirection.normalized;
            rb.AddForce(desiredDirection * currentAcceleration, ForceMode.Acceleration);
        }
        else if (!isSkating && currentSpeed > 0)
        {
            Vector3 frictionForce = rb.velocity.normalized * friction;
            rb.AddForce(frictionForce, ForceMode.Acceleration);
        }

        if (currentSpeed > maxSpeed)
        {
            Vector3 limitedVelocity = rb.velocity.normalized * maxSpeed;
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
