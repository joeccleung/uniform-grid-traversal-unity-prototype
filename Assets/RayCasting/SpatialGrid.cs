using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct SpatialGrid
{
    public Vector3 min;
    public Vector3 max;
    public int dimension;
    
    public float w;    // Grid Width
    public float h;    // Grid Height
    public float d;    // Grid Depth


    public static SpatialGrid CreateSpatialGrid(Vector3 gridBoxMin, Vector3 gridBoxMax, int gridDimension)
    {
        SpatialGrid grids;

        grids.min = gridBoxMin;
        grids.max = gridBoxMax;
        grids.dimension = gridDimension;
        
        grids.w = (grids.max.x - grids.min.x) / gridDimension;
        grids.h = (grids.max.y - grids.min.y) / gridDimension;
        grids.d = (grids.max.z - grids.min.z) / gridDimension;

        return grids;
    }
}
