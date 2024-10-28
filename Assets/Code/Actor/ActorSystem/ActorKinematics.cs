using UnityEngine;

namespace SurgeEngine.Code.ActorSystem
{
    public class ActorKinematics : ActorComponent
    {
        public Rigidbody Rigidbody => _rigidbody;

        private Rigidbody _rigidbody;

        protected override void OnInitialized()
        {
            base.OnInitialized();

            _rigidbody = GetComponent<Rigidbody>();
        }

        public void BaseGroundPhysics(Vector3 point, Vector3 normal)
        {
            Snap(point, normal);
        }

        private void Snap(Vector3 point, Vector3 normal)
        {
            Rigidbody.position = Vector3.Slerp(Rigidbody.position, point + normal, 12 * Time.fixedDeltaTime);
        }
    }
}