using SurgeEngine.Source.Code.UI.OptionBars;
using UnityEngine;
using Zenject;

namespace SurgeEngine.Source.Code.Infrastructure.Tools.Managers.UI
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
            };

            meshBar.OnChanged += b =>
            {
                _graphics.SetMeshQuality((MeshQuality)b.Index);
            };
            
            sunShadowsQualityBar.OnChanged += b =>
            {
                _graphics.SetSunShadowsQuality((ShadowsQuality)b.Index);
            };
            
            punctualShadowsQualityBar.OnChanged += b =>
            {
                _graphics.SetAdditionalShadowsQuality((ShadowsQuality)b.Index);
            };
            
            bloomBar.OnChanged += b =>
            {
                _graphics.SetBloomQuality((BloomQuality)b.Index);
            };
            
            aoBar.OnChanged += b =>
            {
                _graphics.SetAmbientOcclusionQuality((AmbientOcclusionQuality)b.Index);
            };
            
            motionBlurBar.OnChanged += b =>
            {
                bool isEnabled = b.Index == 1;
                
                _graphics.SetMotionBlur(!isEnabled ? MotionBlurState.Off : MotionBlurState.On);
                motionBlurQualityBar.gameObject.SetActive(isEnabled);
            };
            
            motionBlurQualityBar.OnChanged += b =>
            {
                _graphics.SetMotionBlurQuality((MotionBlurQuality)b.Index);
            };
            
            ssrQualityBar.OnChanged += b =>
            {
                _graphics.SetScreenSpaceReflectionsQuality((ScreenSpaceReflectionQuality)b.Index);
            };
            
            subSurfaceScatteringQualityBar.OnChanged += b =>
            {
                _graphics.SetSubSurfaceScattering((SubSurfaceScatteringQuality)b.Index);
            };
            
            textureBar.Set((int)data.TextureQuality.Value);
            meshBar.Set((int)data.MeshQuality.Value);
            sunShadowsQualityBar.Set((int)data.SunShadowsQuality.Value);
            punctualShadowsQualityBar.Set((int)data.AdditionalShadowsQuality.Value);
            bloomBar.Set((int)data.BloomQuality.Value);
            aoBar.Set((int)data.AOQuality.Value);
            motionBlurBar.Set((int)data.MotionBlur.Value);
            motionBlurQualityBar.Set((int)data.MotionBlurQuality.Value);
            ssrQualityBar.Set((int)data.ScreenSpaceReflectionQuality.Value);
            subSurfaceScatteringQualityBar.Set((int)data.SubSurfaceScatteringQuality.Value);
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
                textureBar.Set((int)data.TextureQuality.Value);
                meshBar.Set((int)data.MeshQuality.Value);
                sunShadowsQualityBar.Set((int)data.SunShadowsQuality.Value);
                punctualShadowsQualityBar.Set((int)data.AdditionalShadowsQuality.Value);
                bloomBar.Set((int)data.BloomQuality.Value);
                aoBar.Set((int)data.AOQuality.Value);
                motionBlurBar.Set((int)data.MotionBlur.Value);
                motionBlurQualityBar.Set((int)data.MotionBlurQuality.Value);
                ssrQualityBar.Set((int)data.ScreenSpaceReflectionQuality.Value);
                subSurfaceScatteringQualityBar.Set((int)data.SubSurfaceScatteringQuality.Value);
                
                Save();
            });
        }
    }
}