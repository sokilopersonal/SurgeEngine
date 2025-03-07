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
        private Rigidbody _rigidbody;

        protected override void Awake()
        {
            base.Awake();
            
            _rigidbody = GetComponent<Rigidbody>();
            _rigidbody.centerOfMass -= Vector3.up * 0.5f;

            InitializeConfigs();
            InitializeComponents();
            
            AddStates();
        }

        private void InitializeComponents()
        {
            input?.Set(this);
            stats?.Set(this);
            sounds?.Set(this);
            camera?.Set(this);
            animation?.Set(this);
            effects?.Set(this);
            model?.Set(this);
            kinematics?.Set(this);
        }

        protected virtual void AddStates()
        {
            stateMachine.AddState(new FStateStart(this, _rigidbody));
            stateMachine.AddState(new FStateIdle(this, _rigidbody));
            stateMachine.AddState(new FStateGround(this, _rigidbody));
            stateMachine.AddState(new FStateBrake(this, _rigidbody));
            stateMachine.AddState(new FStateBrakeTurn(this, _rigidbody));
            stateMachine.AddState(new FStateAir(this, _rigidbody));
            stateMachine.AddState(new FStateSpecialJump(this, _rigidbody));
            stateMachine.AddState(new FStateSit(this, _rigidbody));
            stateMachine.AddState(new FStateJump(this, _rigidbody));
            stateMachine.AddState(new FStateGrind(this, _rigidbody));
            stateMachine.AddState(new FStateGrindJump(this, _rigidbody));
            stateMachine.AddState(new FStateGrindSquat(this, _rigidbody));
            stateMachine.AddState(new FStateJumpSelector(this, _rigidbody));
            stateMachine.AddState(new FStateJumpSelectorLaunch(this, _rigidbody));
            stateMachine.AddState(new FStateSwing(this, _rigidbody));
            stateMachine.AddState(new FStateSwingJump(this, _rigidbody));
            stateMachine.AddState(new FStateDamage(this, _rigidbody));
            stateMachine.AddState(new FStateDamageLand(this, _rigidbody));
            stateMachine.AddState(new FStateUpreel(this, _rigidbody));
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