using UnityEngine;
using TMPro;

public class SkateMovementKeyboard : MonoBehaviour
{
    [SerializeField] private float acceleration = 2.0f;
    public float maxSpeed;
    [SerializeField] private TextMeshProUGUI velocityText;

    [SerializeField] private Transform raycastOrigin;
    [SerializeField] private Transform bodyToRotate;
    [SerializeField] private float raycastDistance = 3f;
    [SerializeField] private float rotationSpeed = 5f;

    private Quaternion smoothTilt = Quaternion.identity;

    private float horizontalInput;
    public float currentSpeed;
    private bool isSkating;

    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        PlayerInput();
        Move();
        AlignToGround();
        UpdateVelocityUI();
    }

    private void PlayerInput()
    {
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D))
        {
            isSkating = true;

            // A = -1, D = 1
            horizontalInput = Input.GetKey(KeyCode.D) ? 1f : -1f;

            currentSpeed += acceleration * Time.deltaTime;
            currentSpeed = Mathf.Min(currentSpeed, maxSpeed);
        }
        else
        {
            isSkating = false;
        }

        if (Input.GetKey(KeyCode.Q))
        {
            currentSpeed += 10;
        }

        currentSpeed = Mathf.Max(currentSpeed, 0f);
    }

    private void Move()
    {
        if (isSkating && currentSpeed > 0)
        {
            Vector3 movement = transform.forward * currentSpeed * Time.fixedDeltaTime;

            if (horizontalInput != 0)
            {
                Vector3 right = transform.right;
                movement += right * horizontalInput * currentSpeed * Time.fixedDeltaTime;
            }

            rb.MovePosition(rb.position + movement);
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
        {
            velocityText.text = Mathf.Ceil(currentSpeed).ToString();
        }
    }
}
