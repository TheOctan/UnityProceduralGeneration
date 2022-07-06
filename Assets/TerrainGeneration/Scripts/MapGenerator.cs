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
        [SerializeField, Range(0, 6)] private int _levelOfDetail;

        [Space] [SerializeField] private float _noiseScale = 25f;

        [Space] [SerializeField, Min(0)] private int _octaves = 5;
        [SerializeField, Range(0, 1)] private float _persistance = 0.5f;
        [SerializeField, Min(1)] private float _lacunarity = 2f;
        [SerializeField, Min(1)] private float _meshHeight = 15;
        [SerializeField] private AnimationCurve _meshHeightCurve;

        [Space] [SerializeField] private int _seed;
        [SerializeField] private Vector2 _offset;

        [Space] [SerializeField] private TerrainPreset _terrainPreset;

        private MapRenderer _renderer;

        public void DrawMapInEditor()
        {
            MapData mapData = GenerateMapData();
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
                        MeshGenerator.GenerateTerrainMesh(heightMap, _meshHeight, _meshHeightCurve, _levelOfDetail);
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

        private MapData GenerateMapData()
        {
            float[,] noiseMap =
                Noise.GenerateNoiseMap(MAP_CHUNK_SIZE, MAP_CHUNK_SIZE,
                    _seed, _noiseScale, _octaves, _persistance, _lacunarity, _offset);

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
    }
}