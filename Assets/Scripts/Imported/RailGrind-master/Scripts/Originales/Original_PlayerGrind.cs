using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Splines;

public class Original_PlayerGrind : MonoBehaviour
{
    [Header("Inputs")]
    [SerializeField] bool jump;
    [SerializeField] Vector3 input;

    [Header("Variables")]
    public bool onRail;
    public float grindSpeed;
    [SerializeField] float heightOffset;
    float timeForFullSpline;
    float elapsedTime;
    [SerializeField] float lerpSpeed = 10f;

    [SerializeField] Vector3 playerForward;
    [SerializeField] Vector3 worldPos;
    [SerializeField] Vector3 nextPos;

    [Header("Scripts")]
    [SerializeField] Original_RailScript currentRailScript;
    Rigidbody rb;
    CharacterController charController;

    [Header("FX")]
    [SerializeField] private ParticleSystem grindParticles; 

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        charController = GetComponent<CharacterController>();
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
            MovePlayerAlongRail();

            if (grindParticles != null && !grindParticles.isPlaying)
                grindParticles.Play();
        }
        else
        {
            if (grindParticles != null && grindParticles.isPlaying)
                grindParticles.Stop();
        }
    }

    private void Update()
    {
        playerForward = transform.forward;
    }

    void MovePlayerAlongRail()
    {
        if (currentRailScript != null && onRail)
        {
            float progress = elapsedTime / timeForFullSpline;
            if (progress < 0 || progress > 1)
            {
                ThrowOffRail();
                return;
            }

            float nextTimeNormalised;
            if (currentRailScript.normalDir)
                nextTimeNormalised = (elapsedTime + Time.deltaTime) / timeForFullSpline;
            else
                nextTimeNormalised = (elapsedTime - Time.deltaTime) / timeForFullSpline;

            float3 pos, tangent, up;
            float3 nextPosfloat, nextTan, nextUp;
            SplineUtility.Evaluate(currentRailScript.railSpline.Spline, progress, out pos, out tangent, out up);
            SplineUtility.Evaluate(currentRailScript.railSpline.Spline, nextTimeNormalised, out nextPosfloat, out nextTan, out nextUp);

            worldPos = currentRailScript.LocalToWorldConversion(pos);
            nextPos = currentRailScript.LocalToWorldConversion(nextPosfloat);

            transform.position = worldPos + (transform.up * heightOffset);
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(nextPos - worldPos), lerpSpeed * Time.deltaTime);
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.FromToRotation(transform.up, up) * transform.rotation, lerpSpeed * Time.deltaTime);

            if (currentRailScript.normalDir)
                elapsedTime += Time.deltaTime;
            else
                elapsedTime -= Time.deltaTime;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Rail")
        {
            onRail = true;
            rb.useGravity = false;
            currentRailScript = collision.gameObject.GetComponent<Original_RailScript>();
            CalculateAndSetRailPosition();
        }
    }

    void CalculateAndSetRailPosition()
    {
        timeForFullSpline = currentRailScript.totalSplineLength / grindSpeed;

        Vector3 splinePoint;
        float normalisedTime = currentRailScript.CalculateTargetRailPoint(transform.position, out splinePoint);
        elapsedTime = timeForFullSpline * normalisedTime;

        float3 pos, forward, up;
        SplineUtility.Evaluate(currentRailScript.railSpline.Spline, normalisedTime, out pos, out forward, out up);
        currentRailScript.CalculateDirection(forward, transform.forward);

        transform.position = splinePoint + (transform.up * heightOffset);
    }

    public void ThrowOffRail()
    {
        onRail = false;
        rb.useGravity = true;
        currentRailScript = null;
        transform.position += transform.forward * 3;

        transform.rotation = Quaternion.identity;

        if (grindParticles != null && grindParticles.isPlaying)
            grindParticles.Stop();
    }
}
