using UnityEngine;

public class NewSkateMovement : MonoBehaviour
{
    [Header("Skate Settings")]
    [SerializeField] private float acceleration = 2.0f;
    [SerializeField] private float clickImpulse = 0f;
    [SerializeField] private float airAcceleration;
    public float maxSpeed;
    public float minSpeed;
    [SerializeField] private float friction;
    [SerializeField] private float movingFrictionMultiplier = 0.3f;
    [SerializeField] private float brakeForce = 20f;
    public float currentSpeed;
    [SerializeField] private Original_PlayerGrind Original_PlayerGrind;
    [SerializeField] private SoundController soundController;

    [Header("Charge Impulse")]
    [SerializeField] private float maxChargeTime = 2f;
    [SerializeField] private float minImpulse = 1f;
    [SerializeField] private float maxImpulse = 7f;
    [SerializeField] private float stationaryThreshold = 0.5f;
 




    private float chargeStartTime;
    private bool isCharging;

    [Header("Jump Settings")]
    [SerializeField] private float jumpForce = 3f;

    [Header("Camera")]
    [SerializeField] private Camera mainCamera;

    private Rigidbody rb;
    private Vector3 moveDirection;
    public bool isSkating;
    public bool hasInput = true;
    public bool isNoClip = false;

    [Header("DoubleClick")]
    private float lastRightClickTime = -1f;
    private float doubleClickThreshold = 0.3f;

    [Header("Stop Spam Checker")]
    [SerializeField] private float clickCooldown = 0.2f;
    private float lastLeftClickTime;

    public bool accelerate;
    public bool stop;


    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        Original_PlayerGrind = GetComponent<Original_PlayerGrind>();
    }

    //UPDATES///////////////////////////////////////////
    private void FixedUpdate()
    {
        Original_PlayerGrind.grindSpeed = currentSpeed;
        PlayerInput();
        Move();
        UpdatePlayerRotation();

        if (currentSpeed < 0.1f)
        {
            currentSpeed = 0f;
        }

        // No Clip Cheat
        if (isNoClip)
        {
            if (isSkating)
            {
                Vector3 direction = mainCamera.transform.forward;
                float upDown = 0;
                direction += Vector3.up * upDown;
                transform.Translate(direction.normalized * maxSpeed * Time.deltaTime, Space.World);
            }

            return;
        }

    }

    //////////////////////////////////////////////////

    private bool IsGrounded()
    {
        return Physics.Raycast(transform.position, Vector3.down, 1.1f);
    }

    private void PlayerInput()
    {
        if (hasInput && !Original_PlayerGrind.onRail)
        {
            if (isSkating)
            {
                moveDirection = GetCameraForwardDirection();
            }

            if (!IsGrounded() && currentSpeed < minSpeed)
            {
                Vector3 horizontalVelocity = rb.velocity;
                horizontalVelocity.y = 0;

                Vector3 newVelocity = horizontalVelocity.normalized * minSpeed;
                rb.velocity = new Vector3(newVelocity.x, rb.velocity.y, newVelocity.z);
            }

            if (!IsGrounded())
                return;

            if (stop && currentSpeed > 0.1f)
            {
                Vector3 brakeDirection = -rb.velocity.normalized;
                rb.AddForce(brakeDirection * brakeForce, ForceMode.Acceleration);
            }
        }
        else
        {
            isSkating = false;
        }
    }

    //MOVIMIENTO CON CLICKS IZQ////////////////////////////////////////
    public void ApplyClickImpulse()
    {
        if (!hasInput || Original_PlayerGrind.onRail)
            return;

        if (!IsGrounded())
            return;

        if (Time.time < lastLeftClickTime + clickCooldown)
            return; // todavía está en cooldown

        if (currentSpeed + clickImpulse > maxSpeed)
            return;

        Vector3 direction = GetCameraForwardDirection();
        rb.AddForce(direction * clickImpulse, ForceMode.Impulse);

        lastLeftClickTime = Time.time; // guardamos cuándo fue el click válido
    }

    public void StartCharge()
    {
        if (!IsGrounded() || Original_PlayerGrind.onRail)
            return;

        isCharging = true;
        chargeStartTime = Time.time;
    }

    public void ReleaseCharge()
    {
        if (!isCharging)
            return;

        isCharging = false;

        float heldTime = Time.time - chargeStartTime;
        float chargePercent = Mathf.Clamp01(heldTime / maxChargeTime);

        float finalImpulse = Mathf.Lerp(minImpulse, maxImpulse, chargePercent);

        Vector3 direction = GetCameraForwardDirection();
        rb.AddForce(direction * finalImpulse, ForceMode.Impulse);
    }

    ///////////////////////////////////////////////////////////////

    //SALTO////////////////////////////////////////
    public void Jump()
    {
        float timeSinceLastClick = Time.time - lastRightClickTime;

        if (timeSinceLastClick <= doubleClickThreshold)
        {
            if (Original_PlayerGrind.onRail)
            {
                Original_PlayerGrind.ThrowOffRail();
                Debug.Log("Salto del rail");
            }
            else if (IsGrounded())
            {
                rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
                soundController.JumpSFX();
                Debug.Log("Salto normal");
            }

            lastRightClickTime = -1f; // reset
        }
        else
        {
            lastRightClickTime = Time.time;
        }
    }
    ///////////////////////////////////////////////////////////////

    //MOVIMIENTO MANTENIENDO CLICK IZQ////////////////////////////////////////
    private void Move()
    {
        float currentAcceleration = IsGrounded() ? acceleration : airAcceleration;

        /*if (isSkating && currentSpeed < maxSpeed)
        {
            Vector3 desiredDirection = moveDirection.normalized;
            rb.AddForce(desiredDirection * currentAcceleration, ForceMode.Acceleration);
        }*/
        if (!isSkating && currentSpeed > 0)
        {
            float appliedFriction = friction;

            if (currentSpeed > stationaryThreshold)
            {
                appliedFriction *= movingFrictionMultiplier;
                // Si ya está en movimiento, pierde menos velocidad
            }

            Vector3 frictionForce = -rb.velocity.normalized * appliedFriction;
            rb.AddForce(frictionForce, ForceMode.Acceleration);
        }


        if (currentSpeed > maxSpeed)
        {
            Vector3 limitedVelocity = rb.velocity.normalized * maxSpeed;
            rb.velocity = new Vector3(limitedVelocity.x, rb.velocity.y, limitedVelocity.z);
        }

        currentSpeed = rb.velocity.magnitude;
    }
    ////////////////////////////////////////////////////
    
    //ROTACION DE PLAYER Y DIRECCION DE CAMARA///////////////////////////////////////////
    private void UpdatePlayerRotation()
    {
        transform.rotation = new Quaternion(
            mainCamera.transform.rotation.x,
            mainCamera.transform.rotation.y,
            mainCamera.transform.rotation.z,
            mainCamera.transform.rotation.w
        );
    }

    private Vector3 GetCameraForwardDirection()
    {
        Vector3 cameraForward = mainCamera.transform.forward;
        cameraForward.y = 0;
        return cameraForward.normalized;
    }
    ////////////////////////////////////////////////////

    //CHEATS///////////////////////////////////////////////
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
    ////////////////////////////////////////////////////
}
