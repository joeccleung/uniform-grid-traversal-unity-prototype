using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeGlobalData
{
    public static CubeGlobalData Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new CubeGlobalData();
            }

            return _instance;
        }
    }
    
    private static CubeGlobalData _instance;


    public Vector3 boxMin = Vector3.zero;
    public Vector3 boxMax = Vector3.zero;
    private int dimension = 0;


    private CubeController[, ,] _cubes;


    public int GetDimension()
    {
        return dimension;
    }

    public void SetDimension(int dimen)
    {
        dimension = dimen;
        
        _cubes = new CubeController[dimension, dimension, dimension];
    }


    public void AddCube(int x, int y, int z, CubeController cube)
    {
        _cubes[x, y, z] = cube;
    }



    public CubeController GetGridAtIndex(SpatialGridIndex index)
    {
        if (index.x < 0 || index.x >= _cubes.GetLength(0))
        {
            return null;
        }
        
        if (index.y < 0 || index.y >= _cubes.GetLength(1))
        {
            return null;
        }
        
        if (index.z < 0 || index.z >= _cubes.GetLength(2))
        {
            return null;
        }

        return _cubes[index.x, index.y, index.z];
    }

}
