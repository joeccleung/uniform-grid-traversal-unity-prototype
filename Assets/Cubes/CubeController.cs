using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeController : MonoBehaviour
{

    
    
    public Vector3 Coordinate = Vector3.zero;

    private Vector3 _center = Vector3.zero;
    private Vector3 _leftWall = new Vector3(-0.5f, 0, 0);
    private Vector3 _rightWall = new Vector3(0.5f, 0, 0);
    private Vector3 _topWall = new Vector3(0, 0.5f, 0);
    private Vector3 _bottomWall = new Vector3(0, -0.5f, 0);
    private Vector3 _backWall = new Vector3(0, 0, 0.5f);
    private Vector3 _frontWall = new Vector3(0, 0, -0.5f);

    private Color _defaultColor = new Color(1, 1, 1, 0.1f);
    private Color _color = Color.clear;
    private bool _appliedColor = false;
    private Material _mat;


    private void Start()
    {
        _center = transform.position;
        _leftWall += _center;
        _rightWall += _center;
        _topWall += _center;
        _bottomWall += _center;
        _backWall += _center;
        _frontWall += _center;

        _mat = GetComponent<MeshRenderer>().material;
    }


    private void Update()
    {
        if (!_appliedColor)
        {
            _color = _defaultColor;
        }

        _mat.color = _color;

        _appliedColor = false;
        _color = Color.clear;
    }


    public void ApplyColor(Color color)
    {
        _appliedColor = true;
        _color += color;
    }
    
}
