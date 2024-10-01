using UnityEngine;

namespace SurgeEngine.Code.Parameters
{
    public class FStateDrift : FStateMove
    {
        [SerializeField] private float minTurnSpeed;
        [SerializeField] private float maxTurnSpeed;
        [SerializeField] private float centrifugalForce;

        public override void OnFixedTick(float dt)
        {
            base.OnFixedTick(dt);
            
            Vector3 moveForwardNormal = Quaternion.AngleAxis(Mathf.Lerp(minTurnSpeed, maxTurnSpeed, Mathf.Abs((input.moveVector.x) / 2))
                                                             * 1 * Time.fixedDeltaTime, Vector3.up) * Vector3.forward;
            Vector3 moveRightNormal = Vector3.Cross(actor.transform.up, moveForwardNormal).normalized;
            
            _rigidbody.linearVelocity += stats.inputDir + (moveForwardNormal * stats.planarVelocity.magnitude) + (-moveRightNormal * (centrifugalForce * stats.planarVelocity.magnitude));
        }
    }
}