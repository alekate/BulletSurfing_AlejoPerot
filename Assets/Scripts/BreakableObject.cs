using System.Collections;
using UnityEngine;

public class BreakableObject : MonoBehaviour
{
    [Header("Configuración")]
    public float breakSpeedThreshold = 20f;
    public ParticleSystem destructionParticles;
    public float delayBeforeDestroy = 1.5f;

    private bool isBroken = false;
    private Collider objectCollider;

    public NewSkateMovement skateMovement;

    void Start()
    {
        objectCollider = GetComponent<Collider>();
        if (destructionParticles != null)
        {
            destructionParticles.Stop(); 
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (isBroken) return;

        if (skateMovement != null)
        {
            float currentSpeed = skateMovement.currentSpeed;

            if (currentSpeed >= breakSpeedThreshold)
            {
                StartCoroutine(BreakObject());
            }
        }
    }

    private IEnumerator BreakObject()
    {
        isBroken = true;

        if (objectCollider != null)
            objectCollider.enabled = false;

        if (destructionParticles != null)
            destructionParticles.Play();

        yield return new WaitForSeconds(delayBeforeDestroy);

        Destroy(gameObject);
    }
}
