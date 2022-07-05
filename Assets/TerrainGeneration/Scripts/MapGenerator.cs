using System;
using JetBrains.Annotations;
using UnityEngine;

namespace OctanGames.TerrainGeneration.Scripts
{
    [RequireComponent(typeof(MapRenderer))]
    public class MapGenerator : MonoBehaviour
    {
        [SerializeField, Min(1)] private int _width = 100;
        [SerializeField, Min(1)] private int _height = 100;
        [SerializeField] private float _noiseScale = 27.6f;
        [Space]
        [SerializeField, Min(0)] private int _octaves = 4;
        [SerializeField, Range(0, 1)] private float _persistance = 0.5f;
        [SerializeField, Min(1)] private float _lacunarity = 2f;
        [Space]
        [SerializeField] private int _seed;
        [SerializeField] private Vector2 _offset;
        [Space]
        [SerializeField, UsedImplicitly]
        private bool _autoGenerate = true;

        private MapRenderer _renderer;

        public void GenerateMap()
        {
            float[,] noiseMap =
                Noise.GenerateNoiseMap(_width, _height, _seed, _noiseScale, _octaves, _persistance, _lacunarity,
                    _offset);

            _renderer = GetComponent<MapRenderer>();
            _renderer.DrawNoiseMap(noiseMap);
        }

        private void Reset()
        {
            GenerateMap();
        }
    }
}