using UnityEngine;

namespace SurgeEngine.Code.Core.Actor.Model
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
