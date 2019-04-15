using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeGenerator : MonoBehaviour
{
    [SerializeField] private GameObject m_cubePrefab;
    [SerializeField] private int spawnCount = 3;

    private void Start()
    {
        CubeGlobalData.Instance.SetDimension(spawnCount);
        
        Vector3 lowerLeftFrontCubePosition = new Vector3(0.5f, 0.5f, 0.5f);

        for (int x = 0; x < spawnCount; x++)
        {
            for (int y = 0; y < spawnCount; y++)
            {
                for (int z = 0; z < spawnCount; z++)
                {
                    Vector3 coordinate = new Vector3(x, y, z);
                    GameObject cube = Instantiate(m_cubePrefab, lowerLeftFrontCubePosition + coordinate, Quaternion.identity, transform);
                    cube.name = string.Format("{0} {1} {2}", x, y, z);
                    CubeController controller = cube.GetComponent<CubeController>();
                    controller.Coordinate = coordinate;
                    
                    CubeGlobalData.Instance.AddCube(x, y, z, controller);
                }
            }
        }

        CubeGlobalData.Instance.boxMin = Vector3.zero;
        CubeGlobalData.Instance.boxMax = transform.GetChild(transform.childCount - 1).position + new Vector3(0.5f, 0.5f, 0.5f);

    }
}
