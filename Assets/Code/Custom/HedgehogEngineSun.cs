using SurgeEngine.Code.ActorSystem;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

namespace SurgeEngine.Code.Custom
{
    public class HedgehogEngineSun : MonoBehaviour
    {
        [SerializeField] private int minResolution = 128;
        [SerializeField] private int maxResolution = 128;
        
        private HDAdditionalLightData _light;
        
        private Camera _camera => ActorContext.Context.camera.GetCamera();

        private void Awake()
        {
            _light = GetComponent<HDAdditionalLightData>();
        }

        private void Update()
        {
            float dot = Vector3.Dot(_camera.transform.forward, transform.forward);
            dot = Mathf.Abs(dot);
            _light.SetShadowResolution(Mathf.RoundToInt(Mathf.Lerp(maxResolution, minResolution, dot)));
        }
    }
}