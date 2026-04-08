using SurgeEngine.Source.Code.Core.Character.System;
using SurgeEngine.Source.Code.Gameplay.CommonObjects.Mobility;
using UnityEngine;

namespace SurgeEngine.Source.Code.Core.Character.States
{
    public class FStateSpring : FStateAirObject
    {
        public Spring SpringObject { get; private set; }
        
        private Vector3 _snapVelocity;

        public FStateSpring(CharacterBase owner) : base(owner) { }

        public override void OnEnter()
        {
            base.OnEnter();
            Model.StopAirRestore();
            Kinematics.SetDetachTime(0.1f);
            Model.SetLowerCollision();
            
            _snapVelocity = Vector3.zero;
        }

        public override void OnExit()
        {
            base.OnExit();
            float dot = Mathf.Abs(Vector3.Dot(SpringObject.transform.up, Vector3.up));
            if (SpringObject is not WideSpring && dot < 0.99f)
            {
                Model.StartAirRestore(0.4f);
            }
            SpringObject = null;
            Model.ResetCollisionToDefault();
        }

        public override void OnFixedTick(float dt)
        {
            if (SpringObject == null) return;

            Vector3 dir = SpringObject.Direction;

            Rigidbody.linearVelocity = dir * SpringObject.Speed;
            TravelledDistance += SpringObject.Speed * dt;

            if (SpringObject.ShouldSnap)
            {
                ApplyLateralSnapping(
                    SpringObject.transform.position, 
                    SpringObject.Direction, 
                    ref _snapVelocity, 
                    0.12f
                );
            }

            if (SpringObject.IsWallWalk)
            {
                HandleWallWalk(dir);
            }

            float targetDistance = SpringObject.KeepVelocityDistance;
            if (TravelledDistance >= targetDistance)
            {
                StateMachine.SetState<FStateAir>();
                return;
            }

            Model.VelocityRotation(dir);
        }

        private void HandleWallWalk(Vector3 dir)
        {
            Vector3 pos = SpringObject.transform.position + dir * Mathf.Max(1f, TravelledDistance);
            var ray = new Ray(pos, dir);
            if (Physics.Raycast(ray, out var hit, Character.Config.castDistance, Character.Config.castLayer))
            {
                Kinematics.Normal = hit.normal;
                Vector3 wallDir = Vector3.ProjectOnPlane(dir, hit.normal);
                Rigidbody.linearVelocity = wallDir * SpringObject.Speed;
                Rigidbody.rotation = Quaternion.LookRotation(dir, hit.normal);
                StateMachine.SetState<FStateGround>();
                Model.StopAirRestore();
            }
        }

        public void SetSpringObject(Spring springObject)
        {
            TravelledDistance = 0;
            SpringObject = springObject;
            Rigidbody.linearVelocity = springObject.Direction * SpringObject.Speed;
        }
    }
}