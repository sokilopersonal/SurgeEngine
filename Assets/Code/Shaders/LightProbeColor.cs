using UnityEngine;

namespace SurgeEngine.Code.Shaders
{
    [ExecuteAlways]
    public class LightProbeColor : MonoBehaviour
    {
        [SerializeField] private Material[] material;

        private void Update()
        {
            return;
            if (material == null) return;
            
            Vector3 objectPosition = transform.position;

            LightProbes.GetInterpolatedProbe(objectPosition, null, out var probe);
            
            float r = 0f, g = 0f, b = 0f;
            var sh = probe;
            var direction = Vector3.up;
            
            for (int i = 0; i < 3; i++)
            {
                r += sh[i, 0] + sh[i, 1] * direction.x + sh[i, 2] * direction.y + sh[i, 3] * direction.z;
                g += sh[i, 4] + sh[i, 5] * direction.x + sh[i, 6] * direction.y + sh[i, 7] * direction.z;
                b += sh[i, 8];
            }
            
            Color color = new Color(r, g, b, 1.0f);
            
            for (int i = 0; i < material.Length; i++)
            {
                material[i].SetVector("_RimLightColor", new Vector4(color.r, color.g, color.b, 1.0f));
            }
        }
    }
}