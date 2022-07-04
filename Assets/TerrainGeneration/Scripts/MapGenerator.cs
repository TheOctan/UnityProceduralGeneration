using UnityEngine;

namespace Octan.TerrainGeneration.Scripts
{
    [RequireComponent(typeof(MapRenderer))]
    public class MapGenerator : MonoBehaviour
    {
        [SerializeField] private int _width = 10;
        [SerializeField] private int _height = 10;
        [SerializeField] private float _noiseScale = 0.3f;

        [SerializeField] private bool _autoGenerate;

        private MapRenderer _renderer;

        public void GenerateMap()
        {
            float[,] noiseMap = Noise.GenerateNoiseMap(_width, _height, _noiseScale);

            _renderer = GetComponent<MapRenderer>();
            _renderer.DrawNoiseMap(noiseMap);
        }
    }
}