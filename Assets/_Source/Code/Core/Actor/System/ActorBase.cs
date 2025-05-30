using System;
using System.Collections.Generic;
using NaughtyAttributes;
using SurgeEngine.Code.Core.Actor.CameraSystem;
using SurgeEngine.Code.Core.Actor.States;
using SurgeEngine.Code.Core.Actor.States.Characters.Sonic;
using SurgeEngine.Code.Core.StateMachine;
using SurgeEngine.Code.Gameplay.CommonObjects.Interfaces;
using SurgeEngine.Code.Gameplay.CommonObjects.System;
using SurgeEngine.Code.Infrastructure.Config;
using UnityEngine;
using Random = UnityEngine.Random;

namespace SurgeEngine.Code.Core.Actor.System
{
    public class ActorBase : MonoBehaviour, IDamageable, IPointMarkerLoader
    {
        public new Transform transform => Rigidbody.transform;
        
        [Header("Components")]
        [SerializeField, Required] private ActorInput input;
        [SerializeField, Required] private ActorSounds sounds; 
        [SerializeField, Required] private new ActorCamera camera;
        [SerializeField, Required] private new ActorAnimation animation;
        [SerializeField, Required] private ActorEffects effects;
        [SerializeField, Required] private ActorModel model;
        [SerializeField, Required] private ActorFlags flags;
        [SerializeField, Required] private ActorKinematics kinematics;
        public ActorInput Input => input;
        public ActorSounds Sounds => sounds;
        public ActorCamera Camera => camera; 
        public ActorAnimation Animation => animation;
        public ActorEffects Effects => effects;
        public ActorModel Model => model;
        public ActorFlags Flags => flags;
        public ActorKinematics Kinematics => kinematics;

        [Header("Config")]
        [SerializeField] private BaseActorConfig config;
        [SerializeField] private DamageKickConfig damageKickConfig;
        public BaseActorConfig Config => config;
        public DamageKickConfig DamageKickConfig => damageKickConfig;

        public bool IsDead { get; set; }
        public event Action<ActorBase> OnDied;
        public event Action OnRingLoss;
        
        private readonly Dictionary<Type, ScriptableObject> _configs = new();
        
        private StartData _startData;

        public FStateMachine StateMachine { get; private set; }
        public Rigidbody Rigidbody { get; private set; }

        private void Awake()
        {
            StateMachine = new FStateMachine();
            
            Rigidbody = GetComponentInChildren<Rigidbody>();
            
            ActorContext.Set(this);
            
            InitializeConfigs();
            AddStates();

            InitializeComponents();
        }

        private void Update()
        {
            StateMachine.Tick(Time.deltaTime);
        }

        private void FixedUpdate()
        {
            StateMachine.FixedTick(Time.fixedDeltaTime);
        }

        private void LateUpdate()
        {
            StateMachine.LateTick(Time.deltaTime);
        }

        private void InitializeComponents()
        {
            Input?.Set(this);
            Sounds?.Set(this);
            Camera?.Set(this);
            Animation?.Set(this);
            Effects?.Set(this);
            Model?.Set(this);
            Kinematics?.Set(this);
        }

        protected virtual void AddStates()
        {
            StateMachine.AddState(new FStateStart(this, Rigidbody));
            StateMachine.AddState(new FStateIdle(this, Rigidbody));
            StateMachine.AddState(new FStateGround(this, Rigidbody));
            StateMachine.AddState(new FStateBrake(this, Rigidbody));
            StateMachine.AddState(new FStateBrakeTurn(this, Rigidbody));
            StateMachine.AddState(new FStateAir(this, Rigidbody));
            StateMachine.AddState(new FStateSpecialJump(this, Rigidbody));
            StateMachine.AddState(new FStateSit(this, Rigidbody));
            StateMachine.AddState(new FStateJump(this, Rigidbody));
            StateMachine.AddState(new FStateGrind(this, Rigidbody));
            StateMachine.AddState(new FStateGrindJump(this, Rigidbody));
            StateMachine.AddState(new FStateGrindSquat(this, Rigidbody));
            StateMachine.AddState(new FStateJumpSelector(this, Rigidbody));
            StateMachine.AddState(new FStateJumpSelectorLaunch(this, Rigidbody));
            StateMachine.AddState(new FStateSwing(this, Rigidbody));
            StateMachine.AddState(new FStateSwingJump(this, Rigidbody));
            StateMachine.AddState(new FStateDamage(this, Rigidbody));
            StateMachine.AddState(new FStateDamageLand(this, Rigidbody));
            StateMachine.AddState(new FStateUpreel(this, Rigidbody));
            StateMachine.AddState(new FStateDead(this));
        }

        public void SetStart(StartData data)
        {
            _startData = data;
            
            if (data.startType != StartType.None)
            {
                StateMachine.SetState<FStateStart>().SetData(data);
            }
            else
            {
                StateMachine.SetState<FStateIdle>();
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
            AddConfig(Config);
            AddConfig(DamageKickConfig);
        }
        
        protected void AddConfig(ScriptableObject obj)
        {
            _configs.Add(obj.GetType(), obj);
        }
        
        public void TryGetConfig<T>(out T request) where T : ScriptableObject
        {
            if (_configs.TryGetValue(typeof(T), out var result))
            {
                request = (T)result;
                return;
            }

            request = null;
        }

        public void TakeDamage(MonoBehaviour sender, float damage)
        {
            if (StateMachine.CurrentState is IDamageableState dmgState && !Flags.HasFlag(FlagType.Invincible))
            {
                IsDead = false;
                var damageState = StateMachine.GetState<FStateDamage>();
                
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
                if (!IsDead) Flags.AddFlag(new Flag(FlagType.Invincible, null, true, DamageKickConfig.invincibleTime));
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

            if (StateMachine.CurrentState is IPointMarkerLoader stateLoader)
            {
                stateLoader.Load(loadPosition, loadRotation);
            }
            
            StateMachine.SetState<FStateIdle>(ignoreInactiveDelay: true);
        }

        public virtual void OnDiedInvoke(ActorBase obj, bool isMarkedForDeath)
        {
            IsDead = isMarkedForDeath;
            OnDied?.Invoke(obj);
        }

        public StartData GetStartData() => _startData;
    }
}