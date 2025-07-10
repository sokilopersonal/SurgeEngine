using UnityEngine;

namespace SurgeEngine.Code.Gameplay.CommonObjects.Mobility
{
    public class JumpSelectorView : MonoBehaviour
    {
        [SerializeField] private Renderer _renderer;
        [SerializeField] private JumpSelector _jumpSelector;
        
        private const string _aB = "_BUTTON_A";
        private const string _xB = "_BUTTON_X";
        private const string _bB = "_BUTTON_B";
        private const string _uB = "_BUTTON_U";

        private void Awake()
        {
            if (_renderer != null && _jumpSelector != null)
            {
                Material mat = _renderer.materials[4];
                int button = (int)_jumpSelector.Button;
                mat.DisableKeyword(_aB);
                mat.DisableKeyword(_xB);
                mat.DisableKeyword(_bB);
                mat.DisableKeyword(_uB);
                switch (button)
                {
                    case 0:
                        mat.EnableKeyword(_aB);
                        break;
                    case 1:
                        mat.EnableKeyword(_xB);
                        break;
                    case 2:
                        mat.EnableKeyword(_bB);
                        break;
                    case 3:
                        mat.EnableKeyword(_uB);
                        break;
                }
            }
        }
    }
}