using UnityEngine;
using Random = System.Random;

namespace OctanGames.TerrainGeneration.Scripts
{
    public static class Noise
    {
        private const int OFFSET_RANGE = 100000;

        public static float[,] GenerateNoiseMap(
            int width, int height,
            int seed,
            float scale,
            int octaves,
            float persistance,
            float lacunarity,
            Vector2 offset)
        {
            var noiseMap = new float[width, height];

            var random = new Random(seed);
            var octavesOffsets = new Vector2[octaves];
            for (var i = 0; i < octaves; i++)
            {
                float offsetX = random.Next(-OFFSET_RANGE, OFFSET_RANGE) + offset.x;
                float offsetY = random.Next(-OFFSET_RANGE, OFFSET_RANGE) + offset.y;
                octavesOffsets[i] = new Vector2(offsetX, offsetY);
            }

            if (scale <= 0)
            {
                scale = 0.001f;
            }

            var maxNoiseHeight = float.MinValue;
            var minNoiseHeight = float.MaxValue;

            float halfWidth = width * 0.5f;
            float halfHeight = height * 0.5f;

            for (var y = 0; y < height; y++)
            {
                for (var x = 0; x < width; x++)
                {
                    var amplitude = 1f;
                    var frequency = 1f;
                    var noiseHeight = 0f;

                    for (var i = 0; i < octaves; i++)
                    {
                        float sampleX = (x - halfWidth) / scale * frequency + octavesOffsets[i].x;
                        float sampleY = (y - halfHeight) / scale * frequency + octavesOffsets[i].y;

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