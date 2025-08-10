using SurgeEngine.Code.Core.Actor.CameraSystem.Pans.Data;
using SurgeEngine.Code.Core.Actor.States;
using SurgeEngine.Code.Core.Actor.System;
using SurgeEngine.Code.Infrastructure.Custom;
using UnityEngine;

namespace SurgeEngine.Code.Core.Actor.CameraSystem.Pans
{
    public class CameraAnimState : CameraState
    {
        private Vector3 _playerCenter;
        private readonly CameraAnimStateData _data;
        private float _time;

        public CameraAnimState(CharacterBase owner) : base(owner)
        {
            var startData = owner.GetStartData();
            _playerCenter = owner.transform.position;
            if (startData.startType == StartType.Standing)
            {
                _data = new CameraAnimStateData(owner.transform.forward * 2.75f + Vector3.up * 0.1f, -owner.transform.forward * 3f + Vector3.up * 0.25f, 3.5f);
                
                _playerCenter -= Vector3.up * 0.2f; 
                _data.start += _playerCenter;
                _data.end += _playerCenter;
            }
            else if (startData.startType == StartType.Prepare)
            {
                _data = new CameraAnimStateData(-owner.transform.forward * 2.5f + Vector3.up * 0.25f, owner.transform.forward * 1.5f + owner.transform.right * 1.2f, 3.4f);
                
                _playerCenter -= Vector3.up * 0.3f;
                _data.start += _playerCenter;
                _data.end += _playerCenter;
            }
        }

        public override void OnEnter()
        {
            base.OnEnter();
            
            _time = 0;
        }

        public override void OnTick(float dt)
        {
            base.OnTick(dt);
            
            Vector3 center = (_data.start + _data.end) * 0.5f;
            center -= Character.transform.right;
            
            Vector3 startCenter = _data.start - center;
            Vector3 endCenter = _data.end - center;
            
            StatePosition = Vector3.Slerp(startCenter, endCenter, Easings.Get(Easing.Gens, _time));
            StatePosition += center;
            StateRotation = Quaternion.LookRotation(_playerCenter - StatePosition);
            StateFOV = 50;

            _time += dt / _data.duration;
            
            if (_time >= 1f)
            {
                if (_stateMachine.VolumeCount == 0)
                {
                    _stateMachine.ResetBlendFactor();
                    
                    _stateMachine.CurrentData = new PanData
                    {
                        easeTimeEnter = Character.GetStartData().startType == StartType.Standing ? 0.5f : 0.25f,
                        easeTimeExit = Character.GetStartData().startType == StartType.Standing ? 0.5f : 0.25f
                    };
                    _stateMachine.SetState<NewModernState>();
                }
                else
                {
                    _stateMachine.ApplyTop();
                }
            }
        }
    }

    class CameraAnimStateData
    {
        public Vector3 start;
        public Vector3 end;
        public float duration;

        public CameraAnimStateData(Vector3 start, Vector3 end, float duration)
        {
            this.start = start;
            this.end = end;
            this.duration = duration;
        }
    }
}