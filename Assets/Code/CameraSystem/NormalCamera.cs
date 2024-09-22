using SurgeEngine.Code.ActorSystem;
using SurgeEngine.Code.Parameters;
using UnityEngine;

namespace SurgeEngine.Code.CameraSystem
{
    public class NormalCamera : MonoBehaviour
    {
        public bool active;
        [SerializeField] private int minAngle;
        
        private Vector3 actorNormal;
        private Vector3 cameraNormal;
        
        private void Update()
        {
            var context = ActorContext.Context;
            transform.position = context.transform.position;
            
            if (active)
            {
                actorNormal = context.stats.groundNormal;
                if (context.stateMachine.CurrentState is FStateGround)
                {
                    cameraNormal = actorNormal;

                    if (context.stats.groundAngle >= minAngle)
                    {
                        cameraNormal = actorNormal;
                    }
                    else
                    {
                        cameraNormal = Vector3.up;
                    }
                }
                else if (context.stateMachine.CurrentState is FStateAir)
                {
                    cameraNormal = Vector3.up;
                }
            }
            else
            {
                cameraNormal = Vector3.up;
            }
            
            Quaternion from = Quaternion.FromToRotation(transform.up, cameraNormal) * transform.rotation;
            transform.rotation = Quaternion.Slerp(transform.rotation, from, 5 * Time.deltaTime);
        }

        public void Toggle() => active = !active;
    }
}