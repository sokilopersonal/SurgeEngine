using SurgeEngine.Code.Core.Actor.System;
using SurgeEngine.Code.Gameplay.CommonObjects.Mobility;
using UnityEngine;

namespace SurgeEngine.Code.Core.Actor.States
{
    public class FStateSpring : FStateAirObject
    {
        private Spring _springObject;
        public Spring SpringObject => _springObject;
        
        private Vector3 _startPos;
        private float _snapTimer;

        public FStateSpring(ActorBase owner) : base(owner) { }
 
        public override void OnEnter()
        {
            base.OnEnter();
            
            Model.SetLowerCollision();
        }

        public override void OnExit()
        {
            base.OnExit();
            
            _springObject = null;
            Model.StartAirRestore(0.5f);
            
            Model.ResetCollisionToDefault();
        }

        public override void OnTick(float dt)
        {
            base.OnTick(dt);
            
            Vector3 dir = _springObject.transform.up;
            Vector3 pos = _springObject.transform.position + dir * Mathf.Max(1f, _travelledDistance);
            Rigidbody.position = Vector3.Lerp(_startPos, pos, _snapTimer);
            _travelledDistance += _springObject.Speed * Time.deltaTime;
            _snapTimer += Time.deltaTime / 0.1f;

            if (_travelledDistance >= _springObject.KeepVelocityDistance)
            {
                Rigidbody.isKinematic = false;
                Rigidbody.linearVelocity = dir * _springObject.Speed;
                StateMachine.SetState<FStateAir>();
            }
            
            Model.VelocityRotation(dir);
        }

        public void SetSpringObject(Spring springObject)
        {
            Rigidbody.isKinematic = true;
            _travelledDistance = 0;
            _snapTimer = 0;
            _springObject = springObject;
            _startPos = Rigidbody.position;
        }
    }
}