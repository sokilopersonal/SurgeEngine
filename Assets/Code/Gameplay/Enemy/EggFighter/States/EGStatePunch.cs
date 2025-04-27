using SurgeEngine.Code.Gameplay.Enemy.Base;
using UnityEngine;

namespace SurgeEngine.Code.Gameplay.Enemy.EggFighter.States
{
    public class EGStatePunch : EGState
    {
        private float _stayTimer;
        
        public EGStatePunch(EnemyBase enemy) : base(enemy)
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
        }

        public override void OnTick(float dt)
        {
            base.OnTick(dt);

            eggFighter.rb.linearVelocity = Vector3.zero;
            
            if (_stayTimer < 1f)
            {
                _stayTimer += Time.deltaTime;
            }
            else
            {
                eggFighter.StateMachine.SetState<EGStateIdle>(0);
            }
        }
    }
}