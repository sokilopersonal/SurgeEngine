using System;
using SurgeEngine.Code.Core.Actor.States.BaseStates;
using SurgeEngine.Code.Core.Actor.System;
using UnityEngine;

namespace SurgeEngine.Code.Core.Actor.States
{
    public class FStateStart : FStateMove
    {
        private StartData _startData;
        private float _timer;
        
        public FStateStart(ActorBase owner, Rigidbody rigidbody) : base(owner, rigidbody)
        {
            
        }
        
        public override void OnEnter()
        {
            base.OnEnter();

            Actor.Input.enabled = false;
            
            _timer = 0;
        }

        public override void OnTick(float dt)
        {
            base.OnTick(dt);

            if (_startData.startType != StartType.Dash)
            {
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
                            Actor.Flags.AddFlag(new Flag(FlagType.OutOfControl, null, true, _startData.time));
                        }
                    }

                    Actor.Input.enabled = true;
                    StateMachine.SetState<FStateGround>();
                }
            }
        }

        public void SetData(StartData data)
        {
            _startData = data;
            
            if (_startData.startType == StartType.Dash)
            {
                _rigidbody.linearVelocity = _rigidbody.transform.forward * _startData.speed;
                Actor.Input.enabled = true;
                Actor.Flags.AddFlag(new Flag(FlagType.OutOfControl, null, true, _startData.time));
                StateMachine.SetState<FStateGround>();
            }
        }
        
        public StartType GetStartType() => _startData.startType;
    }

    [Serializable]
    public class StartData
    {
        public Transform StartTransform { get; set; }
        
        public StartType startType = StartType.Standing;
        public float speed;
        public float time;
    }

    public enum StartType
    {
        None,
        Standing,
        Prepare,
        Dash
    }
}