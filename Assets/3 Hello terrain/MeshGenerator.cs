using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class MeshGenerator : MonoBehaviour
{
    [SerializeField] private int _xSize = 20;
    [SerializeField] private int _zSize = 20;

    private MeshFilter _meshFilter;
    private Mesh _mesh;

    private Vector3[] _vertices;
    private int[] _triangles;

    private void Awake()
    {
        _meshFilter = GetComponent<MeshFilter>();

        _mesh = new Mesh();
        _meshFilter.mesh = _mesh;

        CreateMesh();
        UpdateMesh();
    }

    private void CreateMesh()
    {
        _vertices = new Vector3[(_xSize + 1) * (_zSize + 1)];

        for (int i = 0, z = 0; z <= _xSize; z++)
        {
            for (var x = 0; x <= _xSize; x++, i++)
            {
                _vertices[i] = new Vector3(x, 0, z);
            }
        }

        _triangles = new int[6];
        _triangles[0] = 0;
        _triangles[1] = _xSize + 1;
        _triangles[2] = 1;
        _triangles[3] = 1;
        _triangles[4] = _xSize + 1;
        _triangles[5] = _xSize + 2;
    }

    private void UpdateMesh()
    {
        _mesh.Clear();

        _mesh.vertices = _vertices;
        _mesh.triangles = _triangles;

        _mesh.RecalculateNormals();
    }

    private void OnDrawGizmos()
    {
        if (_vertices == null)
        {
            return;
        }

        foreach (Vector3 point in _vertices)
        {
            Gizmos.DrawSphere(transform.TransformPoint(point), 0.1f);
        }
    }
}
