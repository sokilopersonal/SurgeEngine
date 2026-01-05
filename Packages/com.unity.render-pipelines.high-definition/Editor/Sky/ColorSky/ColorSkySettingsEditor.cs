namespace UnityEditor.Rendering.HighDefinition.ColorSky
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(UnityEngine.Rendering.HighDefinition.ColorSky.ColorSky))]
    public class ColorSkySettingsEditor : SkySettingsEditor
    {
        private SerializedDataParameter m_Color;

        public override void OnEnable()
        {
            base.OnEnable();

            m_CommonUIElementsMask = (uint)SkySettingsUIElement.UpdateMode 
                                     | (uint)SkySettingsUIElement.SkyIntensity;
            
            var o = new PropertyFetcher<UnityEngine.Rendering.HighDefinition.ColorSky.ColorSky>(serializedObject);

            m_Color = Unpack(o.Find(x => x.color));
        }

        public override void OnInspectorGUI()
        {
            PropertyField(m_Color);
            
            base.CommonSkySettingsGUI();
        }
    }
}