using System;
using System.Collections.Generic;
using NaughtyAttributes;
using SurgeEngine.Code.ActorStates;
using SurgeEngine.Code.ActorStates.SonicSpecific;
using SurgeEngine.Code.ActorStates.SonicSubStates;
using SurgeEngine.Code.CameraSystem;
using SurgeEngine.Code.CommonObjects;
using SurgeEngine.Code.Config;
using SurgeEngine.Code.StateMachine;
using UnityEngine;

namespace SurgeEngine.Code.ActorSystem
{
    public class Actor : Entity, IDamageable
    {
        [Foldout("Components")] public ActorInput input;
        [Foldout("Components")] public ActorStats stats;
        [Foldout("Components")] public ActorSounds sounds;
        [Foldout("Components")] public new ActorCamera camera;
        [Foldout("Components")] public new ActorAnimation animation;
        [Foldout("Components")] public ActorEffects effects;
        [Foldout("Components")] public ActorModel model;
        [Foldout("Components")] public ActorFlags flags;
        [Foldout("Components")] public ActorKinematics kinematics;
        
        [Foldout("Base Physics")]
        public BaseActorConfig config;
        public DamageKickConfig damageKickConfig;
        
        private StartData _startData;

        protected override void Awake()
        {
            if (!gameObject.activeSelf)
            {
                return;
            }
            
            Rigidbody body = GetComponent<Rigidbody>();
            body.centerOfMass -= Vector3.up * 0.5f;
            
            InitializeConfigs();
            
            stateMachine.AddState(new FStateStart(this, body));
            stateMachine.AddState(new FStateIdle(this, body));
            stateMachine.AddState(new FStateGround(this, body));
            stateMachine.AddState(new FStateBrake(this, body));
            stateMachine.AddState(new FStateBrakeTurn(this, body));
            stateMachine.AddState(new FStateAir(this, body));
            stateMachine.AddState(new FStateSpecialJump(this, body));
            stateMachine.AddState(new FStateSit(this, body));
            stateMachine.AddState(new FStateJump(this, body));
            stateMachine.AddState(new FStateGrind(this, body));
            stateMachine.AddState(new FStateGrindJump(this, body));
            stateMachine.AddState(new FStateGrindSquat(this, body));
            stateMachine.AddState(new FStateJumpSelector(this, body));
            stateMachine.AddState(new FStateJumpSelectorLaunch(this, body));
            stateMachine.AddState(new FStateSwing(this, body));
            stateMachine.AddState(new FStateSwingJump(this, body));
            stateMachine.AddState(new FStateDamage(this, body));
            stateMachine.AddState(new FStateDamageLand(this, body));
            
            animation?.Initialize(stateMachine);
        }

        public void SetStart(StartData data)
        {
            _startData = data;
            
            if (data.startType != StartType.None)
            {
                stateMachine.SetState<FStateStart>().SetData(data);
            }
            else
            {
                stateMachine.SetState<FStateIdle>();
            }
        }

        protected virtual void InitializeConfigs()
        {
            AddConfig(config);
            AddConfig(damageKickConfig);
        }

        public void TakeDamage(Entity sender, float damage)
        {
            if (stateMachine.CurrentState is IDamageableState dmgState && !flags.HasFlag(FlagType.Invincible))
            {
                dmgState.TakeDamage(this, sender);
                flags.AddFlag(new Flag(FlagType.Invincible, null, true, damageKickConfig.invincibleTime));
            }
        }

        public StartData GetStartData() => _startData;
    }
}