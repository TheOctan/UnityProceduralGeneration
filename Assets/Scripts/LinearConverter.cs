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
            // XYZ => -1..1
            // RGB => 0..1
            return (value + 1) * 0.5f;
        }

        public static float ColorToCoordinate(float value)
        {
            // RGB => 0..1
            // XYZ => -1..1
            return value * 2 - 1;
        }
    }
}