using UnityEngine;

public class NewSkateMovement : MonoBehaviour
{
    [Header("Skate Settings")]
    [SerializeField] private float acceleration = 2.0f;
    [SerializeField] private float airAcceleration;
    public float maxSpeed;
    public float minSpeed;
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
    public bool isNoClip = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        PlayerInput();
        Move();

        if (currentSpeed < 0.1f)
        {
            currentSpeed = 0f;
        }

        //No Clip Cheat
        if (isNoClip)
        {
            if (Input.GetMouseButton(0)) 
            {
                Vector3 direction = mainCamera.transform.forward;
                float upDown = 0;

                if (Input.GetKey(KeyCode.E)) upDown = 1f;
                else if (Input.GetKey(KeyCode.Q)) upDown = -1f;

                direction += Vector3.up * upDown;

                transform.Translate(direction.normalized * maxSpeed * Time.deltaTime, Space.World);
            }

            return;
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

            if (!IsGrounded() && currentSpeed < minSpeed)
            {
                Vector3 horizontalVelocity = rb.velocity;
                horizontalVelocity.y = 0;

                Vector3 newVelocity = horizontalVelocity.normalized * minSpeed;
                rb.velocity = new Vector3(newVelocity.x, rb.velocity.y, newVelocity.z);
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

    public void ToggleNoClip()
    {
        isNoClip = !isNoClip;

        if (isNoClip)
        {
            rb.useGravity = false;
            rb.velocity = Vector3.zero;
            rb.isKinematic = true;

            Collider col = GetComponent<Collider>();
            if (col != null)
                col.enabled = false;
        }
        else
        {
            rb.useGravity = true;
            rb.isKinematic = false;

            Collider col = GetComponent<Collider>();
            if (col != null)
                col.enabled = true;
        }
    }

}
