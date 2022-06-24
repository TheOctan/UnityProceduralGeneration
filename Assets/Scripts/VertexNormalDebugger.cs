using System;
using UnityEngine;
using UnityEngine.Serialization;

[RequireComponent(typeof(MeshFilter))]
public class VertexNormalDebugger : MonoBehaviour
{
    [SerializeField] private bool _drawWithSelected = true;

    [Header("Vertex")]
    [SerializeField] private Color _vertexColor = Color.gray;
    [SerializeField] private float _vertexRadius = 0.1f;
    
    [FormerlySerializedAs("_color")]
    [Header("Normal")]
    [SerializeField] private Color _normalColor = Color.red;
    [SerializeField, Min(0)] private float _lenght = 0.5f;

    private MeshFilter _meshFilter;

    private void OnEnable()
    {
        _meshFilter = GetComponent<MeshFilter>();
    }

    private void OnDrawGizmosSelected()
    {
        if (!_drawWithSelected || !IsComponentsReady())
        {
            return;
        }

        Mesh mesh = _meshFilter.sharedMesh;
        if (mesh.vertices != null && mesh.normals != null)
        {
            DrawVertices(mesh);
            DrawNormals(mesh);
        }
    }

    private void OnDrawGizmos()
    {
        if (_drawWithSelected || !IsComponentsReady())
        {
            return;
        }

        Mesh mesh = _meshFilter.sharedMesh;
        if (mesh.vertices != null && mesh.normals != null)
        {
            DrawVertices(mesh);
            DrawNormals(mesh);
        }
    }

    private bool IsComponentsReady()
    {
        return _meshFilter != null &&
               _meshFilter.sharedMesh != null;
    }

    private void DrawVertices(Mesh mesh)
    {
        Gizmos.color = _vertexColor;
        float scale = transform.lossyScale.magnitude;
        foreach (Vector3 vertex in mesh.vertices)
        {
            Gizmos.DrawSphere(transform.TransformPoint(vertex), _vertexRadius * scale);
        }
    }

    private void DrawNormals(Mesh mesh)
    {
        Gizmos.color = _normalColor;
        for (var i = 0; i < mesh.normals.Length; i++)
        {
            Vector3 normal = mesh.normals[i];
            Vector3 vertex = mesh.vertices[i];

            Vector3 startPosition = transform.TransformPoint(vertex);
            Vector3 endPosition = startPosition + transform.TransformDirection(normal) * _lenght;

            Gizmos.DrawLine(startPosition, endPosition);
        }
    }
}
