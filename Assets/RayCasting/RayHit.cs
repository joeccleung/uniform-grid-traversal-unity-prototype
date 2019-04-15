using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum RayXPlaneResult
{
    NoIntersection,
    OneIntersection,
    TwoIntersections,
    LiesOnPlane
}


[Serializable]
public struct RayHit
{
    [HideInInspector] public Vector3 barycentric;
    public Vector3 hitPoint;
    public float distance;
    [HideInInspector] public Vector3 normal;
    [HideInInspector] public RayXPlaneResult result;
    
    
    public static RayHit CreateRayHit()
    {
        RayHit hit = new RayHit();
        hit.barycentric = new Vector3(1.0f, 0.0f, 0.0f);
        hit.hitPoint = Vector3.zero;
        hit.distance = float.PositiveInfinity;
        hit.normal = Vector3.zero;
        return hit;
    }


    public RayHit(float distance, Vector3 hitPoint, RayXPlaneResult result)
    {
        this.barycentric = new Vector3(1.0f, 0.0f, 0.0f);
        this.distance = distance;
        this.hitPoint = hitPoint;
        this.normal = Vector3.zero;
        this.result = result;
    }

    public override string ToString()
    {
        switch (result)
        {
            case RayXPlaneResult.OneIntersection:
                return $"One Hit={hitPoint} t={distance}";
            
            default:
                return $"{result}";
        }
    }
}
