using UnityEngine;

namespace Octan.TerrainGeneration.Scripts
{
    public static class Noise
    {
        public static float[,] GenerateNoiseMap(int width, int height, float scale)
        {
            var noiseMap = new float[width, height];

            if (scale <= 0)
            {
                scale = 0.001f;
            }

            for (var y = 0; y < height; y++)
            {
                for (var x = 0; x < width; x++)
                {
                    var sampleX = x / scale;
                    var sampleY = y / scale;

                    var perlinValue = Mathf.PerlinNoise(sampleX, sampleY);
                    noiseMap[x, y] = perlinValue;
                }
            }

            return noiseMap;
        }
    }
}