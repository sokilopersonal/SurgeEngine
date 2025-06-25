using SurgeEngine.Code.Gameplay.Enemy.Base;
using UnityEngine;

namespace SurgeEngine.Code.Gameplay.Enemy.EggFighter.States
{
    public class EGStateIdle : EGState
    {
        private float _stayTimer;
        
        public EGStateIdle(EnemyBase enemy) : base(enemy)
        {
        }

        public override void OnEnter()
        {
            base.OnEnter();
            
            _stayTimer = 0f;
        }

        public override void OnTick(float dt)
        {
            base.OnTick(dt);

            if (sensor.FindVisibleTargets(out _))
            {
                stateMachine.SetState<EGStateChase>();
            }
        }
    }
}