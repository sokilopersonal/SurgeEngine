using UnityEngine;

namespace SurgeEngine.Code.Misc
{
    public class CopyTransform : MonoBehaviour
    {
        [SerializeField] Transform target;
        void Update()
        {
            transform.position = target.position;
            transform.rotation = target.rotation;
        }
    }
}
