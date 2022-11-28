using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class TexturedMeshGenerator : MonoBehaviour
{
    [SerializeField] private int _xSize = 20;
    [SerializeField] private int _zSize = 20;

    [Header("Noise")]
    [SerializeField] private float _scale = 5;
    [SerializeField] private float _amplitude = 5;
    [SerializeField] private float _xOffset;
    [SerializeField] private float _zOffset;
    
    [Header("Animation")]
    [SerializeField] private bool _isAnimate;
    [SerializeField] private float _xOffsetSpeed = 1f;
    [SerializeField] private float _zOffsetSpeed = 1f;

    private MeshFilter _meshFilter;
    private Mesh _mesh;

    private Vector3[] _vertices;
    private Vector2[] _uvs;
    private int[] _triangles;

    private int VertexCount => (_xSize + 1) * (_zSize + 1);
    private float ResolutionX => 1f / _xSize;
    private float ResolutionZ => 1f / _zSize;

    private void Start()
    {
        _meshFilter = GetComponent<MeshFilter>();
        _mesh = new Mesh();
        _meshFilter.mesh = _mesh;

        GenerateMesh();
    }

    private void Update()
    {
        UpdateVertices();
        UpdateMesh();

        if (_isAnimate)
        {
            _xOffset += Time.deltaTime * _xOffsetSpeed;
            _zOffset += Time.deltaTime * _zOffsetSpeed;
        }
    }

    private void GenerateMesh()
    {
        _mesh.Clear();

        _vertices = new Vector3[VertexCount];
        _triangles = new int[_xSize * _zSize * 6];

        UpdateVertices();

        for (int vert = 0, tris = 0, z = 0; z < _zSize; z++, vert++)
        {
            for (var x = 0; x < _xSize; x++, vert++)
            {
                _triangles[tris + 0] = vert + 0;
                _triangles[tris + 1] = vert + _xSize + 1;
                _triangles[tris + 2] = vert + 1;
                _triangles[tris + 3] = vert + 1;
                _triangles[tris + 4] = vert + _xSize + 1;
                _triangles[tris + 5] = vert + _xSize + 2;

                tris += 6;
            }
        }

        _uvs = new Vector2[VertexCount];
        for (int i = 0, z = 0; z <= _zSize; z++)
        {
            for (var x = 0; x <= _xSize; x++, i++)
            {
                _uvs[i] = new Vector2((float)x / _xSize, (float)z / _zSize);
            }
        }

        _mesh.vertices = _vertices;
        _mesh.triangles = _triangles;
        _mesh.uv = _uvs;
    }

    private void UpdateVertices()
    {
        for (int i = 0, z = 0; z <= _zSize; z++)
        {
            for (var x = 0; x <= _xSize; x++, i++)
            {
                float y = GetNoiseSample(x, z);
                _vertices[i] = new Vector3(x, y, z);
            }
        }
    }

    private float GetNoiseSample(int x, int z)
    {
        return Mathf.PerlinNoise(x * ResolutionX * _scale + _xOffset, z * ResolutionZ * _scale + _zOffset) * _amplitude;
    }

    private void UpdateMesh()
    {
        _mesh.vertices = _vertices;
        _mesh.RecalculateNormals();
    }
}