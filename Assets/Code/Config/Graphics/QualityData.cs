using System;

namespace SurgeEngine.Code.Config.Graphics
{
    [Serializable]
    public class QualityData
    {
        public int shadowsQuality = 2;
        public bool pcssShadows = false;
        public bool bloom = true;
        public bool motionBlur = true;
        public int motionBlurQuality = 2;
        public bool ssr = true;
        public int ssrQuality = 2;
        public int antiAliasing = 3; // 0 - No AA, 1 - FXAA, 2 - SMAA, 3 - TAA
    }
}