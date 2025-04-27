using SurgeEngine.Code.Gameplay.Enemy.Base;
using UnityEngine;

namespace SurgeEngine.Code.Gameplay.Enemy.EggFighter.States
{
    public class EGStateTurn : EGState
    {
        private float _timer;
        private Vector3 _startPos;
        private Quaternion _startRot;
        
        public EGStateTurn(EnemyBase enemy) : base(enemy)
        {
        }

        public override void OnEnter()
        {
            base.OnEnter();

            _timer = 0f;
            _startPos = transform.position;
            _startRot = transform.rotation;
        }

        public override void OnTick(float dt)
        {
            base.OnTick(dt);

            _timer += dt;
            float height = eggFighter.turnHeightCurve.Evaluate(_timer / eggFighter.turnTime);
            transform.position = _startPos + Vector3.up * height;
            
            Quaternion euler = Quaternion.Euler(0, _startRot.eulerAngles.y + 180f, 0);
            float time = eggFighter.turnCurve.Evaluate(_timer / eggFighter.turnTime);
            transform.rotation = Quaternion.Slerp(_startRot, euler, time);
            
            if (_timer >= 1.85f)
            {
                eggFighter.stateMachine.SetState<EGStatePatrol>(1f);
            }
        }
        
        private float EaseInCubic(float x) => x * x * x;
    }
}