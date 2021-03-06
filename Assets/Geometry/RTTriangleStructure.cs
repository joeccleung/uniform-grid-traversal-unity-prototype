﻿
using System;
using UnityEngine;

/// <summary>
/// This structure matches with the RTSphere_t structure defined in RTSphere.compute
/// </summary>
[Serializable]
public struct RTTriangle_t
{
    public int id;  // geometryIndex
    public Vector3 vert0;
    public Vector3 vert1;
    public Vector3 vert2;
    public Vector3 normal;
    public float planeD;
    public float area;
    public int isDoubleSide;
    [HideInInspector] public int materialIndex;
    
    public static int GetSize()
    {
        return Vector3Extension.SizeOf() * 4
               + sizeof(int) * 3
               + sizeof(float) * 2;
    }

    public void SetId(int idInput)
    {
        id = idInput;
    }
}
