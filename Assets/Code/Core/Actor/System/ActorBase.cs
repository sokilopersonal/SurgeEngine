using System;
using NaughtyAttributes;
using SurgeEngine.Code.Core.Actor.CameraSystem;
using SurgeEngine.Code.Core.Actor.States;
using SurgeEngine.Code.Core.Actor.States.SonicSpecific;
using SurgeEngine.Code.Gameplay.CommonObjects.Interfaces;
using SurgeEngine.Code.Gameplay.CommonObjects.System;
using SurgeEngine.Code.Infrastructure.Config;
using UnityEngine;

namespace SurgeEngine.Code.Core.Actor.System
{
    public class ActorBase : Entity, IDamageable, IPointMarkerLoader
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

        public event Action<ActorBase> OnDied; 
        
        private StartData _startData;
        private Rigidbody _rigidbody;
        
        protected override void Awake()
        {
            base.Awake();
            
            _rigidbody = GetComponent<Rigidbody>();
            _rigidbody.centerOfMass -= Vector3.up * 0.5f;
            
            InitializeConfigs();
            AddStates();

            InitializeComponents();
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
            stateMachine.AddState(new FStateDead(this));
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

        public void PutIn(Vector3 position)
        {
            camera.stateMachine.SetLateOffset(transform.position - position);
            transform.position = position;
        }

        public void AddIn(Vector3 position)
        {
            transform.position += position;
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
                
                // Imagine it's over
                if (Stage.Instance.data.RingCount <= 0)
                {
                    var damageState = stateMachine.GetState<FStateDamage>();
                    damageState.SetState(DamageState.Dead);

                    if (damageState.State == DamageState.Dead)
                    {
                        OnDied?.Invoke(this);
                    }
                }
            }
        }

        public virtual void Load(Vector3 loadPosition, Quaternion loadRotation)
        {
            _rigidbody.isKinematic = true;
            _rigidbody.position = loadPosition;
            _rigidbody.rotation = loadRotation;
            if (model) model.root.rotation = loadRotation;
            if (animation) animation.StateAnimator.TransitionToState("Idle", 0f);
            if (flags) flags.AddFlag(new Flag(FlagType.OutOfControl, null, true, 0.5f));
            _rigidbody.isKinematic = false;
            
            stateMachine.SetState<FStateIdle>(ignoreInactiveDelay: true);
        }

        public StartData GetStartData() => _startData;
    }
}