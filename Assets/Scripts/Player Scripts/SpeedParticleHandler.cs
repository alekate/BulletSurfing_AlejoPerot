using UnityEngine;

public class SpeedParticlesHandler : MonoBehaviour
{
    [SerializeField] private ParticleSystem speedLinesParticles;
    [SerializeField] private float particleSpeedThreshold = 20f;
    [SerializeField] private float maxEmissionRate = 100f;
    [SerializeField] private NewSkateMovement skateMovement;

    private void Start()
    {
        skateMovement = FindObjectOfType<NewSkateMovement>();
    }
    private void FixedUpdate()
    {
        float currentSpeed = skateMovement.currentSpeed;
        var emission = speedLinesParticles.emission;

        if (currentSpeed > particleSpeedThreshold)
        {
            if (!speedLinesParticles.isPlaying)
                speedLinesParticles.Play();

            float t = Mathf.InverseLerp(particleSpeedThreshold, skateMovement.maxSpeed, currentSpeed);
            emission.rateOverTime = t * maxEmissionRate;
        }
        else
        {
            if (speedLinesParticles.isPlaying)
                speedLinesParticles.Stop();
        }
    }
}
