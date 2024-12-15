using SurgeEngine.Code.Tools;
using UnityEngine;

namespace SurgeEngine.Code.UI.Settings
{
    public class GraphicsUIController : MonoBehaviour
    {
        [SerializeField] private SettingsBarElement shadowsQualityBar;
        [SerializeField] private SettingsBarElement pcssBar;
        [SerializeField] private SettingsBarElement bloomBar;
        [SerializeField] private SettingsBarElement motionBlurBar;
        [SerializeField] private SettingsBarElement motionBlurQualityBar;
        [SerializeField] private SettingsBarElement reflectionsBar;
        [SerializeField] private SettingsBarElement reflectionsQualityBar;
        [SerializeField] private SettingsBarElement aaBar;

        private void Start()
        {
            var ug = UserGraphics.Instance;
            
            shadowsQualityBar.value = ug.data.shadowsQuality;
            pcssBar.value = ug.data.pcssShadows ? 1 : 0;
            bloomBar.value = ug.data.bloom ? 1 : 0;
            motionBlurBar.value = ug.data.motionBlurQuality;
            motionBlurQualityBar.value = ug.data.motionBlurQuality;
            reflectionsBar.value = ug.data.ssr ? 1 : 0;
            reflectionsQualityBar.value = ug.data.ssrQuality;
            aaBar.value = ug.data.antiAliasing;
            
            shadowsQualityBar.OnValueChanged += i => ug.SetDirectionalLightQuality(shadowsQualityBar.value);
            pcssBar.OnValueChanged += i => ug.SetPCSS(pcssBar.value == 1);
            bloomBar.OnValueChanged += i => ug.SetBloom(bloomBar.value == 1);
            motionBlurBar.OnValueChanged += i => ug.SetMotionBlur(motionBlurBar.value == 1);
            motionBlurQualityBar.OnValueChanged += i => ug.SetMotionBlurQuality(motionBlurQualityBar.value);
            reflectionsBar.OnValueChanged += i => ug.SetSSR(reflectionsBar.value == 1);
            reflectionsQualityBar.OnValueChanged += i => ug.SetSSRQuality(reflectionsBar.value);
            aaBar.OnValueChanged += i => ug.SetAA(aaBar.value);
        }
    }
}