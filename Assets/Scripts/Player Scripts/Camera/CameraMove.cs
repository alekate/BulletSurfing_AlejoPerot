using UnityEngine;

public class CameraMove : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform playerRef;
    [SerializeField] private Transform cameraHolder;
    [SerializeField] private NewSkateMovement skateMovement;
    private Camera cam;

    [Header("Sensitivity & Limits")]
    [SerializeField] private float maxLookAngle = 80f;
    [SerializeField] private float quickTurnSpeed = 12f;
    // Ya no expondremos sensX y sensY aquí porque el Menú los controla, pero los mantenemos privados
    private float sensX;
    private float sensY;

    [Header("Visual Effects (Tilt & FOV)")]
    [SerializeField] private bool enableTilt = true;
    [SerializeField] private float turnTiltMultiplier = 3f;
    [SerializeField] private float baseFOV = 60f;
    [SerializeField] private float maxFOV = 90f;
    [SerializeField] private float fovSpeedMultiplier = 1.5f;
    [SerializeField] private float fovChangeSpeed = 5f;

    [Header("Push Feedback & Anticipation")]
    [SerializeField] private float chargeFOVOffset = -15f;
    [SerializeField] private float kickbackPitchAmount = 5f;
    [SerializeField] private float kickbackRecoverySpeed = 10f;

    public Vector2 inputDir;
    private Vector2 turn;
    private float kickbackOffset;
    private float currentExtraFOV;
    private float quickTurnOffset;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        cam = GetComponentInChildren<Camera>();
        if (skateMovement == null && playerRef != null)
            skateMovement = playerRef.GetComponent<NewSkateMovement>();

        UpdateSensitivity(); // Cargamos la sensibilidad al iniciar
    }

    // NUEVO MÉTODO: Lee el valor guardado y lo doma para el nuevo sistema
    public void UpdateSensitivity()
    {
        float savedSens = PlayerPrefs.GetFloat("MouseSensitivity", 2.5f); // 2.5 será el valor por defecto

        // El 0.1f compensa los altísimos valores de píxeles que envía el "Delta" del ratón
        sensX = savedSens * 0.1f;
        sensY = savedSens * 0.1f;
    }

    private void Update()
    {
        if (playerRef == null) return;
        UpdateRotation();
        UpdateTilt();
        UpdateFOV();
        UpdatePosition();
    }

    private void UpdateRotation()
    {
        turn.x += inputDir.x * sensX;
        turn.y += inputDir.y * sensY;

        turn.y = Mathf.Clamp(turn.y, -maxLookAngle, maxLookAngle);

        kickbackOffset = Mathf.Lerp(kickbackOffset, 0f, Time.deltaTime * kickbackRecoverySpeed);
        quickTurnOffset = Mathf.Lerp(quickTurnOffset, 0f, Time.deltaTime * quickTurnSpeed);

        transform.localRotation = Quaternion.Euler(-turn.y - kickbackOffset, turn.x - quickTurnOffset, 0);
    }

    public void QuickTurn180()
    {
        turn.x += 180f;
        quickTurnOffset += 180f;
    }

    private void UpdateTilt()
    {
        if (!enableTilt || cameraHolder == null) return;
        float tiltAngle = -(inputDir.x * sensX) * turnTiltMultiplier;
        Quaternion targetRotation = Quaternion.Euler(cameraHolder.localEulerAngles.x, cameraHolder.localEulerAngles.y, tiltAngle);
        cameraHolder.localRotation = Quaternion.Lerp(cameraHolder.localRotation, targetRotation, Time.deltaTime * 5f);
    }

    private void UpdateFOV()
    {
        if (cam == null || skateMovement == null) return;
        float speedFOV = skateMovement.currentSpeed * fovSpeedMultiplier;
        float anticipationFOV = 0f;
        if (skateMovement.IsCharging) anticipationFOV = skateMovement.GetChargePercent() * chargeFOVOffset;
        float targetFOV = Mathf.Min(baseFOV + speedFOV + currentExtraFOV + anticipationFOV, maxFOV);
        cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, targetFOV, fovChangeSpeed * Time.deltaTime);
        currentExtraFOV = Mathf.Lerp(currentExtraFOV, 0f, Time.deltaTime * kickbackRecoverySpeed);
    }

    private void UpdatePosition()
    {
        transform.position = playerRef.position;
    }

    public void ApplyPushFeedback(float chargeIntensity)
    {
        kickbackOffset = kickbackPitchAmount * chargeIntensity;
        currentExtraFOV = 15f * chargeIntensity;
    }
}