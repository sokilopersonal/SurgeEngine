using System;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

namespace SurgeEngine.Source.Code.Rendering
{
    [AddComponentMenu("Lighting/Cascade Demand Rendering")]
    public class CascadeDemandRendering : MonoBehaviour
    {
        private HDAdditionalLightData _lightData;
        private int _frameCounter = 1;
        
        private void Awake()
        {
            if (enabled && !_lightData) _lightData = GetComponent<HDAdditionalLightData>();
        }

        private void OnEnable()
        {
            _lightData.shadowUpdateMode = ShadowUpdateMode.OnDemand;
            _lightData.alwaysDrawDynamicShadows = true;
        }

        private void OnDisable()
        {
            _lightData.shadowUpdateMode = ShadowUpdateMode.EveryFrame;
            _lightData.alwaysDrawDynamicShadows = false;
        }

        private void Update()
        {
            if (!_lightData) return;
            if (_lightData.shadowUpdateMode != ShadowUpdateMode.OnDemand) return;
            
            switch (_frameCounter)
            {
                case 0:
                    _lightData.RequestSubShadowMapRendering(0);
                    break;
                case 1:
                    _lightData.RequestSubShadowMapRendering(1);
                    break;
                case 2:
                    _lightData.RequestSubShadowMapRendering(2);
                    break;
                case 3:
                    _lightData.RequestSubShadowMapRendering(3);
                    break;
            }
            
            _frameCounter++;
            
            if (_frameCounter > 3)
                _frameCounter = 0;
        }
    }
}