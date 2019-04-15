using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RayBoxIntersectionCompute
{
    /// <summary>
    /// An Efficient and Robust Ray–Box Intersection Algorithm
    /// Amy Williams, Steve Barrus, R. Keith Morley, Peter Shirley
    /// http://www.cs.utah.edu/~awilliam/box/box.pdf
    /// </summary>
    /// <param name="ray"></param>
    /// <param name="boxMin"></param>
    /// <param name="boxMax"></param>
    /// <param name="t0"></param>
    /// <param name="t1"></param>
    public static bool RayBoxIntersection(Ray ray, Vector3 boxMin, Vector3 boxMax, ref float t0, ref float t1)
    {
        float txmin, txmax, tymin, tymax, tzmin, tzmax;

        if (ray.direction.x >= 0)
        {
            txmin = (boxMin.x - ray.origin.x) / ray.direction.x;
            txmax = (boxMax.x - ray.origin.x) / ray.direction.x;
        }
        else
        {
            txmin = (boxMax.x - ray.origin.x) / ray.direction.x;
            txmax = (boxMin.x - ray.origin.x) / ray.direction.x;
        }
        
        if (ray.direction.y >= 0) 
        {
            tymin = (boxMin.y - ray.origin.y) / ray.direction.y;
            tymax = (boxMax.y - ray.origin.y) / ray.direction.y;
        }
        else 
        {
            tymin = (boxMax.y - ray.origin.y) / ray.direction.y;
            tymax = (boxMin.y - ray.origin.y) / ray.direction.y;
        }
        
        if ( (txmin > tymax) || (tymin > txmax) )
            return false;
        
        if (tymin > txmin)
            txmin = tymin;
        
        if (tymax < txmax)
            txmax = tymax;
        
        if (ray.direction.z >= 0) {
            tzmin = (boxMin.z - ray.origin.z) / ray.direction.z;
            tzmax = (boxMax.z - ray.origin.z) / ray.direction.z;
        }
        else 
        {
            tzmin = (boxMax.z - ray.origin.z) / ray.direction.z;
            tzmax = (boxMin.z - ray.origin.z) / ray.direction.z;
        }
        
        if ( (txmin > tzmax) || (tzmin > txmax) )
            return false;
        
        if (tzmin > txmin)
            txmin = tzmin;
        
        if (tzmax < txmax)
            txmax = tzmax;

        if ((txmin > t1) || (txmax < t0))
        {
            return false;
        }

        t1 = txmin;
        t0 = txmax;
        
        return true;
    }
}
