using UnityEngine;

namespace OctanGames.TerrainGeneration.Scripts
{
    public class MapRenderer : MonoBehaviour
    {
        [SerializeField] private Renderer _renderer;

        public void DrawNoiseMap(float[,] noiseMap)
        {
            int width = noiseMap.GetLength(0);
            int height = noiseMap.GetLength(1);

            var texture = new Texture2D(width, height);
            var colorMap = new Color[width * height];

            for (var y = 0; y < height; y++)
            {
                for (var x = 0; x < width; x++)
                {
                    colorMap[y * width + x] = Color.Lerp(Color.black, Color.white, noiseMap[x, y]);
                }
            }

            texture.SetPixels(colorMap);
            texture.Apply();

            _renderer.sharedMaterial.mainTexture = texture;
            _renderer.transform.localScale = new Vector3(width, 1, height);
        }
    }
}