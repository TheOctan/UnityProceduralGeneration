using UnityEngine;

namespace OctanGames.TerrainGeneration.Scripts
{
    public static class FalloffGenerator
    {
        public static float[,] GenerateFalloffMap(int size)
        {
            var map = new float[size, size];
            for (var i = 0; i < size; i++)
            {
                for (var j = 0; j < size; j++)
                {
                    float x = (float)i / size * 2 - 1;
                    float y = (float)j / size * 2 - 1;

                    float value = Mathf.Max(Mathf.Abs(x), Mathf.Abs(y));
                    map[i, j] = value;
                }
            }

            return map;
        }
    }
}