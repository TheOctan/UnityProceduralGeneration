using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _2_Procedural_texture
{
    public enum TextureType
    {
        Chess,
        UV,
        WhiteNoise
    }

    public class TextureGenerator : MonoBehaviour
    {
        [SerializeField] private Texture2D _texture;
        [SerializeField, Range(2, 512)] private int _resolution = 128;
        [SerializeField] private FilterMode _filterMode;
        [SerializeField] private TextureWrapMode _wrapMode;
        [SerializeField] private TextureType _type;

        private void OnValidate()
        {
            if (_texture == null)
            {
                _texture = new Texture2D(_resolution, _resolution);
                GetComponent<Renderer>().sharedMaterial.mainTexture = _texture;
            }

            if (_texture.width != _resolution)
            {
                _texture.Reinitialize(_resolution, _resolution);
            }

            _texture.filterMode = _filterMode;
            _texture.wrapMode = _wrapMode;

            switch (_type)
            {
                case TextureType.Chess:
                    DrawChessTexture();
                    break;
                case TextureType.UV:
                    DrawUVTexture();
                    break;
                case TextureType.WhiteNoise:
                    DrawWhiteNoise();
                    break;
                default:
                    Debug.LogError("Undefined type");
                    break;
            }

            _texture.Apply();
        }

        private void DrawChessTexture()
        {
            TakeTextureSample((x, y) =>
            {
                if (x % 2 == 0 && y % 2 == 0 || x % 2 != 0 && y % 2 != 0)
                {
                    _texture.SetPixel(x, y, Color.black);
                }
                else
                {
                    _texture.SetPixel(x, y, Color.white);
                }
            });
        }

        private void DrawUVTexture()
        {
            float step = 1f / _resolution;
            TakeTextureSample((x, y) =>
            {
                _texture.SetPixel(x, y, new Color((x + 0.5f) * step, (y + 0.5f) * step, 0f));
            });
        }

        private void DrawWhiteNoise()
        {
            Random.InitState(0);

            TakeTextureSample((x, y) =>
            {
                float randomValue = Random.value;                
                _texture.SetPixel(x,y, new Color(randomValue, randomValue, randomValue));
            });
        }

        private void TakeTextureSample(Action<int, int> sample)
        {
            for (var y = 0; y < _resolution; y++)
            {
                for (var x = 0; x < _resolution; x++)
                {
                    sample?.Invoke(x, y);
                }
            }
        }
    }
}