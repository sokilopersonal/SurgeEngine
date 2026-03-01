using SurgeEngine.Source.Code.Infrastructure.Tools.Managers;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;
using Zenject;

namespace SurgeEngine.Source.Code.Gameplay.CommonObjects.Lighting
{
    /// <summary>
    /// Component required to define a light in the graphics settings. This will be added to UserGraphics lights list to control the shadows.
    /// </summary>
    public class LightDefiner : MonoBehaviour
    {
        [Inject] private UserGraphics _graphics;

        public Light Component { get; private set; }
        public HDAdditionalLightData Data { get; private set; }

        private void Awake()
        {
            Component = GetComponent<Light>();
            Data = GetComponent<HDAdditionalLightData>();
            
            _graphics?.AddLight(this);
        }

        private void OnDestroy()
        {
            _graphics?.RemoveLight(this);
        }
    }
}