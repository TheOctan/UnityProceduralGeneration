using System.Collections.Generic;
using OctanGames.TerrainGeneration.Scripts.Data;
using UnityEngine;

namespace OctanGames.TerrainGeneration.Scripts
{
    [RequireComponent(typeof(MapGenerator))]
    public class EndlessTerrain : MonoBehaviour
    {
        private const float MAX_VIEW_DISTANCE = 450;

        [SerializeField] private Material _material;
        [SerializeField] private Transform _viewer;
        private MapGenerator _mapGenerator;
        private readonly Dictionary<Vector2, TerrainChunk> _terrainChunks = new();
        private readonly List<TerrainChunk> _lastUpdatedChunks = new();

        public static Vector2 ViewerPosition { get; set; }
        private static int ChunkSize => MapGenerator.MAP_CHUNK_SIZE - 1;
        private int CountForwardVisibleChunks { get; set; }

        private void Start()
        {
            _mapGenerator = GetComponent<MapGenerator>();
            CountForwardVisibleChunks = Mathf.RoundToInt(MAX_VIEW_DISTANCE / ChunkSize);
        }

        private void Update()
        {
            Vector3 position = _viewer.position;
            ViewerPosition = new Vector2(position.x, position.z);
            UpdateVisibleChunks();
        }

        private void UpdateVisibleChunks()
        {
            _lastUpdatedChunks.ForEach(chunk => chunk.SetVisible(false));
            _lastUpdatedChunks.Clear();

            int currentChunkCoordX = Mathf.RoundToInt(ViewerPosition.x / ChunkSize);
            int currentChunkCoordY = Mathf.RoundToInt(ViewerPosition.y / ChunkSize);

            for (int yOffset = -CountForwardVisibleChunks; yOffset <= CountForwardVisibleChunks; yOffset++)
            {
                for (int xOffset = -CountForwardVisibleChunks; xOffset <= CountForwardVisibleChunks; xOffset++)
                {
                    var viewedChunkCoord = new Vector2(currentChunkCoordX + xOffset, currentChunkCoordY + yOffset);

                    if (_terrainChunks.ContainsKey(viewedChunkCoord))
                    {
                        TerrainChunk chunk = _terrainChunks[viewedChunkCoord];
                        chunk.Update();

                        if (chunk.IsVisible)
                        {
                            _lastUpdatedChunks.Add(chunk);
                        }
                    }
                    else
                    {
                        _terrainChunks.Add(viewedChunkCoord,
                            new TerrainChunk(viewedChunkCoord, ChunkSize, transform, _material, _mapGenerator));
                    }
                }
            }
        }

        private class TerrainChunk
        {
            private readonly GameObject _meshObject;
            private readonly MapGenerator _mapGenerator;
            private readonly MeshFilter _meshFilter;
            private readonly MeshRenderer _meshRenderer;
            private Bounds _bounds;
            private MapData _mapData;

            public bool IsVisible => _meshObject.activeSelf;

            public TerrainChunk(Vector2 coord, int size, Transform parent, Material material, MapGenerator mapGenerator)
            {
                Vector2 position = coord * size;
                var position3D = new Vector3(position.x, 0, position.y);
                _bounds = new Bounds(position, Vector2.one * size);

                _meshObject = new GameObject($"Chunk {Vector2Int.RoundToInt(coord)}");
                _meshFilter = _meshObject.AddComponent<MeshFilter>();
                _meshRenderer = _meshObject.AddComponent<MeshRenderer>();
                _meshRenderer.material = material;
                _meshObject.transform.position = position3D;
                _meshObject.transform.SetParent(parent);
                SetVisible(false);

                _mapGenerator = mapGenerator;
                _mapGenerator.RequestMapData(OnMapDataReceived);
            }

            public void Update()
            {
                float viewerSqrDistanceFromNearestEdge = _bounds.SqrDistance(ViewerPosition);
                bool visible = viewerSqrDistanceFromNearestEdge <= MAX_VIEW_DISTANCE * MAX_VIEW_DISTANCE;
                SetVisible(visible);
            }

            public void SetVisible(bool visible)
            {
                _meshObject.SetActive(visible);
            }

            private void OnMapDataReceived(MapData mapData)
            {
                _mapGenerator.RequestMeshData(mapData, OnMeshDataReceived);
            }

            private void OnMeshDataReceived(MeshData meshData)
            {
                _meshFilter.mesh = meshData.CreateMesh();
            }
        }
    }
}