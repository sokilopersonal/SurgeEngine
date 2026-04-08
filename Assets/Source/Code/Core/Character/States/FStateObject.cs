using SurgeEngine.Source.Code.Core.Character.States.BaseStates;
using SurgeEngine.Source.Code.Core.Character.System;
using UnityEngine;

namespace SurgeEngine.Source.Code.Core.Character.States
{
    public abstract class FStateObject : FCharacterState
    {
        private float _ignoranceTime;
        protected bool Ignore => _ignoranceTime > 0;
        
        public FStateObject(CharacterBase owner) : base(owner) { }

        public override void OnEnter()
        {
            base.OnEnter();
            
            Kinematics.SetDetachTime(0.2f);
            _ignoranceTime = 0.2f;
        }

        public override void OnTick(float dt)
        {
            base.OnTick(dt);
            
            _ignoranceTime -= dt;
        }
        
        protected void ApplyLateralSnapping(Vector3 origin, Vector3 direction, ref Vector3 currentSnapVelocity, float smoothTime)
        {
            Vector3 currentPos = Rigidbody.position;
            Vector3 projection = Vector3.Project(currentPos - origin, direction);
            Vector3 targetPosition = origin + projection;
            
            Vector3 nextStep = Vector3.SmoothDamp(
                currentPos, 
                targetPosition, 
                ref currentSnapVelocity, 
                smoothTime
            );
            
            Rigidbody.MovePosition(nextStep);
        }
    }
}