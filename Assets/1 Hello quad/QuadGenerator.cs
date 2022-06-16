using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class QuadGenerator : MonoBehaviour
{
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
        _vertices = new[]
        {
            new Vector3(0, 0, 0),
            new Vector3(0, 0, 1),
            new Vector3(1, 0, 0),
            new Vector3(1, 0, 1)
        };

        _triangles = new[]
        {
            0, 1, 2,
            1, 3, 2
        };
    }

    private void UpdateMesh()
    {
        _mesh.Clear();

        _mesh.vertices = _vertices;
        _mesh.triangles = _triangles;

        _mesh.RecalculateNormals();
    }
}