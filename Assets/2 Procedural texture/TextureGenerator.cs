using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum TextureType
{
    Chess,
    UV
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
        }

        _texture.Apply();
    }

    private void DrawChessTexture()
    {
        float step = 1f / _resolution;

        for (var y = 0; y < _resolution; y++)
        {
            for (var x = 0; x < _resolution; x++)
            {
                if (x % 2 == 0 && y % 2 == 0 || x % 2 != 0 && y % 2 != 0)
                {
                    _texture.SetPixel(x, y, Color.black);
                }
                else
                {
                    _texture.SetPixel(x, y, Color.white);
                }
            }
        }
    }

    private void DrawUVTexture()
    {
        float step = 1f / _resolution;
        for (var y = 0; y < _resolution; y++)
        {
            for (var x = 0; x < _resolution; x++)
            {
                _texture.SetPixel(x, y, new Color((x + 0.5f)*step, (y + 0.5f)*step, 0f));
            }
        }
    }
}