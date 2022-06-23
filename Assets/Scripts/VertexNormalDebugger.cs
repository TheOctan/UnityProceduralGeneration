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
        if (!_drawWithSelected)
        {
            return;
        }

        DrawNormals();
    }

    private void OnDrawGizmos()
    {
        if (_drawWithSelected)
        {
            return;
        }

        DrawNormals();
    }

    private void DrawNormals()
    {
        if (IsComponentsReady())
        {
            return;
        }

        Mesh mesh = _meshFilter.mesh;

        if (IsVerticesReady(mesh))
        {
            return;
        }

        for (var i = 0; i < mesh.vertices.Length; i++)
        {
            Vector3 vertex = mesh.vertices[i];
            Vector3 normal = mesh.normals[i];

            Gizmos.color = _vertexColor;
            Gizmos.DrawSphere(transform.TransformPoint(vertex), _vertexRadius);

            Vector3 startPosition = transform.TransformPoint(vertex);
            Vector3 endPosition = startPosition + transform.TransformDirection(normal) * _lenght;

            Gizmos.color = _normalColor;
            Gizmos.DrawLine(startPosition, endPosition);
        }
    }

    private bool IsComponentsReady()
    {
        return _meshFilter == null ||
               _meshFilter.mesh == null;
    }

    private static bool IsVerticesReady(Mesh mesh)
    {
        return mesh.vertices == null || mesh.normals == null;
    }
}
