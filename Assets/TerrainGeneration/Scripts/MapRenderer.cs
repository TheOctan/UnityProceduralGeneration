using OctanGames.TerrainGeneration.Scripts.Data;
using UnityEngine;

namespace OctanGames.TerrainGeneration.Scripts
{
    public class MapRenderer : MonoBehaviour
    {
        [SerializeField] private Renderer _renderer;
        [SerializeField] private MeshFilter _meshFilter;
        [SerializeField] private MeshRenderer _meshRenderer;

        public void DrawTexture(Texture2D texture)
        {
            _renderer.sharedMaterial.mainTexture = texture;
            _renderer.transform.localScale = new Vector3(texture.width, 1, texture.height);
        }

        public void DrawMesh(MeshData mesh, Texture2D texture)
        {
            _meshFilter.sharedMesh = mesh.CreateMesh();
            _meshRenderer.sharedMaterial.mainTexture = texture;
        }
    }
}