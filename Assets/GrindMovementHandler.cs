using UnityEngine;
using Unity.Mathematics;
using UnityEngine.Splines;

[RequireComponent(typeof(Transform))]
public class GrindMovementHandler : MonoBehaviour
{
    [Header("Grind Settings")]
    public bool onRail = false;
    public float grindSpeed = 5f;
    public float heightOffset = 0.3f;
    public float lerpSpeed = 10f;

    private RailScript currentRailScript;
    private float timeForFullSpline;
    private float elapsedTime;

    private void FixedUpdate()
    {
        if (onRail && currentRailScript != null)
        {
            MoveAlongRail();
        }
    }

    private void MoveAlongRail()
    {
        float progress = elapsedTime / timeForFullSpline;
        if (progress < 0 || progress > 1)
        {
            ExitRail();
            return;
        }

        float nextProgress = (elapsedTime + (currentRailScript.normalDir ? Time.deltaTime : -Time.deltaTime)) / timeForFullSpline;

        SplineUtility.Evaluate(currentRailScript.railSpline.Spline, progress, out float3 pos, out float3 tan, out float3 up);
        SplineUtility.Evaluate(currentRailScript.railSpline.Spline, nextProgress, out float3 nextPos, out float3 nextTan, out float3 nextUp);

        Vector3 worldPos = currentRailScript.LocalToWorldConversion(pos);
        Vector3 nextWorldPos = currentRailScript.LocalToWorldConversion(nextPos);

        transform.position = worldPos + (transform.up * heightOffset);
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(nextWorldPos - worldPos), lerpSpeed * Time.deltaTime);
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.FromToRotation(transform.up, up) * transform.rotation, lerpSpeed * Time.deltaTime);

        elapsedTime += currentRailScript.normalDir ? Time.deltaTime : -Time.deltaTime;
    }

    public void EnterRail(RailScript rail, Vector3 contactPosition, Vector3 forwardReference)
    {
        currentRailScript = rail;
        onRail = true;

        timeForFullSpline = currentRailScript.totalSplineLength / grindSpeed;

        float normalisedTime = currentRailScript.CalculateTargetRailPoint(contactPosition, out Vector3 startPos);
        elapsedTime = normalisedTime * timeForFullSpline;

        SplineUtility.Evaluate(currentRailScript.railSpline.Spline, normalisedTime, out float3 pos, out float3 tan, out float3 up);
        currentRailScript.CalculateDirection(tan, forwardReference);

        transform.position = startPos + (transform.up * heightOffset);
    }

    public void ExitRail()
    {
        onRail = false;
        currentRailScript = null;
        transform.position += transform.forward * 1f;
        transform.rotation = Quaternion.identity;
    }
}
