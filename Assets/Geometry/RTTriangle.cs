using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RTTriangle : RTGeometry
{
    /// <summary>
    /// The vertices of the triangle relative to the game object center
    /// </summary>
    [SerializeField] private Vector3 m_vertices0 = Vector3.zero;

    [SerializeField] private Vector3 m_vertices1 = Vector3.zero;
    [SerializeField] private Vector3 m_vertices2 = Vector3.zero;
    [SerializeField] private bool m_isDoubleSide = true;
    [SerializeField] private string m_materialName;

    private int m_materialIndex = -1;


    private Vector4 m_worldVert0 = Vector4.zero;
    private Vector4 m_worldVert1 = Vector4.zero;
    private Vector4 m_worldVert2 = Vector4.zero;
    private Vector3 m_cachedNormal;
    private float m_cachedPlaneD;
    private float m_cachedArea;
    private Vector3 m_cahcedPosition = Vector3.zero;


    public override RTGeometryType GetGeometryType()
    {
        return RTGeometryType.Triangle;
    }

    public string GetMaterialName()
    {
        return m_materialName;
    }

    public void SetMaterialIndex(int index)
    {
        m_materialIndex = index;
    }

    public RTTriangle_t GetGeometry()
    {
        UpdateTriangle();

        return new RTTriangle_t
        {
            vert0 = m_worldVert0,
            vert1 = m_worldVert1,
            vert2 = m_worldVert2,
            normal = m_cachedNormal,
            planeD = m_cachedPlaneD,
            area = m_cachedArea,
            isDoubleSide = m_isDoubleSide ? 1 : 0
        };
    }


    private void UpdateTriangle()
    {
//        m_worldVert0 = (transform.localRotation * m_vertices0) + transform.localPosition;
//        m_worldVert1 = (transform.localRotation * m_vertices1) + transform.localPosition;
//        m_worldVert2 = (transform.localRotation * m_vertices2) + transform.localPosition;

        Matrix4x4 localToWorldMat = transform.localToWorldMatrix;
        m_worldVert0 = localToWorldMat.MultiplyPoint(m_vertices0);
        m_worldVert1 = localToWorldMat.MultiplyPoint(m_vertices1);
        m_worldVert2 = localToWorldMat.MultiplyPoint(m_vertices2);

        Vector3 cross = Vector3.Cross(m_worldVert1 - m_worldVert0, m_worldVert2 - m_worldVert0);
        m_cachedNormal = Vector3.Normalize(cross);
        m_cachedPlaneD = -1 * Vector3.Dot(m_cachedNormal, m_worldVert0);
        m_cachedArea = Vector3.Dot(m_cachedNormal, cross);
    }


    private Vector3 GetScaledVector(Vector3 v, Vector3 scale)
    {
        return new Vector3(v.x * scale.x, v.y + scale.y / 2f, v.z * scale.z);
    }

    public static void IntersectTriangle(Ray ray, ref RayHit bestHit, RTTriangle_t tri)
    {
        // (1) Here we try to intersect the plane equation of the triangle with the ray and see if there is intersect
        // (2) If there is intersection, calculate the Barycentric coordinates

        // (1a) Plane equation <-- Ray equation: dot(n, P) x d = 0 <-- o + tD = P
        // (1b) dot(n, o + tD) + d = 0
        // (1c) dot(n, o) + dot(n, tD) = - d
        // (1d) dot(n, tD) = - d - dot(n, o)
        // (1e) t = - (d + dot(n, o)) / dot(n, D)
        // Here, we need to check |dot(n, D)| < float.Epsilon

        float Epsilon = 1.19e-07f;

        if (tri.area < Epsilon)
        {
            return; // The triangle does not exists
        }

        float nDotD = Vector3.Dot(tri.normal, ray.direction);
        if (Mathf.Abs(nDotD) < Epsilon)
        {
            return; // The support plane is parallel to the ray, no intersection
        }

        float nDotO = Vector3.Dot(tri.normal, ray.origin);
        float t = -1 * (tri.planeD + nDotO) / nDotD;

        // (2a) If the hit distance is negative or farther the bestHit, drop
        // (2b) Calculate the hit point coordinates
        // (2c) Determine if the hit point is on the L.H.S of the triangle edges
        // (2d) If hit point is within triangle, update bestHit

        if (t < 0 || t > bestHit.distance)
        {
            return;
        }

        Vector3 hit = ray.origin + t * ray.direction;

        Vector3 vAQ = hit - tri.vert0;
        Vector3 vBQ = hit - tri.vert1;
        float areaABQ = Vector3.Dot(Vector3.Cross(tri.vert1 - tri.vert0, vAQ), tri.normal);
        if (areaABQ < 0)
        {
            return; // Is on the R.H.S
        }

        float areaBCQ = Vector3.Dot(Vector3.Cross(tri.vert2 - tri.vert1, vBQ), tri.normal);
        if (areaBCQ < 0)
        {
            return; // Is on the R.H.S
        }

        float areaAQC = Vector3.Dot(Vector3.Cross(vAQ, tri.vert2 - tri.vert0), tri.normal);
        if (areaAQC < 0)
        {
            return; // Is on the R.H.S
        }

        bestHit.barycentric = new Vector3( areaBCQ / tri.area, areaAQC / tri.area, areaABQ / tri.area);
        bestHit.hitPoint = hit;
        bestHit.distance = t;

        if (nDotD > 0)
        {
            bestHit.normal = -tri.normal;
        }
        else
        {
            bestHit.normal = tri.normal;
        }
    }
}