using System;
using SurgeEngine.Code.Core.Actor.CameraSystem;
using SurgeEngine.Code.Core.Actor.States;
using SurgeEngine.Code.Core.Actor.States.Characters.Sonic;
using SurgeEngine.Code.Gameplay.CommonObjects.Interfaces;
using SurgeEngine.Code.Gameplay.CommonObjects.System;
using SurgeEngine.Code.Infrastructure.Config;
using UnityEngine;
using Random = UnityEngine.Random;

namespace SurgeEngine.Code.Core.Actor.System
{
    public class ActorBase : Entity, IDamageable, IPointMarkerLoader
    {
        public new Transform transform => Rigidbody.transform;
        
        [SerializeField] private ActorInput input;
        [SerializeField] private ActorStats stats;
        [SerializeField] private ActorSounds sounds; 
        [SerializeField] private new ActorCamera camera;
        [SerializeField] private new ActorAnimation animation;
        [SerializeField] private ActorEffects effects;
        [SerializeField] private ActorModel model;
        [SerializeField] private ActorFlags flags;
        [SerializeField] private ActorKinematics kinematics;
        public ActorInput Input => input;
        public ActorStats Stats => stats;
        public ActorSounds Sounds => sounds;
        public ActorCamera Camera => camera; 
        public ActorAnimation Animation => animation;
        public ActorEffects Effects => effects;
        public ActorModel Model => model;
        public ActorFlags Flags => flags;
        public ActorKinematics Kinematics => kinematics;
        
        public BaseActorConfig config;
        public DamageKickConfig damageKickConfig;

        public bool IsDead { get; set; }
        public event Action<ActorBase> OnDied;

        public event Action OnRingLoss;
        
        private StartData _startData;
        
        public Rigidbody Rigidbody { get; private set; }

        protected override void Awake()
        {
            base.Awake();
            
            Rigidbody = GetComponentInChildren<Rigidbody>();
            
            ActorContext.Set(this);
            
            InitializeConfigs();
            AddStates();

            InitializeComponents();
        }

        private void InitializeComponents()
        {
            Input?.Set(this);
            Stats?.Set(this);
            Sounds?.Set(this);
            Camera?.Set(this);
            Animation?.Set(this);
            Effects?.Set(this);
            Model?.Set(this);
            Kinematics?.Set(this);
        }

        protected virtual void AddStates()
        {
            stateMachine.AddState(new FStateStart(this, Rigidbody));
            stateMachine.AddState(new FStateIdle(this, Rigidbody));
            stateMachine.AddState(new FStateGround(this, Rigidbody));
            stateMachine.AddState(new FStateBrake(this, Rigidbody));
            stateMachine.AddState(new FStateBrakeTurn(this, Rigidbody));
            stateMachine.AddState(new FStateAir(this, Rigidbody));
            stateMachine.AddState(new FStateSpecialJump(this, Rigidbody));
            stateMachine.AddState(new FStateSit(this, Rigidbody));
            stateMachine.AddState(new FStateJump(this, Rigidbody));
            stateMachine.AddState(new FStateGrind(this, Rigidbody));
            stateMachine.AddState(new FStateGrindJump(this, Rigidbody));
            stateMachine.AddState(new FStateGrindSquat(this, Rigidbody));
            stateMachine.AddState(new FStateJumpSelector(this, Rigidbody));
            stateMachine.AddState(new FStateJumpSelectorLaunch(this, Rigidbody));
            stateMachine.AddState(new FStateSwing(this, Rigidbody));
            stateMachine.AddState(new FStateSwingJump(this, Rigidbody));
            stateMachine.AddState(new FStateDamage(this, Rigidbody));
            stateMachine.AddState(new FStateDamageLand(this, Rigidbody));
            stateMachine.AddState(new FStateUpreel(this, Rigidbody));
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
            Camera.stateMachine.SetLateOffset(transform.position - position);
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

        public void TakeDamage(MonoBehaviour sender, float damage)
        {
            if (stateMachine.CurrentState is IDamageableState dmgState && !Flags.HasFlag(FlagType.Invincible))
            {
                IsDead = false;
                var damageState = stateMachine.GetState<FStateDamage>();
                
                // Imagine it's over
                if (Stage.Instance.data.RingCount <= 0)
                {
                    damageState?.SetState(DamageState.Dead);

                    OnDiedInvoke(this, true);
                }
                else
                {
                    // Lose rings
                    const int min = 15;

                    var data = Stage.Instance.data;
                    var ringCount = data.RingCount;

                    int value = Mathf.CeilToInt(Mathf.Max(min, ringCount * Random.Range(0.5f, 0.8f)));
                    data.RingCount -= Mathf.Clamp(value, 0, ringCount);
                    Stage.Instance.data = data;
                    
                    OnRingLoss?.Invoke();
                }
                
                dmgState.TakeDamage(this);
                if (!IsDead) Flags.AddFlag(new Flag(FlagType.Invincible, null, true, damageKickConfig.invincibleTime));
            }
        }

        public virtual void Load(Vector3 loadPosition, Quaternion loadRotation)
        {
            Rigidbody.linearVelocity = Vector3.zero;
            Rigidbody.position = loadPosition;
            Rigidbody.rotation = loadRotation;
            if (Model) Model.root.rotation = loadRotation;
            if (Animation) Animation.StateAnimator.TransitionToState("Idle", 0f);
            if (Flags) Flags.AddFlag(new Flag(FlagType.OutOfControl, null, true, 0.5f));
            if (Input) Input.playerInput.enabled = true;
            
            IsDead = false;

            if (stateMachine.CurrentState is IPointMarkerLoader stateLoader)
            {
                stateLoader.Load(loadPosition, loadRotation);
            }
            
            stateMachine.SetState<FStateIdle>(ignoreInactiveDelay: true);
        }

        public StartData GetStartData() => _startData;

        public virtual void OnDiedInvoke(ActorBase obj, bool isMarkedForDeath)
        {
            IsDead = isMarkedForDeath;
            OnDied?.Invoke(obj);
        }
    }
}