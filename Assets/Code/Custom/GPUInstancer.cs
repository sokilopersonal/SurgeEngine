using UnityEngine;

namespace SurgeEngine.Code.Custom
{
    public class GPUInstancer : MonoBehaviour
    {
        private void Awake()
        {
            MaterialPropertyBlock mpb = new MaterialPropertyBlock();
            foreach (var child in GetComponentsInChildren<MeshRenderer>())
            {
                child.SetPropertyBlock(mpb);
            }
        }
    }
}