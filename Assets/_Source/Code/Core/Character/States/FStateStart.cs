using System;
using SurgeEngine._Source.Code.Core.Character.States.BaseStates;
using SurgeEngine._Source.Code.Core.Character.System;
using UnityEngine;

namespace SurgeEngine._Source.Code.Core.Character.States
{
    public class FStateStart : FCharacterState
    {
        private StartData _startData;
        private float _timer;
        
        public FStateStart(CharacterBase owner) : base(owner)
        {
            
        }
        
        public override void OnEnter()
        {
            base.OnEnter();

            Input.enabled = false;
            
            _timer = 0;
        }

        public override void OnTick(float dt)
        {
            base.OnTick(dt);

            if (_startData.startType != StartType.Dash)
            {
                float time = _startData.startType == StartType.Prepare ? 3.1f : 3.7f;
            
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
                            Rigidbody.linearVelocity = Rigidbody.transform.forward * _startData.speed;
                            character.Flags.AddFlag(new Flag(FlagType.OutOfControl, true, _startData.time));
                        }
                    }

                    Input.enabled = true;
                    StateMachine.SetState<FStateGround>();
                }
            }
        }

        public void SetData(StartData data)
        {
            _startData = data;
            
            if (_startData.startType == StartType.Dash)
            {
                Rigidbody.linearVelocity = Rigidbody.transform.forward * _startData.speed;
                Input.enabled = true;
                character.Flags.AddFlag(new Flag(FlagType.OutOfControl, true, _startData.time));
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