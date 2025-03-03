using SurgeEngine.Code.ActorStates.BaseStates;
using SurgeEngine.Code.ActorSystem;
using SurgeEngine.Code.ActorSystem.Actors;
using SurgeEngine.Code.Config.SonicSpecific;
using SurgeEngine.Code.Custom;
using SurgeEngine.Code.StateMachine;
using UnityEngine;
using UnityEngine.Splines;

namespace SurgeEngine.Code.ActorStates.SonicSpecific
{
    public class FStateQuickstep : FStateMove, IStateTimeout
    {
        private QuickstepDirection _direction;
        private float _timer;
        private float _savedXSpeed;
        private Vector3 _lastPosition;

        private readonly QuickStepConfig _config;

        public FStateQuickstep(Actor owner, Rigidbody rigidbody) : base(owner, rigidbody)
        {
            owner.TryGetConfig(out _config);
        }

        public override void OnEnter()
        {
            base.OnEnter();

            _timer = 0f;
            Timeout = _config.delay;

            if (Kinematics.mode != KinematicsMode.Dash)
            {
                var local = Actor.transform.InverseTransformDirection(_rigidbody.linearVelocity);
                _savedXSpeed = _config.force * (int)_direction;
                local.x = _savedXSpeed;
                _rigidbody.linearVelocity = Actor.transform.TransformDirection(local);
            }
            else
            {
                _lastPosition = _rigidbody.position;
            }
        }

        public override void OnTick(float dt)
        {
            base.OnTick(dt);

            _timer += dt / _config.duration;

            // TODO: Do better spline search and fix "overjump"
            if (Kinematics.mode == KinematicsMode.Dash)
            {
                var path = Kinematics.GetPath();
                Spline nearestSpline = null;
                Vector3 nearestPoint = Vector3.zero;
                float shortestDist = float.MaxValue;

                SplineSample sample = new SplineSample();

                Vector3 searchDirection = Actor.transform.right * (int)_direction;

                for (int i = 0; i < path.Splines.Count; i++)
                {
                    SplineUtility.GetNearestPoint(
                        path.Splines[i],
                        path.transform.InverseTransformPoint(Actor.transform.position),
                        out var point, out var f, 12, 6);

                    float dist = (Actor.transform.position - path.transform.TransformPoint(point)).magnitude;

                    Vector3 splineDirection = path.transform.TransformPoint(point) - Actor.transform.position;
                    float dot = Vector3.Dot(splineDirection.normalized, searchDirection);
                    if (dot > 0f && dist < shortestDist)
                    {
                        nearestSpline = path.Splines[i];
                        nearestPoint = point;
                        shortestDist = dist;

                        sample = new SplineSample
                        {
                            pos = path.EvaluatePosition(nearestSpline, f),
                            tg = ((Vector3)path.EvaluateTangent(nearestSpline, f)).normalized,
                            up = path.EvaluateUpVector(nearestSpline, f)
                        };
                    }
                }

                if (nearestSpline != null)
                {
                    Vector3 targetPosition = path.transform.TransformPoint(nearestPoint);
                    Debug.DrawLine(Actor.transform.position, targetPosition, Color.red, 1f);

                    float tgDot = Vector3.Dot(Actor.transform.forward, sample.tg);
                    if (tgDot < 0f)
                    {
                        sample.tg = -sample.tg;
                    }

                    _rigidbody.position = Vector3.Lerp(_lastPosition, targetPosition, _timer);
                    Actor.transform.rotation = Quaternion.LookRotation(sample.tg, sample.up);
                }
            }

            Vector3 local = Actor.transform.InverseTransformDirection(_rigidbody.linearVelocity);
            local.x = Mathf.Lerp(_savedXSpeed, 0f, _timer);
            _rigidbody.linearVelocity = Actor.transform.TransformDirection(local);

            if (_timer >= 1f)
            {
                StateMachine.SetState<FStateGround>();
            }
        }

        public override void OnFixedTick(float dt)
        {
            base.OnFixedTick(dt);

            if (!Common.CheckForGround(out _))
            {
                Actor.stateMachine.SetState<FStateAir>();
            }
        }

        public void SetDirection(QuickstepDirection direction) => _direction = direction;

        public QuickstepDirection GetDirection()
        {
            return _direction;
        }

        public float Timeout { get; set; }
    }
}