using UnityEngine;
using TMPro;

public class SkateMovement : MonoBehaviour
{
    [SerializeField] private float acceleration = 2.0f;
    public float maxSpeed;
    //[SerializeField] private float friction;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private TextMeshProUGUI velocityText;

    [SerializeField] private Transform raycastOrigin; // Un transform desde donde tirar el raycast
    [SerializeField] private Transform bodyToRotate; // Qué parte del jugador querés inclinar
    [SerializeField] private float raycastDistance = 3f;
    [SerializeField] private float rotationSpeed = 5f;

    private Quaternion smoothTilt = Quaternion.identity;

    private Vector3 moveDirection;
    public float currentSpeed;
    private bool isSkating;

    [SerializeField] private float mouseYawTolerance;
    [SerializeField] private float decreasingFactor = 0f;
    public float horizontalMouseMovementValue { get; private set; }

    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();

    }

    private void FixedUpdate()
    {
        horizontalMouseMovementValue = Input.GetAxis("Mouse X");
        PlayerInput();
        Move();
        AlignToGround();
        UpdateVelocityUI();

        if(Input.GetKeyDown(KeyCode.Q))
        {
            currentSpeed += 10;
        }

        currentSpeed = Mathf.Max(currentSpeed, 0f);

    }


    private void PlayerInput()
    {
        if (Input.GetMouseButton(0)) // Botón izquierdo - patinar
        {
            isSkating = true;

            if (horizontalMouseMovementValue != 0)
            {
                moveDirection = GetCameraForwardDirection();
                currentSpeed += acceleration * Time.deltaTime;
                currentSpeed = Mathf.Min(currentSpeed, maxSpeed);

                if (Mathf.Abs(horizontalMouseMovementValue) > mouseYawTolerance)
                {
                    DecreaseVelocity(); // Imaginando que frena si te movés mucho el mouse
                }
            }
        }
        else
        {
            isSkating = false;
        }

        if (Input.GetMouseButton(1)) // Botón derecho - frenar
        {
            currentSpeed -= acceleration * 2f * Time.deltaTime; // Frenar más rápido
            currentSpeed = Mathf.Max(currentSpeed, 0); // No dejar que baje de 0
        }

    }
 

    private void DecreaseVelocity() //Por movimiento bruzco del mouse
    {
        Debug.Log(horizontalMouseMovementValue);
        currentSpeed -= decreasingFactor;
    }
    private void Move()
    {
        if (isSkating && currentSpeed > 0)
        {
            Vector3 movement = moveDirection * currentSpeed * Time.fixedDeltaTime;
            rb.MovePosition(rb.position + movement);
        }
        else if (!isSkating && currentSpeed > 0)
        {
            //currentSpeed *= 1 - (friction * Time.fixedDeltaTime);

            if (currentSpeed < 0.01f)
            {
                currentSpeed = 0f;
            }

            /*Vector3 frictionMovement = moveDirection * currentSpeed * Time.fixedDeltaTime;
            rb.MovePosition(rb.position + frictionMovement);*/
        }
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


    private void UpdateVelocityUI()
    {
        if (velocityText != null)
        if (velocityText != null)
        {
            velocityText.text = Mathf.Ceil(currentSpeed).ToString();
        }
    }

    private Vector3 GetCameraForwardDirection()
    {
        Vector3 cameraForward = mainCamera.transform.forward;
        cameraForward.y = 0;
        return cameraForward.normalized;
    }
}
