using SurgeEngine.Source.Code.Core.Character.States.BaseStates;
using SurgeEngine.Source.Code.Core.Character.System;
using SurgeEngine.Source.Code.Infrastructure.Config.Sonic;
using UnityEngine;

namespace SurgeEngine.Source.Code.Core.Character.States.Characters.Sonic
{
    public class FStateLightSpeedDash : FCharacterState
    {
        private LightSpeedDashConfig _config;
        
        private Transform _nextRing;
        private Vector3 _targetRingPosition;
        private Collider[] _rings;
        private int _numRingsDetected;
        private bool _isRingsDetected;
        private float _ringDistance;
        private float _closeRingDistance;
    
        public FStateLightSpeedDash(CharacterBase owner) : base(owner)
        {
            owner.TryGetConfig(out _config);
        }

        public override void OnFixedTick(float dt)
        {
            base.OnFixedTick(dt);
        
            Search();
        }

        private void Search()
        {
            var transform = Character.transform;
            var rb = Character.Rigidbody;

            var speed = _config.Speed;
            var distance = _config.Distance;
            var radius = _config.Radius;
            var offset = _config.Offset;
            var mask = LayerMask.GetMask("Ring");
            
            _rings = new Collider[25];
            _numRingsDetected = rb.linearVelocity.magnitude > 0.1f ?
                Physics.OverlapSphereNonAlloc(transform.position + rb.linearVelocity.normalized * distance, radius, _rings, mask) :
                Physics.OverlapSphereNonAlloc(transform.position + transform.forward * distance, radius, _rings, mask);
            _isRingsDetected = _numRingsDetected > 0;

            if (_isRingsDetected)
            {
                _closeRingDistance = Mathf.Infinity;
                for (int i = 0; i < _numRingsDetected; i++)
                {
                    _ringDistance = Vector3.Distance(transform.position, _rings[i].transform.position);
                    if (_ringDistance < _closeRingDistance)
                    {
                        _closeRingDistance = _ringDistance;
                        _nextRing = _rings[i].transform;
                    }
                }
                
                Vector3 targetPosition = _nextRing.position + Vector3.up * offset;
                rb.linearVelocity = speed * (targetPosition - transform.position).normalized;
                rb.rotation = Quaternion.LookRotation(targetPosition - transform.position);
            }
            else
            {
                rb.linearVelocity = transform.forward * speed;
                StateMachine.SetState<FStateAir>();
            }
        }
    }
}