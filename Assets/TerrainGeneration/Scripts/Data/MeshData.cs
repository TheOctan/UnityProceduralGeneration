using UnityEngine;

namespace OctanGames.TerrainGeneration.Scripts.Data
{
    public class MeshData
    {
        public readonly Vector3[] Vertices;
        public readonly Vector2[] Uvs;

        private readonly int[] _triangles;
        private int _triangleIndex;

        public MeshData(int meshWidth, int meshHeight)
        {
            Vertices = new Vector3[meshWidth * meshHeight];
            Uvs = new Vector2[meshWidth * meshHeight];
            _triangles = new int[(meshWidth - 1) * (meshHeight - 1) * 6];
        }

        public void AddTriangle(int a, int b, int c)
        {
            _triangles[_triangleIndex] = a;
            _triangles[_triangleIndex + 1] = b;
            _triangles[_triangleIndex + 2] = c;

            _triangleIndex += 3;
        }

        public Mesh CreateMesh()
        {
            var mesh = new Mesh
            {
                vertices = Vertices,
                triangles = _triangles,
                uv = Uvs
            };
            mesh.RecalculateNormals();

            return mesh;
        }
    }
}