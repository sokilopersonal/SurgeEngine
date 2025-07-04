using SurgeEngine.Code.Core.Actor.States.BaseStates;
using SurgeEngine.Code.Core.Actor.System;
using SurgeEngine.Code.Core.StateMachine.Interfaces;
using SurgeEngine.Code.Infrastructure.Config.SonicSpecific;
using UnityEngine;
using UnityEngine.Splines;

namespace SurgeEngine.Code.Core.Actor.States.Characters.Sonic
{
    public class FStateQuickstep : FActorState, IStateTimeout
    {
        public float Timeout { get; set; }
        
        private QuickstepDirection _direction;
        private float _timer;
        private bool _isRun;
        public bool IsRun => _isRun;

        private readonly QuickStepConfig _config;

        public FStateQuickstep(ActorBase owner) : base(owner)
        {
            owner.TryGetConfig(out _config);
        }

        public override void OnEnter()
        {
            base.OnEnter();

            _timer = 0;
            
            Timeout = _config.delay;

            float speed = !IsRun ? _config.force : _config.runForce;
            var sideDir = _direction == QuickstepDirection.Left ? -speed : speed;
            SetSideVelocity(sideDir);

            if (Kinematics.mode == KinematicsMode.Dash)
            {
                SnapToNearestSpline();
            }
            
            if (StateMachine.PreviousState is FStateSlide)
            {
                Rigidbody.linearVelocity += Rigidbody.transform.forward * 8f; // TODO: Move this QSS value to config
            }
        }

        public override void OnTick(float dt)
        {
            base.OnTick(dt);

            _timer += dt / (!IsRun ? _config.duration : _config.runDuration);
            if (_timer >= 1f)
            {
                SetSideVelocity(0);
                StateMachine.SetState<FStateGround>();
            }
        }

        public override void OnFixedTick(float dt)
        {
            base.OnFixedTick(dt);

            var config = Actor.Config;
            float distance = config.EvaluateCastDistance(config.castDistanceCurve.Evaluate(Kinematics.Speed / config.topSpeed));
            if (Kinematics.CheckForGround(out var hit, castDistance: distance))
            {
                bool predicted = Kinematics.CheckForPredictedGround(dt, distance, 4);
                Kinematics.RotateSnapNormal(hit.normal);
                
                if (predicted) Kinematics.Snap(hit.point, Kinematics.Normal, true);
                Kinematics.Project();
            }
            else
            {
                StateMachine.SetState<FStateAir>();
            }
        }

        private void SetSideVelocity(float sideDir)
        {
            var localVel = Rigidbody.transform.InverseTransformDirection(Rigidbody.linearVelocity);
            localVel.x = sideDir;
            Rigidbody.linearVelocity = Rigidbody.transform.TransformDirection(localVel);
        }
        
        private void SnapToNearestSpline()
        {
            var container = Kinematics.GetPath();
            if (container == null) return;

            float bestDist = float.MaxValue;
            Vector3 bestPos = Actor.transform.position;
            Vector3 bestTangent = Actor.transform.forward;

            for (int i = 0; i < container.Container.Splines.Count; i++)
            {
                var spline = container.Container.Splines[i];
                SplineUtility.GetNearestPoint(spline, container.Container.transform.InverseTransformPoint(Rigidbody.position), out _, out float t);
                Vector3 pos = spline.EvaluatePosition(t);
                Vector3 tangent = spline.EvaluateTangent(t);
                float d = Vector3.Distance(Rigidbody.position, pos);
                if (d < bestDist)
                {
                    bestDist = d;
                    bestPos = pos;
                    bestTangent = tangent;
                }
            }

            Actor.transform.position = bestPos;
            Actor.transform.rotation = Quaternion.LookRotation(bestTangent, Actor.transform.up);
        }

        public FStateQuickstep SetDirection(QuickstepDirection direction)
        {
            _direction = direction;
            return this;
        }

        public FStateQuickstep SetRun(bool isRun)
        {
            _isRun = isRun;
            return this;
        }

        public QuickstepDirection GetDirection()
        {
            return _direction;
        }
    }
    
    public enum QuickstepDirection
    {
        Left = -1,
        Right = 1
    }
}