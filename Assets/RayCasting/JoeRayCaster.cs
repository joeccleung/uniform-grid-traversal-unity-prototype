using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class JoeRayCaster : MonoBehaviour
{
    [SerializeField] private List<int> _GeometryCountList;
    [SerializeField] private List<RTTriangle_t> _GeometryGridList;
    
    [SerializeField] private float m_farThreshold = 20;

    [SerializeField] private RayHit m_bestRayHit;
    [SerializeField, TextArea] private string hitResult;


    private Vector3 cacheOrigin = Vector3.zero;
    private Vector3 cacheDirection = Vector3.zero;
    
    
    public static List<CubeController> voxelsHitList = new List<CubeController>();
    
   
    
    private void Update()
    {
        Vector3 origin = transform.position;
        Vector3 direction = transform.forward;

        if (origin != cacheOrigin || direction != cacheDirection)
        {
            Ray cameraRay = new Ray(origin, direction);
            SpatialGrid spatialGrid = SpatialGrid.CreateSpatialGrid(CubeGlobalData.Instance.boxMin, CubeGlobalData.Instance.boxMax, CubeGlobalData.Instance.GetDimension());
            
            m_bestRayHit = SpatialGridTrace.Trace(cameraRay, spatialGrid, _GeometryCountList, _GeometryGridList, -1);
            cacheOrigin = origin;
            cacheDirection = direction;
        }

        voxelsHitList.ForEach((controller =>
        {
            controller.ApplyColor(Color.red);
        }));
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        
        Vector3 origin = transform.position;
        Vector3 forward = transform.forward;
        
        Gizmos.DrawLine(origin, origin + forward * m_farThreshold);
    }
}
