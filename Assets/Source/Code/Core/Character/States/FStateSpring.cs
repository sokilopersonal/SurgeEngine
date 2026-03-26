using SurgeEngine.Source.Code.Core.Character.System;
using SurgeEngine.Source.Code.Gameplay.CommonObjects.Mobility;
using UnityEngine;

namespace SurgeEngine.Source.Code.Core.Character.States
{
    public class FStateSpring : FStateAirObject
    {
        private Spring _springObject;
        public Spring SpringObject => _springObject;

        public FStateSpring(CharacterBase owner) : base(owner) { }

        public override void OnEnter()
        {
            base.OnEnter();
            
            Kinematics.SetDetachTime(0.1f);
            Model.SetLowerCollision();
        }

        public override void OnExit()
        {
            base.OnExit();

            float dot = Mathf.Abs(Vector3.Dot(_springObject.transform.up, Vector3.up));
            if (_springObject is not WideSpring && dot < 0.99f)
            {
                Model.StartAirRestore(0.4f);
            }
            
            _springObject = null;

            Model.ResetCollisionToDefault();
        }

        public override void OnFixedTick(float dt)
        {
            base.OnFixedTick(dt);
            
            Vector3 dir = _springObject.Direction;
    
            Rigidbody.linearVelocity = dir * _springObject.Speed;
            travelledDistance += _springObject.Speed * dt;

            if (_springObject)
            {
                if (_springObject.IsWallWalk)
                {
                    Vector3 pos = _springObject.transform.position + dir * Mathf.Max(1f, travelledDistance);
                    var ray = new Ray(pos, dir);
                    if (Physics.Raycast(ray, out var hit, Character.Config.castDistance, Character.Config.castLayer))
                    {
                        Kinematics.Normal = hit.normal;

                        Vector3 wallDir = Vector3.ProjectOnPlane(dir, hit.normal);
                        Rigidbody.linearVelocity = wallDir * _springObject.Speed;
                        Rigidbody.rotation = Quaternion.LookRotation(dir, hit.normal);
                        
                        StateMachine.SetState<FStateGround>();
                        
                        Model.StopAirRestore();
                    }
                }
            }
            
            float targetDistance = _springObject.KeepVelocityDistance + 0.25f;
            if (travelledDistance >= targetDistance)
            {
                Vector3 targetPos = _springObject.transform.position + dir * targetDistance;
                Rigidbody.position = targetPos;
        
                StateMachine.SetState<FStateAir>();
            }
            
            Model.VelocityRotation(dir);
        }

        public void SetSpringObject(Spring springObject)
        {
            travelledDistance = 0;
            _springObject = springObject;
            
            Rigidbody.linearVelocity = springObject.Direction * _springObject.Speed;
        }
    }
}