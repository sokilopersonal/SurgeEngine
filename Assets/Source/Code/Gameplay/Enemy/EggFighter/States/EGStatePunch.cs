using SurgeEngine.Source.Code.Gameplay.CommonObjects;
using SurgeEngine.Source.Code.Gameplay.Enemy.Base;
using UnityEngine;

namespace SurgeEngine.Source.Code.Gameplay.Enemy.EggFighter.States
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
            
            EggFighter.PunchAnimationCallback.OnAnimationEvent += Punch;
            
            _stayTimer = 0f;
        }

        public override void OnExit()
        {
            base.OnExit();
            
            EggFighter.PunchAnimationCallback.OnAnimationEvent -= Punch;
        }

        public override void OnTick(float dt)
        {
            base.OnTick(dt);

            Agent.velocity = Vector3.zero;
            if (_stayTimer < 1f)
            {
                _stayTimer += Time.deltaTime;
            }
            else
            {
                EggFighter.StateMachine.SetState<EGStateIdle>().SetStayTimer(2.5f);
            }
        }

        private void Punch()
        {
            HurtBox.CreateWithCollider(EggFighter, EggFighter.PunchColliderReference, HurtBoxTarget.Player);
        }
    }
}