using UnityEngine;
using Unity.Mathematics;
using UnityEngine.Splines;

[RequireComponent(typeof(SplineContainer))]
public class RailScript_NoPlayer : MonoBehaviour
{
    public bool normalDir = true;
    public SplineContainer railSpline;
    public float totalSplineLength;

    private void Awake()
    {
        railSpline = GetComponent<SplineContainer>();
        totalSplineLength = railSpline.CalculateLength();
    }

    public Vector3 LocalToWorldConversion(float3 localPoint)
    {
        return transform.TransformPoint(localPoint);
    }

    public float3 WorldToLocalConversion(Vector3 worldPoint)
    {
        return transform.InverseTransformPoint(worldPoint);
    }

    public float CalculateTargetRailPoint(Vector3 worldPos, out Vector3 closestPointWorld)
    {
        float3 nearestPoint;
        float time;
        SplineUtility.GetNearestPoint(railSpline.Spline, WorldToLocalConversion(worldPos), out nearestPoint, out time);
        closestPointWorld = LocalToWorldConversion(nearestPoint);
        return time;
    }

    public void CalculateDirection(float3 railForward, Vector3 objectForward)
    {
        float angle = Vector3.Angle(railForward, objectForward.normalized);
        normalDir = angle > 90f;
    }
}
