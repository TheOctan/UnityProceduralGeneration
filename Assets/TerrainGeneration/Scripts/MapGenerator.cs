using JetBrains.Annotations;
using UnityEngine;

namespace OctanGames.TerrainGeneration.Scripts
{
    [RequireComponent(typeof(MapRenderer))]
    public class MapGenerator : MonoBehaviour
    {
        [SerializeField] private int _width = 100;
        [SerializeField] private int _height = 100;
        [SerializeField] private float _noiseScale = 27.6f;
        [SerializeField] private int _octaves = 4;
        [SerializeField] private float _persistance = 0.5f;
        [SerializeField] private float _lacunarity = 2f;

        [SerializeField, UsedImplicitly] private bool _autoGenerate = true;

        private MapRenderer _renderer;

        public void GenerateMap()
        {
            float[,] noiseMap =
                Noise.GenerateNoiseMap(_width, _height, _noiseScale, _octaves, _persistance, _lacunarity);

            _renderer = GetComponent<MapRenderer>();
            _renderer.DrawNoiseMap(noiseMap);
        }
    }
}