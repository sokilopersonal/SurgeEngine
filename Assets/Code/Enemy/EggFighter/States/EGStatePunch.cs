using UnityEngine;
using UnityEngine.AI;

namespace SurgeEngine.Code.Enemy.States
{
    public class EGStatePunch : EGState
    {
        private float _stayTimer;
        
        public EGStatePunch(EggFighter eggFighter, Transform transform, Rigidbody rb) : base(eggFighter, transform, rb)
        {
            
        }

        public override void OnEnter()
        {
            base.OnEnter();
            
            _stayTimer = 0f;
            
            Rb.linearVelocity = Vector3.zero;
        }

        public override void OnExit()
        {
            base.OnExit();
            
            eggFighter.animation.ResetAction();
        }

        public override void OnTick(float dt)
        {
            base.OnTick(dt);

            if (_stayTimer < 1f)
            {
                _stayTimer += Time.deltaTime;
            }
            else
            {
                eggFighter.stateMachine.SetState<EGStateIdle>(1.5f);
            }
        }
    }
}