using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class PhysicalRayCaster : MonoBehaviour
{
    [SerializeField] private float m_farThreshold = 20;

    [SerializeField, TextArea] private string hitResult;
    
    private Vector3 cacheOrigin = Vector3.zero;
    private Vector3 cacheDirection = Vector3.up;

    
    private List<CubeController> _voxelHits = new List<CubeController>();

    private void Start()
    {
        Vector3 worldPos = transform.position;
        Vector3 forward = transform.forward;
        PerformRayCasting(worldPos, forward, m_farThreshold);
        cacheOrigin = worldPos;
        cacheDirection = forward;
    }


    private void Update()
    {
        Vector3 worldPos = transform.position;
        Vector3 forward = transform.forward;
        
        if (cacheOrigin != worldPos || cacheDirection != forward)
        {
            PerformRayCasting(worldPos, forward, m_farThreshold);
            cacheOrigin = worldPos;
            cacheDirection = forward;
        }
        
        _voxelHits.ForEach((controller =>
        {
            controller.ApplyColor(Color.green);
        }));
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        
        Vector3 origin = transform.position;
        Vector3 forward = transform.forward;
        
        Gizmos.DrawLine(origin, origin + forward * m_farThreshold);
    }


    private void PerformRayCasting(Vector3 origin, Vector3 direction, float farThreshold)
    {
        RaycastHit[] hits = Physics.RaycastAll(origin, direction, farThreshold);

        _voxelHits.Clear();
        
        if (hits.Length == 0)
        {
            hitResult = "No intersection";
            return;
        }
        
        StringBuilder sb = new StringBuilder();

        for (int i = 0; i < hits.Length; i++)
        {
            CubeController cubeController = hits[i].collider.GetComponent<CubeController>();
            sb.AppendLine(cubeController.Coordinate.ToString());
            
            _voxelHits.Add(cubeController);
        }

        hitResult = sb.ToString();
    }
}
