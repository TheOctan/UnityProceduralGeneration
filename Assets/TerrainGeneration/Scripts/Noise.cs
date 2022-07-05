using UnityEngine;

namespace OctanGames.TerrainGeneration.Scripts
{
    public static class Noise
    {
        public static float[,] GenerateNoiseMap(
            int width, int height,
            float scale,
            int octaves,
            float persistance,
            float lacunarity)
        {
            var noiseMap = new float[width, height];

            if (scale <= 0)
            {
                scale = 0.001f;
            }

            var maxNoiseHeight = float.MinValue;
            var minNoiseHeight = float.MaxValue;

            for (var y = 0; y < height; y++)
            {
                for (var x = 0; x < width; x++)
                {
                    var amplitude = 1f;
                    var frequency = 1f;
                    var noiseHeight = 0f;

                    for (var i = 0; i < octaves; i++)
                    {
                        float sampleX = x / scale * frequency;
                        float sampleY = y / scale * frequency;

                        float perlinValue = LinearConverter.ColorToCoordinate(Mathf.PerlinNoise(sampleX, sampleY));
                        noiseHeight += perlinValue * amplitude;

                        amplitude *= persistance;
                        frequency *= lacunarity;
                    }

                    if (noiseHeight > maxNoiseHeight)
                    {
                        maxNoiseHeight = noiseHeight;
                    }
                    else if (noiseHeight < minNoiseHeight)
                    {
                        minNoiseHeight = noiseHeight;
                    }

                    noiseMap[x, y] = noiseHeight;
                }
            }

            for (var y = 0; y < height; y++)
            {
                for (var x = 0; x < width; x++)
                {
                    noiseMap[x, y] = Mathf.InverseLerp(minNoiseHeight, maxNoiseHeight, noiseMap[x, y]);
                }
            }

            return noiseMap;
        }
    }
}