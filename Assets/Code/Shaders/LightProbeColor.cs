using UnityEngine;
using UnityEngine.Rendering;

namespace SurgeEngine.Code.Shaders
{
    [ExecuteAlways]
    public class LightProbeColor : MonoBehaviour
    {
        [SerializeField] private Material[] material;

        private void Update()
        {
            if (material == null) return;
            
            Vector3 objectPosition = transform.position;

            SphericalHarmonicsL2 probe;
            Vector3[] probeAngle = { Vector3.up, Vector3.right };
            Color[] LightValue = new Color[probeAngle.Length];

            LightProbes.GetInterpolatedProbe(objectPosition, null, out probe);

            probe.Evaluate(probeAngle, LightValue);
            Color color = LightValue[0] * 0.0001f;
            color = new Color(color.r * color.r, color.g * color.g, color.b * color.b);

            for (int i = 0; i < material.Length; i++)
            {
                material[i].SetVector("_RimLightColor", new Vector4(color.r, color.g, color.b, 1.0f));
            }
        }
    }
}