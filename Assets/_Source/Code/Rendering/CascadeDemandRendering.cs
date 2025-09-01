using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

namespace SurgeEngine._Source.Code.Rendering
{
    public class CascadeDemandRendering : MonoBehaviour
    {
        [SerializeField] private HDAdditionalLightData lightData;
 
        private const int Cascade_0 = 1;
        private const int Cascade_1 = 2;
        private const int Cascade_2 = 4;
        private const int Cascade_3 = 5;
 
        private int _frameCounter = 1;
        
        private void Update()
        {
            if (lightData.shadowUpdateMode != ShadowUpdateMode.OnDemand) return;
            
            switch (_frameCounter)
            {
                case 0:
                    lightData.RequestSubShadowMapRendering(0);
                    break;
                    
                case 2:
                    lightData.RequestSubShadowMapRendering(1);
                    break;
                    
                case 4:
                    lightData.RequestSubShadowMapRendering(2);
                    break;
                    
                case 6:
                    lightData.RequestSubShadowMapRendering(3);
                    break;
            }
 
            _frameCounter++;
 
            if (_frameCounter > Cascade_3)
                _frameCounter = 0;
        }
    }
}