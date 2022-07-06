using UnityEngine;

namespace OctanGames.TerrainGeneration.Scripts.Data
{
    public struct MapData
    {
        public float[,] HeightMap { get; }
        public Color[] ColorMap { get; }

        public MapData(float[,] heightMap, Color[] colorMap)
        {
            HeightMap = heightMap;
            ColorMap = colorMap;
        }
    }
}