using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpatialGridTrace
{
    public static Ray CreateRay(Vector3 origin, Vector3 direction)
    {
        Ray ray = new Ray();
        ray.origin = origin;
        ray.direction = direction;
        return ray;
    }
    
    public static Vector3 GetPoint(Ray ray, float distance)
    {
        return ray.origin + ray.direction * distance;
    }


    public static GridStep DetermineGridStep(Ray ray)
    {
        GridStep step = new GridStep();

        step.x = ray.direction.x >= 0 ? 1 : -1;
        step.y = ray.direction.y >= 0 ? 1 : -1;
        step.z = ray.direction.z >= 0 ? 1 : -1;

        return step;
    }

    public static void GetNumberOfGeometryInGrid(SpatialGridIndex index, List<int> geomGridList, int dimension,
        out int start, out int count)
    {
        int i = index.z + index.y * dimension + index.x * dimension * dimension;
        
        try
        {
            start = geomGridList[i];
            count = geomGridList[i + 1] - geomGridList[i]; // The index is offset by 1 due to leading zero field
        }
        catch (ArgumentException e)
        {
               Debug.LogError($"ArgOut i = {i} x = {index.x} y = {index.y} z = {index.z} d = {dimension}");
               start = 0;
               count = 0;
        }
    }
    
    
    
    public static RayHit LocalGridTrace(Ray ray, SpatialGridIndex index, List<int> geomGridList, List<RTTriangle_t> triangles, SpatialGrid grids, int excludeGeometry)
    {
        RayHit bestHit = RayHit.CreateRayHit();
        RayHit currentHit = RayHit.CreateRayHit();

        int start = 0;
        int count = 0;
    
        GetNumberOfGeometryInGrid(index, geomGridList, grids.dimension, out start, out count);
    
        if(count == 0)
        {
            return bestHit;
        }
    
        Vector3 localMin = SpatialGridCompute.GetGridLocalMin(grids, index);
        Vector3 localMax = SpatialGridCompute.GetGridLocalMax(grids, index);
    
        for(int t = 0; t < count; t++)
        {
            if(triangles[start + t].id == excludeGeometry)
            {
                continue;
            }
            RTTriangle.IntersectTriangle(ray, ref currentHit, triangles[start + t]);
            
            if(!(localMin.x <= currentHit.hitPoint.x && currentHit.hitPoint.x <= localMax.x))
            {
                currentHit = bestHit;   // Reset currentHit
                continue;
            }
            
            if(!(localMin.y <= currentHit.hitPoint.y && currentHit.hitPoint.y <= localMax.y))
            {
                currentHit = bestHit;   // Reset currentHit
                continue;
            }
            
            if(!(localMin.z <= currentHit.hitPoint.z && currentHit.hitPoint.z <= localMax.z))
            {
                currentHit = bestHit;   // Reset currentHit
                continue;
            }
            
            bestHit = currentHit;
        
        }
    
        return bestHit;
    }
    
    

    public static RayHit Trace(Ray cameraRay, SpatialGrid spatialGrid, List<int> geometryIndexList,
        List<RTTriangle_t> geometryList, int exclude)
    {
        RayHit bestHit = RayHit.CreateRayHit();

        float t0 = float.NegativeInfinity;
        float t1 = float.PositiveInfinity;

        if (RayBoxIntersectionCompute.RayBoxIntersection(cameraRay, spatialGrid.min, spatialGrid.max, ref t0, ref t1))
        {
            Vector3 entry = cameraRay.GetPoint(t1);

            GridStep step = DetermineGridStep(cameraRay);

            Ray entryToBoundaryRay = CreateRay(entry, cameraRay.direction);
            SpatialGridIndex entryGridIndex = SpatialGridCompute.GetGridIndexAtPoint(spatialGrid, entry);
            
            RayHit deltaX = new RayHit();
            RayHit deltaY = new RayHit();
            RayHit deltaZ = new RayHit();
            SpatialGridCompute.GetDeltaToBoundary(
                entryGridIndex, 
                spatialGrid,
                entryToBoundaryRay,
                step,
                ref deltaX,
                ref deltaY,
                ref deltaZ);
            
            RayHit distToNextX = new RayHit();
            RayHit distToNextY = new RayHit();
            RayHit distToNextZ = new RayHit();
            SpatialGridCompute.DistanceBetweenBoundary(
                cameraRay, 
                step, 
                entryGridIndex,
                spatialGrid,
                deltaX.hitPoint,
                deltaY.hitPoint,
                deltaZ.hitPoint,
                ref distToNextX,
                ref distToNextY,
                ref distToNextZ
            );
            
            float tx = deltaX.distance;
            float ty = deltaY.distance;
            float tz = deltaZ.distance;
            
            SpatialGridIndex current = entryGridIndex;
            
            #region Debug
            CubeController currentGridController = CubeGlobalData.Instance.GetGridAtIndex(current);
            JoeRayCaster.voxelsHitList.Clear();
            JoeRayCaster.voxelsHitList.Add(currentGridController);
            #endregion    //Debug

            bool hasHit = false;
            bool outside = false;
            
            do
            {
                bestHit = LocalGridTrace(
                    cameraRay,
                    current,
                    geometryIndexList,
                    geometryList,
                    spatialGrid,
                    -1);
                                    
                if (tx < ty)
                {
                    if (tx < tz)
                    {
                        // Move On X
                        current.x = current.x + step.x;
                        tx += distToNextX.distance;
                    }
                    else
                    {
                        // Move On Z
                        current.z = current.z + step.z;
                        tz += distToNextZ.distance;
                    }
                }
                else
                {
                    if (ty < tz)
                    {
                        // Move On Y
                        current.y = current.y + step.y;
                        ty += distToNextY.distance;
                    }
                    else
                    {
                        // Move On Z
                        current.z = current.z + step.z;
                        tz += distToNextZ.distance;
                    }
                }
                
                #region Debug
                currentGridController = CubeGlobalData.Instance.GetGridAtIndex(current);
                if (currentGridController != null)
                {
                    JoeRayCaster.voxelsHitList.Add(currentGridController);
                }
                #endregion

                hasHit = (bestHit.distance < float.PositiveInfinity);
                outside = SpatialGridCompute.IsGridIndexOutsideGrid(spatialGrid, current);
            }
            while(!hasHit && !outside);
        }
        else
        {
            JoeRayCaster.voxelsHitList.Clear();
        }

        return bestHit;
    }
}