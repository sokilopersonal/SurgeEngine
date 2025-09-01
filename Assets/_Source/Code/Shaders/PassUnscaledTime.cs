using UnityEngine;
using UnityEngine.UI;

namespace SurgeEngine._Source.Code.Shaders
{
    /// <summary>
    /// A component to pass unscaled time to the shader
    /// </summary>
    [ExecuteAlways]
    public class PassUnscaledTime : MonoBehaviour
    {
        [SerializeField] private Image mat;
        [SerializeField] private Vector2 direction;

        private void Update()
        {
            if (mat != null)
            {
                mat.materialForRendering.SetFloat("_UnscaledTime", Time.unscaledTime);
                mat.materialForRendering.SetVector("_Direction", direction);
            }
        }
    }
}