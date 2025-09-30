using SurgeEngine._Source.Code.Core.Character.States.BaseStates;
using SurgeEngine._Source.Code.Core.Character.States.Characters.Sonic.SubStates;
using SurgeEngine._Source.Code.Core.Character.System;
using SurgeEngine._Source.Code.Gameplay.CommonObjects.Environment;
using SurgeEngine._Source.Code.Infrastructure.Custom;
using UnityEngine;

namespace SurgeEngine._Source.Code.Core.Character.States
{
    public enum DamageState
    {
        Alive,
        Dead
    }
    
    public class FStateDamage : FCharacterState
    {
        private float _timer;

        private DamageState _state;

        public FStateDamage(CharacterBase owner) : base(owner)
        {
            
        }

        public override void OnEnter()
        {
            base.OnEnter();

            StateMachine.GetSubState<FBoost>().Active = false;
            Kinematics.ResetVelocity();
            _timer = 0.25f;

            Rigidbody.linearVelocity = -character.transform.forward * character.Life.DirectionalForce;
        }

        public override void OnFixedTick(float dt)
        {
            base.OnFixedTick(dt);

            Kinematics.ApplyGravity(Kinematics.Gravity);
            
            bool ground = Kinematics.CheckForGround(out var hit);
            if (hit.transform != null)
            {
                var isWater = hit.transform.gameObject.GetGroundTag() == GroundTag.Water;
                if (isWater && hit.transform.TryGetComponent(out WaterSurface water))
                {
                    water.Attach(Rigidbody.position, character);
                    return;
                }
            }
            
            if (ground)
            {
                Kinematics.Point = hit.point;
                Kinematics.Normal = Vector3.up;
                
                Kinematics.Snap(hit.point, Kinematics.Normal);
                Rigidbody.linearVelocity = new Vector3(Rigidbody.linearVelocity.x, 0, Rigidbody.linearVelocity.z);
                
                if (Utility.TickTimer(ref _timer, 0.6f, false))
                {
                    if (_state == DamageState.Alive)
                    {
                        StateMachine.SetState<FStateDamageLand>();
                    }
                    else
                    {
                        Rigidbody.linearVelocity = Vector3.zero;
                    }
                }
            }
        }
        
        public void SetState(DamageState state) => _state = state;
    }
}