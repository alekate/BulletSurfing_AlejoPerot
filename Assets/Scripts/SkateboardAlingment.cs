using UnityEngine;

public class SkateboardAlignment : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private NewSkateMovement skateMovement;
    [SerializeField] private Transform raycastOrigin;
    [SerializeField] private Transform bodyToRotate;
    [SerializeField] private Transform orientationReference;
    [SerializeField] private LayerMask ignoredLayers;
    [SerializeField] private Transform playerTarget;

    [Header("Settings")]
    [SerializeField] private float raycastDistance = 3f;
    [SerializeField] private float rotationSpeed = 10f;
    [SerializeField] private int rayCount = 5;
    [SerializeField] private float raySpacing = 0.5f;
    [SerializeField] private Vector3 positionOffset;

    private Rigidbody playerRb;

    private void Start()
    {
        // Obtenemos automáticamente el Rigidbody del jugador desde el script de movimiento
        if (skateMovement != null)
        {
            playerRb = skateMovement.GetComponent<Rigidbody>();
        }
    }

    private void Update()
    {
        if (playerTarget != null)
        {
            transform.position = playerTarget.position + positionOffset;
        }
    }

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
            AlignBoard(averageNormal);
        }
    }

    private void AlignBoard(Vector3 groundNormal)
    {
        Vector3 currentForward;

        if (playerRb != null && playerRb.linearVelocity.sqrMagnitude > 0.1f)
        {
            currentForward = playerRb.linearVelocity.normalized;
        }
        else
        {
            // Si está quieto, mantiene el frente que ya tenía
            currentForward = bodyToRotate.forward;
        }

        /*if (skateMovement.isSkating)
        {
            // Mira hacia donde apunta la cámara
            //currentForward = orientationReference.forward;
        }
        else
        {
            // Mira hacia la dirección real del movimiento (inercia)
            // Usamos sqrMagnitude > 0.1f para asegurarnos de que solo rote si se está moviendo lo suficiente
            if (playerRb != null && playerRb.linearVelocity.sqrMagnitude > 0.1f)
            {
                currentForward = playerRb.linearVelocity.normalized;
            }
            else
            {
                // Si está quieto, mantiene el frente que ya tenía
                currentForward = bodyToRotate.forward;
            }
        }*/

        // Proyecta el vector hacia adelante sobre el plano del suelo para que no se clave en el piso ni se levante
        Vector3 projectedForward = Vector3.ProjectOnPlane(currentForward, groundNormal).normalized;

        if (projectedForward != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(projectedForward, groundNormal);
            bodyToRotate.rotation = Quaternion.Slerp(bodyToRotate.rotation, targetRotation, Time.fixedDeltaTime * rotationSpeed);
        }
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