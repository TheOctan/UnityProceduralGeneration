using System;
using System.Collections.Generic;
using OctanGames.TerrainGeneration.Scripts.Data;
using UnityEngine;

namespace OctanGames.TerrainGeneration.Scripts
{
    [RequireComponent(typeof(MapGenerator))]
    public class EndlessTerrain : MonoBehaviour
    {
        [SerializeField] private LODInfo[] _detailLevles;
        [SerializeField] private Material _material;
        [SerializeField] private Transform _viewer;

        private readonly Dictionary<Vector2, TerrainChunk> _terrainChunks = new();
        private readonly List<TerrainChunk> _lastUpdatedChunks = new();
        private MapGenerator _mapGenerator;
        private float _maxViewDistance;

        private Vector2 ViewerPosition => new(_viewer.position.x, _viewer.position.z);
        private static int ChunkSize => MapGenerator.MAP_CHUNK_SIZE - 1;
        private int CountForwardVisibleChunks { get; set; }

        private void Start()
        {
            _mapGenerator = GetComponent<MapGenerator>();
            _maxViewDistance = _detailLevles[^1].visibleDistanceThreshold;
            CountForwardVisibleChunks = Mathf.RoundToInt(_maxViewDistance / ChunkSize);
        }

        private void Update()
        {
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
                        chunk.Update(ViewerPosition, _maxViewDistance);

                        if (chunk.IsVisible)
                        {
                            _lastUpdatedChunks.Add(chunk);
                        }
                    }
                    else
                    {
                        _terrainChunks.Add(viewedChunkCoord,
                            new TerrainChunk(viewedChunkCoord, ChunkSize, _detailLevles, transform, _material, _mapGenerator));
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

            private readonly LODInfo[] _detailLevels;
            private readonly LODMesh[] _lodMeshes;
            private Bounds _bounds;
            private MapData _mapData;

            private int _prevLODIndex = -1;
            private bool _mapDataReceived;

            public bool IsVisible => _meshObject.activeSelf;

            public TerrainChunk(Vector2 coord, int size, 
                LODInfo[] detailLevels, Transform parent, 
                Material material, MapGenerator mapGenerator)
            {
                Vector2 position = coord * size;
                var position3D = new Vector3(position.x, 0, position.y);
                _detailLevels = detailLevels;
                _bounds = new Bounds(position, Vector2.one * size);

                _meshObject = new GameObject($"Chunk {Vector2Int.RoundToInt(coord)}");
                _meshFilter = _meshObject.AddComponent<MeshFilter>();
                _meshRenderer = _meshObject.AddComponent<MeshRenderer>();
                _meshRenderer.material = material;
                _meshObject.transform.position = position3D;
                _meshObject.transform.SetParent(parent);
                SetVisible(false);

                _lodMeshes = new LODMesh[_detailLevels.Length];
                for (var i = 0; i < _detailLevels.Length; i++)
                {
                    int lod = _detailLevels[i].lod;
                    _lodMeshes[i] = new LODMesh(lod);
                }

                _mapGenerator = mapGenerator;
                _mapGenerator.RequestMapData(OnMapDataReceived);
            }

            public void Update(Vector2 viewerPosition, float maxViewDistance)
            {
                if (!_mapDataReceived)
                {
                    return;
                }

                float viewerSqrDistanceFromNearestEdge = _bounds.SqrDistance(viewerPosition);
                bool visible = viewerSqrDistanceFromNearestEdge <= maxViewDistance * maxViewDistance;

                if (visible)
                {
                    var lodIndex = 0;

                    for (var i = 0; i < _detailLevels.Length - 1; i++)
                    {
                        float visibleThreshold = _detailLevels[i].visibleDistanceThreshold;
                        if (viewerSqrDistanceFromNearestEdge > visibleThreshold * visibleThreshold)
                        {
                            lodIndex = i + 1;
                        }
                        else
                        {
                            break;
                        }
                    }

                    if (lodIndex != _prevLODIndex)
                    {
                        LODMesh lodMesh = _lodMeshes[lodIndex];
                        if (lodMesh.HasMesh)
                        {
                            _prevLODIndex = lodIndex;
                            _meshFilter.mesh = lodMesh.Mesh;
                        }
                        else if (!lodMesh.HasRequestedMesh)
                        {
                            lodMesh.RequestMesh(_mapData, _mapGenerator);
                        }
                    }
                }

                SetVisible(visible);
            }

            public void SetVisible(bool visible)
            {
                _meshObject.SetActive(visible);
            }

            private void OnMapDataReceived(MapData mapData)
            {
                _mapData = mapData;
                _mapDataReceived = true;
            }
        }

        private class LODMesh
        {
            public Mesh Mesh { get; private set; }
            public bool HasRequestedMesh { get; private set; }
            public bool HasMesh { get; private set; }

            private readonly int _lod;

            public LODMesh(int lod)
            {
                _lod = lod;
            }

            public void RequestMesh(MapData mapData, MapGenerator mapGenerator)
            {
                HasRequestedMesh = true;
                mapGenerator.RequestMeshData(mapData, _lod, OnMeshDataReceived);
            }

            private void OnMeshDataReceived(MeshData meshData)
            {
                Mesh = meshData.CreateMesh();
                HasMesh = true;
            }
        }

        [Serializable]
        private struct LODInfo
        {
            public int lod;
            public float visibleDistanceThreshold;
        }
    }
}