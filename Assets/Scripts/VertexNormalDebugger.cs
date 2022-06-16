using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class VertexNormalDebugger : MonoBehaviour
{
    [SerializeField] private Color _color = Color.red;
    [SerializeField, Min(0)] private float _lenght = 0.5f;
    [SerializeField] private bool _drawWithSelected;

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

        if (_meshFilter == null || _meshFilter.mesh == null)
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

        if (_meshFilter == null || _meshFilter.mesh == null)
        {
            return;
        }

        DrawNormals();
    }

    private void DrawNormals()
    {
        Gizmos.color = _color;

        Mesh mesh = _meshFilter.mesh;
        for (var i = 0; i < mesh.normals.Length; i++)
        {
            Vector3 startPosition = transform.TransformPoint(mesh.vertices[i]);
            Vector3 endPosition = startPosition + transform.TransformDirection(mesh.normals[i]) * _lenght;
            
            Gizmos.DrawLine(startPosition, endPosition);
        }
    }
}
