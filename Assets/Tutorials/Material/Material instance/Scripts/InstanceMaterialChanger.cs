using UnityEngine;

namespace OctanGames
{
    public class InstanceMaterialChanger : MonoBehaviour
    {
        [SerializeField] private KeyCode _keyCode;
        [SerializeField] private Renderer _renderer;
        [SerializeField] private Color _color;

        private Color _originalColor;
        private bool _flag;

        private void Start()
        {
            _originalColor = _renderer.material.color;
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
                    _renderer.material.color = _color;
                    break;
                case true:
                    _flag = false;
                    _renderer.material.color = _originalColor;
                    break;
            }
        }
    }
}