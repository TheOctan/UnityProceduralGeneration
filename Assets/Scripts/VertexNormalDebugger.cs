using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class VertexNormalDebugger : MonoBehaviour
{
    [SerializeField] private Color _color = Color.red;
    [SerializeField, Min(0)] private float _lenght = 0.5f;

    private MeshFilter _meshFilter;

    private void OnEnable()
    {
        _meshFilter = GetComponent<MeshFilter>();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = _color;

        if (_meshFilter == null || _meshFilter.mesh == null)
        {
            return;
        }

        Mesh mesh = _meshFilter.mesh;
        for (var i = 0; i < mesh.normals.Length; i++)
        {
            Vector3 startPosition = mesh.vertices[i];
            Vector3 endPosition = startPosition + mesh.normals[i] * _lenght;
            
            Gizmos.DrawLine(startPosition, endPosition);
        }
    }
}
