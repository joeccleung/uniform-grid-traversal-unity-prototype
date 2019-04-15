using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpatialGridCompute
{
    static Vector3 GridNormalLeft = new Vector3(-1, 0, 0);
    static Vector3 GridNormalRight = new Vector3(1, 0, 0);
    static Vector3 GridNormalUp = new Vector3(0, 1, 0);
    static Vector3 GridNormalDown = new Vector3(0, -1, 0);
    static Vector3 GridNormalBack = new Vector3(0, 0, -1);
    static Vector3 GridNormalForward = new Vector3(0, 0, 1);
    
    public static SpatialGridIndex GetGridIndexAtPoint(SpatialGrid grid, Vector3 pt)
    {
        int d = grid.dimension;

        SpatialGridIndex index;
    
        index.x = Mathf.FloorToInt((pt.x - grid.min.x) / (grid.max.x - grid.min.x) * d);
        index.x = Mathf.Clamp(index.x, 0, d - 1);
    
        index.y = Mathf.FloorToInt((pt.y - grid.min.y) / (grid.max.y - grid.min.y) * d);
        index.y = Mathf.Clamp(index.y, 0, d - 1);
    
        index.z = Mathf.FloorToInt((pt.z - grid.min.z) / (grid.max.z - grid.min.z) * d);
        index.z = Mathf.Clamp(index.z, 0, d - 1);
    
        return index;
    }
    
    public static bool IsGridIndexOutsideGrid(SpatialGrid grid, SpatialGridIndex index)
    {
        return  (index.x >= grid.dimension || index.x < 0) || 
                (index.y >= grid.dimension || index.y < 0) || 
                (index.z >= grid.dimension || index.z < 0);
    }
    
    public static Vector3 GetGridLocalMin(SpatialGrid grids, SpatialGridIndex index)
    {
        Vector3 localMin = new Vector3(0, 0, 0);
        localMin.x = grids.min.x + grids.w * index.x;
        localMin.y = grids.min.y + grids.h * index.y;
        localMin.z = grids.min.z + grids.d * index.z;
        return localMin;
    }

    public static Vector3 GetGridLocalMax(SpatialGrid grids, SpatialGridIndex index)
    {
        Vector3 localMax = new Vector3(0, 0, 0);
        localMax.x = grids.min.x + grids.w * (index.x + 1);
        localMax.y = grids.min.y + grids.h * (index.y + 1);
        localMax.z = grids.min.z + grids.d * (index.z + 1);
        return localMax;
    }
    
    
    
    public static void InnerDistanceToBoundary(Ray ray, GridStep steps, Vector3 min, Vector3 max, ref RayHit rayHitX, ref RayHit rayHitY, ref RayHit rayHitZ)
    {
        if(steps.x >= 0)
        {
            rayHitX = RayPlaneIntersectionCompute.RayPlaneIntersection(ray, GridNormalLeft, max);
        }
        else
        {
            rayHitX = RayPlaneIntersectionCompute.RayPlaneIntersection(ray, GridNormalRight, min);
        }
    
        if(steps.y >= 0)
        {
            rayHitY = RayPlaneIntersectionCompute.RayPlaneIntersection(ray, GridNormalDown, max);
        }
        else
        {
            rayHitY = RayPlaneIntersectionCompute.RayPlaneIntersection(ray, GridNormalUp, min);
        }
    
        if(steps.z >= 0)
        {
            rayHitZ = RayPlaneIntersectionCompute.RayPlaneIntersection(ray, GridNormalBack, max);
        }
        else
        {
            rayHitZ = RayPlaneIntersectionCompute.RayPlaneIntersection(ray, GridNormalForward, min);
        }
    }
    
    public static void DistanceBetweenBoundary(Ray ray, GridStep steps, SpatialGridIndex entryGridIndex, SpatialGrid grids, Vector3 firstHitX, Vector3 firstHitY, Vector3 firstHitZ, ref RayHit rayHitX, ref RayHit rayHitY, ref RayHit rayHitZ)
    {
        Vector3 min = GetGridLocalMin(grids, entryGridIndex);
        Vector3 max = GetGridLocalMax(grids, entryGridIndex);

        Ray rX = SpatialGridTrace.CreateRay(firstHitX, ray.direction);
        Ray rY = SpatialGridTrace.CreateRay(firstHitY, ray.direction);
        Ray rZ = SpatialGridTrace.CreateRay(firstHitZ, ray.direction);
    
        if(steps.x >= 0)
        {
            rayHitX = RayPlaneIntersectionCompute.RayPlaneIntersection(rX, GridNormalLeft, max + GridNormalRight);
        }
        else
        {
            rayHitX = RayPlaneIntersectionCompute.RayPlaneIntersection(rX, GridNormalRight, min + GridNormalLeft);
        }
    
        if(steps.y >= 0)
        {
            rayHitY = RayPlaneIntersectionCompute.RayPlaneIntersection(rY, GridNormalDown, max + GridNormalUp);
        }
        else
        {
            rayHitY = RayPlaneIntersectionCompute.RayPlaneIntersection(rY, GridNormalUp, min + GridNormalDown);
        }
    
        if(steps.z >= 0)
        {
            rayHitZ = RayPlaneIntersectionCompute.RayPlaneIntersection(rZ, GridNormalBack, max + GridNormalForward);
        }
        else
        {
            rayHitZ = RayPlaneIntersectionCompute.RayPlaneIntersection(rZ, GridNormalForward, min + GridNormalBack);
        }
    }
    
    public static void GetDeltaToBoundary(SpatialGridIndex entryGridIndex, SpatialGrid grids, Ray entryToBoundaryRay, GridStep steps, ref RayHit rayHitX, ref RayHit rayHitY, ref RayHit rayHitZ)
    {
        InnerDistanceToBoundary(
            entryToBoundaryRay, 
            steps, 
            GetGridLocalMin(grids, entryGridIndex),
            GetGridLocalMax(grids, entryGridIndex),
            ref rayHitX,
            ref rayHitY,
            ref rayHitZ);
    }
}
