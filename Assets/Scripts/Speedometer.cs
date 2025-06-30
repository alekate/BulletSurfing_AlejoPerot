using UnityEngine;

public class Speedometer : MonoBehaviour
{
    [Header("Settings")]
    public Transform targetObject; // El objeto cuya velocidad medimos
    public Transform needle; // La aguja del velocímetro
    public float additionalNeedleRotation = 0f;
    public bool useKilometersPerHour = true;

    private Vector3 previousPosition;
    private float currentSpeed;

    void Start()
    {
        if (targetObject == null)
        {
            Debug.LogWarning("Speedometer: No target object assigned.");
            enabled = false;
            return;
        }

        previousPosition = targetObject.position;
    }

    void Update()
    {
        float deltaTime = Time.deltaTime;
        if (deltaTime <= 0) return;

        Vector3 currentPosition = targetObject.position;
        Vector3 displacement = currentPosition - previousPosition;

        float speed = displacement.magnitude / deltaTime; // unidades/segundo

        if (useKilometersPerHour)
            currentSpeed = speed * 3.6f; // convertir a km/h
        else
            currentSpeed = speed * 6.5f;

        RotateNeedle(currentSpeed);
        previousPosition = currentPosition;
    }

    private void RotateNeedle(float speed)
    {
        if (needle == null) return;

        float rotationAmount = additionalNeedleRotation - speed; //asi la needle no gira al reves
        Vector3 currentRotation = needle.localEulerAngles;
        needle.localEulerAngles = new Vector3(currentRotation.x, currentRotation.y, rotationAmount);
    }

    public float GetCurrentSpeed()
    {
        return currentSpeed;
    }
}
