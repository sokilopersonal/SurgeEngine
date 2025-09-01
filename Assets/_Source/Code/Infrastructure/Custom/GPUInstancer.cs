using UnityEngine;

namespace SurgeEngine._Source.Code.Infrastructure.Custom
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