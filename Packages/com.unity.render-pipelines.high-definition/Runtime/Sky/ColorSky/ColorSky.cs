using System;

namespace UnityEngine.Rendering.HighDefinition.ColorSky
{
    [VolumeComponentMenu("Sky/Color Sky")]
    [SupportedOnRenderPipeline(typeof(HDRenderPipelineAsset))]
    [SkyUniqueID((int)SkyType.Color)]
    public class ColorSky : SkySettings
    {
        public ColorParameter color = new ColorParameter(Color.white, true, false, true);

        protected override void OnEnable()
        {
            base.OnEnable();

            includeSunInBaking.value = true;
        }

        public override int GetHashCode()
        {
            int hash = base.GetHashCode();
            hash = hash * 23 + color.GetHashCode();

            return hash;
        }

        public override Type GetSkyRendererType()
        {
            return typeof(ColorSkyRenderer);
        }
    }
}