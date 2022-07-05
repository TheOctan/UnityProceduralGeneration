using System;
using JetBrains.Annotations;
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

        [SerializeField] private DrawMode _drawMode;
        [Space]
        [SerializeField, Min(1)] private int _width = 100;
        [SerializeField, Min(1)] private int _height = 100;
        [SerializeField] private float _noiseScale = 25f;
        [Space]
        [SerializeField, Min(0)] private int _octaves = 5;
        [SerializeField, Range(0, 1)] private float _persistance = 0.5f;
        [SerializeField, Min(1)] private float _lacunarity = 2f;
        [Space]
        [SerializeField] private int _seed;
        [SerializeField] private Vector2 _offset;
        [Space]
        [SerializeField] private TerrainPreset _terrainPreset;
        [Space]
        [SerializeField, UsedImplicitly]
        private bool _autoGenerate = true;

        private MapRenderer _renderer;

        public void GenerateMap()
        {
            float[,] noiseMap =
                Noise.GenerateNoiseMap(_width, _height, _seed, _noiseScale, _octaves, _persistance, _lacunarity,
                    _offset);

            var colorMap = new Color[_width * _height];
            if (!ReferenceEquals(_terrainPreset, null))
            {
                for (var y = 0; y < _height; y++)
                {
                    for (var x = 0; x < _width; x++)
                    {
                        float currentHeight = noiseMap[x, y];

                        foreach (TerrainType region in _terrainPreset.Regions)
                        {
                            if (currentHeight <= region.height)
                            {
                                colorMap[y * _width + x] = region.color;
                                break;
                            }
                        }
                    }
                }
            }

            _renderer = GetComponent<MapRenderer>();

            switch (_drawMode)
            {
                case DrawMode.NoiseMap:
                {
                    Texture2D texture = TextureGenerator.TextureFromHeightMap(noiseMap);
                    _renderer.DrawTexture(texture);
                    break;
                }
                case DrawMode.ColorMap:
                {
                    Texture2D texture = TextureGenerator.TextureFromColorMap(colorMap, _width, _height);
                    _renderer.DrawTexture(texture);
                    break;
                }
                case DrawMode.Mesh:
                {
                    MeshData mesh = MeshGenerator.GenerateTerrainMesh(noiseMap);
                    Texture2D texture = TextureGenerator.TextureFromColorMap(colorMap, _width, _height);
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

        private void Reset()
        {
            GenerateMap();
        }
    }
}