using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RayPlaneIntersectionCompute : MonoBehaviour
{
    public static RayHit RayPlaneIntersection(Ray ray, Vector3 pNormal, Vector3 pPoint)
    {
        // From the distance formula of ray plane intersection
        float divisor = Vector3.Dot(pNormal, ray.direction);
        float dividend = Vector3.Dot(pPoint - ray.origin, pNormal);
    
        RayHit rayHit = RayHit.CreateRayHit();
    
        float Epsilon = 1.19e-07f;
    
        if(Mathf.Abs(divisor) <= Epsilon)
        {
            // The line and plane is parallel, check if the line lies on the plane
            if (Mathf.Abs(dividend) <= Epsilon)
            {
                rayHit.distance = 0;
                rayHit.hitPoint = Vector3.zero;
                return rayHit;
            }
            else
            {
                rayHit.distance = float.PositiveInfinity;
                rayHit.hitPoint = Vector3.positiveInfinity;
                return rayHit;
            }
        }
    
        rayHit.distance = dividend / divisor;
        rayHit.hitPoint = SpatialGridTrace.GetPoint(ray, rayHit.distance);
        return rayHit;
    }  
}
