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

            if (!Input.BHeld)
            {
                if (Input.moveVector.magnitude > 0.1f)
                {
                    StateMachine.SetState<FStateGround>();
                }
                else
                {
                    StateMachine.SetState<FStateIdle>();
                }
            }

            if (Input.moveVector.magnitude < 0.1)
            {
                StateMachine.SetState<FStateSit>();
            }
        }

        public override void OnFixedTick(float dt)
        {
            base.OnFixedTick(dt);

            // how the hell do i make him move

            Vector3 prevNormal = Kinematics.Normal;
            BaseActorConfig config = Actor.config;
            float distance = config.castDistance * config.castDistanceCurve
                .Evaluate(Kinematics.HorizontalSpeed / crawlConfig.topSpeed);
            if (Common.CheckForGround(out RaycastHit data, castDistance: distance) &&
                Kinematics.CheckForPredictedGround(_rigidbody.linearVelocity, Kinematics.Normal, Time.fixedDeltaTime, distance, 6))
            {
                Kinematics.Point = data.point;
                Kinematics.Normal = data.normal;

                Vector3 stored = Vector3.ClampMagnitude(_rigidbody.linearVelocity, crawlConfig.maxSpeed);
                _rigidbody.linearVelocity = Quaternion.FromToRotation(_rigidbody.transform.up, prevNormal) * stored;

                Actor.kinematics.BasePhysics(Kinematics.Point, Kinematics.Normal);
                Actor.model.RotateBody(Kinematics.Normal);

                _surfaceTag = data.transform.gameObject.GetGroundTag();
            }
            else
            {
                StateMachine.SetState<FStateAir>();
            }
        }

        public string GetSurfaceTag() => _surfaceTag;
    }
}