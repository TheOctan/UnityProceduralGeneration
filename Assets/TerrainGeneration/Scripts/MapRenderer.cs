using UnityEngine;

namespace OctanGames.TerrainGeneration.Scripts
{
    public class MapRenderer : MonoBehaviour
    {
        [SerializeField] private Renderer _renderer;

        public void DrawTexture(Texture2D texture)
        {
            _renderer.sharedMaterial.mainTexture = texture;
            _renderer.transform.localScale = new Vector3(texture.width, 1, texture.height);
        }
    }
}