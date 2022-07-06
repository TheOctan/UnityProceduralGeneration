using System;
using OctanGames.TerrainGeneration.Scripts.Data;
using UnityEngine;

namespace OctanGames.TerrainGeneration.Scripts
{
    public class TerrainChunk
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

        private Vector2 _viewerPosition;
        private float _maxViewDistance;

        public bool IsVisible => _meshObject.activeSelf;

        public TerrainChunk(Vector2 coord, int size,
            LODInfo[] detailLevels, Transform parent,
            Material material, MapGenerator mapGenerator,
            Vector2 viewerPosition, float maxViewDistance)
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

            _viewerPosition = viewerPosition;
            _maxViewDistance = maxViewDistance;

            _lodMeshes = new LODMesh[_detailLevels.Length];
            for (var i = 0; i < _detailLevels.Length; i++)
            {
                int lod = _detailLevels[i].lod;
                _lodMeshes[i] = new LODMesh(lod, Update);
            }

            _mapGenerator = mapGenerator;
            _mapGenerator.RequestMapData(position, OnMapDataReceived);
        }

        public void Update()
        {
            if (!_mapDataReceived)
            {
                return;
            }

            float viewerSqrDistanceFromNearestEdge = _bounds.SqrDistance(EndlessTerrain.ViewerPosition);
            bool visible = viewerSqrDistanceFromNearestEdge <=
                           EndlessTerrain.MaxViewDistance * EndlessTerrain.MaxViewDistance;

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

            Texture2D texture = TextureGenerator.TextureFromColorMap(mapData.ColorMap, 
                MapGenerator.MAP_CHUNK_SIZE,
                MapGenerator.MAP_CHUNK_SIZE);

            _meshRenderer.material.mainTexture = texture;

            Update();
        }
    }

    public class LODMesh
    {
        public Mesh Mesh { get; private set; }
        public bool HasRequestedMesh { get; private set; }
        public bool HasMesh { get; private set; }

        private Action _updateCallback;
        private readonly int _lod;

        public LODMesh(int lod, Action updateCallback)
        {
            _updateCallback = updateCallback;
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

            _updateCallback?.Invoke();
        }
    }

    [Serializable]
    public struct LODInfo
    {
        public int lod;
        public float visibleDistanceThreshold;
    }
}