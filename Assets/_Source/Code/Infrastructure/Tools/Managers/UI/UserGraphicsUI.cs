using SurgeEngine._Source.Code.UI.OptionBars;
using UnityEngine;
using Zenject;

namespace SurgeEngine._Source.Code.Infrastructure.Tools.Managers.UI
{
    public class UserGraphicsUI : OptionUI
    {
        [SerializeField] private OptionBar textureBar;
        [SerializeField] private OptionBar meshBar;
        [SerializeField] private OptionBar sunShadowsQualityBar;
        [SerializeField] private OptionBar punctualShadowsQualityBar;
        [SerializeField] private OptionBar bloomBar;
        [SerializeField] private OptionBar aoBar;
        [SerializeField] private OptionBar motionBlurBar;
        [SerializeField] private OptionBar motionBlurQualityBar;
        [SerializeField] private OptionBar ssrQualityBar;
        [SerializeField] private OptionBar subSurfaceScatteringQualityBar;

        [Inject] private UserGraphics _graphics;

        protected override void Setup()
        {
            var data = _graphics.GetData();
            
            motionBlurQualityBar.gameObject.SetActive(false);

            textureBar.OnChanged += b =>
            {
                _graphics.SetTextureQuality((TextureQuality)b.Index);
                _graphics.Apply();
            };

            meshBar.OnChanged += b =>
            {
                _graphics.SetMeshQuality((MeshQuality)b.Index);
                _graphics.Apply();
            };
            
            sunShadowsQualityBar.OnChanged += b =>
            {
                _graphics.SetSunShadowsQuality((ShadowsQuality)b.Index);
                _graphics.Apply();
            };
            
            punctualShadowsQualityBar.OnChanged += b =>
            {
                _graphics.SetAdditionalShadowsQuality((ShadowsQuality)b.Index);
                _graphics.Apply();
            };
            
            bloomBar.OnChanged += b =>
            {
                _graphics.SetBloomQuality((BloomQuality)b.Index);
                _graphics.Apply();
            };
            
            aoBar.OnChanged += b =>
            {
                _graphics.SetAmbientOcclusionQuality((AmbientOcclusionQuality)b.Index);
                _graphics.Apply();
            };
            
            motionBlurBar.OnChanged += b =>
            {
                bool isEnabled = b.Index == 1;
                
                _graphics.SetMotionBlur(!isEnabled ? MotionBlurState.Off : MotionBlurState.On);
                _graphics.Apply();
                
                motionBlurQualityBar.gameObject.SetActive(isEnabled);
            };
            
            motionBlurQualityBar.OnChanged += b =>
            {
                _graphics.SetMotionBlurQuality((MotionBlurQuality)b.Index);
                _graphics.Apply();
            };
            
            ssrQualityBar.OnChanged += b =>
            {
                _graphics.SetScreenSpaceReflectionsQuality((ScreenSpaceReflectionQuality)b.Index);
                _graphics.Apply();
            };
            
            subSurfaceScatteringQualityBar.OnChanged += b =>
            {
                _graphics.SetSubSurfaceScattering((SubSurfaceScatteringQuality)b.Index);
                _graphics.Apply();
            };
            
            textureBar.Set((int)data.textureQuality);
            meshBar.Set((int)data.meshQuality);
            sunShadowsQualityBar.Set((int)data.sunShadowsQuality);
            punctualShadowsQualityBar.Set((int)data.additionalShadowsQuality);
            bloomBar.Set((int)data.bloomQuality);
            aoBar.Set((int)data.aoQuality);
            motionBlurBar.Set((int)data.motionBlur);
            motionBlurQualityBar.Set((int)data.motionBlurQuality);
            ssrQualityBar.Set((int)data.screenSpaceReflectionQuality);
            subSurfaceScatteringQualityBar.Set((int)data.subSurfaceScatteringQuality);
        }

        public override void Save()
        {
            _graphics.Save();
            
            base.Save();
        }

        public override void Revert()
        {
            base.Revert();
            
            _graphics.Load(data =>
            {
                textureBar.Set((int)data.textureQuality);
                meshBar.Set((int)data.meshQuality);
                sunShadowsQualityBar.Set((int)data.sunShadowsQuality);
                punctualShadowsQualityBar.Set((int)data.additionalShadowsQuality);
                bloomBar.Set((int)data.bloomQuality);
                aoBar.Set((int)data.aoQuality);
                motionBlurBar.Set((int)data.motionBlur);
                motionBlurQualityBar.Set((int)data.motionBlurQuality);
                ssrQualityBar.Set((int)data.screenSpaceReflectionQuality);
                subSurfaceScatteringQualityBar.Set((int)data.subSurfaceScatteringQuality);
                
                _graphics.Apply();
                Save();
            });
        }
    }
}