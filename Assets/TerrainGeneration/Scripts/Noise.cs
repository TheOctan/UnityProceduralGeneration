using System.Collections.Generic;
using System.Linq;
using OctanGames.Extensions;
using OctanGames.TerrainGeneration.Scripts.Data;
using UnityEngine;
using Random = System.Random;

namespace OctanGames.TerrainGeneration.Scripts
{
    public static class Noise
    {
        public enum NormaliseMode
        {
            Local,
            Global
        }

        private const int OFFSET_RANGE = 100000;

        public static float[,] GenerateNoiseMap(
            int width, int height,
            int seed, float scale,
            int octaves,
            float persistance,
            float lacunarity,
            Vector2 offset,
            NormaliseMode normaliseMode)
        {
            var noiseMap = new float[width, height];

            var random = new Random(seed);
            var octavesOffsets = new Vector2[octaves];

            float maxPossibleNormalizedHeight = 0;
            var amplitude = 1f;

            for (var i = 0; i < octaves; i++)
            {
                float offsetX = random.Next(-OFFSET_RANGE, OFFSET_RANGE) + offset.x;
                float offsetY = random.Next(-OFFSET_RANGE, OFFSET_RANGE) - offset.y;
                octavesOffsets[i] = new Vector2(offsetX, offsetY);

                maxPossibleNormalizedHeight += amplitude;
                amplitude *= persistance;
            }

            if (scale <= 0)
            {
                scale = 0.001f;
            }

            var maxLocalNoiseHeight = float.MinValue;
            var minLocalNoiseHeight = float.MaxValue;

            float halfWidth = width * 0.5f;
            float halfHeight = height * 0.5f;

            for (var y = 0; y < height; y++)
            {
                for (var x = 0; x < width; x++)
                {
                    amplitude = 1f;
                    var frequency = 1f;
                    var noiseHeight = 0f;

                    for (var i = 0; i < octaves; i++)
                    {
                        float sampleX = (x - halfWidth + octavesOffsets[i].x) / scale * frequency;
                        float sampleY = (y - halfHeight + octavesOffsets[i].y) / scale * frequency;

                        float perlinValue = LinearConverter.ColorToCoordinate(Mathf.PerlinNoise(sampleX, sampleY));
                        noiseHeight += perlinValue * amplitude;

                        amplitude *= persistance;
                        frequency *= lacunarity;
                    }

                    if (noiseHeight > maxLocalNoiseHeight)
                    {
                        maxLocalNoiseHeight = noiseHeight;
                    }
                    if (noiseHeight < minLocalNoiseHeight)
                    {
                        minLocalNoiseHeight = noiseHeight;
                    }

                    noiseMap[x, y] = noiseHeight;
                }
            }

            switch (normaliseMode)
            {
                case NormaliseMode.Local:
                    noiseMap.Normalize(minLocalNoiseHeight, maxLocalNoiseHeight);
                    break;
                case NormaliseMode.Global:
                    for (var y = 0; y < height; y++)
                    {
                        for (var x = 0; x < width; x++)
                        {
                            float normalizedHeight =
                                LinearConverter.CoordinateToColor(noiseMap[x, y]) / maxPossibleNormalizedHeight * 2f;
                            noiseMap[x, y] = Mathf.Clamp(normalizedHeight, 0, int.MaxValue);
                        }
                    }
                    break;
                default:
                    Debug.Log("Undefined type of noise normalization");
                    break;
            }

            return noiseMap;
        }
    }
}