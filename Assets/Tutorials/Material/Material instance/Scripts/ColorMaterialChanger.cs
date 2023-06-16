using UnityEngine;

namespace OctanGames
{
    public class ColorMaterialChanger : MonoBehaviour
    {
        [SerializeField] private KeyCode _keyCode;
        [SerializeField] private Material _material;
        [SerializeField] private Color _color;

        private Color _originalColor;
        private bool _flag;

        private void Start()
        {
            _originalColor = _material.color;
        }

        private void Update()
        {
            if (Input.GetKeyDown(_keyCode))
            {
                SwitchColor();
            }
        }

        private void SwitchColor()
        {
            switch (_flag)
            {
                case false:
                    _flag = true;
                    _material.color = _color;
                    break;
                case true:
                    _flag = false;
                    _material.color = _originalColor;
                    break;
            }
        }
    }
}
