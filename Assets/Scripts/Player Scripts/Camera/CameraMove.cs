using UnityEngine;
using Unity.Cinemachine;

public class CameraMove : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform playerRef;
    [SerializeField] private Transform cameraHolder;
    [SerializeField] private NewSkateMovement skateMovement;

    [SerializeField] private CinemachineCamera cinemachineCamera;

    [Header("Sensitivity & Limits")]
    [SerializeField] private float maxLookAngle = 80f; // ¡Regresó el límite de cuello!
    [SerializeField] private float quickTurnSpeed = 12f;
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

    private CinemachinePanTilt panTiltControl;
    private float currentExtraFOV;
    private float kickbackOffset;
    private float quickTurnOffset;

    // Almacenamos los valores ideales para que Cinemachine no los sobreescriba
    private float targetPan;
    private float targetTilt;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;

        if (skateMovement == null && playerRef != null)
            skateMovement = playerRef.GetComponent<NewSkateMovement>();

        if (cinemachineCamera == null)
            cinemachineCamera = FindFirstObjectByType<CinemachineCamera>();

        if (cinemachineCamera != null)
        {
            panTiltControl = cinemachineCamera.GetComponent<CinemachinePanTilt>();
            if (panTiltControl != null)
            {
                targetPan = panTiltControl.PanAxis.Value;
                targetTilt = panTiltControl.TiltAxis.Value;
            }
        }

        UpdateSensitivity();
    }

    public void UpdateSensitivity()
    {
        float savedSens = PlayerPrefs.GetFloat("MouseSensitivity", 2.5f);
        // Bajamos drásticamente el multiplicador para domar el delta del mouse en Cinemachine
        sensX = savedSens * 0.1f;
        sensY = savedSens * 0.1f;
    }

    private void Update()
    {
        if (playerRef == null || panTiltControl == null) return;

        UpdateCinemachineRotation();
        UpdateTilt();
        UpdateCinemachineFOV();
    }

    private void UpdateCinemachineRotation()
    {
        // 1. Manejo del eje Y (Tilt / Arriba y Abajo) CON LÍMITE
        // ¡CAMBIAMOS EL "+" POR UN "-" AQUÍ!
        targetTilt -= inputDir.y * sensY;

        targetTilt = Mathf.Clamp(targetTilt, -maxLookAngle, maxLookAngle);

        kickbackOffset = Mathf.Lerp(kickbackOffset, 0f, Time.deltaTime * kickbackRecoverySpeed);
        panTiltControl.TiltAxis.Value = targetTilt + kickbackOffset;

        // 2. Manejo del eje X (Pan / Izquierda y Derecha)
        targetPan += inputDir.x * sensX;
        quickTurnOffset = Mathf.Lerp(quickTurnOffset, 0f, Time.deltaTime * quickTurnSpeed);
        panTiltControl.PanAxis.Value = targetPan - quickTurnOffset;
    }

    public void QuickTurn180()
    {
        targetPan += 180f;
        quickTurnOffset += 180f;
    }

    private void UpdateTilt()
    {
        if (!enableTilt || cinemachineCamera == null) return;

        // CINEMACHINE TILT: Usamos el parámetro "Dutch" (Inclinación Z de lente) 
        // Esto elimina cualquier conflicto o sobrescritura con el motor.
        float targetDutch = -(inputDir.x * sensX) * turnTiltMultiplier;

        cinemachineCamera.Lens.Dutch = Mathf.Lerp(cinemachineCamera.Lens.Dutch, targetDutch, Time.deltaTime * 5f);
    }

    private void UpdateCinemachineFOV()
    {
        if (cinemachineCamera == null || skateMovement == null) return;

        float speedFOV = skateMovement.currentSpeed * fovSpeedMultiplier;
        float anticipationFOV = 0f;

        if (skateMovement.IsCharging)
            anticipationFOV = skateMovement.GetChargePercent() * chargeFOVOffset;

        float targetFOV = Mathf.Min(baseFOV + speedFOV + currentExtraFOV + anticipationFOV, maxFOV);

        cinemachineCamera.Lens.FieldOfView = Mathf.Lerp(cinemachineCamera.Lens.FieldOfView, targetFOV, fovChangeSpeed * Time.deltaTime);
        currentExtraFOV = Mathf.Lerp(currentExtraFOV, 0f, Time.deltaTime * kickbackRecoverySpeed);
    }

    public void ApplyPushFeedback(float chargeIntensity)
    {
        kickbackOffset = kickbackPitchAmount * chargeIntensity;
        currentExtraFOV = 15f * chargeIntensity;
    }
}