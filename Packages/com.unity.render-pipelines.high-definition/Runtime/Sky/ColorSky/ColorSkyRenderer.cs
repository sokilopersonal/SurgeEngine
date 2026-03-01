namespace UnityEngine.Rendering.HighDefinition.ColorSky
{
    public class ColorSkyRenderer : SkyRenderer
    {
        private Material m_ColorSkyMaterial;
        private MaterialPropertyBlock m_PropertyBlock = new MaterialPropertyBlock();
        
        readonly int _Color = Shader.PropertyToID("_Color");

        public ColorSkyRenderer()
        {
            SupportDynamicSunLight = false;
        }
        
        public override void Build()
        {
            var shaders = GraphicsSettings.GetRenderPipelineSettings<HDRenderPipelineRuntimeShaders>();
            m_ColorSkyMaterial = CoreUtils.CreateEngineMaterial(shaders.colorSkyPS);
        }

        public override void Cleanup()
        {
            CoreUtils.Destroy(m_ColorSkyMaterial);
        }

        public override void RenderSky(BuiltinSkyParameters builtinParams, bool renderForCubemap, bool renderSunDisk)
        {
            var colorSky = builtinParams.skySettings as ColorSky;
            m_ColorSkyMaterial.SetColor(_Color, colorSky.color.value);
            m_ColorSkyMaterial.SetFloat(HDShaderIDs._SkyIntensity, GetSkyIntensity(colorSky, builtinParams.debugSettings));
            
            m_PropertyBlock.SetMatrix(HDShaderIDs._PixelCoordToViewDirWS, builtinParams.pixelCoordToViewDirMatrix);
            
            CoreUtils.DrawFullScreen(builtinParams.commandBuffer, m_ColorSkyMaterial, m_PropertyBlock, renderForCubemap ? 0 : 1);
        }
    }
}