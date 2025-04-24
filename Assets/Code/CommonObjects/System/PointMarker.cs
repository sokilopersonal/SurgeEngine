using UnityEngine;

namespace SurgeEngine.Code.CommonObjects.System
{
    [RequireComponent(typeof(BoxCollider))]
    public class PointMarker : ContactBase
    {
        [SerializeField, Range(0.5f, 5f)] private float length = 2f;
        public float Length => length;

        public override void Contact(Collider msg)
        {
            base.Contact(msg);
            
            Debug.Log("PointMarker Contact");
        }
    }
}