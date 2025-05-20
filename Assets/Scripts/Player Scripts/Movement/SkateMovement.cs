using UnityEngine;
using TMPro;
using Unity.Collections;

public class SkateMovement : MonoBehaviour
{
    [Header("Skate Settings")]
    [SerializeField] private float acceleration = 2.0f;
    public float maxSpeed;
    [SerializeField] private float friction;
    [SerializeField] private float brakeForce = 20f;
    public float currentSpeed;

    [Header("Mouse Input")]
    [SerializeField] private float mouseYawTolerance;
    [SerializeField] private float movementThresholdToAccelerate;
    [SerializeField] private float decreasingFactor = 0f;
    public float horizontalMouseMovementValue { get; private set; }
    [SerializeField, ReadOnly] private float debugMouseX;

    [Header("Camera")]
    [SerializeField] private Camera mainCamera;

    [Header("Ground Alignment")]
    [SerializeField] private Transform raycastOrigin;
    [SerializeField] private Transform bodyToRotate;
    [SerializeField] private float raycastDistance = 3f;
    [SerializeField] private float rotationSpeed = 5f;

    [Header("Speed Particles")]
    [SerializeField] private ParticleSystem speedLinesParticles;
    [SerializeField] private float particleSpeedThreshold = 20f;
    [SerializeField] private float maxEmissionRate = 100f;

    [Header("Audio")]
    [SerializeField] private AudioSource skateAudioSource;
    [SerializeField] private float minVolume = 0.1f;
    [SerializeField] private float maxVolume = 1f;
    [SerializeField] private float minPitch = 0.8f;
    [SerializeField] private float maxPitch = 1.2f;
    [SerializeField] private float minSpeedToPlaySound = 0.5f;

    private Rigidbody rb;
    private Quaternion smoothTilt = Quaternion.identity;
    private Vector3 moveDirection;
    private bool isSkating;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        horizontalMouseMovementValue = Input.GetAxis("Mouse X");
        debugMouseX = horizontalMouseMovementValue;

        PlayerInput();
        Move();
        AlignToGround();
        HandleSpeedParticles();
        UpdateSkateSound();


        if (Input.GetKey(KeyCode.Q))
        {
            currentSpeed += 10;
        }

        currentSpeed = Mathf.Max(currentSpeed, 0f);
    }

    private void PlayerInput()
    {
        if (Input.GetMouseButton(0))
        {
            isSkating = true;
            moveDirection = GetCameraForwardDirection();

            if (Mathf.Abs(horizontalMouseMovementValue) > movementThresholdToAccelerate)
            {
                currentSpeed += acceleration * Time.deltaTime;
                currentSpeed = Mathf.Min(currentSpeed, maxSpeed);
            }

            if (Mathf.Abs(horizontalMouseMovementValue) > mouseYawTolerance)
            {
                DecreaseVelocity();
            }
        }
        else
        {
            isSkating = false;
        }
    }

    private void DecreaseVelocity()
    {
        Debug.Log(horizontalMouseMovementValue);
        currentSpeed -= decreasingFactor;
    }

    private void Move()
    {
        Vector3 flatVelocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
        float currentVelocityMagnitude = flatVelocity.magnitude;

        if (isSkating && currentVelocityMagnitude < maxSpeed)
        {
            Vector3 desiredDirection = moveDirection.normalized;
            rb.AddForce(desiredDirection * acceleration, ForceMode.Acceleration);
        }
        else if (!isSkating && currentVelocityMagnitude > 0)
        {
            Vector3 frictionForce = -flatVelocity.normalized * friction;
            rb.AddForce(frictionForce, ForceMode.Acceleration);
        }

        if (Input.GetMouseButton(1) && currentVelocityMagnitude > 0.1f)
        {
            Vector3 brakeDirection = -flatVelocity.normalized;
            rb.AddForce(brakeDirection * brakeForce, ForceMode.Acceleration);
        }

        if (currentVelocityMagnitude > maxSpeed)
        {
            Vector3 limitedVelocity = flatVelocity.normalized * maxSpeed;
            rb.velocity = new Vector3(limitedVelocity.x, rb.velocity.y, limitedVelocity.z);
        }

        currentSpeed = rb.velocity.magnitude;
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

    private Vector3 GetCameraForwardDirection()
    {
        Vector3 cameraForward = mainCamera.transform.forward;
        cameraForward.y = 0;
        return cameraForward.normalized;
    }

    private void HandleSpeedParticles()
    {
        var emission = speedLinesParticles.emission;

        if (currentSpeed > particleSpeedThreshold)
        {
            if (!speedLinesParticles.isPlaying)
                speedLinesParticles.Play();

            float t = Mathf.InverseLerp(particleSpeedThreshold, maxSpeed, currentSpeed);
            emission.rateOverTime = t * maxEmissionRate;
        }
        else
        {
            if (speedLinesParticles.isPlaying)
                speedLinesParticles.Stop();
        }
    }

    private void UpdateSkateSound()
    {
        if (currentSpeed > minSpeedToPlaySound)
        {
            if (!skateAudioSource.isPlaying)
            {
                skateAudioSource.Play();
            }

            float t = Mathf.InverseLerp(minSpeedToPlaySound, maxSpeed, currentSpeed);
            skateAudioSource.volume = Mathf.Lerp(minVolume, maxVolume, t);
            skateAudioSource.pitch = Mathf.Lerp(minPitch, maxPitch, t);
        }
        else
        {
            if (skateAudioSource.isPlaying)
            {
                skateAudioSource.Stop();
            }
        }
    }

}
