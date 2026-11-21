using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class ReactivePostProcessing : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Volume globalVolume;
    [SerializeField] private NewSkateMovement skateMovement;
    [SerializeField] private CountdownTimer countdownTimer;

    [Header("1. Velocidad (Lens)")]
    [SerializeField] private float maxLensDistortion = -0.25f;
    [SerializeField] private float speedSmooth = 5f;

    [Header("2. Carga (Vignette)")]
    [SerializeField] private float baseVignette = 0.15f;
    [SerializeField] private float maxChargeVignette = 0.45f;
    [SerializeField] private Color normalVignetteColor = Color.black;

    [Header("3. Alerta (<10s) y Derrota")]
    [SerializeField] private float warningTimeThreshold = 10f;
    [SerializeField] private Color warningVignetteColor = Color.red;
    [SerializeField] private float warningPulseSpeed = 4f;
    [SerializeField] private float loseSaturation = -80f;
    [SerializeField] private float effectTransitionSpeed = 6f;

    private LensDistortion lensDistortion;
    private Vignette vignette;
    private ColorAdjustments colorAdjustments;

    private void Start()
    {
        if (globalVolume == null) globalVolume = GetComponent<Volume>();
        if (skateMovement == null) skateMovement = FindFirstObjectByType<NewSkateMovement>();
        if (countdownTimer == null) countdownTimer = FindFirstObjectByType<CountdownTimer>();

        if (globalVolume != null && globalVolume.profile != null)
        {
            globalVolume.profile.TryGet(out lensDistortion);
            globalVolume.profile.TryGet(out vignette);
            globalVolume.profile.TryGet(out colorAdjustments);
        }
    }

    private void Update()
    {
        UpdateSpeedEffects();
        UpdateVignetteAndLoseEffects();
    }

    private void UpdateSpeedEffects()
    {
        if (skateMovement == null) return;

        float speedPercent = Mathf.Clamp01(skateMovement.currentSpeed / skateMovement.maxSpeed);

        if (lensDistortion != null)
        {
            float targetDistortion = speedPercent * maxLensDistortion;
            lensDistortion.intensity.value = Mathf.Lerp(lensDistortion.intensity.value, targetDistortion, Time.deltaTime * speedSmooth);
        }
    }

    private void UpdateVignetteAndLoseEffects()
    {
        bool hasLost = countdownTimer != null && countdownTimer.isGameOver && !countdownTimer.hasWon;
        bool isWarning = countdownTimer != null && !countdownTimer.isGameOver && countdownTimer.CurrentTime <= warningTimeThreshold;

        if (colorAdjustments != null)
        {
            float targetSat = hasLost ? loseSaturation : 0f;
            colorAdjustments.saturation.value = Mathf.Lerp(colorAdjustments.saturation.value, targetSat, Time.deltaTime * effectTransitionSpeed);
        }

        if (vignette != null)
        {
            float targetIntensity = baseVignette;
            Color targetColor = normalVignetteColor;

            if (hasLost)
            {
                targetIntensity = 0.5f;
                targetColor = Color.black;
            }
            else if (isWarning)
            {
                float pulse = Mathf.PingPong(Time.time * warningPulseSpeed, 1f);
                targetIntensity = Mathf.Lerp(0.25f, 0.45f, pulse);
                targetColor = warningVignetteColor;
            }
            else if (skateMovement != null && skateMovement.IsCharging)
            {
                targetIntensity = Mathf.Lerp(baseVignette, maxChargeVignette, skateMovement.GetChargePercent());
            }

            vignette.intensity.value = Mathf.Lerp(vignette.intensity.value, targetIntensity, Time.deltaTime * effectTransitionSpeed);
            vignette.color.value = Color.Lerp(vignette.color.value, targetColor, Time.deltaTime * effectTransitionSpeed);
        }
    }
}