using System;
using SurgeEngine.Code.ActorSystem;
using UnityEngine;
namespace SurgeEngine.Code.Parameters
{
    public class FStateStart : FStateMove
    {
        private StartData _startData;
        private float _timer;
        
        public FStateStart(Actor owner, Rigidbody rigidbody) : base(owner, rigidbody)
        {
            
        }
        
        public override void OnEnter()
        {
            base.OnEnter();

            Actor.input.enabled = false;
            
            _timer = 0;
        }

        public override void OnTick(float dt)
        {
            base.OnTick(dt);

            float time = _startData.startType == StartType.Prepare ? 3f : 3.7f;
            
            if (_timer < time)
            {
                _timer += dt;
            }
            else
            {
                if (_startData.startType == StartType.Prepare)
                {
                    if (_startData.speed > 0)
                    {
                        _rigidbody.linearVelocity = _rigidbody.transform.forward * _startData.speed;
                        Actor.flags.AddFlag(new Flag(FlagType.OutOfControl, null, true, _startData.time));
                    }
                }

                Actor.input.enabled = true;
                StateMachine.SetState<FStateGround>();
            }
        }

        public void SetData(StartData data)
        {
            _startData = data;
        }
        
        public StartType GetStartType() => _startData.startType;
    }

    [Serializable]
    public class StartData
    {
        public StartType startType = StartType.Standing;
        public float speed;
        public float time;
    }

    public enum StartType
    {
        None,
        Standing,
        Prepare
    }
}