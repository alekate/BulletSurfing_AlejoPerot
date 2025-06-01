using UnityEngine;

public class NewSkateMovement : MonoBehaviour
{
    [Header("Skate Settings")]
    [SerializeField] private float acceleration = 2.0f;
    public float maxSpeed;
    [SerializeField] private float friction;
    [SerializeField] private float brakeForce = 20f;
    public float currentSpeed;

    [Header("Camera")]
    [SerializeField] private Camera mainCamera;

    private Rigidbody rb;
    private Vector3 moveDirection;
    private bool isSkating;
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

            if (Input.GetMouseButton(1) && currentSpeed > 0.1f)
            {
                Vector3 brakeDirection = -rb.velocity.normalized;
                rb.AddForce(brakeDirection * brakeForce, ForceMode.Acceleration);
            }
        }
    }

    private void Move()
    {
        if (isSkating && currentSpeed < maxSpeed)
        {
            Vector3 desiredDirection = moveDirection.normalized;
            rb.AddForce(desiredDirection * acceleration, ForceMode.Acceleration);
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
