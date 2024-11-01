using SurgeEngine.Code.ActorSystem;
using SurgeEngine.Code.Custom;
using SurgeEngine.Code.Parameters.SonicSubStates;
using UnityEngine;

namespace SurgeEngine.Code.Parameters
{
    public class FStateSliding : FStateMove
    {
        [SerializeField] private float collisionHeight = 0.3f;
        [SerializeField] private float collisionCenterY = -0.5f;
        [SerializeField] private float slideDeacceleration = 10f;
        public float slideDeactivationSpeed = 7f;

        private Transform _cameraTransform;

        public FStateSliding(Actor owner, Rigidbody rigidbody) : base(owner, rigidbody)
        {
            
        }

        public override void OnEnter()
        {
            base.OnEnter();
            
            StateMachine.GetSubState<FBoost>().Active = false;

            _cameraTransform = Actor.camera.GetCameraTransform();
            
            Actor.model.SetCollisionParam(collisionHeight, collisionCenterY, 0.25f);
        }

        public override void OnExit()
        {
            base.OnExit();

            Actor.model.SetCollisionParam(0,0);
            
            Animation.ResetAction();
        }

        public override void OnTick(float dt)
        {
            base.OnTick(dt);

            if (Stats.currentSpeed < slideDeactivationSpeed || !Input.BHeld)
            {
                if (Stats.currentSpeed > slideDeactivationSpeed)
                {
                    StateMachine.SetState<FStateGround>();
                }
                else
                    StateMachine.SetState<FStateIdle>();
            }
        }

        public override void OnFixedTick(float dt)
        {
            base.OnFixedTick(dt);
            
            if (Common.CheckForGround(out var hit))
            {
                var point = hit.point;
                var normal = hit.normal;
                Kinematics.Normal = normal;
                
                Stats.transformNormal = Vector3.Slerp(Stats.transformNormal, normal, dt * 14f);
                
                Kinematics.BasePhysics(point, normal);
                Actor.model.RotateBody(normal);
            }
            else
            {
                StateMachine.SetState<FStateAir>();
            }
        }
    }
}