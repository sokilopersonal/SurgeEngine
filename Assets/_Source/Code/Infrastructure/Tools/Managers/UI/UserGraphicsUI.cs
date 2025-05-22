using System;
using SurgeEngine.Code.UI.Menus.OptionElements;
using UnityEngine;
using Zenject;

namespace SurgeEngine.Code.Infrastructure.Tools.Managers.UI
{
    public class UserGraphicsUI : MonoBehaviour
    {
        [SerializeField] private OptionBar textureQualityBar;
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

        private void Awake()
        {
            var data = _graphics.GetData();
            var bindings = new (OptionBar bar, Action<int> set, int current)[]
            {
                (textureQualityBar, i => _graphics.SetTextureQuality((TextureQuality)i), (int)data.textureQuality),
                (sunShadowsQualityBar, i => _graphics.SetSunShadowsQuality((ShadowsQuality)i), (int)data.sunShadowsQuality),
                (punctualShadowsQualityBar, i => _graphics.SetAdditionalShadowsQuality((ShadowsQuality)i), (int)data.additionalShadowsQuality),
                (contactShadowsQualityBar, i => _graphics.SetContactShadows((ContactShadowsQuality)i), (int)data.contactShadowsQuality),
                (bloomBar, i => _graphics.SetBloomQuality((BloomQuality)i), (int)data.bloomQuality),
                (aoBar, i => _graphics.SetAmbientOcclusionQuality((AmbientOcclusionQuality)i), (int)data.aoQuality),
                (motionBlurBar, i => _graphics.SetMotionBlurQuality((MotionBlurQuality)i), (int)data.motionBlurQuality),
                (refractionQualityBar, i => _graphics.SetRefractionQuality((RefractionQuality)i), (int)data.refractionQuality),
                (ssrQualityBar, i => _graphics.SetScreenSpaceReflectionsQuality((ScreenSpaceReflectionQuality)i), (int)data.screenSpaceReflectionQuality),
                (subSurfaceScatteringQualityBar, i => _graphics.SetSubSurfaceScattering((SubSurfaceScatteringQuality)i), (int)data.subSurfaceScatteringQuality),
            };

            foreach (var (bar, set, current) in bindings)
            {
                bar.SetIndex(current);
                bar.OnIndexChanged += i => {
                    set(i);
                    _graphics.Apply();
                };
            }

        }

        public void Save()
        {
            _graphics.Save();
        }

        public void Revert()
        {
            _graphics.Load(data =>
            {
                textureQualityBar.SetIndex((int)data.textureQuality);
                sunShadowsQualityBar.SetIndex((int)data.sunShadowsQuality);
                punctualShadowsQualityBar.SetIndex((int)data.additionalShadowsQuality);
                contactShadowsQualityBar.SetIndex((int)data.contactShadowsQuality);
                bloomBar.SetIndex((int)data.bloomQuality);
                aoBar.SetIndex((int)data.aoQuality);
                motionBlurBar.SetIndex((int)data.motionBlurQuality);
                refractionQualityBar.SetIndex((int)data.refractionQuality);
                ssrQualityBar.SetIndex((int)data.screenSpaceReflectionQuality);
                subSurfaceScatteringQualityBar.SetIndex((int)data.subSurfaceScatteringQuality);
                
                _graphics.Apply();
                _graphics.Save();
            });
        }
    }
}