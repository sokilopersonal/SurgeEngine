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
        [field: SerializeField] public Transform Parent { get; private set; }
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
        private readonly Dictionary<Type, ActorComponent> _components = new();

        [Header("Config")]
        [SerializeField] private PhysicsConfig config;
        [SerializeField] private DamageKickConfig damageKickConfig;
        public PhysicsConfig Config => config;
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
            Rigidbody = GetComponent<Rigidbody>();
            
            Parent = transform.parent;
            
            ActorContext.Set(this);
            
            InitializeConfigs();
            AddStates();
            
            foreach (var component in Parent.GetComponentsInChildren<ActorComponent>())
            {
                _components.Add(component.GetType(), component);
                component.Set(this);
            }
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

        protected virtual void AddStates()
        {
            StateMachine.AddState(new FStateStart(this));
            StateMachine.AddState(new FStateIdle(this));
            StateMachine.AddState(new FStateGround(this));
            StateMachine.AddState(new FStateSlip(this));
            StateMachine.AddState(new FStateBrake(this));
            StateMachine.AddState(new FStateBrakeTurn(this));
            StateMachine.AddState(new FStateAir(this));
            StateMachine.AddState(new FStateSpecialJump(this));
            StateMachine.AddState(new FStateSit(this));
            StateMachine.AddState(new FStateJump(this));
            StateMachine.AddState(new FStateGrind(this));
            StateMachine.AddState(new FStateGrindJump(this));
            StateMachine.AddState(new FStateGrindSquat(this));
            StateMachine.AddState(new FStateRailSwitch(this));
            StateMachine.AddState(new FStateJumpSelector(this));
            StateMachine.AddState(new FStateJumpSelectorLaunch(this));
            StateMachine.AddState(new FStateSwing(this));
            StateMachine.AddState(new FStateSwingJump(this));
            StateMachine.AddState(new FStateDamage(this));
            StateMachine.AddState(new FStateDamageLand(this));
            StateMachine.AddState(new FStateUpreel(this));
            StateMachine.AddState(new FStateTrick(this));
            StateMachine.AddState(new FStateDead(this));
        }

        public void SetStart(StartData data)
        {
            _startData = data;
            
            if (data.startType != StartType.None)
            {
                StateMachine.GetState<FStateStart>().SetData(data);
                StateMachine.SetState<FStateStart>();
            }
            else
            {
                StateMachine.SetState<FStateIdle>();
            }
            
            Model.root.forward = transform.forward;
        }

        public void PutIn(Vector3 position)
        {
            Camera.StateMachine.SetLateOffset(transform.position - position);
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

        public T Get<T>() where T : ActorComponent
        {
            foreach (var pair in _components)
            {
                if (typeof(T).IsAssignableFrom(pair.Key))
                {
                    return (T)pair.Value;
                }
            }
            
            return null;
        }

        public void TakeDamage(MonoBehaviour sender, float damage)
        {
            IDamageable damageable = StateMachine.CurrentState switch
            {
                FStateGrind or FStateGrindSquat => new GrindDamage(),
                _ => new GeneralDamage()
            };
            
            if (!Flags.HasFlag(FlagType.Invincible))
            {
                IsDead = false;
                
                // Imagine it's over
                if (Stage.Instance.data.RingCount <= 0)
                {
                    if (damageable is GeneralDamage) StateMachine.GetState<FStateDamage>()?.SetState(DamageState.Dead);

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
                
                damageable.TakeDamage(this, 1);
                Flags.AddFlag(new Flag(FlagType.Invincible, null, true, DamageKickConfig.invincibleTime));
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

    class GeneralDamage : IDamageable
    {
        public void TakeDamage(MonoBehaviour sender, float damage)
        {
            ActorBase owner = (ActorBase)sender;
            
            owner.StateMachine.SetState<FStateDamage>()?.SetState(owner.IsDead ? DamageState.Dead : DamageState.Alive);
        }
    }

    class GrindDamage : IDamageable
    {
        public void TakeDamage(MonoBehaviour sender, float damage)
        {
            ActorBase owner = (ActorBase)sender;
        }
    }
}