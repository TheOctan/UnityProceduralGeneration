using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace OctanGames._2_Procedural_texture
{
    public enum TextureType
    {
        UV,
        NormalMap,
        Chess,
        WhiteNoise,
        PerlinNoise
    }

    public class TextureGenerator : MonoBehaviour
    {
        [SerializeField] private Texture2D _texture;
        [SerializeField, Range(2, 512)] private int _resolution = 128;
        [SerializeField] private FilterMode _filterMode = FilterMode.Bilinear;
        [SerializeField] private TextureWrapMode _wrapMode = TextureWrapMode.Clamp;
        [SerializeField] private TextureType _type;

        [Header("UV")] [SerializeField, Range(0, 1)]
        private float _height;

        [Header("Normal map")] [SerializeField, Min(0)]
        private float _radius = 0.5f;

        [SerializeField] private Vector2 _offset = new(0.5f, 0.5f);

        [Header("White noise")] [SerializeField]
        private int _seed;

        [Header("Perlin noise")] [SerializeField, Min(0)]
        private float _scale = 10;

        private Renderer _renderer;

        private void OnValidate()
        {
            RegenerateTexture();
        }

        private void Reset()
        {
            _renderer.sharedMaterial.mainTexture = null;
            RegenerateTexture();
        }

        private bool InitTexture()
        {
            if (_texture == null)
            {
                _texture = new Texture2D(_resolution, _resolution);
            }

            if (_renderer == null)
            {
                _renderer = GetComponent<Renderer>();
            }

            if (_renderer.sharedMaterial == null)
            {
                return false;
            }

            if (_renderer.sharedMaterial.mainTexture == null)
            {
                _renderer.sharedMaterial.mainTexture = _texture;
            }

            if (_texture.width != _resolution)
            {
                _texture.Reinitialize(_resolution, _resolution);
            }

            _texture.filterMode = _filterMode;
            _texture.wrapMode = _wrapMode;

            return true;
        }

        private void GenerateTexture()
        {
            switch (_type)
            {
                case TextureType.UV:
                    DrawUVTexture();
                    break;
                case TextureType.NormalMap:
                    DrawNormalMap();
                    break;
                case TextureType.Chess:
                    DrawChessTexture();
                    break;
                case TextureType.WhiteNoise:
                    DrawWhiteNoise();
                    break;
                case TextureType.PerlinNoise:
                    DrawPerlinNoise();
                    break;
                default:
                    Debug.LogError("Undefined type of texture");
                    break;
            }

            _texture.Apply();
        }

        private void RegenerateTexture()
        {
            if (!InitTexture())
            {
                return;
            }

            GenerateTexture();
        }

        private void DrawUVTexture()
        {
            float step = 1f / _resolution;
            TakeTextureSample((x, y) =>
            {
                _texture.SetPixel(x, y, new Color((x + 0.5f) * step, (y + 0.5f) * step, _height));
            });
        }

        private void DrawNormalMap()
        {
            float step = 1f / _resolution;
            float r2 = Mathf.Pow(_radius, 2);

            for (var y = 0; y < _resolution; y++)
            {
                float y1 = (y + 0.5f) * step - _offset.y;
                float y2 = Mathf.Pow(y1, 2);

                for (var x = 0; x < _resolution; x++)
                {
                    float x1 = (x + 0.5f) * step - _offset.x;
                    float x2 = Mathf.Pow(x1, 2);

                    if (x2 + y2 <= r2)
                    {
                        float z1 = Mathf.Sqrt(r2 - x2 - y2);
                        Vector3 normal = new Vector3(x1, y1, z1).normalized;

                        _texture.SetPixel(x, y, normal.ToColor());
                    }
                    else
                    {
                        _texture.SetPixel(x, y, Vector3.forward.ToColor());
                    }
                }
            }
        }

        private void DrawChessTexture()
        {
            TakeTextureSample((x, y) =>
            {
                bool isEvenPosition = x % 2 == 0 && y % 2 == 0;
                bool isOddPosition = x % 2 != 0 && y % 2 != 0;

                _texture.SetPixel(x, y, (isEvenPosition || isOddPosition) ? Color.black : Color.white);
            });
        }

        private void DrawWhiteNoise()
        {
            Random.InitState(_seed);

            TakeTextureSample((x, y) =>
            {
                float randomValue = Random.value;
                _texture.SetPixel(x, y, new Color(randomValue, randomValue, randomValue));
            });
        }

        private void DrawPerlinNoise()
        {
            TakeTextureSample((x, y) =>
            {
                _texture.SetPixel(x, y, CalculatePerlinColor(x, y));
            });
        }

        private Color CalculatePerlinColor(int x, int y)
        {
            float xCoord = (x + 0.5f) / _resolution * _scale + _offset.x;
            float yCoord = (y + 0.5f) / _resolution * _scale + _offset.y;

            float sample = Mathf.PerlinNoise(xCoord, yCoord);

            return new Color(sample, sample,sample);
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