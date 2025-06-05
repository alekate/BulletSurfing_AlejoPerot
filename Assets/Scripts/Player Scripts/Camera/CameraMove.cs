using UnityEngine;

public class CameraMove : MonoBehaviour
{
    [Header("Sensibilidad del mouse")]
    public float sensX = 1f;
    public float sensY = 1f;

    [Header("Inclinación")]
    public float turnTiltMultiplier = 3f; 
    [SerializeField] private Transform cameraHolder;

    private Vector2 turn; 
    private float mouseXDelta;

    [SerializeField] private bool haveTilt = false;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        CameraMovement();
        UpdateCameraTilt();
    }

    void CameraMovement()
    {
        float mouseX = Input.GetAxis("Mouse X") * sensX;
        float mouseY = Input.GetAxis("Mouse Y") * sensY;

        mouseXDelta = mouseX; 

        turn.x += mouseX;
        turn.y += mouseY;

        turn.y = Mathf.Clamp(turn.y, -35f, 35f);

        transform.localRotation = Quaternion.Euler(-turn.y, turn.x, 0);
    }

    void UpdateCameraTilt()
    {
        if (haveTilt == true)
        {
            float tiltAngle = -mouseXDelta * turnTiltMultiplier;

            Quaternion targetRotation = Quaternion.Euler(
                cameraHolder.localEulerAngles.x,
                cameraHolder.localEulerAngles.y,
                tiltAngle
            );

            cameraHolder.localRotation = Quaternion.Lerp(
                cameraHolder.localRotation,
                targetRotation,
                Time.deltaTime * 5f
            );
        }
    }
}
