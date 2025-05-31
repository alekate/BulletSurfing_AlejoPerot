using UnityEngine;

public class GroundAlignment : MonoBehaviour
{
    [SerializeField] private Transform raycastOrigin;
    [SerializeField] private Transform bodyToRotate;
    [SerializeField] private float raycastDistance = 3f;
    [SerializeField] private float rotationSpeed = 5f;

    private Quaternion smoothTilt = Quaternion.identity;

    private void FixedUpdate()
    {
        if (Physics.Raycast(raycastOrigin.position, Vector3.down, out RaycastHit hit, raycastDistance))
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
}
