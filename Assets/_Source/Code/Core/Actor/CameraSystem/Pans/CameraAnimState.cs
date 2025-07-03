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

        public CameraAnimState(ActorBase owner) : base(owner)
        {
            var startData = owner.GetStartData();
            _playerCenter = owner.transform.position;
            if (startData.startType == StartType.Standing)
            {
                _data = new CameraAnimStateData(owner.transform.forward * 2f + Vector3.up * 0.4f + -owner.transform.right, -owner.transform.forward * 2.7f + Vector3.up * 0.5f, 3.4f);
                
                _playerCenter -= Vector3.up * 0.2f; 
                _data.start += _playerCenter;
                _data.end += _playerCenter;
            }
            else if (startData.startType == StartType.Prepare)
            {
                _data = new CameraAnimStateData(owner.transform.forward * 2f, -owner.transform.forward * 1f + owner.transform.right * 2, 3.2f);
                
                _playerCenter -= Vector3.up * 0.15f;
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
            center -= _actor.transform.right;
            
            Vector3 startCenter = _data.start - center;
            Vector3 endCenter = _data.end - center;
            
            _stateMachine.Position = Vector3.Slerp(startCenter, endCenter, Easings.Get(Easing.Gens, _time));
            _stateMachine.Position += center;
            _stateMachine.SetRotation(_playerCenter);

            _time += dt / _data.duration;
            
            if (_time >= 1f)
            {
                _stateMachine.CurrentData = new PanData
                {
                    easeTimeExit = 0.75f
                };
                _stateMachine.SetState<RestoreCameraPawn>();
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