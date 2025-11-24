using SurgeEngine.Source.Code.Core.Character.States.BaseStates;
using SurgeEngine.Source.Code.Core.Character.System;
using SurgeEngine.Source.Code.Core.StateMachine.Base;
using SurgeEngine.Source.Code.Gameplay.CommonObjects.Collectables;
using SurgeEngine.Source.Code.Infrastructure.Config.Sonic;
using UnityEngine;

namespace SurgeEngine.Source.Code.Core.Character.States.Characters.Sonic.SubStates
{
    public class FRingDashSearch : FCharacterSubState
    {
        private LightSpeedDashConfig _config;
        
        private readonly Collider[] _rings = new Collider[25];
        
        public FRingDashSearch(CharacterBase owner) : base(owner)
        {
            owner.StateMachine.OnStateAssign += OnStateAssign;
            
            owner.TryGetConfig(out _config);
        }

        public override void OnTick(float dt)
        {
            base.OnTick(dt);


            if (Active)
            {
                if (Character.Input.YPressed)
                {
                    SearchAndDash();
                }
            }
        }

        private void SearchAndDash()
        {
            var transform = Character.transform;
            var rb = Character.Rigidbody;

            var distance = _config.Distance;
            var radius = _config.Radius;
            var mask = LayerMask.GetMask("Ring");
            
            int numRingsDetected = rb.linearVelocity.magnitude > 0.1f ?
                Physics.OverlapSphereNonAlloc(transform.position + rb.linearVelocity.normalized * distance, radius, _rings, mask) :
                Physics.OverlapSphereNonAlloc(transform.position + transform.forward * distance, radius, _rings, mask);

            if (numRingsDetected > 0)
            {
                for (int i = 0; i < numRingsDetected; i++)
                {
                    var ring = _rings[i].GetComponent<Ring>();
                    if (ring != null && ring.IsLightSpeedDashTarget && !ring.InMagnet)
                    {
                        Character.StateMachine.SetState<FStateLightSpeedDash>();
                        return;
                    }
                }
            }
        }

        private void OnStateAssign(FState obj)
        {
            Active = obj is FStateIdle or FStateGround or FStateAir;
        }
    }
}