using UnityEngine;

namespace SurgeEngine.Source.Code.Infrastructure.Custom
{
    public class GPUInstancer : MonoBehaviour
    {
        private void Awake()
        {
            MaterialPropertyBlock mpb = new MaterialPropertyBlock();
            foreach (MeshRenderer child in GetComponentsInChildren<MeshRenderer>())
            {
                child.SetPropertyBlock(mpb);
            }
        }
    }
}