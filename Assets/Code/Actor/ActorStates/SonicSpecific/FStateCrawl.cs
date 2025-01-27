using SurgeEngine.Code.ActorStates.BaseStates;
using SurgeEngine.Code.ActorStates.SonicSubStates;
using SurgeEngine.Code.ActorSystem;
using SurgeEngine.Code.ActorSystem.Actors;
using SurgeEngine.Code.Config;
using SurgeEngine.Code.Config.SonicSpecific;
using SurgeEngine.Code.Custom;
using SurgeEngine.Code.StateMachine;
using UnityEngine;

namespace SurgeEngine.Code.ActorStates.SonicSpecific
{
    public class FStateCrawl : FStateMove
    {
        private string _surfaceTag;

        private float collisionHeight = 0.3f;
        private float collisionCenterY = -0.5f;

        private CrawlConfig crawlConfig;
        BaseActorConfig baseConfig;

        public FStateCrawl(Actor owner, Rigidbody rigidbody) : base(owner, rigidbody)
        {
            crawlConfig = (owner as Sonic).crawlConfig;
            baseConfig = Actor.config;
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
        }

        public override void OnFixedTick(float dt)
        {
            base.OnFixedTick(dt);

            // how the hell do i make him move

            if (Common.CheckForGround(out RaycastHit hit, CheckGroundType.Normal, 5f))
            {
                Vector3 point = hit.point;
                Vector3 normal = hit.normal;
                Kinematics.Normal = normal;

                Kinematics.WriteMovementVector(Input.moveVector);
                Actor.model.RotateBody(normal);
                Kinematics.Snap(point, normal, true);
            }
            else
            {
                StateMachine.SetState<FStateAir>();
            }
        }

        public string GetSurfaceTag() => _surfaceTag;
    }
}