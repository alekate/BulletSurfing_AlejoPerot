using UnityEngine;

public class SkateboardAlingment : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private NewSkateMovement skateMovement;
    [SerializeField] private Transform raycastOrigin;
    [SerializeField] private Transform bodyToRotate;
    [SerializeField] private Transform orientationReference;
    [SerializeField] private LayerMask ignoredLayers;

    [Header("Settings")]
    [SerializeField] private float raycastDistance = 3f;
    [SerializeField] private float rotationSpeed = 5f;
    [SerializeField] private int rayCount = 5;
    [SerializeField] private float raySpacing = 0.5f;

    private Quaternion smoothTilt = Quaternion.identity;

    private void FixedUpdate()
    {
        Vector3 totalNormal = Vector3.zero;
        int hitCount = 0;

        for (int i = 0; i < rayCount; i++)
        {
            float offset = (i - (rayCount - 1) / 2f) * raySpacing;
            Vector3 localOffset = new Vector3(0, 0, offset);
            Vector3 worldOrigin = raycastOrigin.TransformPoint(localOffset);

            if (Physics.Raycast(worldOrigin, Vector3.down, out RaycastHit hit, raycastDistance, ~ignoredLayers))
            {
                totalNormal += hit.normal;
                hitCount++;
            }
        }

        if (hitCount > 0)
        {
            Vector3 averageNormal = (totalNormal / hitCount).normalized;
            AlignBoardToGround(averageNormal);

            if (skateMovement.isSkating)
            {
                AlignBoardToCamera(averageNormal);
            }
        }
    }

    private void AlignBoardToGround(Vector3 groundNormal)
    {
        Quaternion groundTilt = Quaternion.FromToRotation(Vector3.up, groundNormal);
        smoothTilt = Quaternion.Slerp(smoothTilt, groundTilt, Time.fixedDeltaTime * rotationSpeed);
        // Solo inclina, no rota hacia adelante
    }

    private void AlignBoardToCamera(Vector3 groundNormal)
    {
        Vector3 forward = orientationReference.forward;
        Vector3 projectedForward = Vector3.ProjectOnPlane(forward, groundNormal).normalized;
        Quaternion targetRotation = Quaternion.LookRotation(projectedForward, groundNormal);
        bodyToRotate.rotation = Quaternion.Slerp(bodyToRotate.rotation, targetRotation, Time.fixedDeltaTime * rotationSpeed);
    }

    private void OnDrawGizmosSelected()
    {
        if (raycastOrigin == null) return;

        Gizmos.color = Color.green;

        for (int i = 0; i < rayCount; i++)
        {
            float offset = (i - (rayCount - 1) / 2f) * raySpacing;
            Vector3 localOffset = new Vector3(0, 0, offset);
            Vector3 worldOrigin = raycastOrigin.TransformPoint(localOffset);

            Gizmos.DrawLine(worldOrigin, worldOrigin + Vector3.down * raycastDistance);
        }
    }
}
