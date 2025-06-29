using System;
using SurgeEngine.Code.UI.OptionBars;
using UnityEngine;
using Zenject;

namespace SurgeEngine.Code.Infrastructure.Tools.Managers.UI
{
    public class UserGraphicsUI : OptionUI
    {
        [SerializeField] private OptionBar textureBar;
        [SerializeField] private OptionBar sunShadowsQualityBar;
        [SerializeField] private OptionBar punctualShadowsQualityBar;
        [SerializeField] private OptionBar contactShadowsQualityBar;
        [SerializeField] private OptionBar bloomBar;
        [SerializeField] private OptionBar aoBar;
        [SerializeField] private OptionBar motionBlurBar;
        [SerializeField] private OptionBar refractionQualityBar;
        [SerializeField] private OptionBar ssrQualityBar;
        [SerializeField] private OptionBar subSurfaceScatteringQualityBar;

        [Inject] private UserGraphics _graphics;

        protected override void Awake()
        {
            base.Awake();
            
            var data = _graphics.GetData();

            textureBar.OnChanged += b =>
            {
                _graphics.SetTextureQuality((TextureQuality)b.Index);
                _graphics.Apply();
            };
            
            sunShadowsQualityBar.OnChanged += b =>
            {
                data.sunShadowsQuality = (ShadowsQuality)b.Index;
                _graphics.Apply();
            };
            
            punctualShadowsQualityBar.OnChanged += b =>
            {
                data.additionalShadowsQuality = (ShadowsQuality)b.Index;
                _graphics.Apply();
            };
            
            contactShadowsQualityBar.OnChanged += b =>
            {
                data.contactShadowsQuality = (ContactShadowsQuality)b.Index;
                _graphics.Apply();
            };
            
            bloomBar.OnChanged += b =>
            {
                data.bloomQuality = (BloomQuality)b.Index;
                _graphics.Apply();
            };
            
            aoBar.OnChanged += b =>
            {
                data.aoQuality = (AmbientOcclusionQuality)b.Index;
                _graphics.Apply();
            };
            
            motionBlurBar.OnChanged += b =>
            {
                data.motionBlurQuality = (MotionBlurQuality)b.Index;
                _graphics.Apply();
            };
            
            refractionQualityBar.OnChanged += b =>
            {
                data.refractionQuality = (RefractionQuality)b.Index;
                _graphics.Apply();
            };
            
            ssrQualityBar.OnChanged += b =>
            {
                data.screenSpaceReflectionQuality = (ScreenSpaceReflectionQuality)b.Index;
                _graphics.Apply();
            };
            
            subSurfaceScatteringQualityBar.OnChanged += b =>
            {
                data.subSurfaceScatteringQuality = (SubSurfaceScatteringQuality)b.Index;
                _graphics.Apply();
            };
            
            textureBar.Set((int)data.textureQuality);
            sunShadowsQualityBar.Set((int)data.sunShadowsQuality);
            punctualShadowsQualityBar.Set((int)data.additionalShadowsQuality);
            contactShadowsQualityBar.Set((int)data.contactShadowsQuality);
            bloomBar.Set((int)data.bloomQuality);
            aoBar.Set((int)data.aoQuality);
            motionBlurBar.Set((int)data.motionBlurQuality);
            refractionQualityBar.Set((int)data.refractionQuality);
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
                sunShadowsQualityBar.Set((int)data.sunShadowsQuality);
                punctualShadowsQualityBar.Set((int)data.additionalShadowsQuality);
                contactShadowsQualityBar.Set((int)data.contactShadowsQuality);
                bloomBar.Set((int)data.bloomQuality);
                aoBar.Set((int)data.aoQuality);
                motionBlurBar.Set((int)data.motionBlurQuality);
                refractionQualityBar.Set((int)data.refractionQuality);
                ssrQualityBar.Set((int)data.screenSpaceReflectionQuality);
                subSurfaceScatteringQualityBar.Set((int)data.subSurfaceScatteringQuality);
                
                _graphics.Apply();
                Save();
            });
        }
    }
}