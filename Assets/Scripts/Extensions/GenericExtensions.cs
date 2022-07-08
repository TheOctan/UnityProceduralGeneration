using System;
using System.Collections.Generic;

namespace OctanGames.Extensions
{
    public static class GenericExtensions
    {
        public static IList<T> Shuffle<T>(this IList<T> list)
        {
            return list.Shuffle(DateTime.Now.Second);
        }

        public static IList<T> Shuffle<T>(this IList<T> list, int seed)
        {
            var random = new Random(seed);
            for (var i = 0; i < list.Count - 1; i++)
            {
                int randomIndex = random.Next(i, list.Count);

                (list[randomIndex], list[i]) = (list[i], list[randomIndex]);
            }

            return list;
        }

        /// <summary>
        /// Returns min and max values from map
        /// </summary>
        /// <param name="map"></param>
        /// <returns></returns>
        public static (float min, float max) MinMax(this float[,] map)
        {
            int width = map.GetLength(0);
            int height = map.GetLength(1);

            var max = float.MinValue;
            var min = float.MaxValue;

            for (var y = 0; y < height; y++)
            {
                for (var x = 0; x < width; x++)
                {
                    if (map[x, y] > max)
                    {
                        max = map[x, y];
                    }
                    if (map[x, y] < min)
                    {
                        min = map[x, y];
                    }
                }
            }
            
            return (min, max);
        }
    }
}