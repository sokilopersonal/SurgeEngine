using UnityEngine;

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
        }

        public override void OnExit()
        {
            base.OnExit();
            
            eggFighter.animation.ResetAction();
        }

        public override void OnTick(float dt)
        {
            base.OnTick(dt);

            Rb.linearVelocity = Vector3.zero;
            
            if (_stayTimer < 1f)
            {
                _stayTimer += Time.deltaTime;
            }
            else
            {
                eggFighter.stateMachine.SetState<EGStateIdle>(0);
            }
        }
    }
}