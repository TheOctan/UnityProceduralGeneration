using System;
using System.Collections.Generic;
using System.Threading;
using OctanGames.TerrainGeneration.Scripts.Data;
using OctanGames.TerrainGeneration.Scripts.Preset;
using UnityEngine;

namespace OctanGames.TerrainGeneration.Scripts
{
    [RequireComponent(typeof(MapRenderer))]
    public class MapGenerator : MonoBehaviour
    {
        private enum DrawMode
        {
            NoiseMap,
            ColorMap,
            Mesh
        }

        public const int MAP_CHUNK_SIZE = 241;

        [SerializeField] private DrawMode _drawMode;
        [SerializeField, Range(0, 6)] private int _previewLOD;
        [Space] [SerializeField] private float _noiseScale = 25f;
        [Space] [SerializeField, Min(0)] private int _octaves = 5;
        [SerializeField, Range(0, 1)] private float _persistance = 0.5f;
        [SerializeField, Min(1)] private float _lacunarity = 2f;
        [SerializeField, Min(1)] private float _meshHeight = 15;
        [SerializeField] private AnimationCurve _meshHeightCurve;
        [Space] [SerializeField] private int _seed;
        [SerializeField] private Vector2 _offset;
        [Space] [SerializeField] private TerrainPreset _terrainPreset;

        private readonly Queue<MapThreadInfo<MapData>> _mapDataThreadInfoQueue = new();
        private readonly Queue<MapThreadInfo<MeshData>> _meshDataThreadInfoQueue = new();
        private MapRenderer _renderer;

        private void Update()
        {
            lock (_mapDataThreadInfoQueue)
            {
                while(_mapDataThreadInfoQueue.Count > 0)
                {
                    MapThreadInfo<MapData> threadInfo = _mapDataThreadInfoQueue.Dequeue();
                    threadInfo.Callback?.Invoke(threadInfo.Parameter);
                }
            }

            lock (_meshDataThreadInfoQueue)
            {
                while (_meshDataThreadInfoQueue.Count > 0)
                {
                    MapThreadInfo<MeshData> threadInfo = _meshDataThreadInfoQueue.Dequeue();
                    threadInfo.Callback?.Invoke(threadInfo.Parameter);
                }
            }
        }

        public void DrawMapInEditor()
        {
            MapData mapData = GenerateMapData(Vector2.zero);
            float[,] heightMap = mapData.HeightMap;
            Color[] colorMap = mapData.ColorMap;

            _renderer = GetComponent<MapRenderer>();

            switch (_drawMode)
            {
                case DrawMode.NoiseMap:
                {
                    Texture2D texture = TextureGenerator.TextureFromHeightMap(heightMap);
                    _renderer.DrawTexture(texture);
                    break;
                }
                case DrawMode.ColorMap:
                {
                    Texture2D texture =
                        TextureGenerator.TextureFromColorMap(colorMap, MAP_CHUNK_SIZE, MAP_CHUNK_SIZE);
                    _renderer.DrawTexture(texture);
                    break;
                }
                case DrawMode.Mesh:
                {
                    MeshData mesh =
                        MeshGenerator.GenerateTerrainMesh(heightMap, _meshHeight, _meshHeightCurve, _previewLOD);
                    Texture2D texture =
                        TextureGenerator.TextureFromColorMap(colorMap, MAP_CHUNK_SIZE, MAP_CHUNK_SIZE);
                    _renderer.DrawMesh(mesh, texture);
                    break;
                }
                default:
                {
                    Debug.LogError("Undefined type of draw mode");
                    break;
                }
            }
        }

        public void RequestMapData(Vector2 centre, Action<MapData> callback)
        {
            var thread = new Thread(() =>
            {
                MapData mapData = GenerateMapData(centre);
                lock (_mapDataThreadInfoQueue)
                {
                    _mapDataThreadInfoQueue.Enqueue(new MapThreadInfo<MapData>(callback, mapData));
                }
            });
            thread.Start();
        }

        public void RequestMeshData(MapData mapData, int lod, Action<MeshData> callback)
        {
            var thread = new Thread(() =>
            {
                AnimationCurve heightCurve;
                lock (_meshHeightCurve)
                {
                    heightCurve = new AnimationCurve(_meshHeightCurve.keys);
                }
                MeshData meshData =
                    MeshGenerator.GenerateTerrainMesh(mapData.HeightMap, _meshHeight, heightCurve, lod);
                lock (_meshDataThreadInfoQueue)
                {
                    _meshDataThreadInfoQueue.Enqueue(new MapThreadInfo<MeshData>(callback, meshData));
                }
            });
            thread.Start();
        }

        private MapData GenerateMapData(Vector2 centre)
        {
            float[,] noiseMap =
                Noise.GenerateNoiseMap(MAP_CHUNK_SIZE, MAP_CHUNK_SIZE,
                    _seed, _noiseScale, _octaves, _persistance, _lacunarity, centre + _offset);

            var colorMap = new Color[MAP_CHUNK_SIZE * MAP_CHUNK_SIZE];
            if (!ReferenceEquals(_terrainPreset, null))
            {
                for (var y = 0; y < MAP_CHUNK_SIZE; y++)
                {
                    for (var x = 0; x < MAP_CHUNK_SIZE; x++)
                    {
                        float currentHeight = noiseMap[x, y];

                        foreach (TerrainType region in _terrainPreset.Regions)
                        {
                            if (currentHeight <= region.height)
                            {
                                colorMap[y * MAP_CHUNK_SIZE + x] = region.color;
                                break;
                            }
                        }
                    }
                }
            }

            return new MapData(noiseMap, colorMap);
        }

        private void Reset()
        {
            DrawMapInEditor();
        }

        private struct MapThreadInfo<T>
        {
            public Action<T> Callback { get; }
            public T Parameter { get; }

            public MapThreadInfo(Action<T> callback, T parameter)
            {
                Callback = callback;
                Parameter = parameter;
            }
        }
    }
}