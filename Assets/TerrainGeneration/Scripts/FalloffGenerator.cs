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
                    map[i, j] = Evaluate(value);
                }
            }

            return map;
        }

        private static float Evaluate(float value)
        {
            const float a = 3f;
            const float b = 2.2f;

            return Mathf.Pow(value, a) / (Mathf.Pow(value, a) + Mathf.Pow(b - b * value, a));
        }
    }
}