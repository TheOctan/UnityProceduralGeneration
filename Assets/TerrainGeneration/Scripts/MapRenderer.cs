using System;
using OctanGames.TerrainGeneration.Scripts.Data;
using UnityEngine;
using UnityEngine.Serialization;

namespace OctanGames.TerrainGeneration.Scripts
{
    public class MapRenderer : MonoBehaviour
    {
        [SerializeField] private Renderer _textureRenderer;
        [SerializeField] private MeshFilter _meshFilter;
        [SerializeField] private MeshRenderer _meshRenderer;

        private void Start()
        {
            _textureRenderer.gameObject.SetActive(false);
            _meshRenderer.gameObject.SetActive(false);
        }

        public void DrawTexture(Texture2D texture)
        {
            _textureRenderer.gameObject.SetActive(true);
            _meshRenderer.gameObject.SetActive(false);

            _textureRenderer.sharedMaterial.mainTexture = texture;
            _textureRenderer.transform.localScale = new Vector3(texture.width, 1, texture.height);
        }

        public void DrawMesh(MeshData mesh, Texture2D texture)
        {
            _textureRenderer.gameObject.SetActive(false);
            _meshRenderer.gameObject.SetActive(true);

            _meshFilter.sharedMesh = mesh.CreateMesh();
            _meshRenderer.sharedMaterial.mainTexture = texture;
        }
    }
}