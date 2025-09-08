using SurgeEngine._Source.Code.Core.Character.System;
using SurgeEngine._Source.Code.Gameplay.CommonObjects.Mobility;
using UnityEngine;

namespace SurgeEngine._Source.Code.Core.Character.States
{
    public class FStateSpring : FStateAirObject
    {
        private Spring _springObject;
        public Spring SpringObject => _springObject;
        
        private Vector3 _startPos;
        private float _snapTimer;

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
            
            _springObject = null;
            Model.StartAirRestore(0.3f);
            
            Model.ResetCollisionToDefault();
        }

        public override void OnTick(float dt)
        {
            base.OnTick(dt);
            
            Vector3 dir = _springObject.transform.up;
            Vector3 pos = _springObject.transform.position + dir * Mathf.Max(1f, _travelledDistance);
            Vector3 endPos = Vector3.Lerp(_startPos, pos, _snapTimer);
            
            Rigidbody.MovePosition(endPos);
            _travelledDistance += _springObject.Speed * Time.deltaTime;
            _snapTimer += Time.deltaTime / 0.1f;

            if (_travelledDistance >= _springObject.KeepVelocityDistance + 0.25f)
            {
                Rigidbody.linearVelocity = dir * _springObject.Speed;
                StateMachine.SetState<FStateAir>();
            }
            
            Model.VelocityRotation(dir);
        }

        public void SetSpringObject(Spring springObject)
        {
            _travelledDistance = 0;
            _snapTimer = 0;
            _springObject = springObject;
            _startPos = Rigidbody.position;

            Rigidbody.linearVelocity = Vector3.zero;
        }
    }
}