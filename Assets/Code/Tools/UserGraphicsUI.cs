using System;
using SurgeEngine.Code.UI.Menus.OptionElements;
using UnityEngine;
using Zenject;

namespace SurgeEngine.Code.Tools
{
    public class UserGraphicsUI : MonoBehaviour
    {
        [SerializeField] private OptionBar textureQualityBar;
        [SerializeField] private OptionBar sunShadowsQualityBar;
        [SerializeField] private OptionBar punctualShadowsQualityBar;
        [SerializeField] private OptionBar contactShadowsQualityBar;
        [SerializeField] private OptionBar pcssBar;
        [SerializeField] private OptionBar bloomBar;
        [SerializeField] private OptionBar aoBar;
        [SerializeField] private OptionBar motionBlurBar;
        [SerializeField] private OptionBar refractionQualityBar;
        [SerializeField] private OptionBar ssrQualityBar;
        [SerializeField] private OptionBar antiAliasingQualityBar;
        [SerializeField] private OptionBar subSurfaceScatteringQualityBar;
        
        [Inject] private UserGraphics _graphics;

        private void Awake()
        {
            var data = _graphics.GetGraphicsData();
            
            textureQualityBar.OnIndexChanged += index => _graphics.SetTextureQuality((TextureQuality)index);
            sunShadowsQualityBar.OnIndexChanged += index => _graphics.SetSunShadowsQuality((ShadowsQuality)index);
            punctualShadowsQualityBar.OnIndexChanged += index => _graphics.SetAdditionalShadowsQuality((ShadowsQuality)index);
            contactShadowsQualityBar.OnIndexChanged +=
                index => _graphics.SetContactShadows((ContactShadowsQuality)index);
            bloomBar.OnIndexChanged += index => _graphics.SetBloomQuality((BloomQuality)index);
            aoBar.OnIndexChanged += index => _graphics.SetAmbientOcclusionQuality((AmbientOcclusionQuality)index);
            motionBlurBar.OnIndexChanged += index => _graphics.SetMotionBlurQuality((MotionBlurQuality)index);
            refractionQualityBar.OnIndexChanged += index => _graphics.SetRefractionQuality((RefractionQuality)index);
            ssrQualityBar.OnIndexChanged += index => _graphics.SetScreenSpaceReflectionsQuality((ScreenSpaceReflectionQuality)index);
            antiAliasingQualityBar.OnIndexChanged += index => _graphics.SetAntiAliasing((AntiAliasingQuality)index);
            subSurfaceScatteringQualityBar.OnIndexChanged += index => _graphics.SetSubSurfaceScattering((SubSurfaceScatteringQuality)index);
            
            textureQualityBar.SetIndex((int)data.textureQuality);
            sunShadowsQualityBar.SetIndex((int)data.sunShadowsQuality);
            punctualShadowsQualityBar.SetIndex((int)data.additionalShadowsQuality);
            contactShadowsQualityBar.SetIndex((int)data.contactShadowsQuality);
            bloomBar.SetIndex((int)data.bloomQuality);
            aoBar.SetIndex((int)data.aoQuality);
            motionBlurBar.SetIndex((int)data.motionBlurQuality);
            refractionQualityBar.SetIndex((int)data.refractionQuality);
            ssrQualityBar.SetIndex((int)data.screenSpaceReflectionQuality);
            antiAliasingQualityBar.SetIndex((int)data.antiAliasingQuality);
            subSurfaceScatteringQualityBar.SetIndex((int)data.subSurfaceScatteringQuality);
        }

        public void Save()
        {
            _graphics.Apply();
            _graphics.Save();
        }
    }
}