using SurgeEngine.Code.Gameplay.CommonObjects;
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
            
            eggFighter.PunchAnimationCallback.OnAnimationEvent += Punch;
            
            _stayTimer = 0f;
        }

        public override void OnExit()
        {
            base.OnExit();
            
            eggFighter.PunchAnimationCallback.OnAnimationEvent -= Punch;
        }

        public override void OnTick(float dt)
        {
            base.OnTick(dt);

            eggFighter.Agent.velocity = Vector3.zero;
            
            if (_stayTimer < 1f)
            {
                _stayTimer += Time.deltaTime;
            }
            else
            {
                eggFighter.StateMachine.SetState<EGStateIdle>();
            }
        }

        private void Punch()
        {
            HurtBox.CreateAttached(eggFighter, eggFighter.transform, eggFighter.transform.forward + eggFighter.transform.up * 0.6f, Vector3.one * 1.5f, HurtBoxTarget.Player);
        }
    }
}