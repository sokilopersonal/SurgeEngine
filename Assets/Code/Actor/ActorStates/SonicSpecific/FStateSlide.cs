using SurgeEngine.Code.ActorStates.BaseStates;
using SurgeEngine.Code.ActorStates.SonicSubStates;
using SurgeEngine.Code.ActorSystem;
using SurgeEngine.Code.ActorSystem.Actors;
using SurgeEngine.Code.Config.SonicSpecific;
using SurgeEngine.Code.Custom;
using UnityEngine;

namespace SurgeEngine.Code.ActorStates
{
    public class FStateSlide : FStateMove
    {
        private float collisionHeight = 0.3f;
        private float collisionCenterY = -0.5f;

        private SlideConfig _config;

        public FStateSlide(Actor owner, Rigidbody rigidbody) : base(owner, rigidbody)
        {
            _config = (owner as Sonic).slideConfig;
        }

        public override void OnEnter()
        {
            base.OnEnter();
            
            StateMachine.GetSubState<FBoost>().Active = false;
            
            Actor.model.SetCollisionParam(collisionHeight, -0.75f, 0.25f);
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

            if (Stats.currentSpeed < _config.minSpeed || !Input.BHeld)
            {
                if (Stats.currentSpeed > _config.minSpeed)
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
                
                Kinematics.ModifyTurnRate(0.2f);
                if (Kinematics.MoveDot < 0.75f) Kinematics.BasePhysics(point, normal);
                Kinematics.Deceleration(_config.deceleration, _config.deceleration);
                Actor.model.RotateBody(normal);
            }
            else
            {
                StateMachine.SetState<FStateAir>();
            }
        }
        
        public SlideConfig GetConfig() => _config;
    }
}