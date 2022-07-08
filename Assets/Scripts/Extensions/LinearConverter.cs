using UnityEngine;

namespace OctanGames
{
    public static class LinearConverter
    {
        public static Color ToColor(this Vector3 vector)
        {
            float r = CoordinateToColor(vector.x);
            float g = CoordinateToColor(vector.y);
            float b = CoordinateToColor(vector.z);

            return new Color(r, g, b);
        }

        public static Vector3 ToVector(this Color color)
        {
            float x = ColorToCoordinate(color.r);
            float y = ColorToCoordinate(color.g);
            float z = ColorToCoordinate(color.b);

            return new Vector3(x, y, z);
        }

        public static float CoordinateToColor(float value)
        {
            // XYZ=[-1..1] => RGB=[0..1]
            // return Mathf.InverseLerp(-1, 1, value);
            return (value + 1) * 0.5f;
        }

        public static float ColorToCoordinate(float value)
        {
            // RGB=[0..1] => XYZ=[-1..1]
            // return Mathf.Lerp(-1, 1, value);
            return value * 2 - 1;
        }

        /// <summary>
        /// Convert all values of map to color
        /// </summary>
        /// <param name="map"></param>
        public static void ConvertToColor(this float[,] map)
        {
            int width = map.GetLength(0);
            int height = map.GetLength(1);

            for (var y = 0; y < height; y++)
            {
                for (var x = 0; x < width; x++)
                {
                    map[x, y] = CoordinateToColor(map[x, y]);
                }
            }
        }

        /// <summary>
        /// Copy map and convert all values of map to color
        /// </summary>
        /// <param name="map"></param>
        public static float[,] GetConvertedToColor(this float[,] map)
        {
            int width = map.GetLength(0);
            int height = map.GetLength(1);

            var newMap = new float[width, height];

            for (var y = 0; y < height; y++)
            {
                for (var x = 0; x < width; x++)
                {
                    newMap[x, y] = CoordinateToColor(map[x, y]);
                }
            }

            return newMap;
        }

        /// <summary>
        /// Normalize all values between min and max
        /// </summary>
        /// <param name="map"></param>
        /// <param name="minValue"></param>
        /// <param name="maxValue"></param>
        public static void Normalize(this float[,] map, float minValue, float maxValue)
        {
            int width = map.GetLength(0);
            int height = map.GetLength(1);
            
            for (var y = 0; y < height; y++)
            {
                for (var x = 0; x < width; x++)
                {
                    map[x, y] = Mathf.InverseLerp(minValue, maxValue, map[x, y]);
                }
            }
        }

        /// <summary>
        /// Copy map and normalize all values between min and max
        /// </summary>
        /// <param name="map"></param>
        /// <param name="minValue"></param>
        /// <param name="maxValue"></param>
        /// <returns></returns>
        public static float[,] GetNormalized(this float[,] map, float minValue, float maxValue)
        {
            int width = map.GetLength(0);
            int height = map.GetLength(1);

            var newMap = new float[width, height];

            for (var y = 0; y < height; y++)
            {
                for (var x = 0; x < width; x++)
                {
                    newMap[x, y] = Mathf.InverseLerp(minValue, maxValue, map[x, y]);
                }
            }

            return newMap;
        }
    }
}