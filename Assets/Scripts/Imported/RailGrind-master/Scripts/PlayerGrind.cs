using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Splines;

public class PlayerGrind : MonoBehaviour
{
    [Header("Inputs")]
    [SerializeField] bool jump;         //Inputs aren't used in the tutorial
    [SerializeField] Vector3 input;     //But they're here for rail switching


    [Header("Variables")]
    public bool onRail;

    [SerializeField] float heightOffset;
    float timeForFullSpline;
    float elapsedTime;
    [SerializeField] float lerpSpeed = 10f;

    [Header("Scripts")]
    [SerializeField] RailScript currentRailScript;
    [SerializeField] NewSkateMovement skateMovement;
    private Rigidbody rb;

    private void Start()
    {
        skateMovement = FindObjectOfType<NewSkateMovement>();
        rb = GetComponent<Rigidbody>();

    }
    public void HandleJump(InputAction.CallbackContext context)
    {
        jump = Convert.ToBoolean(context.ReadValue<float>());
    }
    public void HandleMovement(InputAction.CallbackContext context)
    {
        Vector2 rawInput = context.ReadValue<Vector2>();
        input.x = rawInput.x;
    }
    private void FixedUpdate()
    {
        if (onRail)
        {
            skateMovement.hasInput = false;
            MovePlayerAlongRail();
            skateMovement.currentSpeed += 1f * Time.deltaTime;

            JumpOffRail(); //  Esta línea permite salir del riel saltando
        }
    }

    void StartGrinding(RailScript railScript)
    {
        onRail = true;
        currentRailScript = railScript;

        if (skateMovement.currentSpeed < 5)
        {
            // No iniciar grind si la velocidad es muy baja
            return;
        }

        // Calcular duración total del rail
        timeForFullSpline = currentRailScript.totalSplineLength / skateMovement.currentSpeed;

        // Obtener punto más cercano en la spline y normalisedTime
        Vector3 splinePoint;
        float normalisedTime = currentRailScript.CalculateTargetRailPoint(transform.position, out splinePoint);
        elapsedTime = timeForFullSpline * normalisedTime;

        // Evaluar spline para obtener orientación
        float3 pos, forward, up;
        SplineUtility.Evaluate(currentRailScript.railSpline.Spline, normalisedTime, out pos, out forward, out up);

        // Calcular dirección del grind
        currentRailScript.CalculateDirection(forward, transform.forward);

        // Posicionar jugador correctamente
        rb.MovePosition(splinePoint + (transform.up * heightOffset));

        // Desactivar input mientras grindea
        skateMovement.hasInput = false;
    }

    void MovePlayerAlongRail()
    {
        if (currentRailScript != null && onRail) //This is just some additional error checking.
        {
            //Calculate a 0 to 1 normalised time value which is the progress along the rail.
            //Elapsed time divided by the full time needed to traverse the spline will give you that value.
            float progress = elapsedTime / timeForFullSpline;

            //If progress is less than 0, the player's position is before the start of the rail.
            //If greater than 1, their position is after the end of the rail.
            //In either case, the player has finished their grind.
            if (progress < 0 || progress > 1)
            {
                ThrowOffRail();
                return;
            }
            //The rest of this code will not execute if the player is thrown off.

            //Next Time Normalised is the player's progress value for the next update.
            //This is used for calculating the player's rotation.
            //Depending on the direction of the player on the spline, it will either add or subtract time from the current elapsed time.
            float nextTimeNormalised;
            if (currentRailScript.normalDir)
                nextTimeNormalised = (elapsedTime + Time.deltaTime) / timeForFullSpline;
            else
                nextTimeNormalised = (elapsedTime - Time.deltaTime) / timeForFullSpline;

            //Calculating the local positions of the player's current position and next position using current progress and the progress for the next update.
            float3 pos, tangent, up;
            float3 nextPosfloat, nextTan, nextUp;
            SplineUtility.Evaluate(currentRailScript.railSpline.Spline, progress, out pos, out tangent, out up);
            SplineUtility.Evaluate(currentRailScript.railSpline.Spline, nextTimeNormalised, out nextPosfloat, out nextTan, out nextUp);

            //Converting the local positions into world positions.
            Vector3 worldPos = currentRailScript.LocalToWorldConversion(pos);
            Vector3 nextPos = currentRailScript.LocalToWorldConversion(nextPosfloat);

            //Setting the player's position and adding a height offset so that they're sitting on top of the rail instead of being in the middle of it.
            rb.MovePosition(worldPos + (transform.up * heightOffset));

            //Lerping the player's current rotation to the direction of where they are to where they're going.
            Vector3 forward = (nextPos - worldPos).normalized;
            Quaternion targetRotation = Quaternion.LookRotation(forward, up);

            Quaternion lookRot = Quaternion.LookRotation(forward, up);
            Quaternion upRot = Quaternion.FromToRotation(transform.up, up) * transform.rotation;
            Quaternion finalRot = Quaternion.Lerp(transform.rotation, lookRot, lerpSpeed * Time.deltaTime);
            finalRot = Quaternion.Lerp(finalRot, upRot, lerpSpeed * Time.deltaTime);
            rb.MoveRotation(finalRot);

            //Finally incrementing or decrementing elapsed time for the next update based on direction.
            float delta = Time.deltaTime * skateMovement.currentSpeed / currentRailScript.totalSplineLength;

            if (currentRailScript.normalDir)
                elapsedTime += delta;
            else
                elapsedTime -= delta;

        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Rail"))
        {
            RailScript railScript = collision.gameObject.GetComponent<RailScript>();
            if (railScript != null)
            {
                StartGrinding(railScript);
            }
        }
    }

    void CalculateAndSetRailPosition()
    {
        if (skateMovement.currentSpeed < 5)
        {
            return;
        }

        //Figure out the amount of time it would take for the player to cover the rail.
        timeForFullSpline = currentRailScript.totalSplineLength / skateMovement.currentSpeed;

        //This is going to be the world position of where the player is going to start on the rail.
        Vector3 splinePoint;

        //The 0 to 1 value of the player's position on the spline. We also get the world position of where that point is.
        float normalisedTime = currentRailScript.CalculateTargetRailPoint(transform.position, out splinePoint);
        elapsedTime = timeForFullSpline * normalisedTime;
        //Multiply the full time for the spline by the normalised time to get elapsed time. This will be used in the movement code.

        //Spline evaluate takes the 0 to 1 normalised time above, and uses it to give you a local position, a tangent (forward), and up
        float3 pos, forward, up;
        SplineUtility.Evaluate(currentRailScript.railSpline.Spline, normalisedTime, out pos, out forward, out up);

        //Calculate the direction the player is going down the rail
        currentRailScript.CalculateDirection(forward, transform.forward);

        //Set player's initial position on the rail before starting the movement code.
        transform.position = splinePoint + (transform.up * heightOffset);
    }
    void ThrowOffRail()
    {
        if (!onRail) return; // Evita múltiples ejecuciones

        onRail = false;

        // Impulso de salida del riel
        Vector3 launchDirection = (transform.forward + Vector3.up).normalized;
        float launchForce = 5f; // Ajustable según lo que quieras sentir

        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = Vector3.zero; // Reset para evitar acumulaciones raras
            rb.AddForce(launchDirection * launchForce, ForceMode.VelocityChange);
        }

        // Restaurar estado
        transform.localRotation = Quaternion.identity;
        currentRailScript = null;
        skateMovement.hasInput = true;
    }
    void JumpOffRail()
    {
        if (!onRail || !jump) return;

        // Resetear el flag del salto para que no se repita automáticamente
        jump = false;

        // Aplicar una fuerza de salto específica
        Vector3 jumpDirection = (transform.forward + Vector3.up).normalized;
        float jumpForce = 7f; // Podés ajustar esto para más impulso

        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = Vector3.zero;
            rb.AddForce(jumpDirection * jumpForce, ForceMode.VelocityChange);
        }

        // Restaurar estado normal
        onRail = false;
        currentRailScript = null;
        skateMovement.hasInput = true;
    }


}
