using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class NewSkateMovement : MonoBehaviour
{
    [Header("Speed & Movement")]
    public float maxSpeed = 20f;
    [SerializeField] private float minSpeed = 2f;
    [SerializeField] private float groundDrag = 1f;
    [SerializeField] private float airDrag = 0.2f;
    [SerializeField] private float stopThreshold = 0.1f;

    [Header("Braking")]
    public bool stop;
    [SerializeField] private float brakeForce = 20f;

    [Header("Over-Speed Handling")]
    [Tooltip("Fuerza con la que el patín intenta frenar hasta maxSpeed si vas demasiado rápido")]
    [SerializeField] private float overSpeedDrag = 10f;

    [Header("Steering & Charging (Clic Izquierdo)")]
    [Tooltip("Qué tan rápido gira el patín hacia la cámara al mantener un solo clic")]
    [SerializeField] private float steerSharpness = 8f;
    [SerializeField] private float maxChargeTime = 1.5f;
    [SerializeField] private float minImpulse = 5f;
    [SerializeField] private float maxImpulse = 20f;

    [Header("Jumping (Clic Derecho)")]
    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private float doubleClickThreshold = 0.3f; // Tiempo máximo entre clics para saltar/cargar

    [Header("State (For External Scripts)")]
    public bool isSkating;
    public bool IsCharging => isCharging;

    [Header("References")]
    [SerializeField] private Animator armAnimator;
    [SerializeField] private CameraMove cameraController;
    [SerializeField] private Original_PlayerGrind playerGrind;
    [SerializeField] private SoundController soundController;

    [Header("Cheats")]
    public bool isNoClip = false;
    [SerializeField] private float noClipSpeed = 15f;

    public float currentSpeed
    {
        get
        {
            Vector3 flatVel = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
            return flatVel.magnitude;
        }
    }

    private Rigidbody rb;
    private Collider playerCollider;

    // Variables de control de estado
    private float chargeStartTime;
    private bool isCharging;
    private bool isSteering;

    // Temporizadores separados para no confundir clics derechos e izquierdos
    private float lastLeftClickTime = -1f;
    private float lastRightClickTime = -1f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        playerCollider = GetComponent<Collider>();
        if (playerGrind == null) playerGrind = GetComponent<Original_PlayerGrind>();
    }

    private void Update()
    {
        UpdateArmAnimations();
    }

    private void FixedUpdate()
    {
        if (playerGrind != null)
            playerGrind.grindSpeed = currentSpeed;

        HandleNoClip();

        if (isNoClip) return;

        UpdateDrag();
        ApplyBraking();
        UpdatePlayerRotation();
        HandleSteering();
        LimitSpeed();
        FreezeIfTooSlow();
    }

    private void UpdateArmAnimations()
    {
        if (armAnimator == null) return;

        bool isMoving = currentSpeed > 0.1f;
        bool grounded = IsGrounded();

        armAnimator.SetBool("isGrounded", grounded);

        if (isCharging && grounded)
        {
            armAnimator.SetBool("isPushing", true);
            armAnimator.SetBool("isSkating", false);
        }
        else if (isMoving && grounded)
        {
            armAnimator.SetBool("isPushing", false);
            armAnimator.SetBool("isSkating", true);
        }
        else
        {
            armAnimator.SetBool("isPushing", false);
            armAnimator.SetBool("isSkating", false);
        }
    }

    private bool IsGrounded()
    {
        return Physics.Raycast(transform.position + Vector3.up * 0.1f, Vector3.down, 1.2f);
    }

    private void UpdateDrag()
    {
        rb.linearDamping = IsGrounded() ? groundDrag : airDrag;
    }

    private void ApplyBraking()
    {
        if (stop && IsGrounded() && currentSpeed > stopThreshold)
        {
            Vector3 brakeDirection = -new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z).normalized;
            rb.AddForce(brakeDirection * brakeForce, ForceMode.Acceleration);
        }
    }

    private void LimitSpeed()
    {
        Vector3 flatVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);

        if (flatVelocity.magnitude > maxSpeed)
        {
            float overSpeedAmount = flatVelocity.magnitude - maxSpeed;
            Vector3 brakeDirection = -flatVelocity.normalized;
            rb.AddForce(brakeDirection * (overSpeedAmount * overSpeedDrag), ForceMode.Acceleration);
        }
    }

    private void FreezeIfTooSlow()
    {
        if (IsGrounded() && currentSpeed < stopThreshold)
        {
            rb.linearVelocity = new Vector3(0f, rb.linearVelocity.y, 0f);
        }
    }

    private void HandleSteering()
    {
        if (isSteering && IsGrounded() && !isCharging && currentSpeed > stopThreshold)
        {
            Vector3 targetDir = GetCameraForwardDirection();
            Vector3 currentVel = rb.linearVelocity;
            Vector3 flatVel = new Vector3(currentVel.x, 0f, currentVel.z);
            float currentMag = flatVel.magnitude;

            Vector3 newDir = Vector3.Lerp(flatVel.normalized, targetDir, Time.fixedDeltaTime * steerSharpness).normalized;
            rb.linearVelocity = new Vector3(newDir.x * currentMag, currentVel.y, newDir.z * currentMag);
        }
    }

    // --- CLIC IZQUIERDO: Dirección (1 clic) o Carga (Doble clic) ---
    public void OnSteerChargePressed()
    {
        if (Time.time - lastLeftClickTime <= doubleClickThreshold)
        {
            StartCharge();
            isSteering = false;
        }
        else
        {
            isSteering = true;
            isCharging = false;
        }

        lastLeftClickTime = Time.time;
    }

    public void OnSteerChargeReleased()
    {
        if (isCharging)
        {
            ReleaseCharge();
        }
        isSteering = false;
    }

    public float GetChargePercent()
    {
        if (!isCharging) return 0f;
        float heldTime = Time.time - chargeStartTime;
        return Mathf.Clamp01(heldTime / maxChargeTime);
    }

    private void StartCharge()
    {
        if (!IsGrounded() || (playerGrind != null && playerGrind.onRail)) return;
        isSkating = true;
        isCharging = true;
        chargeStartTime = Time.time;
    }

    private void ReleaseCharge()
    {
        if (!isCharging) return;
        float chargePercent = GetChargePercent();
        isCharging = false;
        isSkating = false;

        float finalImpulse = Mathf.Lerp(minImpulse, maxImpulse, chargePercent);
        Vector3 pushDirection = GetCameraForwardDirection();
        rb.AddForce(pushDirection * finalImpulse, ForceMode.VelocityChange);

        if (cameraController != null) cameraController.ApplyPushFeedback(chargePercent);
    }

    // --- CLIC DERECHO: Salto (Doble clic) ---
    // (El freno se sigue leyendo desde InputController con JumpBrake.IsPressed())
    public void Jump()
    {
        if (Time.time - lastRightClickTime <= doubleClickThreshold)
        {
            if (playerGrind != null && playerGrind.onRail)
            {
                playerGrind.ThrowOffRail();
            }
            else if (IsGrounded())
            {
                rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
                rb.AddForce(Vector3.up * jumpForce, ForceMode.VelocityChange);
                if (soundController != null) soundController.JumpSFX();
            }
            // Reseteamos el tiempo para que no haga un salto triple si haces clic muy rápido
            lastRightClickTime = -1f;
        }
        else
        {
            lastRightClickTime = Time.time;
        }
    }

    private void UpdatePlayerRotation()
    {
        if (cameraController == null) return;
        Vector3 camEuler = cameraController.transform.eulerAngles;
        transform.rotation = Quaternion.Euler(0f, camEuler.y, 0f);
    }

    private Vector3 GetCameraForwardDirection()
    {
        if (cameraController == null) return transform.forward;
        Vector3 forward = cameraController.transform.forward;
        forward.y = 0;
        return forward.normalized;
    }

    private void HandleNoClip()
    {
        if (!isNoClip) return;
        Vector3 direction = cameraController.transform.forward;
        transform.Translate(direction.normalized * noClipSpeed * Time.deltaTime, Space.World);
    }

    public void ToggleNoClip()
    {
        isNoClip = !isNoClip;
        rb.useGravity = !isNoClip;
        rb.isKinematic = isNoClip;
        if (playerCollider != null) playerCollider.enabled = !isNoClip;
        if (isNoClip) rb.linearVelocity = Vector3.zero;
    }
}